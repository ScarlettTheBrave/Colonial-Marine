using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Stories.Marsoc;

[Serializable, NetSerializable]
public enum STMarsocBriefingUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public enum STMarsocFlightPhase : byte
{
    DockedInDeepSpace,
    InTransit,
    ArrivedInOrbit
}

[Serializable, NetSerializable]
public sealed class STMarsocDeploymentBannerEvent : EntityEventArgs
{
    public readonly string OperationName;
    public readonly string TimeAndDate;
    public readonly string PlanetName;
    public readonly string UnitAssignment;

    public STMarsocDeploymentBannerEvent(
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
public sealed class STMarsocParachuteDropEvent : EntityEventArgs
{
    public readonly NetEntity Target;
    public readonly float Duration;
    public readonly float FallHeight;

    public STMarsocParachuteDropEvent(NetEntity target, float duration, float fallHeight)
    {
        Target = target;
        Duration = duration;
        FallHeight = fallHeight;
    }
}

[Serializable, NetSerializable]
public sealed class STMarsocBriefingBuiState : BoundUserInterfaceState
{
    public readonly string OperationTitle;
    public readonly string SpeakerFullNameAndRank;
    public readonly string PortraitTexturePath;
    public readonly string DetectedIntelSummary;
    public readonly List<string> DialogueSegments;
    public readonly List<string> Objectives;
    public readonly string SelectedSectorName;
    public readonly STMarsocFlightPhase FlightPhase;
    public readonly int RemainingFlightSeconds;
    public readonly bool SquadDeployed;
    public readonly bool SectorLocked;
    public readonly bool ParadropActive;

    public STMarsocBriefingBuiState(
        string operationTitle,
        string speakerFullNameAndRank,
        string portraitTexturePath,
        string detectedIntelSummary,
        List<string> dialogueSegments,
        List<string> objectives,
        string selectedSectorName,
        STMarsocFlightPhase flightPhase,
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
public sealed class STMarsocBriefingDeployMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class STMarsocBriefingLaunchFlightMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class STMarsocBriefingCycleSectorMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class STMarsocBriefingLockSectorMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class STMarsocBriefingToggleParadropMessage : BoundUserInterfaceMessage;

[DataDefinition]
public sealed partial class STMarsocSubInsertObjective
{
    [DataField(required: true)]
    public string MarkerPrototype;

    [DataField(required: true)]
    public LocId ObjectiveLoc;
}

[DataDefinition]
public sealed partial class STMarsocMissionEntry
{
    [DataField]
    public string? MapKeyword;

    [DataField]
    public LocId InsertDisplayName = "st-marsoc-intel-standard";

    [DataField]
    public List<string> InsertKeywords = new();

    [DataField]
    public EntProtoId SpeakerProto = "STMarsocHandlerDummy";

    [DataField]
    public LocId SpeakerName = "st-marsoc-speaker-handler";

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
[Access(typeof(SharedSTMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class STMarsocBriefingConsoleComponent : Component
{
    [DataField]
    public List<string> PortraitTextures = new()
    {
        "/Textures/_Stories/Interface/MarsocPortraits/speaker_1.png",
        "/Textures/_Stories/Interface/MarsocPortraits/speaker_2.png",
        "/Textures/_Stories/Interface/MarsocPortraits/speaker_3.png"
    };

    [DataField]
    public List<LocId> SpeakerRanksAndNames = new()
    {
        "st-marsoc-speaker-random-1",
        "st-marsoc-speaker-random-2",
        "st-marsoc-speaker-random-3",
        "st-marsoc-speaker-random-4"
    };

    [DataField]
    public string AssignedPortraitPath = "/Textures/_Stories/Interface/MarsocPortraits/speaker_1.png";

    [DataField]
    public string AssignedSpeakerTitle = string.Empty;

    [DataField]
    public int SelectedMissionIndex;

    [DataField]
    public List<LocId> GeneratedObjectives = new();

    [DataField]
    public string DetectedIntelText = string.Empty;

    [DataField]
    public STMarsocFlightPhase FlightPhase = STMarsocFlightPhase.DockedInDeepSpace;

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
    public List<STMarsocMissionEntry> MissionPool = new();

    [DataField]
    public List<STMarsocSubInsertObjective> SubInsertObjectives = new();

    [DataField]
    public LocId AccessDeniedPopup = "st-marsoc-briefing-access-denied";
}