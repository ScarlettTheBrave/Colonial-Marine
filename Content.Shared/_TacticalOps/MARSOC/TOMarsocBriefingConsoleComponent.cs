using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._TacticalOps.MARSOC;

[Serializable, NetSerializable]
public enum TOMarsocShuttlePhase : byte
{
   
    PreFlight = 0,


    InTransit = 1,

 
    InOrbit = 2
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TOMarsocShuttleComponent : Component
{
    [DataField, AutoNetworkedField]
    public TOMarsocShuttlePhase Phase = TOMarsocShuttlePhase.PreFlight;

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
public sealed class TOMarsocDropSectorEntry : IEquatable<TOMarsocDropSectorEntry>
{
    public string Name;
    public Vector2i Coords;

    public TOMarsocDropSectorEntry(string name, Vector2i coords)
    {
        Name = name;
        Coords = coords;
    }

    public bool Equals(TOMarsocDropSectorEntry? other)
    {
        if (other is null)
            return false;

        return Name == other.Name && Coords.Equals(other.Coords);
    }

    public override bool Equals(object? obj) => Equals(obj as TOMarsocDropSectorEntry);
    public override int GetHashCode() => HashCode.Combine(Name, Coords);
}

[Serializable, NetSerializable]
public sealed class TOMarsocBriefingBuiState : BoundUserInterfaceState
{
    public TOMarsocShuttlePhase Phase;
    public bool SectorLocked;
    public bool AirlocksLocked;
    public bool SquadSpawned;
    public int SelectedSectorIndex;
    public List<TOMarsocDropSectorEntry> Sectors;
    public string PrimaryObjectiveText;
    public string SecondaryObjectiveText;
    public string DetectedInsertName;

    public TOMarsocBriefingBuiState(
        TOMarsocShuttlePhase phase,
        bool sectorLocked,
        bool airlocksLocked,
        bool squadSpawned,
        int selectedSectorIndex,
        List<TOMarsocDropSectorEntry> sectors,
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
public sealed class TOMarsocCycleSectorBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class TOMarsocToggleLockSectorBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class TOMarsocLaunchOrbitBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class TOMarsocToggleAirlocksBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class TOMarsocSpawnSquadBuiMsg : BoundUserInterfaceMessage;