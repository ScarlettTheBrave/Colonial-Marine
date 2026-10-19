using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared._Stories.AdvanceEngiHUD.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StoriesEngineerHudComponent : Component
{
    [DataField, AutoNetworkedField]
    public float MaxRange = 10f;

    /// <summary>
    /// Fallback maximum structural health when ReceiverXenoClawsComponent and RepairableDamageLimit are absent.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float DefaultMaxDamage = 300f;

    [DataField, AutoNetworkedField]
    public Color FilterColor = new(1.0f, 0.42f, 0.0f, 0.16f);

    [DataField]
    public LocId IdleTagLoc = "stories-bot-hud-idle-tag";
}