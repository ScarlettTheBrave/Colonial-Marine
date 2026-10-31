using Content.Server.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared._Stories.Marsoc;
using Robust.Server.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Stories.Marsoc;

public sealed class STMarsocBriefingSystem : SharedSTMarsocSystem
{
    [Dependency] private readonly STMarsocSystem _marsoc = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    private static readonly LocId IntelStandardLoc = "st-marsoc-intel-standard";
    private static readonly LocId FlightNotArrivedLoc = "st-marsoc-flight-not-arrived";
    private static readonly LocId SectorNotLockedLoc = "st-marsoc-sector-not-locked";

    private readonly List<string> _objectivesBuffer = new();
    private readonly List<string> _segmentsBuffer = new();
    private readonly List<LocId> _optionalPoolBuffer = new();
    private readonly HashSet<string> _planetProtoIds = new(StringComparer.OrdinalIgnoreCase);

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<STMarsocBriefingConsoleComponent, MapInitEvent>(OnConsoleMapInit);
        SubscribeLocalEvent<STMarsocBriefingConsoleComponent, BoundUIOpenedEvent>(OnConsoleUiOpened);
        SubscribeLocalEvent<STMarsocBriefingConsoleComponent, STMarsocBriefingDeployMessage>(OnDeployMessage);
        SubscribeLocalEvent<STMarsocBriefingConsoleComponent, STMarsocBriefingLaunchFlightMessage>(OnLaunchFlightMessage);
        SubscribeLocalEvent<STMarsocBriefingConsoleComponent, STMarsocBriefingCycleSectorMessage>(OnCycleSectorMessage);
        SubscribeLocalEvent<STMarsocBriefingConsoleComponent, STMarsocBriefingLockSectorMessage>(OnLockSectorMessage);
        SubscribeLocalEvent<STMarsocBriefingConsoleComponent, STMarsocBriefingToggleParadropMessage>(OnToggleParadropMessage);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<STMarsocBriefingConsoleComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            if (comp.FlightPhase != STMarsocFlightPhase.InTransit)
                continue;

            var gridUid = xform.GridUid;
            var ftlStillActive = gridUid != null && HasComp<FTLComponent>(gridUid.Value);

