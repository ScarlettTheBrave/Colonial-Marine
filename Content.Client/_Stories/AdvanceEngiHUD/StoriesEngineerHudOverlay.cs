using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Repairable;
using Content.Shared._RMC14.Xenonids.ClawSharpness;
using Content.Shared._Stories.AdvanceEngiHUD.Components;
using Content.Shared._Stories.Robotics;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._Stories.AdvanceEngiHUD;

public sealed class StoriesEngineerHudOverlay : Overlay
{
    private const string EyesSlotId = "eyes";

    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IPlayerManager _players = default!;
    [Dependency] private readonly IResourceCache _resCache = default!;

    private readonly ContainerSystem _container;
    private readonly InventorySystem _inventory;
    private readonly TransformSystem _transform;
    private readonly Font _font;

    public override OverlaySpace Space => OverlaySpace.WorldSpace | OverlaySpace.ScreenSpace;

    private readonly List<HudRenderData> _renderQueue = new();
    private readonly List<BotOffscreenData> _offscreenQueue = new();
    private readonly Vector2[] _chevronVertices = new Vector2[3];
    private string _cachedIdleTag = "IDLE";

    public StoriesEngineerHudOverlay()
    {
        IoCManager.InjectDependencies(this);

        _container = _entity.System<ContainerSystem>();
        _inventory = _entity.System<InventorySystem>();
        _transform = _entity.System<TransformSystem>();

        _font = new VectorFont(_resCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf"), 10);
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (_players.LocalEntity is not { } player)
            return;

        if (!TryGetActiveHud(player, out var hudComp))
            return;

        var eye = args.Viewport.Eye;
        if (eye == null)
            return;

        if (_entity.TryGetComponent<EyeComponent>(player, out var playerEye) && playerEye.Eye != eye)
            return;

        var mapId = eye.Position.MapId;
        var eyeRot = eye.Rotation;

        if (args.Space == OverlaySpace.WorldSpace)
        {
            DrawWorldSpace(args, player, hudComp, mapId, eyeRot);
        }
        else if (args.Space == OverlaySpace.ScreenSpace)
        {
            DrawScreenSpace(args);
        }
    }

    private void DrawWorldSpace(
        in OverlayDrawArgs args,
        EntityUid player,
        StoriesEngineerHudComponent hudComp,
        MapId mapId,
        Angle eyeRot)
    {
        var handle = args.WorldHandle;
        var playerPos = _transform.GetWorldPosition(player);
        var maxRangeSq = hudComp.MaxRange * hudComp.MaxRange;

        _cachedIdleTag = Loc.GetString(hudComp.IdleTagLoc);

        handle.SetTransform(Matrix3x2.Identity);
        handle.DrawRect(args.WorldBounds, hudComp.FilterColor);

        _renderQueue.Clear();
        _offscreenQueue.Clear();

        var barLocalOffset = (-eyeRot).RotateVec(new Vector2(0f, -0.18f));

        var query = _entity.EntityQueryEnumerator<RMCRepairableComponent, DamageableComponent, SpriteComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var repairable, out var damageable, out _, out var xform))
        {
            if (xform.MapID != mapId)
                continue;

            if (_container.IsEntityOrParentInContainer(uid, xform: xform))
                continue;

            var pos = _transform.GetWorldPosition(xform);
            var distSq = (pos - playerPos).LengthSquared();
            var isBot = _entity.TryGetComponent<StoriesBotBatteryComponent>(uid, out var botBattery);

            // Always display offscreen compass arrows for bots anywhere on the map until they enter the screen bounds.
            if (!args.WorldAABB.Contains(pos))
            {
                if (isBot)
                {
                    var distance = MathF.Sqrt(distSq);
                    _offscreenQueue.Add(new BotOffscreenData(pos, distance, botBattery!.IsIdle));
                }

                continue;
            }

            if (!isBot && distSq > maxRangeSq)
                continue;

            var damage = damageable.TotalDamage.Float();

            float maxDamage;
            if (isBot)
            {
                maxDamage = botBattery!.MaxHealth > 0f ? botBattery.MaxHealth : hudComp.DefaultMaxDamage;
            }
            else if (_entity.TryGetComponent<ReceiverXenoClawsComponent>(uid, out var clawsComp) && clawsComp.MaxHealth > 0)
            {
                maxDamage = clawsComp.MaxHealth;
            }
            else if (repairable.RepairableDamageLimit > 0f)
            {
                maxDamage = repairable.RepairableDamageLimit / 0.75f;
            }
            else
            {
                maxDamage = hudComp.DefaultMaxDamage;
            }

            var currentHp = (int)MathF.Max(0f, maxDamage - damage);
            var maxHp = (int)MathF.Round(maxDamage);
            var ratio = Math.Clamp(1f - damage / maxDamage, 0f, 1f);
            var percent = (int)MathF.Round(ratio * 100f);
            var isIdle = isBot && botBattery!.IsIdle;

            var barWorldPos = pos + barLocalOffset;

            var rotationMatrix = Matrix3Helpers.CreateRotation(-eyeRot);
            var translationMatrix = Matrix3Helpers.CreateTranslation(barWorldPos);
            handle.SetTransform(Matrix3x2.Multiply(rotationMatrix, translationMatrix));

            DrawStructureBarLocal(handle, ratio, damage, repairable.RepairableDamageLimit, isBot, isIdle);

            if (isBot || damage > 0f)
            {
                _renderQueue.Add(new HudRenderData(barWorldPos, percent, currentHp, maxHp, isBot, isIdle));
            }
        }

