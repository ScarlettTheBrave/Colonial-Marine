using Content.Shared._TacticalOps.Marsoc;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client._TacticalOps.MARSOC;

[UsedImplicitly]
public sealed class TOMarsocBriefingBui : BoundUserInterface
{
    private TOMarsocBriefingWindow? _window;

    public TOMarsocBriefingBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new TOMarsocBriefingWindow();
        _window.OnClose += Close;
        _window.OnDeployRequested += () => SendMessage(new TOMarsocBriefingDeployMessage());
        _window.OnLaunchFlightRequested += () => SendMessage(new TOMarsocBriefingLaunchFlightMessage());
        _window.OnCycleSectorRequested += () => SendMessage(new TOMarsocBriefingCycleSectorMessage());
        _window.OnLockSectorRequested += () => SendMessage(new TOMarsocBriefingLockSectorMessage());
        _window.OnToggleParadropRequested += () => SendMessage(new TOMarsocBriefingToggleParadropMessage());
        _window.OpenCentered();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is TOMarsocBriefingBuiState briefingState)
            _window?.UpdateState(briefingState);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
            return;

        _window?.Close();
        _window = null;
    }
}