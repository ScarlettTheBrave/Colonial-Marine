using Content.Shared._ColonialMarine.Marines;
using Content.Shared._ColonialMarine.Vendors;
using Content.Shared._TacticalOps.AdvanceEngiHUD.Components;
using Content.Shared.Access.Systems;
using Robust.Shared.GameObjects;

namespace Content.Server._TacticalOps.AdvanceEngiHUD;

public sealed class TOAdvEngineerVendorSystem : EntitySystem
{
    [Dependency] private readonly SharedIdCardSystem _idCard = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TOAdvEngineerVendorComponent, AfterItemVendedEvent>(OnAfterItemVended);
    }

    private void OnAfterItemVended(Entity<TOAdvEngineerVendorComponent> ent, ref AfterItemVendedEvent args)
    {
        if (MetaData(args.Item).EntityPrototype?.ID != ent.Comp.EquipmentCase.Id)
            return;

        if (!_idCard.TryGetIdCard(args.User, out var idCard))
            return;

        _idCard.TryChangeJobTitle(idCard.Owner, Loc.GetString(ent.Comp.SpecialistRole), idCard.Comp, args.User);

        if (ent.Comp.SpecialistPrefix is { } specialistPrefix)
        {
            var prefix = EnsureComp<JobPrefixComponent>(args.User);
            if (prefix.AdditionalPrefix != specialistPrefix)
            {
                prefix.AdditionalPrefix = specialistPrefix;
                Dirty(args.User, prefix);
            }
        }
    }
}