using System;
using Content.Shared.FixedPoint;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Stories.Robotics;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class StoriesBotDrillerComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool FollowMode;

    [DataField, AutoNetworkedField]
    public EntityUid? CurrentTarget;

    [DataField]
    public float DrillRange = 1.25f;

    [DataField]
    public float FollowStopDistance = 2.0f;

    [DataField]
    public TimeSpan CheckInterval = TimeSpan.FromSeconds(1.0);

    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(5);

    [DataField]
    public TimeSpan ReinforcedDuration = TimeSpan.FromSeconds(10);

    [DataField]
    public FixedPoint2 StructuralDamage = 9999;

    [DataField]
    public EntityWhitelist? Blacklist;

    [DataField]
    public SoundSpecifier? DrillSound = new SoundPathSpecifier("/Audio/Items/welder.ogg");

    [DataField]
    public SoundSpecifier? FinishSound = new SoundCollectionSpecifier("MetalBreak");

    [DataField]
    public LocId BreachCompleteAnnouncement = "stories-bot-announce-drill-complete";

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextCheck;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan DrillFinishTime;
}