﻿using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardBladeDancer : AbstractSubclass
{
    internal WizardBladeDancer()
    {
        var proficiencyBladeDancerLightArmor = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBladeDancerLightArmor")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Armor, ArmorCategoryDefinitions.LightArmorCategory.Name)
            .AddToDB();

        var proficiencyBladeDancerMartialWeapon = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBladeDancerMartialWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Weapon, WeaponCategoryDefinitions.MartialWeaponCategory.Name)
            .AddToDB();

        var replaceAttackWithCantripBladeDancer = FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripBladeDancer")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        ConditionBladeDancerBladeDance = ConditionDefinitionBuilder
            .Create("ConditionBladeDancerBladeDance")
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .Configure(
                RuleDefinitions.DurationType.Minute,
                1,
                false,
                FeatureDefinitionMovementAffinitys.MovementAffinityBarbarianFastMovement,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierBladeDancerBladeDance")
                    .SetGuiPresentation(Category.Feature)
                    .SetModifiedAttribute(AttributeDefinitions.ArmorClass)
                    .SetModifierAbilityScore(AttributeDefinitions.Intelligence, true)
                    .SetSituationalContext((RuleDefinitions.SituationalContext)
                        ExtendedSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityIslandHalflingAcrobatics,
                        "AbilityCheckAffinityBladeDancerBladeDanceAcrobatics")
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create("AbilityCheckAffinityBladeDancerBladeDanceConstitution")
                    .SetGuiPresentationNoContent(true)
                    .BuildAndSetAffinityGroups(
                        RuleDefinitions.CharacterAbilityCheckAffinity.None,
                        RuleDefinitions.DieType.D1,
                        4,
                        (AttributeDefinitions.Constitution, string.Empty))
                    .AddToDB())
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetTerminateWhenRemoved(true)
            .AddToDB();

        var effectBladeDance = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self)
            .SetCreatedByCharacter()
            .SetDurationData(
                RuleDefinitions.DurationType.Minute, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionBladeDancerBladeDance, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        var powerBladeDancerBladeDance = FeatureDefinitionPowerBuilder
            .Create("PowerBladeDancerBladeDance")
            .SetGuiPresentation(
                "Feature/&FeatureBladeDanceTitle",
                "Condition/&ConditionBladeDancerBladeDanceDescription",
                FeatureDefinitionPowers.PowerClericDivineInterventionWizard.GuiPresentation.SpriteReference)
            .Configure(
                0, RuleDefinitions.UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Intelligence,
                effectBladeDance,
                true)
            .SetCustomSubFeatures(
                new ValidatorPowerUse(IsBladeDanceValid))
            .AddToDB();

        ConditionBladeDancerDanceOfDefense = ConditionDefinitionBuilder
            .Create(ConditionBladeDancerBladeDance, "ConditionBladeDancerDanceOfDefense")
            .SetGuiPresentation("ConditionBladeDancerBladeDance", Category.Condition,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .AddFeatures(
                FeatureDefinitionReduceDamageBuilder
                    .Create("ReduceDamageBladeDancerDanceOfDefense")
                    .SetGuiPresentation(Category.Feature)
                    .SetNotificationTag("DanceOfDefense")
                    .SetReducedDamage(-5)
                    .SetSourceType(RuleDefinitions.FeatureSourceType.CharacterFeature)
                    .SetSourceName("ReduceDamageBladeDancerDanceOfDefense")
                    .AddToDB())
            .AddToDB();

        var effectDanceOfDefense = EffectDescriptionBuilder
            .Create(effectBladeDance)
            .ClearEffectForms()
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionBladeDancerDanceOfDefense, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        var powerBladeDancerDanceOfDefense = FeatureDefinitionPowerBuilder
            .Create(powerBladeDancerBladeDance, "PowerBladeDancerDanceOfDefense")
            .SetEffectDescription(effectDanceOfDefense)
            .SetOverriddenPower(powerBladeDancerBladeDance)
            .AddToDB();

        ConditionBladeDancerDanceOfVictory = ConditionDefinitionBuilder
            .Create(ConditionBladeDancerDanceOfDefense, "ConditionBladeDancerDanceOfVictory")
            .SetGuiPresentation("ConditionBladeDancerBladeDance", Category.Condition,
                ConditionDefinitions.ConditionHeroism.GuiPresentation.SpriteReference)
            .AddFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierBladeDancerDanceOfVictory")
                    .SetGuiPresentation(Category.Feature)
                    .Configure(
                        RuleDefinitions.AttackModifierMethod.None,
                        0,
                        String.Empty,
                        RuleDefinitions.AttackModifierMethod.FlatValue,
                        5)
                    .AddToDB())
            .AddToDB();

        var effectDanceOfVictory = EffectDescriptionBuilder
            .Create(effectBladeDance)
            .ClearEffectForms()
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionBladeDancerDanceOfVictory, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        var powerBladeDancerDanceOfVictory = FeatureDefinitionPowerBuilder
            .Create(powerBladeDancerBladeDance, "PowerBladeDancerDanceOfVictory")
            .SetEffectDescription(effectDanceOfVictory)
            .SetOverriddenPower(powerBladeDancerDanceOfDefense)
            .AddToDB();

        //
        // use sets for better descriptions on level up
        //

        var featureSetBladeDancerBladeDance = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetBladeDancerBladeDance")
            .SetGuiPresentation("FeatureBladeDance", Category.Feature)
            .SetFeatureSet(powerBladeDancerBladeDance)
            .AddToDB();

        var featureSetBladeDancerDanceOfDefense = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetBladeDancerDanceOfDefense")
            .SetGuiPresentation("ReduceDamageBladeDancerDanceOfDefense", Category.Feature)
            .SetFeatureSet(powerBladeDancerDanceOfDefense)
            .AddToDB();

        var featureSetBladeDancerDanceOfVictory = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetBladeDancerDanceOfVictory")
            .SetGuiPresentation("AttackModifierBladeDancerDanceOfVictory", Category.Feature)
            .SetFeatureSet(powerBladeDancerDanceOfVictory)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardBladeDancer")
            .SetGuiPresentation(Category.Subclass, DomainMischief.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2,
                proficiencyBladeDancerLightArmor,
                proficiencyBladeDancerMartialWeapon,
                featureSetBladeDancerBladeDance)
            .AddFeaturesAtLevel(6, replaceAttackWithCantripBladeDancer)
            .AddFeaturesAtLevel(6, FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack)
            .AddFeaturesAtLevel(10, featureSetBladeDancerDanceOfDefense)
            .AddFeaturesAtLevel(14, featureSetBladeDancerDanceOfVictory)
            .AddToDB();
    }

    // ReSharper disable once InconsistentNaming
    private static CharacterSubclassDefinition Subclass { get; set; }

    private static ConditionDefinition ConditionBladeDancerBladeDance { get; set; }

    private static ConditionDefinition ConditionBladeDancerDanceOfDefense { get; set; }

    private static ConditionDefinition ConditionBladeDancerDanceOfVictory { get; set; }

    private static bool IsBladeDanceValid(RulesetCharacter hero)
    {
        return !hero.IsWearingMediumArmor()
               && !hero.IsWearingHeavyArmor()
               && !hero.IsWearingShield()
               && !hero.IsWieldingTwoHandedWeapon();
    }

    internal static void OnItemEquipped([NotNull] RulesetCharacterHero hero, [NotNull] RulesetItem _)
    {
        if (IsBladeDanceValid(hero))
        {
            return;
        }

        if (hero.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionBladeDancerBladeDance.Name))
        {
            hero.RemoveConditionOfCategory(AttributeDefinitions.TagEffect,
                new RulesetCondition { conditionDefinition = ConditionBladeDancerBladeDance });
        }

        if (hero.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionBladeDancerDanceOfDefense.Name))
        {
            hero.RemoveConditionOfCategory(AttributeDefinitions.TagEffect,
                new RulesetCondition { conditionDefinition = ConditionBladeDancerDanceOfDefense });
        }

        if (hero.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, ConditionBladeDancerDanceOfVictory.Name))
        {
            hero.RemoveConditionOfCategory(AttributeDefinitions.TagEffect,
                new RulesetCondition { conditionDefinition = ConditionBladeDancerDanceOfVictory });
        }
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
