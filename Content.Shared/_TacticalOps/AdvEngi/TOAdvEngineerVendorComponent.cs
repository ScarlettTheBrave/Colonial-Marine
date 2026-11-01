using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared._TacticalOps.AdvanceEngiHUD.Components;

[RegisterComponent]
public sealed partial class TOAdvEngineerVendorComponent : Component
{
    [DataField(required: true)]
    public EntProtoId EquipmentCase = default!;

    [DataField(required: true)]
    public LocId SpecialistRole = string.Empty;

    [DataField]
    public LocId? SpecialistPrefix;
}