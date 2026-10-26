using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._Stories.Marsoc;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class STMarsocDropZoneComponent : Component
{
    
    [DataField, AutoNetworkedField]
    public LocId SectorNameLoc = "st-marsoc-sector-north";

  
    [DataField, AutoNetworkedField]
    public int Order;
}