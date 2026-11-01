using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._TacticalOps.Marsoc;

[Serializable, NetSerializable]
public enum TOMarsocBriefingUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public enum TOMarsocFlightPhase : byte
{
    DockedInDeepSpace,
    InTransit,
    ArrivedInOrbit
}

[Serializable, NetSerializable]
public sealed class TOMarsocDeploymentBannerEvent : EntityEventArgs
{
    public readonly string OperationName;
    public readonly string TimeAndDate;
    public readonly string PlanetName;
    public readonly string UnitAssignment;

    public TOMarsocDeploymentBannerEvent(
        string operationName,
        string timeAndDate,
        string planetName,
        string unitAssignment)
    {
        OperationName = operationName;
        TimeAndDate = timeAndDate;
        PlanetName = planetName;
        UnitAssignment = unitAssignment;
    }
}

[Serializable, NetSerializable]
public sealed class TOMarsocParachuteDropEvent : EntityEventArgs
{
    public readonly NetEntity Target;
    public readonly float Duration;
    public readonly float FallHeight;

    public TOMarsocParachuteDropEvent(NetEntity target, float duration, float fallHeight)
    {
        Target = target;
        Duration = duration;
        FallHeight = fallHeight;
    }
}

[Serializable, NetSerializable]
public sealed class TOMarsocBriefingBuiState : BoundUserInterfaceState
{
    public readonly string OperationTitle;
    public readonly string SpeakerFullNameAndRank;
    public readonly string PortraitTexturePath;
    public readonly string DetectedIntelSummary;
    public readonly List<string> DialogueSegments;
    public readonly List<string> Objectives;
    public readonly string SelectedSectorName;
    public readonly TOMarsocFlightPhase FlightPhase;
    public readonly int RemainingFlightSeconds;
    public readonly bool SquadDeployed;
    public readonly bool SectorLocked;
    public readonly bool ParadropActive;

    public TOMarsocBriefingBuiState(
        string operationTitle,
        string speakerFullNameAndRank,
        string portraitTexturePath,
        string detectedIntelSummary,
        List<string> dialogueSegments,
        List<string> objectives,
        string selectedSectorName,
        TOMarsocFlightPhase flightPhase,
        int remainingFlightSeconds,
        bool squadDeployed,
        bool sectorLocked,
        bool paradropActive)
    {
        OperationTitle = operationTitle;
        SpeakerFullNameAndRank = speakerFullNameAndRank;
        PortraitTexturePath = portraitTexturePath;
        DetectedIntelSummary = detectedIntelSummary;
        DialogueSegments = dialogueSegments;
        Objectives = objectives;
        SelectedSectorName = selectedSectorName;
        FlightPhase = flightPhase;
        RemainingFlightSeconds = remainingFlightSeconds;
        SquadDeployed = squadDeployed;
        SectorLocked = sectorLocked;
        ParadropActive = paradropActive;
    }
}

[Serializable, NetSerializable]
public sealed class TOMarsocBriefingDeployMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class TOMarsocBriefingLaunchFlightMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class TOMarsocBriefingCycleSectorMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class TOMarsocBriefingLockSectorMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class TOMarsocBriefingToggleParadropMessage : BoundUserInterfaceMessage;

[DataDefinition]
public sealed partial class TOMarsocSubInsertObjective
{
    [DataField(required: true)]
    public string MarkerPrototype;

    [DataField(required: true)]
    public LocId ObjectiveLoc;
}

[DataDefinition]
public sealed partial class TOMarsocMissionEntry
{
    [DataField]
    public string? MapKeyword;

    [DataField]
    public LocId InsertDisplayName = "to-marsoc-intel-standard";

    [DataField]
    public List<string> InsertKeywords = new();

    [DataField]
    public EntProtoId SpeakerProto = "TOMarsocHandlerDummy";

    [DataField]
    public LocId SpeakerName = "to-marsoc-speaker-handler";

    [DataField(required: true)]
    public LocId Title;

    [DataField(required: true)]
    public LocId Dialogue;

    [DataField]
    public List<LocId> Objectives = new();

    [DataField]
    public List<LocId> MandatoryObjectives = new();

    [DataField]
    public List<LocId> OptionalObjectivesPool = new();
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentPause]
[Access(typeof(SharedTOMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class TOMarsocBriefingConsoleComponent : Component
{
    [DataField]
    public List<string> PortraitTextures = new()
    {
        "/Textures/_TacticalOps/Interface/MarsocPortraits/speaker_1.png",
        "/Textures/_TacticalOps/Interface/MarsocPortraits/speaker_2.png",
        "/Textures/_TacticalOps/Interface/MarsocPortraits/speaker_3.png"
    };

    [DataField]
    public List<LocId> SpeakerRanksAndNames = new()
    {
        "to-marsoc-speaker-random-1",
        "to-marsoc-speaker-random-2",
        "to-marsoc-speaker-random-3",
        "to-marsoc-speaker-random-4"
    };

    [DataField]
    public string AssignedPortraitPath = "/Textures/_TacticalOps/Interface/MarsocPortraits/speaker_1.png";

    [DataField]
    public string AssignedSpeakerTitle = string.Empty;

    [DataField]
    public int SelectedMissionIndex;

    [DataField]
    public List<LocId> GeneratedObjectives = new();

    [DataField]
    public string DetectedIntelText = string.Empty;

    [DataField]
    public TOMarsocFlightPhase FlightPhase = TOMarsocFlightPhase.DockedInDeepSpace;

    [DataField]
    public TimeSpan FlightDuration = TimeSpan.FromSeconds(20);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan FlightArrivalTime;

    [DataField]
    public bool SectorLocked;

    [DataField]
    public string FlightParallax = "FastSpace";

    [DataField]
    public string OrbitParallax = "Default";

    [DataField]
    public bool MissionInitialized;

    [DataField]
    public List<TOMarsocMissionEntry> MissionPool = new();

    [DataField]
    public List<TOMarsocSubInsertObjective> SubInsertObjectives = new();

    [DataField]
    public LocId AccessDeniedPopup = "to-marsoc-briefing-access-denied";
}