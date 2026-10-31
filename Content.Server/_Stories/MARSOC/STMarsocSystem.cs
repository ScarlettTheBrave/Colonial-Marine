using System.Globalization;
using System.Numerics;
using System.Text;
using Content.Server._RMC14.Dropship;
using Content.Server._RMC14.Marines;
using Content.Server._RMC14.Rules.DistressSignal;
using Content.Server._RMC14.Shuttles;
using Content.Server.Actions;
using Content.Server.Chat.Managers;
using Content.Server.Destructible;
using Content.Server.GameTicking;
using Content.Server.Ghost.Roles.Events;
using Content.Server.Mind;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Doors;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.ParaDrop;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Roles;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Survivor;
using Content.Shared._RMC14.Synth;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._Stories.Marsoc;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Humanoid;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Paper;
using Content.Shared.Parallax;
using Content.Shared.Power;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Server.Roles;
using Content.Shared.Shuttles.Components;
using Robust.Shared.Containers;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Stories.Marsoc;

public sealed class STMarsocSystem : SharedSTMarsocSystem
{
    [Dependency] private readonly AccessReaderSystem _accessReader = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly BatterySystem _battery = default!;
    [Dependency] private readonly RMCCameraShakeSystem _cameraShake = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly CMDoorSystem _cmDoor = default!;
    [Dependency] private readonly IComponentFactory _compFactory = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedDoorSystem _door = default!;
    [Dependency] private readonly DropshipSystem _dropship = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedIdCardSystem _idCard = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly MarineAnnounceSystem _marineAnnounce = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly PaperSystem _paper = default!;
    [Dependency] private readonly SharedParaDropSystem _paraDrop = default!;
    [Dependency] private readonly PlayTimeTrackingSystem _playTime = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedRoleSystem _roles = default!;
    [Dependency] private readonly ShuttleSystem _shuttle = default!;
    [Dependency] private readonly StationSpawningSystem _stationSpawning = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDef = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private static readonly ProtoId<DamageTypePrototype> BluntDamageType = "Blunt";
    private static readonly ProtoId<JobPrototype> MarsocOfficerJob = "STMarsocOfficer";
    private static readonly EntProtoId DropZoneMarkerProto = "STMarsocDropZoneMarker";
    private static readonly EntProtoId OfficerSpawnerProto = "STSpawnerMarsocOfficer";
    private static readonly EntProtoId WeYaPmcDeathSquadSpawnerProto = "RMCRandomHumanoidCorporateSecurity";
    private static readonly ResPath MarsocShuttleMapPath = new("/Maps/_Stories/Shuttles/st_marsoc_shuttle.yml");
    private static readonly ResPath FallbackShuttleMapPath = new("/Maps/_RMC14/Shuttles/ert_shuttle_big.yml");

    private static readonly EntProtoId[] CorporateCaseLootProtos =
    {
        "STMarsocLoreReport1",
        "STMarsocLoreReport2",
        "STMarsocLoreReport3",
        "STMarsocLoreReport4",
        "STMarsocWeYaBlackLedger",
        "STMarsocWeYaResearchData",
        "STMarsocWeYaCommandProtocols",
        "STMarsocAbominationFlask"
    };

    private static readonly LocId[] ParamarineObjectivePool =
    {
        "st-marsoc-para-objective-1",
        "st-marsoc-para-objective-2",
        "st-marsoc-para-objective-3"
    };

    private static readonly LocId ParadropEnabledLoc = "st-marsoc-paradrop-enabled";
    private static readonly LocId ParadropDisabledLoc = "st-marsoc-paradrop-disabled";
    private static readonly LocId NoActiveBeaconLoc = "st-marsoc-beacon-none-active";
    private static readonly LocId CannotDeployOnShuttleLoc = "st-marsoc-beacon-on-shuttle-error";
    private static readonly LocId CannotDeployUnderRoofLoc = "st-marsoc-beacon-under-roof-error";
    private static readonly LocId ExtractionAbortedNoMarsocLoc = "st-marsoc-beacon-aborted-no-operative";
    private static readonly LocId DefaultOperationNameLoc = "st-marsoc-banner-operation-default";
    private static readonly LocId DefaultAssignmentLoc = "st-marsoc-banner-assignment";
    private static readonly LocId RoleWelcomeChatFormatLoc = "st-marsoc-role-welcome-chat";

    private static readonly (LocId NameLoc, Vector2 ViewerCoords)[] HybrisaDropZones =
    {
        (new LocId("st-marsoc-sector-hybrisa-nw-checkpoint"), new Vector2(25f, 189f)),
        (new LocId("st-marsoc-sector-hybrisa-nw-lot"), new Vector2(56f, 230f)),
        (new LocId("st-marsoc-sector-hybrisa-ne-streets"), new Vector2(188f, 236f))
    };

    private static readonly LocId[] SectorCardinalNames =
    {
        "st-marsoc-sector-north",
        "st-marsoc-sector-south",
        "st-marsoc-sector-west",
        "st-marsoc-sector-east"
    };

    private const int LoreYear = 2126;
    private const float MinDistanceFromSurvivors = 24f;
    private const float BunkerPowerRadius = 18f;

    // Константы шкалы обнаружения (Exposure System)
    private const float ExposureXenoTier1 = 2.5f;
    private const float ExposureXenoTier2 = 3.0f;
    private const float ExposureXenoTier3 = 5.0f;
    private const float ExposureMarineBulletHit = 0.2f;
    private const float ExposureMarineKill = 4.0f;
    private const float ExposureMarsocDeath = 20.0f;
    private const float ExposureMarineLosTick = 1.5f;
    private const float ExposureLosRange = 10.0f;
    private const float ExposureWarningThreshold = 75.0f;
    private const float ExposureMaxThreshold = 100.0f;

    /// <summary>
    /// В тестовом сценарии true = 100% спавн MARSOC. Для продакшена поставить false (1%).
    /// </summary>
    private const bool TestModeAlwaysSpawnMarsoc = true;
    private const float ProductionMarsocSpawnChance = 0.01f;

    private readonly List<string> _availableCallsigns = new();
    private readonly List<EntityUid> _synthEligibleSpawners = new();
    private readonly List<EntityCoordinates> _troopCoordsList = new();
    private readonly List<(string Name, EntityCoordinates Coords)> _planetSectors = new();
    private readonly List<(int Order, string Name, EntityCoordinates Coords)> _mappedMarkers = new();
    private readonly List<EntityUid> _spawnedDropMarkers = new();
    private readonly List<Vector2> _survivorPositions = new();
    private readonly List<EntityUid> _exteriorStepCandidates = new();
    private readonly List<EntityCoordinates> _bunkerAnchors = new();
    private readonly Dictionary<EntityUid, string> _assignedVipBriefings = new();
    private readonly Dictionary<EntityUid, int> _consecutiveLosTicks = new();
    private readonly HashSet<string> _roundUsedCallsigns = new();
    private readonly HashSet<EntityUid> _extractionTargets = new();
    private readonly HashSet<EntityUid> _bunkerControlledDoors = new();
    private readonly HashSet<EntityUid> _bunkerPoweredApcs = new();
    private readonly HashSet<EntityUid> _bunkerNearbyEntities = new();
    private readonly HashSet<EntityUid> _losNearbyEntities = new();
    private readonly HashSet<string> _bunkerLinkedIds = new(StringComparer.OrdinalIgnoreCase);

    private bool _roundSynthAssigned;
    private int _selectedDropSectorIndex;
    private bool _dropSectorsGenerated;
    private bool _marsocRoundRolled;
    private bool _marsocActiveThisRound;

    private bool _bunkerInitialized;
    private bool _bunkerCountdownStarted;
    private bool _bunkerShuttersPermanentlyOpen;
    private TimeSpan? _bunkerScheduledOpenTime;
    private TimeSpan _nextBunkerPowerRefresh;
    private LocId? _activeParamarineObjective;

