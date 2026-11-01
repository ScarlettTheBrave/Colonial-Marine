using Content.Shared.Actions;
using Content.Shared.Dataset;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._TacticalOps.Marsoc;

public sealed partial class TOMarsocDeploySquadActionEvent : InstantActionEvent;

public sealed partial class TOMarsocToggleParadropActionEvent : InstantActionEvent;

public sealed partial class TOMarsocConfirmExtractionActionEvent : InstantActionEvent;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedTOMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class TOMarsocOfficerComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId DeployActionId = "TOActionMarsocDeploySquad";

    [DataField, AutoNetworkedField]
    public EntityUid? DeployActionEntity;

    [DataField, AutoNetworkedField]
    public EntProtoId ToggleParadropActionId = "TOActionMarsocToggleParadrop";

    [DataField, AutoNetworkedField]
    public EntityUid? ToggleParadropActionEntity;

    [DataField, AutoNetworkedField]
    public bool SquadDeployed;

    [DataField, AutoNetworkedField]
    public bool ParadropActive;

    [DataField]
    public EntProtoId TeamLeaderSpawner = "TOSpawnerMarsocTeamLeader";

    [DataField]
    public EntProtoId MedicSpawner = "TOSpawnerMarsocMedic";

    [DataField]
    public EntProtoId OperativeSpawner = "TOSpawnerMarsocOperative";

    [DataField]
    public EntProtoId SpecialistSpawner = "TOSpawnerMarsocSpecialist";

    [DataField]
    public EntProtoId ShuttleMedicMarkerProto = "CMERTSpawnMarkerMedic";

    [DataField]
    public EntProtoId ShuttleEngineerMarkerProto = "CMERTSpawnMarkerEngineer";

    [DataField]
    public EntProtoId ShuttleTroopMarkerProto = "CMERTSpawnMarker";

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
[Access(typeof(SharedTOMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class TOMarsocOperativeComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public LocId RankPrefix;

    [DataField(required: true)]
    public ProtoId<LocalizedDatasetPrototype> CallsignDataset;

    [DataField]
    public LocId RoleDutiesLoc = "to-ghost-role-marsoc-operative-desc";

    [DataField, AutoNetworkedField]
    public bool CanBeSynth = true;

    [DataField, AutoNetworkedField]
    public bool ForceSynth;

    [DataField, AutoNetworkedField]
    public bool CanDeployBeacon;

    [DataField, AutoNetworkedField]
    public bool CanConfirmExtraction;

    [DataField]
    public EntProtoId ConfirmExtractionActionId = "TOActionMarsocConfirmExtraction";

    [DataField, AutoNetworkedField]
    public EntityUid? ConfirmExtractionActionEntity;

    [DataField]
    public LocId SynthTag = "to-marsoc-synth-tag";

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
[Access(typeof(SharedTOMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class TOMarsocPendingSpawnComponent : Component
{
    [DataField]
    public EntityUid? OfficerUid;

    [DataField]
    public bool SpawnAsSynth;
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(SharedTOMarsocSystem), Other = AccessPermissions.ReadWriteExecute)]
public sealed partial class TOMarsocExtractionBeaconComponent : Component
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
    public EntProtoId FlarePrototype = "TOMarsocExtractionFlare";

    [DataField]
    public List<EntityUid> SpawnedFlares = new();

    [DataField]
    public EntityCoordinates? TargetShuttleCoordinates;

    [DataField]
    public LocId NoPermissionPopup = "to-marsoc-beacon-no-permission";

    [DataField]
    public LocId DeployedPopup = "to-marsoc-beacon-deployed";

    [DataField]
    public LocId ConfirmedPopup = "to-marsoc-beacon-confirmed";

    [DataField]
    public LocId ExtractedPopup = "to-marsoc-beacon-extracted";
}