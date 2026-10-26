using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._Stories.MARSOC;

[Serializable, NetSerializable]
public enum STMarsocShuttlePhase : byte
{
   
    PreFlight = 0,


    InTransit = 1,

 
    InOrbit = 2
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class STMarsocShuttleComponent : Component
{
    [DataField, AutoNetworkedField]
    public STMarsocShuttlePhase Phase = STMarsocShuttlePhase.PreFlight;

    [DataField, AutoNetworkedField]
    public bool SectorLocked;

    [DataField, AutoNetworkedField]
    public bool AirlocksLocked = true;

    [DataField, AutoNetworkedField]
    public int SelectedSectorIndex;

    [DataField]
    public EntityUid? TargetPlanetGrid;

    [DataField, AutoNetworkedField]
    public Vector2i SelectedDropCoords = new(25, 189);
}

[Serializable, NetSerializable]
public sealed class STMarsocDropSectorEntry : IEquatable<STMarsocDropSectorEntry>
{
    public string Name;
    public Vector2i Coords;

    public STMarsocDropSectorEntry(string name, Vector2i coords)
    {
        Name = name;
        Coords = coords;
    }

    public bool Equals(STMarsocDropSectorEntry? other)
    {
        if (other is null)
            return false;

        return Name == other.Name && Coords.Equals(other.Coords);
    }

    public override bool Equals(object? obj) => Equals(obj as STMarsocDropSectorEntry);
    public override int GetHashCode() => HashCode.Combine(Name, Coords);
}

[Serializable, NetSerializable]
public sealed class STMarsocBriefingBuiState : BoundUserInterfaceState
{
    public STMarsocShuttlePhase Phase;
    public bool SectorLocked;
    public bool AirlocksLocked;
    public bool SquadSpawned;
    public int SelectedSectorIndex;
    public List<STMarsocDropSectorEntry> Sectors;
    public string PrimaryObjectiveText;
    public string SecondaryObjectiveText;
    public string DetectedInsertName;

    public STMarsocBriefingBuiState(
        STMarsocShuttlePhase phase,
        bool sectorLocked,
        bool airlocksLocked,
        bool squadSpawned,
        int selectedSectorIndex,
        List<STMarsocDropSectorEntry> sectors,
        string primaryObjectiveText,
        string secondaryObjectiveText,
        string detectedInsertName)
    {
        Phase = phase;
        SectorLocked = sectorLocked;
        AirlocksLocked = airlocksLocked;
        SquadSpawned = squadSpawned;
        SelectedSectorIndex = selectedSectorIndex;
        Sectors = sectors;
        PrimaryObjectiveText = primaryObjectiveText;
        SecondaryObjectiveText = secondaryObjectiveText;
        DetectedInsertName = detectedInsertName;
    }
}

[Serializable, NetSerializable]
public sealed class STMarsocCycleSectorBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class STMarsocToggleLockSectorBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class STMarsocLaunchOrbitBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class STMarsocToggleAirlocksBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class STMarsocSpawnSquadBuiMsg : BoundUserInterfaceMessage;