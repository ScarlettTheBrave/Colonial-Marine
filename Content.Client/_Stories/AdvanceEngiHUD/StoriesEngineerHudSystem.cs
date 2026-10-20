using Robust.Client.Graphics;

namespace Content.Client._Stories.AdvanceEngiHUD;

public sealed class StoriesEngineerHudSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();
        if (!_overlay.HasOverlay<StoriesEngineerHudOverlay>())
            _overlay.AddOverlay(new StoriesEngineerHudOverlay());
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlay.RemoveOverlay<StoriesEngineerHudOverlay>();
    }
}