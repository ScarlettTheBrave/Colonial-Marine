using System;
using Content.Shared._ColonialMarine.Marines.Skills;
using Content.Shared.Tag;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._TacticalOps.Robotics;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(TacticalBotBatterySystem))]
public sealed partial class TacticalBotBatteryComponent : Component
{
    [DataField, AutoNetworkedField]
    public string SlotId = "power_cell";

    [DataField, AutoNetworkedField]
    public bool Unlocked;

    [DataField, AutoNetworkedField]
    public bool IsIdle;

    [DataField, AutoNetworkedField]
    public bool RemoteDisabled;

    [DataField, AutoNetworkedField]
    public EntityUid? Master;

    [DataField]
    public float MaxHealth = 450f;

    [DataField]
    public float CriticalHealthFraction = 0.35f;

    [DataField]
    public TimeSpan AlertCooldown = TimeSpan.FromSeconds(30);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextCriticalAlert;

    [DataField]
    public float OverkillDamageThreshold = 500f;

    [DataField]
    public EntProtoId? IntactBoardProto;

    [DataField]
    public EntProtoId? DestroyedBoardProto = "TOCircuitboardDestroyed";

    [DataField]
    public EntProtoId? ScrapMaterialProto = "CMSheetMetal1";

    [DataField]
    public SoundSpecifier DestroySound = new SoundCollectionSpecifier("MetalBreak");

    [DataField]
    public bool Destroying;

    [DataField]
    public float VisionRadius = 7.0f;

    [DataField]
    public EntProtoId<SkillDefinitionComponent> RequiredSkill = "CMSkillEngineer";

    [DataField]
    public int RequiredSkillLevel = 5;

    [DataField]
    public ProtoId<ToolQualityPrototype> PryQuality = "Prying";

    [DataField]
    public ProtoId<TagPrototype> PowerCellTag = "PowerCell";
}

[ByRefEvent]
public readonly record struct TacticalBotAnnouncementEvent(EntityUid Bot, EntityUid Recipient, LocId MessageLoc);