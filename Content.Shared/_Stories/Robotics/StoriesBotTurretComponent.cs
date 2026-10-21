using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Stories.Robotics;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class StoriesBotTurretComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool FollowMode = true;

    [DataField, AutoNetworkedField]
    public float AggroRange = 7.5f;

    [DataField, AutoNetworkedField]
    public EntityUid? Target;

    [DataField, AutoNetworkedField]
    public EntityUid? Master;

    [DataField, AutoNetworkedField]
    public int Ammo = 75;

    [DataField, AutoNetworkedField]
    public int MaxAmmo = 75;

    [DataField]
    public EntProtoId AmmoProto = "RMCCartridgeHMG10x28mmTungsten";

    [DataField]
    public ProtoId<TagPrototype> ReloadMagazineTag = "RMCMagazineM2C";

    [DataField]
    public SoundSpecifier ReloadSound = new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/batrifle_magin.ogg");

    [DataField]
    public EntProtoId<SkillDefinitionComponent> RequiredSkill = "RMCSkillEngineer";

    [DataField]
    public int RequiredSkillLevel = 5;

    [DataField]
    public int ShotsPerBurst = 5;

    [DataField]
    public int CurrentBurstShots;

    [DataField]
    public TimeSpan ShotInterval = TimeSpan.FromSeconds(0.18);

    [DataField]
    public TimeSpan BurstCooldown = TimeSpan.FromSeconds(2.2);

    [DataField]
    public bool IsCoolingDown;

    [DataField]
    public float RotationSpeedDegrees = 180f;

    [DataField]
    public Angle FireAngleTolerance = Angle.FromDegrees(15);

    [DataField]
    public float FollowStopDistance = 2.2f;

    [DataField]
    public TimeSpan ThinkInterval = TimeSpan.FromSeconds(0.05);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextThinkTime;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextFireTime;
}