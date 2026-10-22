using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Repairable;
using Content.Shared._RMC14.Xenonids.ClawSharpness;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._Stories.Robotics;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Stories.Robotics;

public sealed class StoriesBotDrillerSystem : EntitySystem
{
    private static readonly ProtoId<TagPrototype> WallTag = "Wall";
    private static readonly ProtoId<DamageTypePrototype> StructuralDamageType = "Structural";
    private const float BotMoveSpeed = 2.4f;

    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly StoriesBotAnnouncementSystem _botAnnounce = default!;
    [Dependency] private readonly StoriesBotBatterySystem _botBattery = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly SkillsSystem _skills = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly RMCWeldEffectSystem _weldEffect = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    private readonly HashSet<EntityUid> _frontEntities = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<StoriesBotDrillerComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<StoriesBotDrillerComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<StoriesBotDrillerComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAltVerbs);
        SubscribeLocalEvent<StoriesBotDrillerComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnMapInit(Entity<StoriesBotDrillerComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.CheckInterval = TimeSpan.FromSeconds(0.1);
        ent.Comp.NextCheck = _timing.CurTime + ent.Comp.CheckInterval;
    }

    private void OnShutdown(Entity<StoriesBotDrillerComponent> ent, ref ComponentShutdown args)
    {
        CancelDrilling(ent);
        StopBotMovement(ent.Owner);
    }

    private void OnInteractHand(Entity<StoriesBotDrillerComponent> ent, ref InteractHandEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<StoriesBotBatteryComponent>(ent, out var battery))
            return;

        if (!_skills.HasSkill(args.User, battery.RequiredSkill, battery.RequiredSkillLevel))
            return;

        args.Handled = true;
        ToggleFollowMode(ent, (ent.Owner, battery), args.User);
    }

    private void OnGetAltVerbs(Entity<StoriesBotDrillerComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if (!TryComp<StoriesBotBatteryComponent>(ent, out var battery))
            return;

        if (!_skills.HasSkill(args.User, battery.RequiredSkill, battery.RequiredSkillLevel))
            return;

        var user = args.User;
        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString(ent.Comp.FollowMode ? "stories-bot-driller-verb-drill" : "stories-bot-driller-verb-follow"),
            Priority = 2,
            Act = () => ToggleFollowMode(ent, (ent.Owner, battery), user),
        });
    }

    private void ToggleFollowMode(
        Entity<StoriesBotDrillerComponent> ent,
        Entity<StoriesBotBatteryComponent> battery,
        EntityUid user)
    {
        _botBattery.SetMaster(battery, user);

        ent.Comp.FollowMode = !ent.Comp.FollowMode;
        Dirty(ent);

        if (ent.Comp.FollowMode)
        {
            CancelDrilling(ent);
            _popup.PopupEntity(Loc.GetString("stories-bot-driller-mode-follow"), ent.Owner, user, PopupType.Medium);
        }
        else
        {
            StopBotMovement(ent.Owner);
            _popup.PopupEntity(Loc.GetString("stories-bot-driller-mode-drill"), ent.Owner, user, PopupType.Medium);
        }
    }

    public override void Update(float frameTime)
    {
        var now = _timing.CurTime;
        var query = EntityQueryEnumerator<StoriesBotDrillerComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var driller, out var xform))
        {
            if (now < driller.NextCheck)
                continue;

            driller.NextCheck = now + driller.CheckInterval;

            TryComp<StoriesBotBatteryComponent>(uid, out var battery);

            if (!_mobState.IsAlive(uid) || (battery != null && battery.IsIdle))
            {
                CancelDrilling((uid, driller));
                StopBotMovement(uid);
                continue;
            }

            if (battery != null)
                _botAnnounce.TryAssignNearestEngineer((uid, battery));

            if (driller.FollowMode)
            {
                CancelDrilling((uid, driller));
                UpdateFollowMovement(uid, driller, battery, xform);
                continue;
            }

            ProcessDrilling((uid, driller), battery, xform, now);
        }
    }

    private void UpdateFollowMovement(
        EntityUid uid,
        StoriesBotDrillerComponent driller,
        StoriesBotBatteryComponent? battery,
        TransformComponent xform)
    {
        if (battery?.Master is not { } master || TerminatingOrDeleted(master))
        {
            StopBotMovement(uid);
            return;
        }

        var masterXform = Transform(master);
        if (masterXform.MapID != xform.MapID)
        {
            StopBotMovement(uid);
            return;
        }

        var botPos = _transform.GetWorldPosition(xform);
        var masterPos = _transform.GetWorldPosition(masterXform);
        var diff = masterPos - botPos;
        var dist = diff.Length();

        if (dist <= driller.FollowStopDistance || dist > 25f)
        {
            StopBotMovement(uid);
            return;
        }

        var moveDir = diff / dist;
        _transform.SetWorldRotation(xform, moveDir.ToWorldAngle());

        if (TryComp<PhysicsComponent>(uid, out var physics))
        {
            _physics.WakeBody(uid, body: physics);
            _physics.SetLinearVelocity(uid, moveDir * BotMoveSpeed, body: physics);
        }
    }

    private void StopBotMovement(EntityUid uid)
    {
        if (TryComp<PhysicsComponent>(uid, out var physics) && physics.LinearVelocity != Vector2.Zero)
            _physics.SetLinearVelocity(uid, Vector2.Zero, body: physics);
    }

    private void ProcessDrilling(
        Entity<StoriesBotDrillerComponent> ent,
        StoriesBotBatteryComponent? battery,
        TransformComponent xform,
        TimeSpan now)
    {
        var frontTarget = FindObstacleNearby(ent, xform, out var distSq);
        if (frontTarget == null)
        {
            CancelDrilling(ent);
            StopBotMovement(ent.Owner);
            return;
        }

        var botPos = _transform.GetWorldPosition(xform);
        var targetPos = _transform.GetWorldPosition(frontTarget.Value);
        var delta = targetPos - botPos;
        var dist = MathF.Sqrt(distSq);

        // Drive directly up to the obstacle until within drilling contact range (1.35 tiles).
        if (dist > 1.35f)
        {
            CancelDrilling(ent);
            var moveDir = delta / dist;
            _transform.SetWorldRotation(xform, moveDir.ToWorldAngle());

            if (TryComp<PhysicsComponent>(ent.Owner, out var physics))
            {
                _physics.WakeBody(ent.Owner, body: physics);
                _physics.SetLinearVelocity(ent.Owner, moveDir * BotMoveSpeed, body: physics);
            }

            return;
        }

        StopBotMovement(ent.Owner);

        if (ent.Comp.CurrentTarget != frontTarget)
        {
            CancelDrilling(ent);
            ent.Comp.CurrentTarget = frontTarget;
            var duration = IsReinforced(frontTarget.Value) ? ent.Comp.ReinforcedDuration : ent.Comp.Duration;
            ent.Comp.DrillFinishTime = now + duration;
            Dirty(ent);

            if (delta.LengthSquared() > 0.001f)
                _transform.SetWorldRotation(xform, delta.ToWorldAngle());

            _weldEffect.SpawnWeldEffect(frontTarget.Value, duration);
            _audio.PlayPvs(ent.Comp.DrillSound, Transform(frontTarget.Value).Coordinates);
            return;
        }

        if (now < ent.Comp.DrillFinishTime)
            return;

        var target = frontTarget.Value;
        CancelDrilling(ent);

        if (!TryComp<DamageableComponent>(target, out var damageable))
            return;

        var damage = new DamageSpecifier(_prototype.Index(StructuralDamageType), ent.Comp.StructuralDamage);
        _damageable.TryChangeDamage(target, damage, true, damageable: damageable, origin: ent.Owner, tool: ent.Owner);
        _audio.PlayPvs(ent.Comp.FinishSound, Transform(target).Coordinates);

        if (battery?.Master is { } master)
        {
            var ev = new StoriesBotAnnouncementEvent(ent.Owner, master, ent.Comp.BreachCompleteAnnouncement);
            RaiseLocalEvent(ent.Owner, ref ev);
        }
    }

    private EntityUid? FindObstacleNearby(Entity<StoriesBotDrillerComponent> ent, TransformComponent xform, out float bestDistSq)
    {
        bestDistSq = float.MaxValue;
        if (xform.MapID == MapId.Nullspace)
            return null;

        _frontEntities.Clear();
        _lookup.GetEntitiesInRange(ent.Owner, 3.5f, _frontEntities, LookupFlags.Uncontained);

        var botPos = _transform.GetWorldPosition(xform);
        EntityUid? bestTarget = null;

        foreach (var candidate in _frontEntities)
        {
            if (candidate == ent.Owner || TerminatingOrDeleted(candidate))
                continue;

            if (!IsDrillableObstacle(ent, candidate))
                continue;

            var distSq = (_transform.GetWorldPosition(candidate) - botPos).LengthSquared();
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                bestTarget = candidate;
            }
        }

        return bestTarget;
    }

    private bool IsDrillableObstacle(Entity<StoriesBotDrillerComponent> ent, EntityUid target)
    {
        if (!HasComp<DamageableComponent>(target))
            return false;

        if (_whitelist.IsBlacklistPass(ent.Comp.Blacklist, target))
            return false;

        return _tag.HasTag(target, WallTag) || HasComp<XenoConstructComponent>(target);
    }

    private bool IsReinforced(EntityUid wall)
    {
        return TryComp<ReceiverXenoClawsComponent>(wall, out var claws) &&
               claws.MinimumClawStrength >= XenoClawType.VerySharp;
    }

    private void CancelDrilling(Entity<StoriesBotDrillerComponent> ent)
    {
        if (ent.Comp.CurrentTarget is { } oldTarget && !TerminatingOrDeleted(oldTarget))
            _weldEffect.ClearActiveEffect(oldTarget);

        if (ent.Comp.CurrentTarget != null)
        {
            ent.Comp.CurrentTarget = null;
            Dirty(ent);
        }
    }
}