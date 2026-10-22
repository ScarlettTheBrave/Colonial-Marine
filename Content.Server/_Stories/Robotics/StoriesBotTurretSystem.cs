using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Sentry;
using Content.Shared._RMC14.Sentry.Laptop;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._Stories.Robotics;
using Content.Shared.Access.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Stories.Robotics;

public sealed class StoriesBotTurretSystem : EntitySystem
{
    private static readonly ProtoId<NpcFactionPrototype> FactionUNMC = "UNMC";
    private static readonly ProtoId<NpcFactionPrototype> FactionXeno = "RMCXeno";
    private static readonly ProtoId<NpcFactionPrototype> FactionColonist = "Colonist";
    private static readonly ProtoId<TagPrototype> WallTag = "Wall";
    private static readonly EntProtoId<IFFFactionComponent> MarineIffFaction = "FactionMarine";
    private const string FactionHumanoidGroup = "Humanoid";
    private const float BotMoveSpeed = 2.4f;

    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly StoriesBotAnnouncementSystem _botAnnounce = default!;
    [Dependency] private readonly StoriesBotBatterySystem _botBattery = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;
    [Dependency] private readonly GunIFFSystem _iff = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedSentryTargetingSystem _sentryTargeting = default!;
    [Dependency] private readonly SkillsSystem _skills = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private readonly HashSet<EntityUid> _nearby = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<StoriesBotTurretComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<StoriesBotTurretComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<StoriesBotTurretComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<StoriesBotTurretComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAltVerbs);
        SubscribeLocalEvent<StoriesBotTurretComponent, TakeAmmoEvent>(OnTakeAmmo);
        SubscribeLocalEvent<StoriesBotTurretComponent, GetAmmoCountEvent>(OnGetAmmoCount);
    }

    private void OnMapInit(Entity<StoriesBotTurretComponent> ent, ref MapInitEvent args)
    {
        var now = _timing.CurTime;
        ent.Comp.NextThinkTime = now + ent.Comp.ThinkInterval;
        ent.Comp.NextFireTime = now;

        if (!HasComp<GunIFFComponent>(ent) && HasComp<GunComponent>(ent))
            _iff.EnableIntrinsicIFF(ent.Owner);

        var targeting = EnsureComp<SentryTargetingComponent>(ent.Owner);
        if (targeting.FriendlyFactions.Count == 0)
            _sentryTargeting.ResetToDefault((ent.Owner, targeting));
    }

    private void OnInteractHand(Entity<StoriesBotTurretComponent> ent, ref InteractHandEvent args)
    {
        if (args.Handled)
            return;

        if (!_skills.HasSkill(args.User, ent.Comp.RequiredSkill, ent.Comp.RequiredSkillLevel))
            return;

        args.Handled = true;
        ToggleFollowMode(ent, args.User);
    }

    private void OnGetAltVerbs(Entity<StoriesBotTurretComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if (!_skills.HasSkill(args.User, ent.Comp.RequiredSkill, ent.Comp.RequiredSkillLevel))
            return;

        var user = args.User;
        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString(ent.Comp.FollowMode ? "stories-bot-turret-verb-hold" : "stories-bot-turret-verb-follow"),
            Priority = 2,
            Act = () => ToggleFollowMode(ent, user),
        });
    }

    private void ToggleFollowMode(Entity<StoriesBotTurretComponent> ent, EntityUid user)
    {
        ent.Comp.Master = user;
        ent.Comp.FollowMode = !ent.Comp.FollowMode;
        Dirty(ent);

        if (TryComp<StoriesBotBatteryComponent>(ent, out var battery))
            _botBattery.SetMaster((ent.Owner, battery), user);

        if (ent.Comp.FollowMode)
        {
            _popup.PopupEntity(Loc.GetString("stories-bot-turret-mode-follow"), ent.Owner, user, PopupType.Medium);
        }
        else
        {
            StopBotMovement(ent.Owner);
            _popup.PopupEntity(Loc.GetString("stories-bot-turret-mode-hold"), ent.Owner, user, PopupType.Medium);
        }
    }

    private void OnInteractUsing(Entity<StoriesBotTurretComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (_tag.HasTag(args.Used, ent.Comp.ReloadMagazineTag))
        {
            args.Handled = true;
            if (ent.Comp.Ammo >= ent.Comp.MaxAmmo)
            {
                _popup.PopupEntity(Loc.GetString("stories-bot-turret-ammo-full"), ent.Owner, args.User, PopupType.Small);
                return;
            }

            ent.Comp.Ammo = ent.Comp.MaxAmmo;
            Dirty(ent);

            QueueDel(args.Used);
            _audio.PlayPvs(ent.Comp.ReloadSound, ent.Owner);
            _popup.PopupEntity(Loc.GetString("stories-bot-turret-reloaded"), ent.Owner, args.User, PopupType.Medium);
            return;
        }

        if (!HasComp<IdCardComponent>(args.Used) && !HasComp<SentryLaptopComponent>(args.Used))
            return;

        if (_skills.HasSkill(args.User, ent.Comp.RequiredSkill, ent.Comp.RequiredSkillLevel))
        {
            ent.Comp.Master = args.User;
            Dirty(ent);

            if (TryComp<StoriesBotBatteryComponent>(ent, out var battery))
                _botBattery.SetMaster((ent.Owner, battery), args.User);
        }
    }

    private void OnTakeAmmo(Entity<StoriesBotTurretComponent> ent, ref TakeAmmoEvent args)
    {
        for (var i = 0; i < args.Shots; i++)
        {
            if (ent.Comp.Ammo <= 0)
                break;

            ent.Comp.Ammo--;
            var cartridgeUid = Spawn(ent.Comp.AmmoProto, args.Coordinates);
            if (TryComp<CartridgeAmmoComponent>(cartridgeUid, out var cartridge))
                args.Ammo.Add((cartridgeUid, cartridge));
            else
                QueueDel(cartridgeUid);
        }

        Dirty(ent);
    }

    private void OnGetAmmoCount(Entity<StoriesBotTurretComponent> ent, ref GetAmmoCountEvent args)
    {
        args.Capacity = ent.Comp.MaxAmmo;
        args.Count = ent.Comp.Ammo;
    }

    public override void Update(float frameTime)
    {
        var now = _timing.CurTime;
        var query = EntityQueryEnumerator<StoriesBotTurretComponent, GunComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var turret, out var gun, out var xform))
        {
            if (now < turret.NextThinkTime)
                continue;

            var dt = (float)turret.ThinkInterval.TotalSeconds;
            turret.NextThinkTime = now + turret.ThinkInterval;

            TryComp<StoriesBotBatteryComponent>(uid, out var battery);

            if (!_mobState.IsAlive(uid) || (battery != null && battery.IsIdle))
            {
                turret.CurrentBurstShots = 0;
                turret.IsCoolingDown = false;
                StopBotMovement(uid);
                SetTurretTarget((uid, turret), null);
                continue;
            }

            if (turret.IsCoolingDown && now >= turret.NextFireTime)
                turret.IsCoolingDown = false;

            if (battery != null)
            {
                _botAnnounce.TryAssignNearestEngineer((uid, battery));
                if (turret.Master == null && battery.Master != null)
                {
                    turret.Master = battery.Master;
                    Dirty(uid, turret);
                }
            }

            TryComp<SentryTargetingComponent>(uid, out var targeting);
            SyncGunIFF(uid, targeting);

            var target = turret.Ammo > 0 ? AcquireTarget((uid, turret), xform, targeting) : null;
            SetTurretTarget((uid, turret), target);

            if (target == null)
            {
                if (!turret.IsCoolingDown)
                    turret.CurrentBurstShots = 0;

                if (turret.FollowMode)
                    UpdateFollowMovement((uid, turret), xform);
                else
                    StopBotMovement(uid);

                continue;
            }

            StopBotMovement(uid);

            var botPos = _transform.GetWorldPosition(xform);
            var targetPos = _transform.GetWorldPosition(target.Value);
            var delta = targetPos - botPos;
            var distToTarget = delta.Length();

            if (distToTarget <= 0.001f)
                continue;

            var desiredAngle = delta.ToWorldAngle();
            var currentAngle = _transform.GetWorldRotation(xform);
            var maxStep = MathHelper.DegreesToRadians(turret.RotationSpeedDegrees * 2.5f) * dt;
            var newAngle = StepAngleTowards(currentAngle, desiredAngle, maxStep);
            _transform.SetWorldRotation(xform, newAngle);

            if (now < turret.NextFireTime)
                continue;

            var angleDiff = Math.Abs(Angle.ShortestDistance(newAngle, desiredAngle).Theta);
            if (angleDiff > turret.FireAngleTolerance.Theta * 2.0)
                continue;

            var targetCoords = new EntityCoordinates(target.Value, Vector2.Zero);
            _gun.AttemptShoot(uid, uid, gun, targetCoords);

            turret.CurrentBurstShots++;
            if (turret.CurrentBurstShots >= turret.ShotsPerBurst)
            {
                turret.CurrentBurstShots = 0;
                turret.IsCoolingDown = true;
                turret.NextFireTime = now + turret.BurstCooldown;
            }
            else
            {
                turret.NextFireTime = now + turret.ShotInterval;
            }
        }
    }

    private void SyncGunIFF(EntityUid uid, SentryTargetingComponent? targeting)
    {
        if (targeting == null)
            return;

        IReadOnlySet<string> friendlyFactions = targeting.FriendlyFactions;
        var unmcFriendly = friendlyFactions.Contains(FactionUNMC.Id) || friendlyFactions.Contains(FactionHumanoidGroup);

        if (unmcFriendly)
        {
            if (!HasComp<GunIFFComponent>(uid))
                _iff.EnableIntrinsicIFF(uid);
        }
        else
        {
            if (HasComp<GunIFFComponent>(uid))
                RemComp<GunIFFComponent>(uid);
        }
    }

    private void UpdateFollowMovement(Entity<StoriesBotTurretComponent> ent, TransformComponent xform)
    {
        if (ent.Comp.Master is not { } master || TerminatingOrDeleted(master))
        {
            StopBotMovement(ent.Owner);
            return;
        }

        var masterXform = Transform(master);
        if (masterXform.MapID != xform.MapID)
        {
            StopBotMovement(ent.Owner);
            return;
        }

        var botPos = _transform.GetWorldPosition(xform);
        var masterPos = _transform.GetWorldPosition(masterXform);
        var diff = masterPos - botPos;
        var dist = diff.Length();

        if (dist <= ent.Comp.FollowStopDistance || dist > 25f)
        {
            StopBotMovement(ent.Owner);
            return;
        }

        var moveDir = diff / dist;
        _transform.SetWorldRotation(xform, moveDir.ToWorldAngle());

        if (TryComp<PhysicsComponent>(ent.Owner, out var physics))
        {
            _physics.WakeBody(ent.Owner, body: physics);
            _physics.SetLinearVelocity(ent.Owner, moveDir * BotMoveSpeed, body: physics);
        }
    }

    private void StopBotMovement(EntityUid uid)
    {
        if (TryComp<PhysicsComponent>(uid, out var physics) && physics.LinearVelocity != Vector2.Zero)
            _physics.SetLinearVelocity(uid, Vector2.Zero, body: physics);
    }

    private static Angle StepAngleTowards(Angle current, Angle target, float maxStepRadians)
    {
        var diff = Angle.ShortestDistance(current, target).Theta;
        if (Math.Abs(diff) <= maxStepRadians)
            return target;

        return new Angle(current.Theta + Math.Sign(diff) * maxStepRadians);
    }

    private EntityUid? AcquireTarget(
        Entity<StoriesBotTurretComponent> ent,
        TransformComponent botXform,
        SentryTargetingComponent? targeting)
    {
        _nearby.Clear();
        _lookup.GetEntitiesInRange(ent.Owner, ent.Comp.AggroRange, _nearby, LookupFlags.Uncontained);

        var botPos = _transform.GetWorldPosition(botXform);
        var bestDistSq = float.MaxValue;
        EntityUid? bestTarget = null;

        foreach (var candidate in _nearby)
        {
            if (candidate == ent.Owner)
                continue;

            if (!IsValidHostileTarget(candidate, targeting))
                continue;

            var candidatePos = _transform.GetWorldPosition(candidate);
            var distSq = (candidatePos - botPos).LengthSquared();
            if (distSq >= bestDistSq)
                continue;

            if (!_examine.InRangeUnOccluded(
                    new MapCoordinates(botPos, botXform.MapID),
                    new MapCoordinates(candidatePos, botXform.MapID),
                    ent.Comp.AggroRange,
                    e => e == ent.Owner || e == candidate || !_tag.HasTag(e, WallTag)))
            {
                continue;
            }

            bestDistSq = distSq;
            bestTarget = candidate;
        }

        return bestTarget;
    }

    private bool IsValidHostileTarget(EntityUid candidate, SentryTargetingComponent? targeting)
    {
        if (!HasComp<MobStateComponent>(candidate) || _mobState.IsDead(candidate))
            return false;

        if (HasComp<StoriesBotBatteryComponent>(candidate) || HasComp<SentryComponent>(candidate))
            return false;

        if (targeting == null)
            return HasComp<XenoComponent>(candidate);

        IReadOnlySet<string> friendlyFactions = targeting.FriendlyFactions;

        if (HasComp<XenoComponent>(candidate))
            return !friendlyFactions.Contains(FactionXeno.Id);

        if (friendlyFactions.Contains(FactionHumanoidGroup))
            return false;

        if (TryComp<NpcFactionMemberComponent>(candidate, out var npcFaction))
        {
            IReadOnlyCollection<ProtoId<NpcFactionPrototype>> candidateFactions = npcFaction.Factions;
            if (candidateFactions.Count > 0)
            {
                foreach (var faction in candidateFactions)
                {
                    if (friendlyFactions.Contains(faction.Id))
                        return false;
                }

                return true;
            }
        }

        if (_iff.IsInFaction(candidate, MarineIffFaction) || HasComp<MarineComponent>(candidate))
            return !friendlyFactions.Contains(FactionUNMC.Id);

        return !friendlyFactions.Contains(FactionColonist.Id);
    }

    private void SetTurretTarget(Entity<StoriesBotTurretComponent> ent, EntityUid? target)
    {
        if (ent.Comp.Target == target)
            return;

        ent.Comp.Target = target;
        Dirty(ent);
    }
}