        handle.SetTransform(Matrix3x2.Identity);
    }

    private void DrawScreenSpace(in OverlayDrawArgs args)
    {
        var handle = args.ScreenHandle;
        var viewport = args.Viewport;

        foreach (var data in _renderQueue)
        {
            var offset = data.IsBot ? new Vector2(-26f, -16f) : new Vector2(-12f, -15f);
            var screenPos = viewport.WorldToLocal(data.BarWorldPosition) + offset;

            string text;
            if (data.IsBot)
            {
                text = data.IsIdle
                    ? $"{data.CurrentHp}/{data.MaxHp} [{_cachedIdleTag}]"
                    : $"{data.CurrentHp}/{data.MaxHp} ({data.Percent}%)";
            }
            else
            {
                text = $"{data.Percent}%";
            }

            var color = data.IsIdle || data.Percent < 30 ? Color.Red : Color.White;
            DrawOutlinedString(handle, screenPos, text, color);
        }

        if (_offscreenQueue.Count == 0)
            return;

        var screenCenter = new Vector2(viewport.Size.X * 0.5f, viewport.Size.Y * 0.5f);
        var compassRadius = MathF.Min(viewport.Size.X, viewport.Size.Y) * 0.36f;

        foreach (var bot in _offscreenQueue)
        {
            var targetScreenPos = viewport.WorldToLocal(bot.WorldPosition);
            var delta = targetScreenPos - screenCenter;
            if (delta.LengthSquared() < 1f)
                continue;

            var dir = Vector2.Normalize(delta);
            var markerPos = screenCenter + dir * compassRadius;
            var markerColor = bot.IsIdle ? Color.Red : new Color(1.0f, 0.65f, 0.1f);

            DrawDirectionChevron(handle, markerPos, dir, markerColor);

            var labelPos = markerPos - dir * 20f + new Vector2(-14f, -6f);
            var distText = bot.IsIdle
                ? $"{(int)MathF.Round(bot.Distance)}m {_cachedIdleTag}"
                : $"{(int)MathF.Round(bot.Distance)}m";
            DrawOutlinedString(handle, labelPos, distText, markerColor);
        }
    }

    private void DrawOutlinedString(DrawingHandleScreen handle, Vector2 pos, string text, Color color)
    {
        handle.DrawString(_font, pos + new Vector2(1, 1), text, Color.Black);
        handle.DrawString(_font, pos + new Vector2(-1, -1), text, Color.Black);
        handle.DrawString(_font, pos + new Vector2(1, -1), text, Color.Black);
        handle.DrawString(_font, pos + new Vector2(-1, 1), text, Color.Black);
        handle.DrawString(_font, pos, text, color);
    }

    private void DrawDirectionChevron(DrawingHandleScreen handle, Vector2 center, Vector2 dir, Color color)
    {
        var perp = new Vector2(-dir.Y, dir.X);

        _chevronVertices[0] = center + dir * 10f;
        _chevronVertices[1] = center - dir * 7f + perp * 7f;
        _chevronVertices[2] = center - dir * 7f - perp * 7f;
        handle.DrawPrimitives(DrawPrimitiveTopology.TriangleList, _chevronVertices, Color.Black);

        _chevronVertices[0] = center + dir * 8f;
        _chevronVertices[1] = center - dir * 5f + perp * 5f;
        _chevronVertices[2] = center - dir * 5f - perp * 5f;
        handle.DrawPrimitives(DrawPrimitiveTopology.TriangleList, _chevronVertices, color);
    }

    private bool TryGetActiveHud(EntityUid player, out StoriesEngineerHudComponent comp)
    {
        if (_entity.TryGetComponent(player, out comp!))
            return true;

        if (_inventory.TryGetSlotEntity(player, EyesSlotId, out var eyesUid) &&
            _entity.TryGetComponent(eyesUid.Value, out comp!))
        {
            return true;
        }

        comp = default!;
        return false;
    }

    private static void DrawStructureBarLocal(
        DrawingHandleWorld handle,
        float ratio,
        float damage,
        float repairLimit,
        bool isBot,
        bool isIdle)
    {
        const float halfWidth = 0.32f;
        const float barHeight = 0.07f;

        if (!isBot && damage <= 0f && !isIdle)
        {
            var markerBox = new Box2(-0.14f, 0f, 0.14f, 0.03f);
            handle.DrawRect(markerBox, new Color(1.0f, 0.6f, 0.1f, 0.5f));
            return;
        }

        var bgBox = new Box2(
            -halfWidth - 0.02f,
            -0.02f,
            halfWidth + 0.02f,
            barHeight + 0.02f);

        var beyondWelderLimit = repairLimit > 0f && damage > repairLimit;
        var bgColor = beyondWelderLimit
            ? new Color(0.6f, 0.05f, 0.4f, 0.85f)
            : new Color(0.05f, 0.05f, 0.05f, 0.8f);

        handle.DrawRect(bgBox, bgColor);

        var fillWidth = (halfWidth * 2f) * ratio;
        var fillBox = new Box2(-halfWidth, 0f, -halfWidth + fillWidth, barHeight);

        var fillColor = isIdle
            ? Color.Red
            : Color.InterpolateBetween(Color.Red, Color.LimeGreen, ratio);
        handle.DrawRect(fillBox, fillColor);
    }

    private readonly struct HudRenderData(
        Vector2 barWorldPosition,
        int percent,
        int currentHp,
        int maxHp,
        bool isBot,
        bool isIdle)
    {
        public readonly Vector2 BarWorldPosition = barWorldPosition;
        public readonly int Percent = percent;
        public readonly int CurrentHp = currentHp;
        public readonly int MaxHp = maxHp;
        public readonly bool IsBot = isBot;
        public readonly bool IsIdle = isIdle;
    }

    private readonly struct BotOffscreenData(Vector2 worldPosition, float distance, bool isIdle)
    {
        public readonly Vector2 WorldPosition = worldPosition;
        public readonly float Distance = distance;
        public readonly bool IsIdle = isIdle;
    }
}