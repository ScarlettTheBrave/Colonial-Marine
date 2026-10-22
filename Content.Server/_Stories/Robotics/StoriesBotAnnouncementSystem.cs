using System.Collections.Generic;
using Content.Server._RMC14.Announce;
using Content.Shared._RMC14.Announce;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._Stories.Robotics;
using Content.Shared.Mobs.Systems;
using Robust.Server.GameStates;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._Stories.Robotics;

public sealed class StoriesBotAnnouncementSystem : EntitySystem
{
    private static readonly ProtoId<AnnouncementPresetPrototype> PresetEngineeringBot = "STEngineeringBot";
    private static readonly SoundPathSpecifier DefaultBeepSound = new("/Audio/Machines/twobeep.ogg");

    [Dependency] private readonly AnnouncementRouterSystem _announce = default!;
    [Dependency] private readonly StoriesBotBatterySystem _botBattery = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly PvsOverrideSystem _pvsOverride = default!;
    [Dependency] private readonly SkillsSystem _skills = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private readonly HashSet<EntityUid> _nearby = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<StoriesBotBatteryComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<StoriesBotBatteryComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<StoriesBotBatteryComponent, StoriesBotAnnouncementEvent>(OnBotAnnouncement);
    }

    private void OnMapInit(Entity<StoriesBotBatteryComponent> ent, ref MapInitEvent args)
    {
        _pvsOverride.AddGlobalOverride(ent.Owner);
        TryAssignNearestEngineer(ent);
    }

    private void OnShutdown(Entity<StoriesBotBatteryComponent> ent, ref ComponentShutdown args)
    {
        _pvsOverride.RemoveGlobalOverride(ent.Owner);
    }

    public void TryAssignNearestEngineer(Entity<StoriesBotBatteryComponent> ent)
    {
        if (ent.Comp.Master is { } current && !TerminatingOrDeleted(current) && _mobState.IsAlive(current))
            return;

        _nearby.Clear();
        _lookup.GetEntitiesInRange(ent.Owner, 6.0f, _nearby, LookupFlags.Uncontained);

        var botPos = _transform.GetWorldPosition(ent.Owner);
        var bestDistSq = float.MaxValue;
        EntityUid? bestMaster = null;

        foreach (var candidate in _nearby)
        {
            if (!HasComp<MarineComponent>(candidate) || !_mobState.IsAlive(candidate))
                continue;

            if (!_skills.HasSkill(candidate, ent.Comp.RequiredSkill, ent.Comp.RequiredSkillLevel))
                continue;

            var distSq = (_transform.GetWorldPosition(candidate) - botPos).LengthSquared();
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                bestMaster = candidate;
            }
        }

        if (bestMaster != null)
            _botBattery.SetMaster(ent, bestMaster);
    }

    private void OnBotAnnouncement(Entity<StoriesBotBatteryComponent> ent, ref StoriesBotAnnouncementEvent args)
    {
        var recipient = args.Recipient;
        if (TerminatingOrDeleted(recipient))
            return;

        if (!_skills.HasSkill(recipient, ent.Comp.RequiredSkill, ent.Comp.RequiredSkillLevel))
            return;

        if (!TryComp<ActorComponent>(recipient, out var actor))
            return;

        var botName = Name(args.Bot);
        var message = Loc.GetString(args.MessageLoc, ("bot", botName));

        var request = new AnnouncementRequest
        {
            Message = message,
            Preset = PresetEngineeringBot,
            PriorityOverride = 1f,
            CanInterrupt = false,
            CanBeInterrupted = true,
            Route = new AnnouncementRoute
            {
                Target = AnnouncementTarget.Marines,
                Source = args.Bot,
                Speaker = args.Bot,
                SpeakerNameOverride = botName,
                Channels = AnnouncementChannels.Overlay | AnnouncementChannels.Sound,
            },
            Sound = new AnnouncementSoundOptions
            {
                Sound = DefaultBeepSound,
            },
        };

        _announce.Announce(request, Filter.SinglePlayer(actor.PlayerSession));
    }
}