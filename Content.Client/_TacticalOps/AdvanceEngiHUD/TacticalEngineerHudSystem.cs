using Robust.Client.Graphics;

namespace Content.Client._TacticalOps.AdvanceEngiHUD;

public sealed class TacticalEngineerHudSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();
        if (!_overlay.HasOverlay<TacticalEngineerHudOverlay>())
            _overlay.AddOverlay(new TacticalEngineerHudOverlay());
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlay.RemoveOverlay<TacticalEngineerHudOverlay>();
    }
}