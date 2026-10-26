using Content.Shared.Actions;
using Content.Shared.Dataset;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Stories.Marsoc;

public sealed partial class STMarsocDeploySquadActionEvent : InstantActionEvent;

public sealed partial class STMarsocToggleParadropActionEvent : InstantActionEvent;

public sealed partial class STMarsocConfirmExtractionActionEvent : InstantActionEvent;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedSTMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class STMarsocOfficerComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId DeployActionId = "STActionMarsocDeploySquad";

    [DataField, AutoNetworkedField]
    public EntityUid? DeployActionEntity;

    [DataField, AutoNetworkedField]
    public EntProtoId ToggleParadropActionId = "STActionMarsocToggleParadrop";

    [DataField, AutoNetworkedField]
    public EntityUid? ToggleParadropActionEntity;

    [DataField, AutoNetworkedField]
    public bool SquadDeployed;

    [DataField, AutoNetworkedField]
    public bool ParadropActive;

    [DataField]
    public EntProtoId TeamLeaderSpawner = "STSpawnerMarsocTeamLeader";

    [DataField]
    public EntProtoId MedicSpawner = "STSpawnerMarsocMedic";

    [DataField]
    public EntProtoId OperativeSpawner = "STSpawnerMarsocOperative";

    [DataField]
    public EntProtoId SpecialistSpawner = "STSpawnerMarsocSpecialist";

    [DataField]
    public EntProtoId ShuttleMedicMarkerProto = "RMCERTSpawnMarkerMedic";

    [DataField]
    public EntProtoId ShuttleEngineerMarkerProto = "RMCERTSpawnMarkerEngineer";

    [DataField]
    public EntProtoId ShuttleTroopMarkerProto = "RMCERTSpawnMarker";

    [DataField]
    public int BaseOperativeCount = 2;

    [DataField]
    public float SpecialistSpawnChance = 0.5f;

    [DataField, AutoNetworkedField]
    public bool SynthSlotAvailable = true;

    [DataField]
    public HashSet<string> AssignedCallsigns = new();
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedSTMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class STMarsocOperativeComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public LocId RankPrefix;

    [DataField(required: true)]
    public ProtoId<LocalizedDatasetPrototype> CallsignDataset;

    [DataField]
    public LocId RoleDutiesLoc = "st-ghost-role-marsoc-operative-desc";

    [DataField, AutoNetworkedField]
    public bool CanBeSynth = true;

    [DataField, AutoNetworkedField]
    public bool ForceSynth;

    [DataField, AutoNetworkedField]
    public bool CanDeployBeacon;

    [DataField, AutoNetworkedField]
    public bool CanConfirmExtraction;

    [DataField]
    public EntProtoId ConfirmExtractionActionId = "STActionMarsocConfirmExtraction";

    [DataField, AutoNetworkedField]
    public EntityUid? ConfirmExtractionActionEntity;

    [DataField]
    public LocId SynthTag = "st-marsoc-synth-tag";

    [DataField]
    public string IdSlotId = "id";

    [DataField, AutoNetworkedField]
    public EntityUid? CommandingOfficer;

    [DataField, AutoNetworkedField]
    public EntityUid? HomeShuttleGrid;

    [DataField, AutoNetworkedField]
    public EntityCoordinates? ReturnCoordinates;

    [DataField, AutoNetworkedField]
    public string? AssignedFullName;

    [DataField]
    public bool CallsignApplied;

    [DataField]
    public bool DutiesPrinted;

    [DataField]
    public bool ShuttleDoorsHardened;

    [DataField]
    public float SpaceInstaKillDamage = 5000f;
}

[RegisterComponent]
[Access(typeof(SharedSTMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class STMarsocPendingSpawnComponent : Component
{
    [DataField]
    public EntityUid? OfficerUid;

    [DataField]
    public bool SpawnAsSynth;
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(SharedSTMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class STMarsocExtractionBeaconComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Deployed;

    [DataField, AutoNetworkedField]
    public bool CountdownActive;

    [DataField]
    public int HalfExtent = 2;

    [DataField]
    public float ExtractionRange = 2.8f;

    [DataField]
    public TimeSpan ExtractionDelay = TimeSpan.FromSeconds(15);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField, AutoNetworkedField]
    public TimeSpan ExtractionEndTime;

    [DataField]
    public EntProtoId FlarePrototype = "STMarsocExtractionFlare";

    [DataField]
    public List<EntityUid> SpawnedFlares = new();

    [DataField]
    public EntityCoordinates? TargetShuttleCoordinates;

    [DataField]
    public LocId NoPermissionPopup = "st-marsoc-beacon-no-permission";

    [DataField]
    public LocId DeployedPopup = "st-marsoc-beacon-deployed";

    [DataField]
    public LocId ConfirmedPopup = "st-marsoc-beacon-confirmed";

    [DataField]
    public LocId ExtractedPopup = "st-marsoc-beacon-extracted";
}