            if (curTime >= comp.FlightArrivalTime && !ftlStillActive)
            {
                comp.FlightPhase = STMarsocFlightPhase.ArrivedInOrbit;
                if (gridUid is { } validGrid)
                {
                    _marsoc.CompleteShuttleOrbitalArrival(validGrid, comp.FlightParallax);
                    if (comp.SectorLocked)
                        _marsoc.SetShuttleParadropState(validGrid, true);
                }

                Dirty(uid, comp);
                UpdateConsoleUi((uid, comp));
            }
            else if (_ui.IsUiOpen(uid, STMarsocBriefingUiKey.Key))
            {
                UpdateConsoleUi((uid, comp));
            }
        }
    }

    private void OnConsoleMapInit(Entity<STMarsocBriefingConsoleComponent> ent, ref MapInitEvent args)
    {
        InitializeMissionAndSpeaker(ent);
    }

    private void OnConsoleUiOpened(Entity<STMarsocBriefingConsoleComponent> ent, ref BoundUIOpenedEvent args)
    {
        InitializeMissionAndSpeaker(ent);
        UpdateConsoleUi(ent);
    }

    private void InitializeMissionAndSpeaker(Entity<STMarsocBriefingConsoleComponent> ent)
    {
        if (ent.Comp.MissionInitialized || ent.Comp.MissionPool.Count == 0)
            return;

        ent.Comp.MissionInitialized = true;

        if (ent.Comp.PortraitTextures.Count > 0)
            ent.Comp.AssignedPortraitPath = _random.Pick(ent.Comp.PortraitTextures);

        if (ent.Comp.SpeakerRanksAndNames.Count > 0)
            ent.Comp.AssignedSpeakerTitle = Loc.GetString(_random.Pick(ent.Comp.SpeakerRanksAndNames));

        var consoleGrid = Transform(ent.Owner).GridUid;
        _marsoc.EnsureSafePlanetSectors(consoleGrid);

        _planetProtoIds.Clear();
        var entQuery = EntityQueryEnumerator<MetaDataComponent, TransformComponent>();
        while (entQuery.MoveNext(out _, out var meta, out var xform))
        {
            if (xform.GridUid is not { } gridUid || !_marsoc.IsPlanetGrid(gridUid, consoleGrid))
                continue;

            if (meta.EntityPrototype?.ID is { } protoId)
                _planetProtoIds.Add(protoId);
        }

        var selectedMissionIndex = -1;
        for (var i = 0; i < ent.Comp.MissionPool.Count; i++)
        {
            var entry = ent.Comp.MissionPool[i];
            if (entry.InsertKeywords.Count == 0)
                continue;

            var allMatched = true;
            foreach (var requiredProto in entry.InsertKeywords)
            {
                if (!_planetProtoIds.Contains(requiredProto))
                {
                    allMatched = false;
                    break;
                }
            }

            if (allMatched)
            {
                selectedMissionIndex = i;
                ent.Comp.DetectedIntelText = Loc.GetString(entry.InsertDisplayName);
                break;
            }
        }

        if (selectedMissionIndex < 0)
        {
            var gridQuery = EntityQueryEnumerator<MapGridComponent, MetaDataComponent>();
            while (gridQuery.MoveNext(out var gridUid, out _, out var meta))
            {
                if (!_marsoc.IsPlanetGrid(gridUid, consoleGrid))
                    continue;

                var mapName = meta.EntityName;
                var protoId = meta.EntityPrototype?.ID ?? string.Empty;

                for (var i = 0; i < ent.Comp.MissionPool.Count; i++)
                {
                    var keyword = ent.Comp.MissionPool[i].MapKeyword;
                    if (string.IsNullOrEmpty(keyword))
                        continue;

                    if (mapName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        protoId.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedMissionIndex = i;
                        ent.Comp.DetectedIntelText = Loc.GetString(ent.Comp.MissionPool[i].InsertDisplayName);
                        break;
                    }
                }

                if (selectedMissionIndex >= 0)
                    break;
            }
        }

        if (selectedMissionIndex < 0)
        {
            selectedMissionIndex = 0;
            ent.Comp.DetectedIntelText = Loc.GetString(IntelStandardLoc);
        }

        ent.Comp.SelectedMissionIndex = selectedMissionIndex;
        GenerateMissionObjectives(ent);
    }

    private void GenerateMissionObjectives(Entity<STMarsocBriefingConsoleComponent> ent)
    {
        ent.Comp.GeneratedObjectives.Clear();
        var index = Math.Clamp(ent.Comp.SelectedMissionIndex, 0, ent.Comp.MissionPool.Count - 1);
        var entry = ent.Comp.MissionPool[index];

        var primaryList = entry.MandatoryObjectives.Count > 0 ? entry.MandatoryObjectives : entry.Objectives;
        foreach (var mandatory in primaryList)
        {
            if (ent.Comp.GeneratedObjectives.Count < 4)
                ent.Comp.GeneratedObjectives.Add(mandatory);
        }

        foreach (var subInsert in ent.Comp.SubInsertObjectives)
        {
            if (ent.Comp.GeneratedObjectives.Count >= 4)
                break;

            if (_planetProtoIds.Contains(subInsert.MarkerPrototype) &&
                !ent.Comp.GeneratedObjectives.Contains(subInsert.ObjectiveLoc))
            {
                ent.Comp.GeneratedObjectives.Add(subInsert.ObjectiveLoc);
            }
        }

        _optionalPoolBuffer.Clear();
        _optionalPoolBuffer.AddRange(entry.OptionalObjectivesPool);
        _random.Shuffle(_optionalPoolBuffer);

        var targetTotal = _random.Next(3, 5);
        foreach (var opt in _optionalPoolBuffer)
        {
            if (ent.Comp.GeneratedObjectives.Count >= targetTotal)
                break;

            if (!ent.Comp.GeneratedObjectives.Contains(opt))
                ent.Comp.GeneratedObjectives.Add(opt);
        }

        _marsoc.RefreshAllMarsocCharacterBriefings();
    }

    private void OnDeployMessage(Entity<STMarsocBriefingConsoleComponent> ent, ref STMarsocBriefingDeployMessage args)
    {
        if (!TryValidateOfficer(ent, args.Actor, out var officerUid))
            return;

        _marsoc.TryDeploySquad(officerUid);
        UpdateConsoleUi(ent);
    }

    private void OnLaunchFlightMessage(Entity<STMarsocBriefingConsoleComponent> ent, ref STMarsocBriefingLaunchFlightMessage args)
    {
        if (!TryValidateOfficer(ent, args.Actor, out _))
            return;

        if (ent.Comp.FlightPhase != STMarsocFlightPhase.DockedInDeepSpace)
            return;

        const float startupSeconds = 5.5f;
        var hyperspaceSeconds = Math.Max(10f, (float)ent.Comp.FlightDuration.TotalSeconds);

        if (Transform(ent.Owner).GridUid is { } shuttleGrid)
            _marsoc.StartShuttleOrbitalFlight(shuttleGrid, ent.Comp.FlightParallax, startupSeconds, hyperspaceSeconds);

        ent.Comp.FlightPhase = STMarsocFlightPhase.InTransit;
        ent.Comp.FlightArrivalTime = _timing.CurTime + TimeSpan.FromSeconds(startupSeconds + hyperspaceSeconds);
        Dirty(ent);

        UpdateConsoleUi(ent);
    }

    private void OnCycleSectorMessage(Entity<STMarsocBriefingConsoleComponent> ent, ref STMarsocBriefingCycleSectorMessage args)
    {
        if (!TryValidateOfficer(ent, args.Actor, out _))
            return;

        if (ent.Comp.SectorLocked)
            return;

        var shuttleGrid = Transform(ent.Owner).GridUid;
        _marsoc.CycleDropSector(shuttleGrid);
        UpdateConsoleUi(ent);
    }

    private void OnLockSectorMessage(Entity<STMarsocBriefingConsoleComponent> ent, ref STMarsocBriefingLockSectorMessage args)
    {
        if (!TryValidateOfficer(ent, args.Actor, out _))
            return;

        ent.Comp.SectorLocked = !ent.Comp.SectorLocked;
        Dirty(ent);

        if (Transform(ent.Owner).GridUid is { } shuttleGrid &&
            ent.Comp.FlightPhase == STMarsocFlightPhase.ArrivedInOrbit)
        {
            _marsoc.SetShuttleParadropState(shuttleGrid, ent.Comp.SectorLocked);
        }

        UpdateConsoleUi(ent);
    }

    private void OnToggleParadropMessage(Entity<STMarsocBriefingConsoleComponent> ent, ref STMarsocBriefingToggleParadropMessage args)
    {
        if (!TryValidateOfficer(ent, args.Actor, out var officerUid))
            return;

        if (ent.Comp.FlightPhase != STMarsocFlightPhase.ArrivedInOrbit)
        {
            _popup.PopupEntity(Loc.GetString(FlightNotArrivedLoc), officerUid, officerUid);
            return;
        }

        if (!ent.Comp.SectorLocked)
        {
            _popup.PopupEntity(Loc.GetString(SectorNotLockedLoc), officerUid, officerUid);
            return;
        }

        _marsoc.TryToggleParadrop(officerUid);
        UpdateConsoleUi(ent);
    }

    private bool TryValidateOfficer(
        Entity<STMarsocBriefingConsoleComponent> ent,
        EntityUid? actor,
        out EntityUid officerUid)
    {
        if (actor is { } uid && HasComp<STMarsocOfficerComponent>(uid))
        {
            officerUid = uid;
            return true;
        }

        if (actor is { } deniedUser)
            _popup.PopupEntity(Loc.GetString(ent.Comp.AccessDeniedPopup), deniedUser, deniedUser);

        officerUid = default;
        return false;
    }

    private void UpdateConsoleUi(Entity<STMarsocBriefingConsoleComponent> ent)
    {
        var title = string.Empty;
        _objectivesBuffer.Clear();
        _segmentsBuffer.Clear();

        var officerCallsign = _marsoc.GetActiveOfficerCallsign();

        if (ent.Comp.MissionPool.Count > 0)
        {
            var index = Math.Clamp(ent.Comp.SelectedMissionIndex, 0, ent.Comp.MissionPool.Count - 1);
            var entry = ent.Comp.MissionPool[index];

            title = Loc.GetString(entry.Title);

            var rawDialogue = Loc.GetString(entry.Dialogue, ("officer", officerCallsign));
            SplitIntoTransmissionSegments(rawDialogue, _segmentsBuffer);

            foreach (var objKey in ent.Comp.GeneratedObjectives)
            {
                _objectivesBuffer.Add(Loc.GetString(objKey));
            }
        }

        var baseIntel = !string.IsNullOrWhiteSpace(ent.Comp.DetectedIntelText)
            ? ent.Comp.DetectedIntelText
            : Loc.GetString(IntelStandardLoc);

        var exposurePercent = _marsoc.GetExposurePercentText();
        var intelSummary = $"{baseIntel} | {exposurePercent}";

        var squadDeployed = false;
        var paradropActive = false;

        var officerQuery = EntityQueryEnumerator<STMarsocOfficerComponent>();
        while (officerQuery.MoveNext(out _, out var officer))
        {
            squadDeployed = officer.SquadDeployed;
            paradropActive = officer.ParadropActive;
            break;
        }

        var remainingSeconds = 0;
        if (ent.Comp.FlightPhase == STMarsocFlightPhase.InTransit)
        {
            var rem = ent.Comp.FlightArrivalTime - _timing.CurTime;
            remainingSeconds = Math.Max(0, (int)Math.Ceiling(rem.TotalSeconds));
        }

        var shuttleGrid = Transform(ent.Owner).GridUid;
        var sectorName = _marsoc.GetCurrentDropSectorName(shuttleGrid);

        var state = new STMarsocBriefingBuiState(
            title,
            ent.Comp.AssignedSpeakerTitle,
            ent.Comp.AssignedPortraitPath,
            intelSummary,
            new List<string>(_segmentsBuffer),
            new List<string>(_objectivesBuffer),
            sectorName,
            ent.Comp.FlightPhase,
            remainingSeconds,
            squadDeployed,
            ent.Comp.SectorLocked,
            paradropActive);

        _ui.SetUiState(ent.Owner, STMarsocBriefingUiKey.Key, state);
    }

    private static void SplitIntoTransmissionSegments(string text, List<string> output)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        var sentences = text.Split(". ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var current = string.Empty;

        foreach (var sentence in sentences)
        {
            var formatted = sentence.EndsWith('.') ? sentence : $"{sentence}.";
            if (current.Length > 0 && current.Length + formatted.Length > 150)
            {
                output.Add(current);
                current = formatted;
            }
            else
            {
                current = current.Length == 0 ? formatted : $"{current} {formatted}";
            }
        }

        if (!string.IsNullOrWhiteSpace(current))
            output.Add(current);
    }
}