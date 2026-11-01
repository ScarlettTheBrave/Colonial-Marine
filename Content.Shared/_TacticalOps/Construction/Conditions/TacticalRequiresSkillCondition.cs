using Content.Shared._ColonialMarine.Marines.Skills;
using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared._TacticalOps.Construction.Conditions;

[DataDefinition]
public sealed partial class TacticalRequiresSkillCondition : IConstructionCondition
{
    [DataField]
    public EntProtoId<SkillDefinitionComponent> Skill = "CMSkillEngineer";

    [DataField]
    public int Level = 5;

    [DataField]
    public LocId GuideText = "tactical-construction-requires-skill";

    public ConstructionGuideEntry GenerateGuideEntry()
    {
        return new ConstructionGuideEntry
        {
            Localization = GuideText,
        };
    }

    public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
    {
        var entities = IoCManager.Resolve<IEntityManager>();
        var skills = entities.System<SkillsSystem>();
        return skills.HasSkill(user, Skill, Level);
    }
}