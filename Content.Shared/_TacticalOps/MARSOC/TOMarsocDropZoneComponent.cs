using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._TacticalOps.Marsoc;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TOMarsocDropZoneComponent : Component
{
    
    [DataField, AutoNetworkedField]
    public LocId SectorNameLoc = "to-marsoc-sector-north";

  
    [DataField, AutoNetworkedField]
    public int Order;
}