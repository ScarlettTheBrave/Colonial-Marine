using System.Numerics;
using Content.Shared._ColonialMarine.Marines.Skills;
using Content.Shared._ColonialMarine.Sentry.Laptop;
using Content.Shared._ColonialMarine.Xenonids;
using Content.Shared.Access.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.PowerCell.Components;
using Content.Shared.Tag;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared._TacticalOps.Robotics;

public sealed class TacticalBotBatterySystem : EntitySystem
{
    private static readonly LocId AnnounceLinkedLoc = "tactical-bot-announce-linked";
    private static readonly LocId AnnounceNoPowerLoc = "tactical-bot-announce-no-power";
    private static readonly LocId AnnounceCriticalDamageLoc = "tactical-bot-announce-critical-damage";
    private const float LargeXenoMassThreshold = 150f;

    [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedSentryLaptopSystem _sentryLaptop = default!;
    [Dependency] private readonly SkillsSystem _skills = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedToolSystem _tool = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<TacticalBotBatteryComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<TacticalBotBatteryComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<TacticalBotBatteryComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeed);
        SubscribeLocalEvent<TacticalBotBatteryComponent, PowerCellChangedEvent>(OnPowerCellChanged);
        SubscribeLocalEvent<TacticalBotBatteryComponent, DamageChangedEvent>(OnDamageChanged);
        SubscribeLocalEvent<TacticalBotBatteryComponent, StartCollideEvent>(OnStartCollide);
    }

    // Allows only large xenomorphs and the Queen (mass >= 150) to shove robotic units aside on collision.
    private void OnStartCollide(Entity<TacticalBotBatteryComponent> ent, ref StartCollideEvent args)
    {
        if (!_net.IsServer || TerminatingOrDeleted(ent.Owner))
            return;

        var other = args.OtherEntity;
        if (!HasComp<XenoComponent>(other) || args.OtherBody.FixturesMass < LargeXenoMassThreshold)
            return;

        var botXform = Transform(ent.Owner);
        var botPos = _transform.GetWorldPosition(botXform);
        var xenoPos = _transform.GetWorldPosition(other);
        var pushDir = botPos - xenoPos;

        if (pushDir.LengthSquared() < 0.001f)
            return;

        pushDir = Vector2.Normalize(pushDir);
        var targetPos = botPos + pushDir * 0.65f;
        var originMap = new MapCoordinates(botPos, botXform.MapID);
        var targetMap = new MapCoordinates(targetPos, botXform.MapID);

        if (_examine.InRangeUnOccluded(originMap, targetMap, 0.8f, e => e == ent.Owner || e == other))
            _transform.SetWorldPosition(ent.Owner, targetPos);
    }

    private void OnPowerCellChanged(Entity<TacticalBotBatteryComponent> ent, ref PowerCellChangedEvent args)
    {
        if (!args.Ejected)
            return;

        var wasIdle = ent.Comp.IsIdle;
        SetIdle(ent, true);

        if (_net.IsServer && !wasIdle && ent.Comp.Master is { } master)
        {
            var ev = new TacticalBotAnnouncementEvent(ent.Owner, master, AnnounceNoPowerLoc);
            RaiseLocalEvent(ent.Owner, ref ev);
        }
    }

    private void OnDamageChanged(Entity<TacticalBotBatteryComponent> ent, ref DamageChangedEvent args)
    {
        if (!_net.IsServer || ent.Comp.Destroying || TerminatingOrDeleted(ent.Owner))
            return;

        var totalDamage = args.Damageable.TotalDamage.Float();
        if (totalDamage >= ent.Comp.MaxHealth)
        {
            ent.Comp.Destroying = true;
            var coords = Transform(ent.Owner).Coordinates;

            _audio.PlayPvs(ent.Comp.DestroySound, coords);

            if (ent.Comp.ScrapMaterialProto is { } scrapProto)
                Spawn(scrapProto, coords);

            var boardToSpawn = totalDamage >= ent.Comp.OverkillDamageThreshold
                ? ent.Comp.DestroyedBoardProto
                : ent.Comp.IntactBoardProto;

            if (boardToSpawn is { } boardProto)
                Spawn(boardProto, coords);

            QueueDel(ent.Owner);
            return;
        }

        if (!args.DamageIncreased || ent.Comp.Master is not { } master)
            return;

        var remainingFraction = (ent.Comp.MaxHealth - totalDamage) / ent.Comp.MaxHealth;
        var now = _timing.CurTime;
        if (remainingFraction <= ent.Comp.CriticalHealthFraction && now >= ent.Comp.NextCriticalAlert)
        {
            ent.Comp.NextCriticalAlert = now + ent.Comp.AlertCooldown;
            var ev = new TacticalBotAnnouncementEvent(ent.Owner, master, AnnounceCriticalDamageLoc);
            RaiseLocalEvent(ent.Owner, ref ev);
        }
    }

    private void OnInteractUsing(Entity<TacticalBotBatteryComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (TryComp<SentryLaptopComponent>(args.Used, out var laptop))
        {
            args.Handled = true;
            if (!_skills.HasSkill(args.User, ent.Comp.RequiredSkill, ent.Comp.RequiredSkillLevel))
            {
                _popup.PopupClient(Loc.GetString("tactical-bot-battery-no-skill"), ent, args.User, PopupType.SmallCaution);
                return;
            }

            if (_net.IsServer)
            {
                ent.Comp.Master = args.User;
                Dirty(ent);

                if (_sentryLaptop.TryLinkBotToLaptop((args.Used, laptop), ent.Owner, args.User))
                {
                    var ev = new TacticalBotAnnouncementEvent(ent.Owner, args.User, AnnounceLinkedLoc);
                    RaiseLocalEvent(ent.Owner, ref ev);
                }
            }

            return;
        }

        if (HasComp<IdCardComponent>(args.Used))
        {
            args.Handled = true;
            if (!_skills.HasSkill(args.User, ent.Comp.RequiredSkill, ent.Comp.RequiredSkillLevel))
            {
                _popup.PopupClient(Loc.GetString("tactical-bot-battery-no-skill"), ent, args.User, PopupType.SmallCaution);
                return;
            }

            ent.Comp.Master = args.User;
            ent.Comp.Unlocked = !ent.Comp.Unlocked;
            Dirty(ent);

            var msgKey = ent.Comp.Unlocked ? "tactical-bot-battery-unlocked" : "tactical-bot-battery-locked";
            _popup.PopupClient(Loc.GetString(msgKey), ent, args.User, PopupType.Medium);
            return;
        }

        if (_tool.HasQuality(args.Used, ent.Comp.PryQuality))
        {
            args.Handled = true;
            if (!ent.Comp.Unlocked)
            {
                _popup.PopupClient(Loc.GetString("tactical-bot-battery-need-id"), ent, args.User, PopupType.SmallCaution);
                return;
            }

            if (!_container.TryGetContainer(ent, ent.Comp.SlotId, out var container) ||
                container.ContainedEntities.Count == 0)
            {
                _popup.PopupClient(Loc.GetString("tactical-bot-battery-empty"), ent, args.User, PopupType.Small);
                return;
            }

            if (_net.IsServer)
            {
                var batteryUid = container.ContainedEntities[0];
                _itemSlots.SetLock(ent, ent.Comp.SlotId, false);
                if (_container.Remove(batteryUid, container))
                {
                    _hands.TryPickupAnyHand(args.User, batteryUid);
                }
                _itemSlots.SetLock(ent, ent.Comp.SlotId, true);
                SetIdle(ent, true);
            }

            _popup.PopupClient(Loc.GetString("tactical-bot-battery-ejected"), ent, args.User, PopupType.Medium);
            return;
        }

        if (_tag.HasTag(args.Used, ent.Comp.PowerCellTag))
        {
            args.Handled = true;
            if (!ent.Comp.Unlocked)
            {
                _popup.PopupClient(Loc.GetString("tactical-bot-battery-need-id"), ent, args.User, PopupType.SmallCaution);
                return;
            }

            if (!_container.TryGetContainer(ent, ent.Comp.SlotId, out var container) ||
                container.ContainedEntities.Count > 0)
            {
                return;
            }

            if (_net.IsServer)
            {
                _itemSlots.SetLock(ent, ent.Comp.SlotId, false);
                _container.Insert(args.Used, container);
                _itemSlots.SetLock(ent, ent.Comp.SlotId, true);
                SetIdle(ent, ent.Comp.RemoteDisabled);
            }

            _popup.PopupClient(Loc.GetString("tactical-bot-battery-inserted"), ent, args.User, PopupType.Medium);
        }
    }

    private void OnExamined(Entity<TacticalBotBatteryComponent> ent, ref ExaminedEvent args)
    {
        using (args.PushGroup(nameof(TacticalBotBatteryComponent)))
        {
            if (TryComp<DamageableComponent>(ent, out var damageable))
            {
                var dmg = damageable.TotalDamage.Float();
                if (dmg >= ent.Comp.MaxHealth * 0.5f)
                    args.PushMarkup(Loc.GetString("tactical-bot-examine-damage-severe"));
                else if (dmg > 0f)
                    args.PushMarkup(Loc.GetString("tactical-bot-examine-damage-minor"));
            }

            if (ent.Comp.IsIdle)
                args.PushMarkup(Loc.GetString("tactical-bot-battery-examine-idle"));

            if (ent.Comp.Unlocked)
                args.PushMarkup(Loc.GetString("tactical-bot-battery-examine-unlocked"));
        }
    }

    private void OnRefreshMovementSpeed(Entity<TacticalBotBatteryComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        if (ent.Comp.IsIdle)
            args.ModifySpeed(0f, 0f);
    }

    public bool HasPowerCell(Entity<TacticalBotBatteryComponent> ent)
    {
        return _container.TryGetContainer(ent, ent.Comp.SlotId, out var container) &&
               container.ContainedEntities.Count > 0;
    }

    public void TryToggleRemotePower(Entity<TacticalBotBatteryComponent> ent)
    {
        if (!HasPowerCell(ent))
        {
            ent.Comp.RemoteDisabled = true;
            SetIdle(ent, true);
            return;
        }

        ent.Comp.RemoteDisabled = !ent.Comp.RemoteDisabled;
        Dirty(ent);
        SetIdle(ent, ent.Comp.RemoteDisabled);
    }

    public void SetRemotePower(Entity<TacticalBotBatteryComponent> ent, bool powerOn)
    {
        if (!HasPowerCell(ent))
        {
            ent.Comp.RemoteDisabled = true;
            SetIdle(ent, true);
            return;
        }

        ent.Comp.RemoteDisabled = !powerOn;
        Dirty(ent);
        SetIdle(ent, ent.Comp.RemoteDisabled);
    }

    public void SetIdle(Entity<TacticalBotBatteryComponent> ent, bool isIdle)
    {
        if (ent.Comp.IsIdle == isIdle)
            return;

        ent.Comp.IsIdle = isIdle;
        Dirty(ent);
        _movementSpeed.RefreshMovementSpeedModifiers(ent);
        _actionBlocker.UpdateCanMove(ent.Owner);
    }

    public void SetMaster(Entity<TacticalBotBatteryComponent> ent, EntityUid? master)
    {
        if (ent.Comp.Master == master)
            return;

        ent.Comp.Master = master;
        Dirty(ent);
    }
}