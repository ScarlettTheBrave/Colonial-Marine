using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._TacticalOps.Marsoc;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class TOMarsocBunkerControllerComponent : Component
{
    [DataField]
    public EntProtoId SupervisorSpawnerProto = "CMRandomHumanoidCorporateExecutiveSupervisor";

    [DataField]
    public EntProtoId AssistantSpawnerProto = "CMRandomHumanoidCorporateAssistantManager";

    [DataField, AutoNetworkedField]
    public bool AlwaysSpawnInTestMode = true;

    [DataField, AutoNetworkedField]
    public float SpawnChanceWhenMarsocActive = 1.0f;

    [DataField]
    public TimeSpan ButtonOpenDelay = TimeSpan.FromMinutes(3);

    [DataField]
    public TimeSpan AutoOpenRoundTime = TimeSpan.FromMinutes(10);

    [DataField]
    public HashSet<string> ControlledLockdownIds = new()
    {
        "North Bunker Lockdown",
        "South Bunker Lockdown",
        "Bunker Main Doors Lockdown",
        "Bunker Shutters",
        "Elevator Lockdown",
        "Bunker Elevator Lockdown"
    };

    [DataField]
    public LocId CountdownStartedPopup = "to-marsoc-bunker-countdown-started";

    [DataField]
    public LocId CountdownAlreadyActivePopup = "to-marsoc-bunker-countdown-active";

    [DataField]
    public LocId ShuttersOpenedAnnouncement = "to-marsoc-bunker-shutters-opened";

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField, AutoNetworkedField]
    public TimeSpan? ScheduledOpenTime;

    [DataField, AutoNetworkedField]
    public bool VIPSpawnChecked;

    [DataField, AutoNetworkedField]
    public bool CountdownTriggeredByButton;

    [DataField, AutoNetworkedField]
    public bool ShuttersOpened;
}


[RegisterComponent, NetworkedComponent]
public sealed partial class TOMarsocBunkerButtonComponent : Component
{
}


[RegisterComponent, NetworkedComponent]
public sealed partial class TOMarsocCryoPodObjectiveComponent : Component
{
    [DataField]
    public bool DataExtracted;

    [DataField]
    public EntProtoId ExtractedDiskProto = "TOMarsocWeYaResearchData";
}


[RegisterComponent, NetworkedComponent]
public sealed partial class TOMarsocCorporateCaseComponent : Component
{
    [DataField]
    public bool OpenedOnce;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class TOMarsocCommandProtocolsComponent : Component
{
    [DataField]
    public bool Used;
}

[RegisterComponent]
public sealed partial class TOMarsocTrackedTargetComponent : Component
{
    [DataField]
    public EntityUid? LastMarsocAttacker;
}