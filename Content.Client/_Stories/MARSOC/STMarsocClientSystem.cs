using System.Numerics;
using Content.Shared._Stories.Marsoc;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Animations;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._Stories.MARSOC;

public sealed class STMarsocClientSystem : SharedSTMarsocSystem
{
    [Dependency] private readonly AnimationPlayerSystem _animPlayer = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;

    private const string ParachuteAnimKey = "st-marsoc-parachute-drop";
    private STMarsocDeploymentBannerWidget? _activeBanner;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<STMarsocDeploymentBannerEvent>(OnDeploymentBanner);
        SubscribeNetworkEvent<STMarsocParachuteDropEvent>(OnParachuteDrop);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_activeBanner is { IsFinished: true })
        {
            if (_activeBanner.Parent != null)
                _activeBanner.Orphan();

            _activeBanner = null;
        }
    }

    private void OnParachuteDrop(STMarsocParachuteDropEvent ev)
    {
        if (!TryGetEntity(ev.Target, out var uid) || !Exists(uid))
            return;

        var player = EnsureComp<AnimationPlayerComponent>(uid.Value);
        if (_animPlayer.HasRunningAnimation(uid.Value, player, ParachuteAnimKey))
            return;

        var fallAnim = new Animation
        {
            Length = TimeSpan.FromSeconds(ev.Duration),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Offset),
                    InterpolationMode = AnimationInterpolationMode.Cubic,
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(new Vector2(0f, ev.FallHeight), 0f),
                        new AnimationTrackProperty.KeyFrame(Vector2.Zero, ev.Duration)
                    }
                }
            }
        };

        _animPlayer.Play((uid.Value, player), fallAnim, ParachuteAnimKey);
    }

    private void OnDeploymentBanner(STMarsocDeploymentBannerEvent ev)
    {
        var root = _uiManager.ActiveScreen;
        if (root == null)
            return;

        if (_activeBanner != null && _activeBanner.Parent != null)
            _activeBanner.Orphan();

        _activeBanner = new STMarsocDeploymentBannerWidget(
            ev.OperationName.ToUpperInvariant(),
            ev.TimeAndDate,
            ev.PlanetName,
            ev.UnitAssignment);

        root.AddChild(_activeBanner);
    }
}

internal sealed class STMarsocDeploymentBannerWidget : BoxContainer
{
    private readonly RichTextLabel _label;
    private readonly string _operation;
    private readonly string _timeAndDate;
    private readonly string _planet;
    private readonly string _assignment;
    private readonly int _totalChars;

    private float _elapsed;
    private int _lastRenderedChars = -1;

    public bool IsFinished { get; private set; }

    private const float TotalLifetime = 10.5f;
    private const float FadeStart = 8.0f;
    private const float CharDelay = 0.035f;

    public STMarsocDeploymentBannerWidget(
        string operation,
        string timeAndDate,
        string planet,
        string assignment)
    {
        _operation = operation;
        _timeAndDate = timeAndDate;
        _planet = planet;
        _assignment = assignment;
        _totalChars = _operation.Length + _timeAndDate.Length + _planet.Length + _assignment.Length;

        MouseFilter = MouseFilterMode.Ignore;
        HorizontalAlignment = HAlignment.Left;
        VerticalAlignment = VAlignment.Center;
        Margin = new Thickness(48, 0, 0, 0);

        _label = new RichTextLabel
        {
            MouseFilter = MouseFilterMode.Ignore,
            MinWidth = 480
        };

        AddChild(_label);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (IsFinished)
            return;

        _elapsed += args.DeltaSeconds;
        if (_elapsed >= TotalLifetime)
        {
            Visible = false;
            IsFinished = true;
            return;
        }

        var visibleChars = Math.Min(_totalChars, (int)(_elapsed / CharDelay));
        if (visibleChars != _lastRenderedChars)
        {
            _lastRenderedChars = visibleChars;
            RenderPartialBanner(visibleChars);
        }

        if (_elapsed >= FadeStart)
        {
            var alpha = Math.Clamp(1f - (_elapsed - FadeStart) / (TotalLifetime - FadeStart), 0f, 1f);
            Modulate = Color.White.WithAlpha(alpha);
        }
    }

    private void RenderPartialBanner(int remaining)
    {
        var opPart = FormattedMessage.EscapeText(TakeChars(_operation, ref remaining));
        var timePart = FormattedMessage.EscapeText(TakeChars(_timeAndDate, ref remaining));
        var planetPart = FormattedMessage.EscapeText(TakeChars(_planet, ref remaining));
        var assignPart = FormattedMessage.EscapeText(TakeChars(_assignment, ref remaining));

        var markup = $"[font=\"CozetteBold\"]{opPart}[/font]\n{timePart}\n{planetPart}\n{assignPart}";
        _label.SetMessage(FormattedMessage.FromMarkupPermissive(markup));
    }

    private static string TakeChars(string source, ref int remaining)
    {
        if (remaining <= 0 || string.IsNullOrEmpty(source))
            return string.Empty;

        var count = Math.Min(source.Length, remaining);
        remaining -= count;
        return source[..count];
    }
}