    private float _exposureLevel;
    private bool _exposure75Announced;
    private bool _exposure100Triggered;
    private TimeSpan _nextExposureLosCheck;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundCleanup);
        SubscribeLocalEvent<RulePlayerSpawningEvent>(OnRulePlayerSpawning, before: [typeof(CMDistressSignalRuleSystem)]);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnAnyPlayerSpawnComplete);
        SubscribeLocalEvent<HumanoidAppearanceComponent, PlayerAttachedEvent>(OnHumanoidPlayerAttached);

        SubscribeLocalEvent<STMarsocDropZoneComponent, MapInitEvent>(OnDropZoneMarkerMapInit);

        SubscribeLocalEvent<STMarsocOfficerComponent, MapInitEvent>(OnOfficerMapInit);
        SubscribeLocalEvent<STMarsocOfficerComponent, ComponentShutdown>(OnOfficerShutdown);
        SubscribeLocalEvent<STMarsocOfficerComponent, STMarsocDeploySquadActionEvent>(OnDeploySquad);
        SubscribeLocalEvent<STMarsocOfficerComponent, STMarsocToggleParadropActionEvent>(OnToggleParadrop);

        SubscribeLocalEvent<STMarsocPendingSpawnComponent, GhostRoleSpawnerUsedEvent>(OnGhostSpawnerUsed);
        SubscribeLocalEvent<STMarsocOperativeComponent, STMarsocConfirmExtractionActionEvent>(OnConfirmExtraction);
        SubscribeLocalEvent<STMarsocOperativeComponent, MobStateChangedEvent>(OnOperativeMobStateChanged);

        SubscribeLocalEvent<STMarsocExtractionBeaconComponent, UseInHandEvent>(OnBeaconUseInHand);
        SubscribeLocalEvent<STMarsocExtractionBeaconComponent, ComponentShutdown>(OnBeaconShutdown);

        SubscribeLocalEvent<STMarsocBunkerButtonComponent, ActivateInWorldEvent>(OnBunkerDoorButtonActivate, before: [typeof(CMDoorSystem)]);
        SubscribeLocalEvent<STMarsocCryoPodObjectiveComponent, ActivateInWorldEvent>(OnCryoPodActivate, before: [typeof(CMDoorSystem)]);
        SubscribeLocalEvent<STMarsocCryoPodObjectiveComponent, InteractHandEvent>(OnCryoPodInteractHand);
        SubscribeLocalEvent<STMarsocCorporateCaseComponent, ActivateInWorldEvent>(OnCorporateCaseActivate, before: [typeof(CMDoorSystem)]);
        SubscribeLocalEvent<STMarsocCorporateCaseComponent, InteractHandEvent>(OnCorporateCaseInteractHand);
        SubscribeLocalEvent<STMarsocCorporateCaseComponent, UseInHandEvent>(OnCorporateCaseUseInHand);
        SubscribeLocalEvent<STMarsocCommandProtocolsComponent, UseInHandEvent>(OnCommandProtocolsUseInHand);

        SubscribeLocalEvent<STMarsocTrackedTargetComponent, DamageChangedEvent>(OnTrackedTargetDamageChanged);
        SubscribeLocalEvent<STMarsocTrackedTargetComponent, MobStateChangedEvent>(OnTrackedTargetMobStateChanged);
    }

    private void OnRoundCleanup(RoundRestartCleanupEvent ev)
    {
        _roundUsedCallsigns.Clear();
        _planetSectors.Clear();
        _mappedMarkers.Clear();
        _spawnedDropMarkers.Clear();
        _exteriorStepCandidates.Clear();
        _bunkerAnchors.Clear();
        _bunkerControlledDoors.Clear();
        _bunkerPoweredApcs.Clear();
        _bunkerNearbyEntities.Clear();
        _losNearbyEntities.Clear();
        _bunkerLinkedIds.Clear();
        _assignedVipBriefings.Clear();
        _consecutiveLosTicks.Clear();

        _roundSynthAssigned = false;
        _dropSectorsGenerated = false;
        _selectedDropSectorIndex = 0;
        _marsocRoundRolled = false;
        _marsocActiveThisRound = false;

        _bunkerInitialized = false;
        _bunkerCountdownStarted = false;
        _bunkerShuttersPermanentlyOpen = false;
        _bunkerScheduledOpenTime = null;
        _nextBunkerPowerRefresh = TimeSpan.Zero;
        _activeParamarineObjective = null;

        _exposureLevel = 0f;
        _exposure75Announced = false;
        _exposure100Triggered = false;
        _nextExposureLosCheck = TimeSpan.Zero;
    }

    public string GetExposurePercentText()
    {
        return Loc.GetString("st-marsoc-exposure-console-status", ("percent", $"{_exposureLevel:0.0}"));
    }

    private void OnRulePlayerSpawning(RulePlayerSpawningEvent ev)
    {
        if (_marsocRoundRolled)
            return;

        _marsocRoundRolled = true;
        _activeParamarineObjective = _random.Pick(ParamarineObjectivePool);

        var chance = TestModeAlwaysSpawnMarsoc ? 1.0f : ProductionMarsocSpawnChance;
        if (!_random.Prob(chance))
            return;

        _marsocActiveThisRound = true;
        TrySpawnMarsocShuttleAndRoundstartOfficer(ev);
    }

    private void TrySpawnMarsocShuttleAndRoundstartOfficer(RulePlayerSpawningEvent ev)
    {
        if (EntityQueryEnumerator<STMarsocOfficerComponent>().MoveNext(out _, out _))
            return;

        _map.CreateMap(out var shuttleMapId);
        if (!_mapLoader.TryLoadGrid(shuttleMapId, MarsocShuttleMapPath, out var grid) &&
            !_mapLoader.TryLoadGrid(shuttleMapId, FallbackShuttleMapPath, out grid))
        {
            return;
        }

        var shuttleGrid = grid.Value.Owner;
        RemComp<DropshipComponent>(shuttleGrid);

        EntityCoordinates? spawnCoords = null;
        var query = EntityQueryEnumerator<MetaDataComponent, TransformComponent>();
        while (query.MoveNext(out _, out var meta, out var xform))
        {
            if (xform.GridUid != shuttleGrid || meta.EntityPrototype is not { } proto)
                continue;

            if (proto.ID == "RMCERTSpawnMarkerLeader" || proto.ID == "RMCERTSpawnMarker")
            {
                spawnCoords = xform.Coordinates;
                break;
            }
        }

        var finalCoords = spawnCoords ?? new EntityCoordinates(shuttleGrid, Vector2.Zero);

        ICommonSession? selectedPlayer = null;
        var bestPriority = JobPriority.Never;

        foreach (var session in ev.PlayerPool)
        {
            if (!ev.Profiles.TryGetValue(session.UserId, out var profile))
                continue;

            if (!profile.JobPriorities.TryGetValue(MarsocOfficerJob, out var prio) || prio <= bestPriority)
                continue;

            if (!IsOfficerCandidateEligible(session))
                continue;

            bestPriority = prio;
            selectedPlayer = session;
        }

        if (selectedPlayer != null && ev.Profiles.TryGetValue(selectedPlayer.UserId, out var chosenProfile))
        {
            ev.PlayerPool.Remove(selectedPlayer);
            _gameTicker.PlayerJoinGame(selectedPlayer);

            var officerMob = _stationSpawning.SpawnPlayerMob(finalCoords, MarsocOfficerJob, chosenProfile, null);
            if (!_mind.TryGetMind(selectedPlayer.UserId, out var mind))
                mind = _mind.CreateMind(selectedPlayer.UserId);

            _mind.TransferTo(mind.Value, officerMob);
            _roles.MindAddJobRole(mind.Value, jobPrototype: MarsocOfficerJob);
            _playTime.PlayerRolesChanged(selectedPlayer);

            RaiseLocalEvent(officerMob, new PlayerSpawnCompleteEvent(
                officerMob,
                selectedPlayer,
                MarsocOfficerJob,
                false,
                true,
                0,
                default,
                chosenProfile), true);

            return;
        }

        if (_prototypes.HasIndex(OfficerSpawnerProto))
            Spawn(OfficerSpawnerProto, finalCoords);
    }

    private bool IsOfficerCandidateEligible(ICommonSession session)
    {
        if (!_prototypes.TryIndex(MarsocOfficerJob, out _))
            return false;

        if (!TestModeAlwaysSpawnMarsoc && !_playTime.IsAllowed(session, MarsocOfficerJob))
            return false;

        return true;
    }

    private void OnAnyPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
    {
        if (HasComp<MarineComponent>(ev.Mob) || HasComp<XenoComponent>(ev.Mob))
            EnsureComp<STMarsocTrackedTargetComponent>(ev.Mob);

        if (string.IsNullOrEmpty(ev.JobId))
            return;

        var jobId = ev.JobId;

        if (jobId.Contains("Xenoarchaeologist", StringComparison.OrdinalIgnoreCase))
        {
            if (_prototypes.HasIndex("STMarsocExcavationJournal"))
            {
                var journal = Spawn("STMarsocExcavationJournal", Transform(ev.Mob).Coordinates);
                ApplyWeYaStamp(journal);
                _hands.TryPickupAnyHand(ev.Mob, journal);
            }
        }

        if (_marsocActiveThisRound &&
            jobId.Contains("Para", StringComparison.OrdinalIgnoreCase) &&
            !jobId.Contains("Paramedic", StringComparison.OrdinalIgnoreCase))
        {
            AssignParamarineBriefing(ev.Mob, ev.Player, jobId);
        }
    }

    private void AssignParamarineBriefing(EntityUid mobUid, ICommonSession session, string? jobId = null)
    {
        _activeParamarineObjective ??= _random.Pick(ParamarineObjectivePool);
        var objText = Loc.GetString(_activeParamarineObjective.Value);
        var msg = Loc.GetString("st-marsoc-para-briefing-chat", ("objective", FormattedMessage.EscapeText(objText)));

        ProtoId<JobPrototype>? fallbackJob = null;
        if (!string.IsNullOrEmpty(jobId))
            fallbackJob = new ProtoId<JobPrototype>(jobId);

        SetPlayerCharacterBriefing(mobUid, msg, fallbackJob);

        _chatManager.ChatMessageToOne(ChatChannel.Server, msg, msg, EntityUid.Invalid, false, session.Channel);
    }

    private void OnHumanoidPlayerAttached(Entity<HumanoidAppearanceComponent> ent, ref PlayerAttachedEvent args)
    {
        if (TryComp<STMarsocOperativeComponent>(ent.Owner, out var operative))
        {
            EnforceCallsignName((ent.Owner, operative));
            SendDeploymentBannerToSession(args.Player, null);
            SendRoleDutiesToChat((ent.Owner, operative), args.Player);
            UpdateOperativeCharacterBriefing(ent.Owner);
            return;
        }

        if (HasComp<MarineComponent>(ent.Owner))
            EnsureComp<STMarsocTrackedTargetComponent>(ent.Owner);

        if (_marsocActiveThisRound &&
            TryComp<OriginalRoleComponent>(ent.Owner, out var origRole) &&
            origRole.Job is { } jobProto &&
            jobProto.Id.Contains("Para", StringComparison.OrdinalIgnoreCase) &&
            !jobProto.Id.Contains("Paramedic", StringComparison.OrdinalIgnoreCase))
        {
            AssignParamarineBriefing(ent.Owner, args.Player, jobProto.Id);
            return;
        }

        TryAssignCorporateVipBriefing(ent.Owner, args.Player);
    }

    private void TryAssignCorporateVipBriefing(EntityUid mobUid, ICommonSession session)
    {
        var roleId = ResolveCorporateVipRoleId(mobUid);
        if (roleId == null)
            return;

        if (!_assignedVipBriefings.TryGetValue(mobUid, out var briefingText))
        {
            if (roleId == "CMCorporateExecutiveSupervisor")
            {
                if (_prototypes.HasIndex("CMStampWeYa"))
                {
                    var stamp = Spawn("CMStampWeYa", Transform(mobUid).Coordinates);
                    if (!_inventory.TryEquip(mobUid, stamp, "pocket1", true, false))
                        _hands.TryPickupAnyHand(mobUid, stamp);
                }

                var chosenLoc = PickSupervisorObjectiveBasedOnWorld();
                briefingText = Loc.GetString(chosenLoc);
            }
            else
            {
                var chosenLoc = _random.Prob(0.5f)
                    ? new LocId("st-marsoc-assistant-obj-help")
                    : new LocId("st-marsoc-assistant-obj-kill");

                briefingText = Loc.GetString(chosenLoc);
            }

            _assignedVipBriefings[mobUid] = briefingText;
        }

        SetPlayerCharacterBriefing(mobUid, briefingText, roleId);

        var safeMsg = FormattedMessage.EscapeText(briefingText);
        _chatManager.ChatMessageToOne(ChatChannel.Server, safeMsg, safeMsg, EntityUid.Invalid, false, session.Channel);
    }

    private string? ResolveCorporateVipRoleId(EntityUid mobUid)
    {
        if (TryComp<OriginalRoleComponent>(mobUid, out var origRole) && origRole.Job is { } jobProto)
        {
            if (jobProto.Id == "CMCorporateExecutiveSupervisor" || jobProto.Id == "CMCorporateAssistantManager")
                return jobProto.Id;
        }

        if (_inventory.TryGetSlotEntity(mobUid, "jumpsuit", out var suitUid))
        {
            var suitProto = MetaData(suitUid.Value).EntityPrototype?.ID;
            if (suitProto == "CMJumpsuitLiaisonIvy")
                return "CMCorporateExecutiveSupervisor";
            if (suitProto == "RMCJumpsuitLiaisonManager")
                return "CMCorporateAssistantManager";
        }

        return null;
    }

    private LocId PickSupervisorObjectiveBasedOnWorld()
    {
        var available = new List<LocId>(4);

        var caseExists = EntityQueryEnumerator<STMarsocCorporateCaseComponent>().MoveNext(out _, out _);
        var cryoExists = EntityQueryEnumerator<STMarsocCryoPodObjectiveComponent>().MoveNext(out _, out _);

        if (caseExists)
        {
            available.Add("st-marsoc-supervisor-obj-ledger");
            available.Add("st-marsoc-supervisor-obj-flask");
        }

        if (cryoExists)
            available.Add("st-marsoc-supervisor-obj-research");

        available.Add("st-marsoc-supervisor-obj-journal");

        return _random.Pick(available);
    }

    public void RefreshAllMarsocCharacterBriefings()
    {
        var query = EntityQueryEnumerator<STMarsocOperativeComponent>();
        while (query.MoveNext(out var uid, out _))
        {
            UpdateOperativeCharacterBriefing(uid);
        }
    }

    public void UpdateOperativeCharacterBriefing(EntityUid operativeUid)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Loc.GetString("st-marsoc-charinfo-header"));

        var foundConsole = false;
        var consoleQuery = EntityQueryEnumerator<STMarsocBriefingConsoleComponent>();
        while (consoleQuery.MoveNext(out _, out var console))
        {
            if (console.GeneratedObjectives.Count == 0)
                continue;

            foundConsole = true;
            foreach (var objLoc in console.GeneratedObjectives)
            {
                sb.AppendLine($"- {Loc.GetString(objLoc)}");
            }
            break;
        }

        if (!foundConsole)
            sb.AppendLine($"- {Loc.GetString("st-marsoc-charinfo-default-obj")}");

        SetPlayerCharacterBriefing(operativeUid, sb.ToString().TrimEnd());
    }

    public void SetPlayerCharacterBriefing(EntityUid mobUid, string briefingText, ProtoId<JobPrototype>? fallbackJob = null)
    {
        if (!TryComp<MindContainerComponent>(mobUid, out var mindContainer) ||
            mindContainer.Mind is not { } mindId)
        {
            return;
        }

        var foundMindRole = false;
        var roleQuery = EntityQueryEnumerator<MindRoleComponent, TransformComponent>();
        while (roleQuery.MoveNext(out var roleUid, out _, out var roleXform))
        {
            if (roleXform.ParentUid != mindId)
                continue;

            if (!foundMindRole)
            {
                foundMindRole = true;
                var rb = EnsureComp<RoleBriefingComponent>(roleUid);
                rb.Briefing = briefingText;
                Dirty(roleUid, rb);
            }
        }

        if (!foundMindRole && fallbackJob is { } jobProto)
        {
            _roles.MindAddJobRole(mindId, jobPrototype: jobProto);
            var retryQuery = EntityQueryEnumerator<MindRoleComponent, TransformComponent>();
            while (retryQuery.MoveNext(out var roleUid, out _, out var roleXform))
            {
                if (roleXform.ParentUid != mindId)
                    continue;

                var rb = EnsureComp<RoleBriefingComponent>(roleUid);
                rb.Briefing = briefingText;
                Dirty(roleUid, rb);
                break;
            }
        }

        var mindBriefing = EnsureComp<RoleBriefingComponent>(mindId);
        mindBriefing.Briefing = briefingText;
        Dirty(mindId, mindBriefing);
    }

    private void OnOperativeMobStateChanged(Entity<STMarsocOperativeComponent> ent, ref MobStateChangedEvent args)
    {
        if (!_marsocActiveThisRound || _exposure100Triggered || args.NewMobState != MobState.Dead)
            return;

        AddExposure(ExposureMarsocDeath, Loc.GetString("st-marsoc-exposure-reason-operative-dead"));
    }

    private void OnTrackedTargetDamageChanged(Entity<STMarsocTrackedTargetComponent> ent, ref DamageChangedEvent args)
    {
        if (!_marsocActiveThisRound || _exposure100Triggered || !args.DamageIncreased)
            return;

        if (args.Origin is not { } attacker || !HasComp<STMarsocOperativeComponent>(attacker))
            return;

        if (ent.Owner == attacker || HasComp<STMarsocOperativeComponent>(ent.Owner))
            return;

        ent.Comp.LastMarsocAttacker = attacker;

        if (HasComp<MarineComponent>(ent.Owner) && !HasComp<RMCSurvivorComponent>(ent.Owner))
        {
            AddExposure(ExposureMarineBulletHit, null);
        }
    }

    private void OnTrackedTargetMobStateChanged(Entity<STMarsocTrackedTargetComponent> ent, ref MobStateChangedEvent args)
    {
        if (!_marsocActiveThisRound || _exposure100Triggered || args.NewMobState != MobState.Dead)
            return;

        var killedByMarsoc = (args.Origin is { } origin && HasComp<STMarsocOperativeComponent>(origin)) ||
                             ent.Comp.LastMarsocAttacker != null;

        if (!killedByMarsoc)
            return;

        if (TryComp<XenoComponent>(ent.Owner, out var xeno))
        {
            var delta = xeno.Tier switch
            {
                <= 1 => ExposureXenoTier1,
                2 => ExposureXenoTier2,
                _ => ExposureXenoTier3
            };

            AddExposure(delta, null);
            return;
        }

        if (HasComp<MarineComponent>(ent.Owner) && !HasComp<RMCSurvivorComponent>(ent.Owner))
        {
            AddExposure(ExposureMarineKill, Loc.GetString("st-marsoc-exposure-reason-marine-killed"));
        }
    }

    private void UpdateExposureLineOfSight(TimeSpan curTime)
    {
        if (!_marsocActiveThisRound || _exposure100Triggered || curTime < _nextExposureLosCheck)
            return;

        _nextExposureLosCheck = curTime + TimeSpan.FromSeconds(3);

        // Авто-привязка трекера к новым ксеноморфам и морпехам раз в 3 секунды
        var xenoQuery = EntityQueryEnumerator<XenoComponent>();
        while (xenoQuery.MoveNext(out var xenoUid, out _))
        {
            if (!HasComp<STMarsocTrackedTargetComponent>(xenoUid))
                EnsureComp<STMarsocTrackedTargetComponent>(xenoUid);
        }

        var marineQuery = EntityQueryEnumerator<MarineComponent>();
        while (marineQuery.MoveNext(out var marUid, out _))
        {
            if (!HasComp<STMarsocOperativeComponent>(marUid) && !HasComp<STMarsocTrackedTargetComponent>(marUid))
                EnsureComp<STMarsocTrackedTargetComponent>(marUid);
        }

        var opQuery = EntityQueryEnumerator<STMarsocOperativeComponent, TransformComponent>();
        while (opQuery.MoveNext(out var opUid, out var opComp, out var opXform))
        {
            if (_mobState.IsDead(opUid) ||
                opXform.GridUid is not { } gridUid ||
                !IsPlanetGrid(gridUid, opComp.HomeShuttleGrid))
            {
                _consecutiveLosTicks.Remove(opUid);
                continue;
            }

            _losNearbyEntities.Clear();
            _lookup.GetEntitiesInRange(opXform.Coordinates, ExposureLosRange, _losNearbyEntities, LookupFlags.Uncontained);

            var spottedByMarine = false;
            foreach (var other in _losNearbyEntities)
            {
                if (other == opUid ||
                    !HasComp<MarineComponent>(other) ||
                    HasComp<STMarsocOperativeComponent>(other) ||
                    HasComp<RMCSurvivorComponent>(other) ||
                    _mobState.IsDead(other))
                {
                    continue;
                }

                if (_examine.InRangeUnOccluded(opUid, other, ExposureLosRange))
                {
                    spottedByMarine = true;
                    break;
                }
            }

            if (!spottedByMarine)
            {
                _consecutiveLosTicks.Remove(opUid);
                continue;
            }

            var ticks = _consecutiveLosTicks.GetValueOrDefault(opUid) + 1;
            _consecutiveLosTicks[opUid] = ticks;

            if (ticks >= 2)
            {
                _popup.PopupEntity(Loc.GetString("st-marsoc-exposure-los-warning"), opUid, opUid);
                AddExposure(ExposureMarineLosTick, null);
            }
        }
    }

    public void AddExposure(float amount, string? reason)
    {
        if (_exposure100Triggered || amount <= 0f)
            return;

        _exposureLevel = Math.Clamp(_exposureLevel + amount, 0f, ExposureMaxThreshold);

        if (!string.IsNullOrWhiteSpace(reason))
        {
            var alertMsg = Loc.GetString("st-marsoc-exposure-squad-alert",
                ("reason", FormattedMessage.EscapeText(reason)),
                ("percent", $"{_exposureLevel:0.0}"));
            BroadcastToMarsocSquad(alertMsg);
        }

        if (!_exposure75Announced && _exposureLevel >= ExposureWarningThreshold && _exposureLevel < ExposureMaxThreshold)
        {
            _exposure75Announced = true;
            var aresWarn = Loc.GetString("st-marsoc-exposure-ares-75-warning");
            _marineAnnounce.AnnounceARESStaging(default, aresWarn);

            BroadcastToMarsocSquad(Loc.GetString("st-marsoc-exposure-squad-75-warning"));
        }

        if (!_exposure100Triggered && _exposureLevel >= ExposureMaxThreshold)
        {
            _exposure100Triggered = true;
            TriggerExposure100Compromise();
        }
    }

    private void TriggerExposure100Compromise()
    {
        var ares100 = Loc.GetString("st-marsoc-exposure-ares-100-compromised");
        _marineAnnounce.AnnounceARESStaging(default, ares100);

        BroadcastToMarsocSquad(Loc.GetString("st-marsoc-exposure-squad-100-compromised"));

        var officerQuery = EntityQueryEnumerator<STMarsocOfficerComponent, TransformComponent>();
        while (officerQuery.MoveNext(out _, out var officer, out var xform))
        {
            officer.ParadropActive = false;
            if (xform.GridUid is { } gridUid)
                SetShuttleParadropState(gridUid, false);
        }

        var beaconQuery = EntityQueryEnumerator<STMarsocExtractionBeaconComponent>();
        while (beaconQuery.MoveNext(out var beaconUid, out var beacon))
        {
            CleanupBeaconFlares((beaconUid, beacon));
            QueueDel(beaconUid);
        }

        if (_planetSectors.Count > 0 && _prototypes.HasIndex(WeYaPmcDeathSquadSpawnerProto))
        {
            var dropCoords = _planetSectors[0].Coords;
            for (var i = 0; i < 4; i++)
            {
                Spawn(WeYaPmcDeathSquadSpawnerProto, dropCoords.Offset(new Vector2(i * 0.5f, 0f)));
            }
        }
    }

    private void BroadcastToMarsocSquad(string message)
    {
        var safeMsg = FormattedMessage.EscapeText(message);
        var query = EntityQueryEnumerator<STMarsocOperativeComponent, ActorComponent>();
        while (query.MoveNext(out _, out _, out var actor))
        {
            _chatManager.ChatMessageToOne(
                ChatChannel.Server,
                safeMsg,
                safeMsg,
                EntityUid.Invalid,
                false,
                actor.PlayerSession.Channel);
        }
    }

    private void OnDropZoneMarkerMapInit(Entity<STMarsocDropZoneComponent> ent, ref MapInitEvent args)
    {
        _dropSectorsGenerated = false;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _exteriorStepCandidates.Clear();

        var opQuery = EntityQueryEnumerator<STMarsocOperativeComponent, TransformComponent>();
        while (opQuery.MoveNext(out var uid, out var comp, out var xform))
        {
            if (!comp.CallsignApplied)
            {
                comp.CallsignApplied = true;
                EnsureComp<ParaDroppableComponent>(uid);

                if (xform.GridUid is { } gridUid && IsShuttleGrid(gridUid))
                {
                    comp.HomeShuttleGrid = gridUid;
                    comp.ReturnCoordinates = xform.Coordinates;

                    if (!comp.ShuttleDoorsHardened)
                    {
                        comp.ShuttleDoorsHardened = true;
                        MakeShuttleDoorsIndestructible(gridUid);
                    }
                }

                ApplySynthAndCallsign((uid, comp));
                Dirty(uid, comp);
            }

            if (IsOperativeSteppingIntoSpace((uid, comp), xform))
                _exteriorStepCandidates.Add(uid);
        }

        foreach (var candidateUid in _exteriorStepCandidates)
        {
            HandleOperativeSpaceStep(candidateUid);
        }

        var curTime = _timing.CurTime;
        var beaconQuery = EntityQueryEnumerator<STMarsocExtractionBeaconComponent, TransformComponent>();
        while (beaconQuery.MoveNext(out var beaconUid, out var beacon, out var beaconXform))
        {
            if (!beacon.CountdownActive || curTime < beacon.ExtractionEndTime)
                continue;

            ExecuteBeaconExtraction((beaconUid, beacon), beaconXform);
        }

        UpdateBunkerControllers(curTime);
        UpdateExposureLineOfSight(curTime);
    }

    private bool IsBunkerEntity(EntityUid uid, string? customId)
    {
        var protoId = MetaData(uid).EntityPrototype?.ID ?? string.Empty;
        if (protoId.Contains("HybrisaIndestructibleUltra", StringComparison.OrdinalIgnoreCase))
            return true;

        var name = Name(uid);
        if (name.Contains("Bunker", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Бункер", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Elevator Lockdown", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(customId) &&
            (customId.Contains("bunker", StringComparison.OrdinalIgnoreCase) ||
             customId.Contains("бункер", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }

    private void UpdateBunkerControllers(TimeSpan curTime)
    {
        var marsocPresent = _marsocActiveThisRound ||
                            EntityQueryEnumerator<STMarsocOfficerComponent>().MoveNext(out _, out _);

        var ctrlQuery = EntityQueryEnumerator<STMarsocBunkerControllerComponent, TransformComponent>();
        while (ctrlQuery.MoveNext(out var uid, out var bunker, out var xform))
        {
            if ((marsocPresent || bunker.AlwaysSpawnInTestMode) && !bunker.VIPSpawnChecked)
            {
                bunker.VIPSpawnChecked = true;
                var shouldSpawn = bunker.AlwaysSpawnInTestMode || _random.Prob(bunker.SpawnChanceWhenMarsocActive);
                if (shouldSpawn)
                {
                    var supervisorCoords = xform.Coordinates;
                    var assistantCoords = supervisorCoords.Offset(new Vector2(1f, 0f));

                    if (_prototypes.HasIndex(bunker.SupervisorSpawnerProto))
                        Spawn(bunker.SupervisorSpawnerProto, supervisorCoords);

                    if (_prototypes.HasIndex(bunker.AssistantSpawnerProto))
                        Spawn(bunker.AssistantSpawnerProto, assistantCoords);
                }

                Dirty(uid, bunker);
            }
        }

        if (!_bunkerInitialized)
        {
            var foundAnyBunker = false;
            _bunkerAnchors.Clear();

            var btnQuery = EntityQueryEnumerator<RMCDoorButtonComponent, TransformComponent>();
            while (btnQuery.MoveNext(out var btnUid, out var btnComp, out var btnXform))
            {
                var btnId = btnComp.Id ?? Name(btnUid);
                if (!IsBunkerEntity(btnUid, btnId))
                    continue;

                foundAnyBunker = true;
                EnsureComp<STMarsocBunkerButtonComponent>(btnUid);
                ForcePowerEntity(btnUid);
                _bunkerAnchors.Add(btnXform.Coordinates);

                if (!string.IsNullOrWhiteSpace(btnId))
                    _bunkerLinkedIds.Add(btnId);
            }

            var doorQuery = EntityQueryEnumerator<RMCPodDoorComponent, DoorComponent, TransformComponent>();
            while (doorQuery.MoveNext(out var doorUid, out _, out var doorComp, out var doorXform))
            {
                var doorName = Name(doorUid);
                if (!IsBunkerEntity(doorUid, doorName) && !_bunkerLinkedIds.Contains(doorName))
                    continue;

                foundAnyBunker = true;
                _bunkerControlledDoors.Add(doorUid);
                _bunkerAnchors.Add(doorXform.Coordinates);
                ForcePowerEntity(doorUid);

                _bunkerLinkedIds.Add(doorName);

                if (doorComp.State == DoorState.Open)
                    _door.StartClosing(doorUid, doorComp);
            }

            foreach (var controlledDoor in _bunkerControlledDoors)
            {
                RemComp<RMCPodDoorComponent>(controlledDoor);
            }

            BindPlanetQuestProps();

            if (foundAnyBunker)
            {
                _bunkerInitialized = true;
                RefreshBunkerIndependentPower();
            }
        }

        if (!_bunkerInitialized)
            return;

        if (curTime >= _nextBunkerPowerRefresh)
        {
            _nextBunkerPowerRefresh = curTime + TimeSpan.FromSeconds(2);
            BindPlanetQuestProps();
            RefreshBunkerIndependentPower();
        }

        if (_bunkerShuttersPermanentlyOpen)
        {
            foreach (var doorUid in _bunkerControlledDoors)
            {
                if (TerminatingOrDeleted(doorUid) || !TryComp<DoorComponent>(doorUid, out var doorComp))
                    continue;

                if (doorComp.State != DoorState.Open && doorComp.State != DoorState.Opening)
                    _door.TryOpen(doorUid, doorComp);
            }

            return;
        }

        var roundDuration = _gameTicker.RoundDuration();
        var autoOpenTime = TimeSpan.FromMinutes(10);

        var timeExpired = roundDuration >= autoOpenTime ||
                          (_bunkerCountdownStarted && _bunkerScheduledOpenTime is { } openAt && curTime >= openAt);

        if (!timeExpired)
            return;

        _bunkerShuttersPermanentlyOpen = true;
        OpenAllBunkerShutters();
    }

    private void BindPlanetQuestProps()
    {
        var propQuery = EntityQueryEnumerator<MetaDataComponent>();
        while (propQuery.MoveNext(out var propUid, out var meta))
        {
            var protoId = meta.EntityPrototype?.ID;
            if (protoId == "RMCPropGiantXenoCryo" && !HasComp<STMarsocCryoPodObjectiveComponent>(propUid))
            {
                EnsureComp<STMarsocCryoPodObjectiveComponent>(propUid);
            }
            else if (protoId == "RMCPortableVendorCorporate" && !HasComp<STMarsocCorporateCaseComponent>(propUid))
            {
                EnsureComp<STMarsocCorporateCaseComponent>(propUid);
                RemComp<AccessReaderComponent>(propUid);
                ForcePowerEntity(propUid);
            }
        }
    }

    private void RefreshBunkerIndependentPower()
    {
        foreach (var anchor in _bunkerAnchors)
        {
            _bunkerNearbyEntities.Clear();
            _lookup.GetEntitiesInRange(anchor, BunkerPowerRadius, _bunkerNearbyEntities, LookupFlags.Uncontained);

            foreach (var ent in _bunkerNearbyEntities)
            {
                if (TryComp<RMCApcComponent>(ent, out _))
                {
                    _bunkerPoweredApcs.Add(ent);
                    if (_container.TryGetContainer(ent, "cell", out var cellContainer))
                    {
                        foreach (var cellUid in cellContainer.ContainedEntities)
                        {
                            if (TryComp<BatteryComponent>(cellUid, out var battery))
                                _battery.SetCharge(cellUid, battery.MaxCharge, battery);
                        }
                    }
                }

                ForcePowerEntity(ent);
            }
        }

        foreach (var apcUid in _bunkerPoweredApcs)
        {
            if (TerminatingOrDeleted(apcUid) || !TryComp<RMCApcComponent>(apcUid, out var apc))
                continue;

            if (_container.TryGetContainer(apcUid, apc.CellContainerSlot, out var cellContainer))
            {
                foreach (var cellUid in cellContainer.ContainedEntities)
                {
                    if (TryComp<BatteryComponent>(cellUid, out var battery))
                        _battery.SetCharge(cellUid, battery.MaxCharge, battery);
                }
            }
        }
    }

    private void ForcePowerEntity(EntityUid uid)
    {
        if (!TryComp<ApcPowerReceiverComponent>(uid, out var receiver))
            return;

        if (!receiver.NeedsPower && receiver.Powered)
            return;

        receiver.NeedsPower = false;
        receiver.Powered = true;
        Dirty(uid, receiver);

        var ev = new PowerChangedEvent(true, 0);
        RaiseLocalEvent(uid, ref ev);

        if (TryComp<AppearanceComponent>(uid, out var appearance))
            _appearance.SetData(uid, PowerDeviceVisuals.Powered, true, appearance);
    }

    private void OnBunkerDoorButtonActivate(Entity<STMarsocBunkerButtonComponent> ent, ref ActivateInWorldEvent args)
    {
        if (args.Handled || !TryComp<RMCDoorButtonComponent>(ent.Owner, out var buttonComp))
            return;

        var button = new Entity<RMCDoorButtonComponent>(ent.Owner, buttonComp);
        var user = args.User;

        ForcePowerEntity(ent.Owner);

        if (_bunkerShuttersPermanentlyOpen)
        {
            _popup.PopupEntity(Loc.GetString("st-marsoc-bunker-shutters-opened"), button, user);
            args.Handled = true;
            return;
        }

        if (!_accessReader.IsAllowed(user, button))
        {
            _popup.PopupEntity(Loc.GetString("cm-vending-machine-access-denied"), button, user);
            _cmDoor.DoPodDoorButtonAnimation(button, button.Comp.DeniedState);
            args.Handled = true;
            return;
        }

        var curTime = _timing.CurTime;
        if (_bunkerCountdownStarted && _bunkerScheduledOpenTime is { } scheduled)
        {
            var remainingSec = Math.Max(1, (int)Math.Ceiling((scheduled - curTime).TotalSeconds));
            var activeMsg = Loc.GetString("st-marsoc-bunker-countdown-active", ("seconds", remainingSec));
            _popup.PopupEntity(activeMsg, button, user);
            args.Handled = true;
            return;
        }

        _bunkerCountdownStarted = true;
        _bunkerScheduledOpenTime = curTime + TimeSpan.FromMinutes(3);

        _cmDoor.DoPodDoorButtonAnimation(button, button.Comp.OnState);

        var startMsg = Loc.GetString("st-marsoc-bunker-countdown-started", ("minutes", 3));
        _popup.PopupEntity(startMsg, button);

        args.Handled = true;
    }

    private void OpenAllBunkerShutters()
    {
        EntityCoordinates? announceCoords = null;
        foreach (var doorUid in _bunkerControlledDoors)
        {
            if (TerminatingOrDeleted(doorUid) || !TryComp<DoorComponent>(doorUid, out var doorComp))
                continue;

            announceCoords ??= Transform(doorUid).Coordinates;

            if (doorComp.State != DoorState.Open)
                _door.TryOpen(doorUid, doorComp);
        }

        if (announceCoords is { } coords)
            _popup.PopupCoordinates(Loc.GetString("st-marsoc-bunker-shutters-opened"), coords);
    }

    private void OnCryoPodActivate(Entity<STMarsocCryoPodObjectiveComponent> ent, ref ActivateInWorldEvent args)
    {
        if (args.Handled)
            return;

        if (TryExtractCryoPodData(ent, args.User))
            args.Handled = true;
    }

    private void OnCryoPodInteractHand(Entity<STMarsocCryoPodObjectiveComponent> ent, ref InteractHandEvent args)
    {
        if (args.Handled)
            return;

        if (TryExtractCryoPodData(ent, args.User))
            args.Handled = true;
    }

    private bool TryExtractCryoPodData(Entity<STMarsocCryoPodObjectiveComponent> ent, EntityUid user)
    {
        if (ent.Comp.DataExtracted)
        {
            _popup.PopupEntity(Loc.GetString("st-marsoc-cryopod-already-extracted"), ent.Owner, user);
            return true;
        }

        if (!_prototypes.HasIndex(ent.Comp.ExtractedDiskProto))
            return false;

        ent.Comp.DataExtracted = true;
        var disk = Spawn(ent.Comp.ExtractedDiskProto, Transform(user).Coordinates);
        ApplyWeYaStamp(disk);
        _hands.TryPickupAnyHand(user, disk);

        _popup.PopupEntity(Loc.GetString("st-marsoc-cryopod-extracted"), ent.Owner, user);
        return true;
    }

    private void OnCorporateCaseActivate(Entity<STMarsocCorporateCaseComponent> ent, ref ActivateInWorldEvent args)
    {
        if (DispenseCorporateCaseLoot(ent, args.User))
            args.Handled = true;
    }

    private void OnCorporateCaseInteractHand(Entity<STMarsocCorporateCaseComponent> ent, ref InteractHandEvent args)
    {
        if (DispenseCorporateCaseLoot(ent, args.User))
            args.Handled = true;
    }

    private void OnCorporateCaseUseInHand(Entity<STMarsocCorporateCaseComponent> ent, ref UseInHandEvent args)
    {
        if (DispenseCorporateCaseLoot(ent, args.User))
            args.Handled = true;
    }

    private bool DispenseCorporateCaseLoot(Entity<STMarsocCorporateCaseComponent> ent, EntityUid? user)
    {
        if (ent.Comp.OpenedOnce)
            return false;

        ent.Comp.OpenedOnce = true;
        var coords = Transform(ent.Owner).Coordinates;

        foreach (var proto in CorporateCaseLootProtos)
        {
            if (!_prototypes.HasIndex(proto))
                continue;

            var spawned = Spawn(proto, coords);
            ApplyWeYaStamp(spawned);

            if (_container.TryGetContainer(ent.Owner, "storagebase", out var storageContainer))
                _container.Insert(spawned, storageContainer);
        }

        if (user is { } validUser)
            _popup.PopupEntity(Loc.GetString("st-marsoc-case-dispensed"), ent.Owner, validUser);

        return true;
    }

    private void ApplyWeYaStamp(EntityUid documentUid)
    {
        if (!TryComp<PaperComponent>(documentUid, out var paper))
            return;

        var stampName = "stamp-component-stamped-name-we-ya";
        var stampColor = Color.FromHex("#1B3B6F");
        var stampState = "paper_stamp-weya";

        if (_prototypes.TryIndex<EntityPrototype>("CMStampWeYa", out var stampProto) &&
            stampProto.TryGetComponent<StampComponent>(out var stampComp, _compFactory))
        {
            stampName = stampComp.StampedName;
            stampColor = stampComp.StampedColor;
            stampState = stampComp.StampState;
        }

        var info = new StampDisplayInfo
        {
            StampedName = stampName,
            StampedColor = stampColor
        };

        _paper.TryStamp((documentUid, paper), info, stampState);
    }

    private void OnCommandProtocolsUseInHand(Entity<STMarsocCommandProtocolsComponent> ent, ref UseInHandEvent args)
    {
        if (args.Handled || ent.Comp.Used)
            return;

        ent.Comp.Used = true;
        _popup.PopupEntity(Loc.GetString("st-marsoc-protocols-activated"), args.User, args.User);
        args.Handled = true;
    }

    public bool IsPlanetGrid(EntityUid gridUid, EntityUid? shuttleGrid)
    {
        if (gridUid == shuttleGrid)
            return false;

        if (HasComp<RMCPlanetComponent>(gridUid))
            return true;

        if (HasComp<AlmayerComponent>(gridUid))
            return false;

        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return false;

        var aabb = grid.LocalAABB;
        if (aabb.Width < 70f && aabb.Height < 70f)
            return false;

        var name = MetaData(gridUid).EntityName;
        return !name.Contains("Almayer", StringComparison.OrdinalIgnoreCase) &&
               !name.Contains("Алмайер", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsShuttleGrid(EntityUid gridUid)
    {
        if (HasComp<RMCPlanetComponent>(gridUid) || HasComp<AlmayerComponent>(gridUid))
            return false;

        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return false;

        var aabb = grid.LocalAABB;
        return aabb.Width < 70f && aabb.Height < 70f;
    }

    private bool IsOperativeSteppingIntoSpace(Entity<STMarsocOperativeComponent> ent, TransformComponent xform)
    {
        if (!xform.Initialized || xform.MapUid == null)
            return false;

        if (_mobState.IsDead(ent.Owner))
            return false;

        if (HasComp<ParaDroppingComponent>(ent.Owner) || HasComp<SkyFallingComponent>(ent.Owner))
            return false;

        if (xform.GridUid == null)
            return true;

        var gridUid = xform.GridUid.Value;
        if (IsPlanetGrid(gridUid, ent.Comp.HomeShuttleGrid) || HasComp<AlmayerComponent>(gridUid))
            return false;

        if (ent.Comp.HomeShuttleGrid != null && gridUid != ent.Comp.HomeShuttleGrid.Value)
            return false;

        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return false;

        if (!_map.TryGetTileRef(gridUid, grid, xform.Coordinates, out var tileRef) || tileRef.Tile.IsEmpty)
            return true;

        var tileDef = _tileDef[tileRef.Tile.TypeId];
        return tileDef.ID == "CMShuttleTileInvisible" ||
               tileDef.ID.Contains("Invisible", StringComparison.OrdinalIgnoreCase) ||
               tileDef.ID == "Space";
    }

    private void HandleOperativeSpaceStep(EntityUid uid)
    {
        if (TerminatingOrDeleted(uid) || !TryComp<STMarsocOperativeComponent>(uid, out var comp))
            return;

        if (HasComp<ParaDroppingComponent>(uid) || HasComp<SkyFallingComponent>(uid))
            return;

        if (IsShuttleInTransit(comp.HomeShuttleGrid) || _exposure100Triggered)
        {
            if (comp.ReturnCoordinates is { } returnCoords && Exists(returnCoords.EntityId))
                _transform.SetCoordinates(uid, returnCoords);

            return;
        }

        if (IsParadropReadyForOperative(comp.HomeShuttleGrid))
        {
            if (TryFindPlanetDropTarget(comp.HomeShuttleGrid, out var dropCoords))
            {
                ExecuteParachuteDrop(uid, dropCoords);
                return;
            }

            if (comp.ReturnCoordinates is { } fallbackReturn && Exists(fallbackReturn.EntityId))
                _transform.SetCoordinates(uid, fallbackReturn);

            return;
        }

        var damage = new DamageSpecifier(_prototypes.Index(BluntDamageType), FixedPoint2.New(comp.SpaceInstaKillDamage));
        _damageable.TryChangeDamage(uid, damage, true);
    }

    private bool IsShuttleInTransit(EntityUid? shuttleGrid)
    {
        if (shuttleGrid is { } gridUid && HasComp<FTLComponent>(gridUid))
            return true;

        var query = EntityQueryEnumerator<STMarsocBriefingConsoleComponent, TransformComponent>();
        while (query.MoveNext(out _, out var console, out var xform))
        {
            if (shuttleGrid != null && xform.GridUid != shuttleGrid)
                continue;

            if (console.FlightPhase == STMarsocFlightPhase.InTransit)
                return true;
        }

        return false;
    }

    public void SetShuttleFlightParallax(EntityUid shuttleGrid, string parallaxId)
    {
        var xform = Transform(shuttleGrid);
        if (xform.MapUid is { } mapUid)
        {
            var mapParallax = EnsureComp<ParallaxComponent>(mapUid);
            mapParallax.Parallax = parallaxId;
            Dirty(mapUid, mapParallax);
        }

        var gridParallax = EnsureComp<ParallaxComponent>(shuttleGrid);
        gridParallax.Parallax = parallaxId;
        Dirty(shuttleGrid, gridParallax);
    }

    public void StartShuttleOrbitalFlight(EntityUid shuttleGrid, string flightParallax, float startupSeconds, float hyperspaceSeconds)
    {
        RemComp<DropshipComponent>(shuttleGrid);
        RemComp<NoFTLComponent>(shuttleGrid);
        EnsureComp<DropshipInFlyByComponent>(shuttleGrid);

        SetShuttleParadropState(shuttleGrid, false);

        var shuttleXform = Transform(shuttleGrid);
        if (shuttleXform.MapUid is { } mapUid && !HasComp<FTLComponent>(shuttleGrid))
        {
            var shuttleComp = EnsureComp<ShuttleComponent>(shuttleGrid);
            var currentPos = _transform.GetWorldPosition(shuttleXform);
            var targetCoords = new EntityCoordinates(mapUid, currentPos);

            _shuttle.FTLToCoordinates(
                shuttleGrid,
                shuttleComp,
                targetCoords,
                shuttleXform.LocalRotation,
                startupSeconds,
                hyperspaceSeconds);
        }
    }

    public void CompleteShuttleOrbitalArrival(EntityUid shuttleGrid, string flyByParallax)
    {
        EnsureSafePlanetSectors(shuttleGrid);
        SetShuttleFlightParallax(shuttleGrid, flyByParallax);
        ShakeShuttlePassengers(shuttleGrid, 4, 3);
    }

    public void SetShuttleParadropState(EntityUid shuttleGrid, bool active)
    {
        if (_exposure100Triggered)
            active = false;

        var officerQuery = EntityQueryEnumerator<STMarsocOfficerComponent, TransformComponent>();
        while (officerQuery.MoveNext(out var officerUid, out var officer, out var officerXform))
        {
            if (officerXform.GridUid != shuttleGrid)
                continue;

            officer.ParadropActive = active;
            Dirty(officerUid, officer);
        }

        var boltQuery = EntityQueryEnumerator<DoorBoltComponent, TransformComponent>();
        while (boltQuery.MoveNext(out var doorUid, out var bolt, out var doorXform))
        {
            if (doorXform.GridUid != shuttleGrid)
                continue;

            RemComp<RMCDockingSafetyLockedComponent>(doorUid);
            RemComp<RMCDropshipDoorConsoleLockComponent>(doorUid);

            if (active)
                _dropship.UnlockDoor((doorUid, bolt), forceSafetyUnlock: true);
            else
                _dropship.LockDoor((doorUid, bolt));
        }
    }

    private void ShakeShuttlePassengers(EntityUid shuttleGrid, int shakes, int strength)
    {
        foreach (var session in Filter.BroadcastGrid(shuttleGrid).Recipients)
        {
            if (session.AttachedEntity is not { } passenger)
                continue;

            _cameraShake.ShakeCamera(passenger, shakes, strength);
        }
    }

    public string GetActiveOfficerCallsign()
    {
        var query = EntityQueryEnumerator<STMarsocOfficerComponent, STMarsocOperativeComponent, MetaDataComponent>();
        while (query.MoveNext(out _, out _, out var op, out var meta))
        {
            var name = !string.IsNullOrWhiteSpace(op.AssignedFullName) ? op.AssignedFullName : meta.EntityName;
            if (!string.IsNullOrWhiteSpace(name))
                return name.ToUpperInvariant();
        }

        return Loc.GetString("st-marsoc-default-officer-callsign");
    }

    private void MakeShuttleDoorsIndestructible(EntityUid shuttleGrid)
    {
        var doorQuery = EntityQueryEnumerator<DoorComponent, TransformComponent>();
        while (doorQuery.MoveNext(out var doorUid, out _, out var xform))
        {
            if (xform.GridUid != shuttleGrid)
                continue;

            RemComp<DestructibleComponent>(doorUid);
            RemComp<DamageableComponent>(doorUid);
        }
    }

    private void OnOfficerMapInit(Entity<STMarsocOfficerComponent> ent, ref MapInitEvent args)
    {
        var xform = Transform(ent.Owner);
        if (xform.GridUid is not { } gridUid)
            return;

        RemComp<DropshipComponent>(gridUid);

        if (!_prototypes.HasIndex<EntityPrototype>("STComputerMarsocBriefing"))
            return;

        var query = EntityQueryEnumerator<MetaDataComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var meta, out var compXform))
        {
            if (compXform.GridUid != gridUid)
                continue;

            if (meta.EntityPrototype?.ID == "RMCPropCommunicationsConsole")
            {
                var coords = compXform.Coordinates;
                var rot = compXform.LocalRotation;
                QueueDel(uid);

                var briefingConsole = Spawn("STComputerMarsocBriefing", coords);
                _transform.SetLocalRotation(briefingConsole, rot);
                break;
            }
        }
    }

    private void OnOfficerShutdown(Entity<STMarsocOfficerComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.DeployActionEntity is { } deployAction)
            _actions.RemoveAction(ent.Owner, deployAction);

        if (ent.Comp.ToggleParadropActionEntity is { } dropAction)
            _actions.RemoveAction(ent.Owner, dropAction);
    }

    private void OnToggleParadrop(Entity<STMarsocOfficerComponent> ent, ref STMarsocToggleParadropActionEvent args)
    {
        if (args.Handled)
            return;

        if (TryToggleParadrop(ent.Owner))
            args.Handled = true;
    }

    public bool TryToggleParadrop(EntityUid officerUid)
    {
        if (_exposure100Triggered || !TryComp<STMarsocOfficerComponent>(officerUid, out var comp))
            return false;

        var nextState = !comp.ParadropActive;
        var xform = Transform(officerUid);
        if (xform.GridUid is { } gridUid)
        {
            SetShuttleParadropState(gridUid, nextState);
        }
        else
        {
            comp.ParadropActive = nextState;
            Dirty(officerUid, comp);
        }

        var msg = Loc.GetString(comp.ParadropActive ? ParadropEnabledLoc : ParadropDisabledLoc);
        _popup.PopupEntity(msg, officerUid, officerUid);
        return true;
    }

    private void ExecuteParachuteDrop(EntityUid operativeUid, EntityCoordinates dropCoords)
    {
        var droppable = EnsureComp<ParaDroppableComponent>(operativeUid);

        _paraDrop.DoParaDrop(
            operativeUid,
            dropCoords,
            skyFallDuration: 1.2f,
            dropDuration: 3.0f,
            dropSound: droppable.DropSound,
            dropScatter: 2);

        if (!HasComp<SkyFallingComponent>(operativeUid) && !HasComp<ParaDroppingComponent>(operativeUid))
        {
            _transform.SetCoordinates(operativeUid, dropCoords);
            EnsureComp<ParaDroppingComponent>(operativeUid);
        }
    }

    private bool IsParadropReadyForOperative(EntityUid? shuttleGrid)
    {
        if (_exposure100Triggered)
            return false;

        var officerQuery = EntityQueryEnumerator<STMarsocOfficerComponent, TransformComponent>();
        while (officerQuery.MoveNext(out _, out var officer, out var officerXform))
        {
            if (shuttleGrid != null && officerXform.GridUid != shuttleGrid)
                continue;

            if (officer.ParadropActive)
                return true;
        }

        var consoleQuery = EntityQueryEnumerator<STMarsocBriefingConsoleComponent, TransformComponent>();
        while (consoleQuery.MoveNext(out _, out var console, out var xform))
        {
            if (shuttleGrid != null && xform.GridUid != shuttleGrid)
                continue;

            if (console.FlightPhase == STMarsocFlightPhase.ArrivedInOrbit || console.SectorLocked)
                return true;
        }

        return false;
    }

    public string CycleDropSector(EntityUid? shuttleGrid)
    {
        EnsureSafePlanetSectors(shuttleGrid);
        if (_planetSectors.Count == 0)
            return Loc.GetString(HybrisaDropZones[0].NameLoc);

        _selectedDropSectorIndex = (_selectedDropSectorIndex + 1) % _planetSectors.Count;
        return _planetSectors[_selectedDropSectorIndex].Name;
    }

    public string GetCurrentDropSectorName(EntityUid? shuttleGrid)
    {
        EnsureSafePlanetSectors(shuttleGrid);
        if (_planetSectors.Count == 0)
            return Loc.GetString(HybrisaDropZones[0].NameLoc);

        var idx = Math.Clamp(_selectedDropSectorIndex, 0, _planetSectors.Count - 1);
        return _planetSectors[idx].Name;
    }

    public void EnsureSafePlanetSectors(EntityUid? shuttleGrid)
    {
        if (_dropSectorsGenerated && _planetSectors.Count > 0)
            return;

        _planetSectors.Clear();
        _mappedMarkers.Clear();
        _survivorPositions.Clear();

        var markerQuery = EntityQueryEnumerator<STMarsocDropZoneComponent, TransformComponent>();
        while (markerQuery.MoveNext(out _, out var marker, out var markerXform))
        {
            if (markerXform.GridUid is not { } markerGrid || markerGrid == shuttleGrid)
                continue;

            if (HasComp<AlmayerComponent>(markerGrid) || IsShuttleGrid(markerGrid))
                continue;

            var baseName = Loc.GetString(marker.SectorNameLoc);
            _mappedMarkers.Add((marker.Order, baseName, markerXform.Coordinates));
        }

        if (_mappedMarkers.Count > 0)
        {
            _mappedMarkers.Sort((a, b) => a.Order.CompareTo(b.Order));

            for (var i = 0; i < _mappedMarkers.Count; i++)
            {
                var item = _mappedMarkers[i];
                var duplicateCount = 0;
                for (var j = 0; j < _mappedMarkers.Count; j++)
                {
                    if (_mappedMarkers[j].Name == item.Name)
                        duplicateCount++;
                }

                var finalName = duplicateCount > 1 ? $"{item.Name} #{i + 1}" : item.Name;
                _planetSectors.Add((finalName, item.Coords));
            }

            _dropSectorsGenerated = true;
            return;
        }

        EntityUid? targetPlanetGrid = null;
        var bestArea = 0f;

        var planetQuery = EntityQueryEnumerator<RMCPlanetComponent, MapGridComponent>();
        while (planetQuery.MoveNext(out var gridUid, out _, out var gridComp))
        {
            var aabb = gridComp.LocalAABB;
            var area = aabb.Width * aabb.Height;
            if (area > bestArea)
            {
                bestArea = area;
                targetPlanetGrid = gridUid;
            }
        }

        if (targetPlanetGrid == null)
        {
            var gridQuery = EntityQueryEnumerator<MapGridComponent>();
            while (gridQuery.MoveNext(out var gridUid, out var gridComp))
            {
                if (!IsPlanetGrid(gridUid, shuttleGrid))
                    continue;

                var aabb = gridComp.LocalAABB;
                var area = aabb.Width * aabb.Height;
                if (area > bestArea)
                {
                    bestArea = area;
                    targetPlanetGrid = gridUid;
                }
            }
        }

        if (targetPlanetGrid is not { } planetUid || !TryComp<MapGridComponent>(planetUid, out var planetGrid))
            return;

        var mobQuery = EntityQueryEnumerator<HumanoidAppearanceComponent, MobStateComponent, TransformComponent>();
        while (mobQuery.MoveNext(out var mobUid, out _, out var mobState, out var mobXform))
        {
            if (mobXform.GridUid != planetUid || HasComp<STMarsocOperativeComponent>(mobUid))
                continue;

            if (_mobState.IsDead(mobUid, mobState))
                continue;

            _survivorPositions.Add(mobXform.LocalPosition);
        }

        var bounds = planetGrid.LocalAABB;
        var planetName = MetaData(planetUid).EntityName;
        var isHybrisa = planetName.Contains("Hybrisa", StringComparison.OrdinalIgnoreCase) ||
                        planetName.Contains("Гибрис", StringComparison.OrdinalIgnoreCase);

        if (isHybrisa)
        {
            foreach (var (nameLoc, viewerCoords) in HybrisaDropZones)
            {
                var localTarget = new Vector2(bounds.Left + viewerCoords.X, bounds.Bottom + viewerCoords.Y);
                if (TryFindOpenSkyTileNear(planetUid, planetGrid, localTarget, out var safeCoords))
                {
                    RegisterDropSector(Loc.GetString(nameLoc), safeCoords);
                }
            }
        }

        if (_planetSectors.Count == 0)
        {
            var center = bounds.Center;
            var offsets = new[]
            {
                new Vector2(0f, bounds.Height * 0.25f),
                new Vector2(0f, -bounds.Height * 0.25f),
                new Vector2(-bounds.Width * 0.25f, 0f),
                new Vector2(bounds.Width * 0.25f, 0f)
            };

            for (var i = 0; i < offsets.Length; i++)
            {
                var desiredPos = center + offsets[i];
                if (TryFindOpenSkyTileNear(planetUid, planetGrid, desiredPos, out var safeCoords))
                {
                    RegisterDropSector(Loc.GetString(SectorCardinalNames[i]), safeCoords);
                }
            }
        }

        if (_planetSectors.Count > 0)
            _dropSectorsGenerated = true;
    }

    private void RegisterDropSector(string sectorName, EntityCoordinates coords)
    {
        _planetSectors.Add((sectorName, coords));

        if (_prototypes.HasIndex(DropZoneMarkerProto))
        {
            var marker = Spawn(DropZoneMarkerProto, coords);
            _metaData.SetEntityName(marker, sectorName);
            _spawnedDropMarkers.Add(marker);
        }
    }

    private bool TryFindOpenSkyTileNear(
        EntityUid gridUid,
        MapGridComponent grid,
        Vector2 targetLocalPos,
        out EntityCoordinates safeCoords)
    {
        EntityCoordinates? fallbackOpenSky = null;

        for (var radius = 0; radius <= 60; radius += 2)
        {
            for (var dx = -radius; dx <= radius; dx += 2)
            {
                for (var dy = -radius; dy <= radius; dy += 2)
                {
                    var probePos = targetLocalPos + new Vector2(dx, dy);
                    var probeCoords = new EntityCoordinates(gridUid, probePos);

                    if (!IsOpenSkyPlanetTile(gridUid, grid, probeCoords))
                        continue;

                    fallbackOpenSky ??= probeCoords;

                    var tooCloseToSurvivor = false;
                    foreach (var survPos in _survivorPositions)
                    {
                        if (Vector2.Distance(probePos, survPos) < MinDistanceFromSurvivors)
                        {
                            tooCloseToSurvivor = true;
                            break;
                        }
                    }

                    if (tooCloseToSurvivor)
                        continue;

                    safeCoords = probeCoords;
                    return true;
                }
            }
        }

        safeCoords = fallbackOpenSky ?? new EntityCoordinates(gridUid, targetLocalPos);
        return true;
    }

    private bool IsOpenSkyPlanetTile(EntityUid gridUid, MapGridComponent grid, EntityCoordinates coords)
    {
        if (!_map.TryGetTileRef(gridUid, grid, coords, out var tileRef) || tileRef.Tile.IsEmpty)
            return false;

        var tileDef = _tileDef[tileRef.Tile.TypeId];
        var id = tileDef.ID;

        if (id == "Space" ||
            id.Contains("Invisible", StringComparison.OrdinalIgnoreCase) ||
            id.Contains("Underground", StringComparison.OrdinalIgnoreCase) ||
            id.Contains("Shale", StringComparison.OrdinalIgnoreCase) ||
            id.Contains("Asteroid", StringComparison.OrdinalIgnoreCase) ||
            id.Contains("Cave", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var isOutdoorSurface = id.StartsWith("RMCPlanet", StringComparison.OrdinalIgnoreCase) ||
                               id.StartsWith("CMPlanet", StringComparison.OrdinalIgnoreCase) ||
                               id.Contains("Asphalt", StringComparison.OrdinalIgnoreCase) ||
                               id.Contains("Sidewalk", StringComparison.OrdinalIgnoreCase) ||
                               id.Contains("Dirt", StringComparison.OrdinalIgnoreCase) ||
                               id.Contains("Grass", StringComparison.OrdinalIgnoreCase) ||
                               id.Contains("Sand", StringComparison.OrdinalIgnoreCase) ||
                               id.Contains("Snow", StringComparison.OrdinalIgnoreCase);

        return isOutdoorSurface;
    }

    private bool TryFindPlanetDropTarget(EntityUid? shuttleGrid, out EntityCoordinates dropCoords)
    {
        EnsureSafePlanetSectors(shuttleGrid);
        if (_planetSectors.Count > 0)
        {
            var idx = Math.Clamp(_selectedDropSectorIndex, 0, _planetSectors.Count - 1);
            var baseCoords = _planetSectors[idx].Coords;
            var gridUid = baseCoords.EntityId;

            if (TryComp<MapGridComponent>(gridUid, out var grid))
            {
                for (var attempt = 0; attempt < 16; attempt++)
                {
                    var scatter = new Vector2(_random.NextFloat(-1.5f, 1.5f), _random.NextFloat(-1.5f, 1.5f));
                    var candidate = baseCoords.Offset(scatter);
                    if (_map.TryGetTileRef(gridUid, grid, candidate, out var tileRef) &&
                        !tileRef.Tile.IsEmpty &&
                        _tileDef[tileRef.Tile.TypeId].ID != "Space")
                    {
                        dropCoords = candidate;
                        return true;
                    }
                }
            }

            dropCoords = baseCoords;
            return true;
        }

        dropCoords = default;
        return false;
    }

    private void OnBeaconUseInHand(Entity<STMarsocExtractionBeaconComponent> ent, ref UseInHandEvent args)
    {
        if (args.Handled || ent.Comp.Deployed || _exposure100Triggered)
            return;

        var user = args.User;
        if (!CanUserDeployBeacon(user))
        {
            _popup.PopupEntity(Loc.GetString(ent.Comp.NoPermissionPopup), user, user);
            return;
        }

        var userXform = Transform(user);
        if (userXform.GridUid is not { } currentGrid || !TryComp<MapGridComponent>(currentGrid, out var gridComp))
            return;

        var isOnShuttle = !HasComp<RMCPlanetComponent>(currentGrid) &&
                          (IsShuttleGrid(currentGrid) ||
                           (TryComp<STMarsocOperativeComponent>(user, out var userOp) && userOp.HomeShuttleGrid == currentGrid));

        if (isOnShuttle)
        {
            _popup.PopupEntity(Loc.GetString(CannotDeployOnShuttleLoc), user, user);
            return;
        }

        var dropCoords = _transform.GetMoverCoordinates(user);
        if (!IsOpenSkyPlanetTile(currentGrid, gridComp, dropCoords))
        {
            _popup.PopupEntity(Loc.GetString(CannotDeployUnderRoofLoc), user, user);
            return;
        }

        _hands.TryDrop(user, ent.Owner);
        _transform.SetCoordinates(ent.Owner, dropCoords);

        ent.Comp.Deployed = true;
        if (TryComp<STMarsocOperativeComponent>(user, out var opComp) && opComp.ReturnCoordinates is { } retCoords)
            ent.Comp.TargetShuttleCoordinates = retCoords;

        SpawnFlarePerimeter(ent, currentGrid, dropCoords);
        GrantConfirmActionToCommanders();

        _popup.PopupEntity(Loc.GetString(ent.Comp.DeployedPopup), ent.Owner);
        Dirty(ent);
        args.Handled = true;
    }

    private bool CanUserDeployBeacon(EntityUid user)
    {
        if (HasComp<STMarsocOfficerComponent>(user) || HasComp<SynthComponent>(user))
            return true;

        return TryComp<STMarsocOperativeComponent>(user, out var op) && op.CanDeployBeacon;
    }

    private void SpawnFlarePerimeter(
        Entity<STMarsocExtractionBeaconComponent> ent,
        EntityUid gridUid,
        EntityCoordinates centerCoords)
    {
        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return;

        var r = ent.Comp.HalfExtent;

        for (var x = -r; x <= r; x++)
        {
            for (var y = -r; y <= r; y++)
            {
                if (Math.Abs(x) != r && Math.Abs(y) != r)
                    continue;

                var flareCoords = centerCoords.Offset(new Vector2(x, y));
                if (!IsOpenSkyPlanetTile(gridUid, grid, flareCoords))
                    continue;

                var flare = Spawn(ent.Comp.FlarePrototype, flareCoords);
                ent.Comp.SpawnedFlares.Add(flare);
            }
        }
    }

    private void GrantConfirmActionToCommanders()
    {
        var query = EntityQueryEnumerator<STMarsocOperativeComponent>();
        while (query.MoveNext(out var uid, out var op))
        {
            if (!op.CanConfirmExtraction)
                continue;

            _actions.AddAction(uid, ref op.ConfirmExtractionActionEntity, op.ConfirmExtractionActionId);
            Dirty(uid, op);
        }
    }

    private void OnConfirmExtraction(Entity<STMarsocOperativeComponent> ent, ref STMarsocConfirmExtractionActionEvent args)
    {
        if (args.Handled || _exposure100Triggered)
            return;

        var found = false;
        var query = EntityQueryEnumerator<STMarsocExtractionBeaconComponent>();
        while (query.MoveNext(out var beaconUid, out var beacon))
        {
            if (!beacon.Deployed || beacon.CountdownActive)
                continue;

            beacon.CountdownActive = true;
            beacon.ExtractionEndTime = _timing.CurTime + beacon.ExtractionDelay;
            if (beacon.TargetShuttleCoordinates == null && ent.Comp.ReturnCoordinates is { } ret)
                beacon.TargetShuttleCoordinates = ret;

            _popup.PopupEntity(Loc.GetString(beacon.ConfirmedPopup), beaconUid);
            Dirty(beaconUid, beacon);
            found = true;
        }

        if (!found)
        {
            _popup.PopupEntity(Loc.GetString(NoActiveBeaconLoc), ent.Owner, ent.Owner);
            return;
        }

        if (ent.Comp.ConfirmExtractionActionEntity is { } action)
            _actions.RemoveAction(ent.Owner, action);

        args.Handled = true;
    }

    private void ExecuteBeaconExtraction(Entity<STMarsocExtractionBeaconComponent> ent, TransformComponent beaconXform)
    {
        ent.Comp.CountdownActive = false;

        if (_exposure100Triggered ||
            beaconXform.GridUid is not { } gridUid ||
            !TryComp<MapGridComponent>(gridUid, out var grid) ||
            !IsOpenSkyPlanetTile(gridUid, grid, beaconXform.Coordinates))
        {
            _popup.PopupCoordinates(Loc.GetString(CannotDeployUnderRoofLoc), beaconXform.Coordinates);
            CleanupBeaconFlares(ent);
            QueueDel(ent.Owner);
            return;
        }

        _extractionTargets.Clear();
        _lookup.GetEntitiesInRange(ent.Owner, ent.Comp.ExtractionRange, _extractionTargets, LookupFlags.Uncontained);

        var hasAliveMarsoc = false;
        foreach (var candidate in _extractionTargets)
        {
            if (!HasComp<STMarsocOperativeComponent>(candidate) || _mobState.IsDead(candidate))
                continue;

            var candidateXform = Transform(candidate);
            if (candidateXform.GridUid != gridUid || !IsOpenSkyPlanetTile(gridUid, grid, candidateXform.Coordinates))
                continue;

            hasAliveMarsoc = true;
            break;
        }

        if (!hasAliveMarsoc)
        {
            _popup.PopupCoordinates(Loc.GetString(ExtractionAbortedNoMarsocLoc), beaconXform.Coordinates);
            CleanupBeaconFlares(ent);
            QueueDel(ent.Owner);
            return;
        }

        var destCoords = ent.Comp.TargetShuttleCoordinates ?? FindFallbackShuttleCoordinates();
        if (destCoords is { } targetCoords && Exists(targetCoords.EntityId))
        {
            foreach (var target in _extractionTargets)
            {
                if (!HasComp<MobStateComponent>(target) ||
                    HasComp<ParaDroppingComponent>(target) ||
                    HasComp<SkyFallingComponent>(target))
                {
                    continue;
                }

                var targetXform = Transform(target);
                if (targetXform.GridUid != gridUid || !IsOpenSkyPlanetTile(gridUid, grid, targetXform.Coordinates))
                    continue;

                _transform.SetCoordinates(target, targetCoords);
            }
        }

        _popup.PopupCoordinates(Loc.GetString(ent.Comp.ExtractedPopup), beaconXform.Coordinates);
        CleanupBeaconFlares(ent);
        QueueDel(ent.Owner);
    }

    private EntityCoordinates? FindFallbackShuttleCoordinates()
    {
        var query = EntityQueryEnumerator<STMarsocOperativeComponent>();
        while (query.MoveNext(out _, out var op))
        {
            if (op.ReturnCoordinates is { } coords)
                return coords;
        }

        return null;
    }

    private void OnBeaconShutdown(Entity<STMarsocExtractionBeaconComponent> ent, ref ComponentShutdown args)
    {
        CleanupBeaconFlares(ent);
    }

    private void CleanupBeaconFlares(Entity<STMarsocExtractionBeaconComponent> ent)
    {
        foreach (var flare in ent.Comp.SpawnedFlares)
        {
            if (!TerminatingOrDeleted(flare))
                QueueDel(flare);
        }

        ent.Comp.SpawnedFlares.Clear();
    }

    private void OnDeploySquad(Entity<STMarsocOfficerComponent> ent, ref STMarsocDeploySquadActionEvent args)
    {
        if (args.Handled)
            return;

        if (TryDeploySquad(ent.Owner))
            args.Handled = true;
    }

    public bool TryDeploySquad(EntityUid officerUid)
    {
        if (!TryComp<STMarsocOfficerComponent>(officerUid, out var comp) || comp.SquadDeployed)
            return false;

        comp.SquadDeployed = true;
        if (comp.DeployActionEntity is { } action)
            _actions.RemoveAction(officerUid, action);

        Dirty(officerUid, comp);

        var officerTransform = Transform(officerUid);
        var fallbackCoords = _transform.GetMoverCoordinates(officerUid);
        var shuttleGrid = officerTransform.GridUid;

        _synthEligibleSpawners.Clear();
        _troopCoordsList.Clear();
        EntityCoordinates? medicCoords = null;
        EntityCoordinates? specCoords = null;

        if (shuttleGrid is { } gridUid)
        {
            var query = EntityQueryEnumerator<MetaDataComponent, TransformComponent>();
            while (query.MoveNext(out _, out var meta, out var xform))
            {
                if (xform.GridUid != gridUid || meta.EntityPrototype is not { } proto)
                    continue;

                if (proto.ID == comp.ShuttleMedicMarkerProto.Id)
                    medicCoords = xform.Coordinates;
                else if (proto.ID == comp.ShuttleEngineerMarkerProto.Id)
                    specCoords = xform.Coordinates;
                else if (proto.ID == comp.ShuttleTroopMarkerProto.Id)
                    _troopCoordsList.Add(xform.Coordinates);
            }
        }

        var tlCoords = _troopCoordsList.Count > 0 ? _troopCoordsList[0] : fallbackCoords;
        var tlSpawner = Spawn(comp.TeamLeaderSpawner, tlCoords);
        AttachPendingSpawn(tlSpawner, officerUid, false);

        var medSpawner = Spawn(comp.MedicSpawner, medicCoords ?? fallbackCoords);
        _synthEligibleSpawners.Add(medSpawner);

        var specProto = _random.Prob(comp.SpecialistSpawnChance)
            ? comp.SpecialistSpawner
            : comp.OperativeSpawner;
        var specSpawner = Spawn(specProto, specCoords ?? fallbackCoords);
        _synthEligibleSpawners.Add(specSpawner);

        for (var i = 0; i < comp.BaseOperativeCount; i++)
        {
            var opCoords = i + 1 < _troopCoordsList.Count ? _troopCoordsList[i + 1] : fallbackCoords;
            var opSpawner = Spawn(comp.OperativeSpawner, opCoords);
            _synthEligibleSpawners.Add(opSpawner);
        }

        EntityUid? chosenSynthSpawner = null;
        if (comp.SynthSlotAvailable && _synthEligibleSpawners.Count > 0)
        {
            chosenSynthSpawner = _random.Pick(_synthEligibleSpawners);
            comp.SynthSlotAvailable = false;
        }

        foreach (var spawner in _synthEligibleSpawners)
        {
            AttachPendingSpawn(spawner, officerUid, spawner == chosenSynthSpawner);
        }

        return true;
    }

    private void AttachPendingSpawn(EntityUid spawner, EntityUid officer, bool isSynth)
    {
        var pending = EnsureComp<STMarsocPendingSpawnComponent>(spawner);
        pending.OfficerUid = officer;
        pending.SpawnAsSynth = isSynth;
    }

    private void OnGhostSpawnerUsed(Entity<STMarsocPendingSpawnComponent> ent, ref GhostRoleSpawnerUsedEvent args)
    {
        if (!TryComp<STMarsocOperativeComponent>(args.Spawned, out var operative))
            return;

        operative.CommandingOfficer = ent.Comp.OfficerUid;

        if (ent.Comp.SpawnAsSynth && operative.CanBeSynth)
            operative.ForceSynth = true;

        operative.CallsignApplied = false;
        Dirty(args.Spawned, operative);
    }

    private void SendRoleDutiesToChat(Entity<STMarsocOperativeComponent> ent, ICommonSession session)
    {
        var rawCallsign = !string.IsNullOrWhiteSpace(ent.Comp.AssignedFullName)
            ? ent.Comp.AssignedFullName
            : MetaData(ent.Owner).EntityName;

        var safeCallsign = FormattedMessage.EscapeText(rawCallsign);
        var safeDuties = FormattedMessage.EscapeText(Loc.GetString(ent.Comp.RoleDutiesLoc));
        var message = Loc.GetString(RoleWelcomeChatFormatLoc, ("callsign", safeCallsign), ("duties", safeDuties));

        _chatManager.ChatMessageToOne(
            ChatChannel.Server,
            message,
            message,
            EntityUid.Invalid,
            false,
            session.Channel);
    }

    private void SendDeploymentBannerToSession(ICommonSession session, string? customOperationTitle)
    {
        var operation = !string.IsNullOrWhiteSpace(customOperationTitle)
            ? customOperationTitle
            : Loc.GetString(DefaultOperationNameLoc);

        var now = DateTime.UtcNow;
        var timeStr = now.ToString("HHmm", CultureInfo.InvariantCulture);
        var dateStr = $"{now.ToString("dd-MMM", CultureInfo.InvariantCulture).ToUpperInvariant()}-{LoreYear}";
        var numericDateStr = now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

        var formattedTime = Loc.GetString(
            "rmc-announcement-first-deployment-time",
            ("time", timeStr),
            ("date", dateStr),
            ("numericDate", numericDateStr));

        var planetName = ResolveActivePlanetName();
        var unitName = Loc.GetString(DefaultAssignmentLoc);

        RaiseNetworkEvent(new STMarsocDeploymentBannerEvent(operation, formattedTime, planetName, unitName), session);
    }

    private string ResolveActivePlanetName()
    {
        var planetQuery = EntityQueryEnumerator<RMCPlanetComponent, MetaDataComponent>();
        while (planetQuery.MoveNext(out _, out _, out var meta))
        {
            if (!string.IsNullOrWhiteSpace(meta.EntityName))
                return meta.EntityName;
        }

        var gridQuery = EntityQueryEnumerator<MapGridComponent, MetaDataComponent>();
        while (gridQuery.MoveNext(out var uid, out _, out var meta))
        {
            if (!IsPlanetGrid(uid, null) || string.IsNullOrWhiteSpace(meta.EntityName))
                continue;

            return meta.EntityName;
        }

        return "Classified Sector";
    }

    private void ApplySynthAndCallsign(Entity<STMarsocOperativeComponent> ent)
    {
        if (ent.Comp.CanBeSynth && !_roundSynthAssigned && (ent.Comp.ForceSynth || _random.Prob(0.35f)))
        {
            _roundSynthAssigned = true;
            ent.Comp.ForceSynth = true;
            EnsureComp<SynthComponent>(ent.Owner);
        }

        if (string.IsNullOrEmpty(ent.Comp.AssignedFullName))
        {
            if (!_prototypes.TryIndex(ent.Comp.CallsignDataset, out var dataset))
                return;

            _availableCallsigns.Clear();
            var count = dataset.Values.Count;
            var prefix = dataset.Values.Prefix;

            for (var i = 1; i <= count; i++)
            {
                var resolved = Loc.GetString($"{prefix}{i}");
                if (!_roundUsedCallsigns.Contains(resolved))
                    _availableCallsigns.Add(resolved);
            }

            var fallbackIndex = _random.Next(1, count + 1);
            var callsign = _availableCallsigns.Count > 0
                ? _random.Pick(_availableCallsigns)
                : Loc.GetString($"{prefix}{fallbackIndex}");

            _roundUsedCallsigns.Add(callsign);

            var rank = Loc.GetString(ent.Comp.RankPrefix);
            var isSynth = HasComp<SynthComponent>(ent.Owner);
            ent.Comp.AssignedFullName = isSynth
                ? $"{rank} {callsign} ({Loc.GetString(ent.Comp.SynthTag)})"
                : $"{rank} {callsign}";
        }

        EnforceCallsignName(ent);
    }

    private void EnforceCallsignName(Entity<STMarsocOperativeComponent> ent)
    {
        if (string.IsNullOrEmpty(ent.Comp.AssignedFullName))
            return;

        var fullName = ent.Comp.AssignedFullName;
        _metaData.SetEntityName(ent.Owner, fullName);

        if (_inventory.TryGetSlotEntity(ent.Owner, ent.Comp.IdSlotId, out var idUid))
            _idCard.TryChangeFullName(idUid.Value, fullName);

        if (TryComp<MindContainerComponent>(ent.Owner, out var mindContainer) &&
            mindContainer.Mind is { } mindId &&
            TryComp<MindComponent>(mindId, out var mind))
        {
            mind.CharacterName = fullName;
        }
    }
}