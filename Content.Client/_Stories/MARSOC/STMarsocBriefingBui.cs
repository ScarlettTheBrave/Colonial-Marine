using Content.Shared._Stories.Marsoc;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client._Stories.MARSOC;

[UsedImplicitly]
public sealed class STMarsocBriefingBui : BoundUserInterface
{
    private STMarsocBriefingWindow? _window;

    public STMarsocBriefingBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new STMarsocBriefingWindow();
        _window.OnClose += Close;
        _window.OnDeployRequested += () => SendMessage(new STMarsocBriefingDeployMessage());
        _window.OnLaunchFlightRequested += () => SendMessage(new STMarsocBriefingLaunchFlightMessage());
        _window.OnCycleSectorRequested += () => SendMessage(new STMarsocBriefingCycleSectorMessage());
        _window.OnLockSectorRequested += () => SendMessage(new STMarsocBriefingLockSectorMessage());
        _window.OnToggleParadropRequested += () => SendMessage(new STMarsocBriefingToggleParadropMessage());
        _window.OpenCentered();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is STMarsocBriefingBuiState briefingState)
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