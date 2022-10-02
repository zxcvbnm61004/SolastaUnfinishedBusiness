﻿using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheDistantHand : AbstractSubclass
{
    private const string ZenArrowTag = "ZenArrow";

    // Zen Archer's Monk weapons are bows and darts ranged weapons.
    private static readonly List<WeaponTypeDefinition> ZenArcherWeapons = new()
    {
        WeaponTypeDefinitions.ShortbowType, WeaponTypeDefinitions.LongbowType
    };

    public WayOfTheDistantHand()
    {
        var zenArrow =
            CustomIcons.CreateAssetReferenceSprite("ZenArrow", Resources.ZenArrow, 128, 64);

        //
        // LEVEL 03
        //

        var proficiencyWayOfTheDistantHandCombat = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyWayOfTheDistantHandCombat")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Weapon,
                WeaponTypeDefinitions.LongbowType.Name, WeaponTypeDefinitions.ShortbowType.Name)
            .SetCustomSubFeatures(
                new ZenArcherMarker(),
                new RangedAttackInMeleeDisadvantageRemover(
                    IsMonkWeapon, ValidatorsCharacter.NoArmor, ValidatorsCharacter.NoShield),
                new AddTagToWeaponAttack(ZenArrowTag, IsZenArrowAttack)
            )
            .AddToDB();

        // ZEN ARROW

        var powerWayOfTheDistantHandZenArrowTechnique = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowTechnique")
            .SetGuiPresentation(Category.Feature, zenArrow)
            .SetActivationTime(ActivationTime.OnAttackHit)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetCustomSubFeatures(new ReactionAttackModeRestriction(
                (mode, _, _, _) => mode != null && mode.AttackTags.Contains(ZenArrowTag)
            ))
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowProne = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowProne")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Dexterity,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .SetMotionForm(MotionForm.MotionType.FallProne, 0)
                    .Build())
                .Build())
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowPush = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowPush")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Strength,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                    .Build())
                .Build())
            .AddToDB();

        var conditionWayOfTheDistantHandDistract = ConditionDefinitionBuilder
            .Create("ConditionWayOfTheDistantHandDistract")
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionDazzled.GuiPresentation.SpriteReference)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                .Create("CombatAffinityWayOfTheDistantHandDistract")
                .SetGuiPresentationNoContent(true)
                .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                .AddToDB())
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowDistract = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowDistract")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Wisdom,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetConditionForm(conditionWayOfTheDistantHandDistract, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        PowersBundleContext.RegisterPowerBundle(powerWayOfTheDistantHandZenArrowTechnique, true,
            powerWayOfTheDistantHandZenArrowProne,
            powerWayOfTheDistantHandZenArrowPush,
            powerWayOfTheDistantHandZenArrowDistract);

        //
        // LEVEL 06
        //

        var additionalActionWayOfTheDistantHandExtraAttack1 = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionWayOfTheDistantHandExtraAttack1")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new AddExtraMainHandAttack(ActionDefinitions.ActionType.Bonus, true,
                ValidatorsCharacter.NoArmor, ValidatorsCharacter.NoShield, WieldsZenArcherWeapon))
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
            .AddToDB();

        var additionalActionWayOfTheDistantHandExtraAttack2 = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionWayOfTheDistantHandExtraAttack2")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
            .AddToDB();

        var conditionWayOfTheDistantHandAttackedWithMonkWeapon = ConditionDefinitionBuilder
            .Create("ConditionWayOfTheDistantHandAttackedWithMonkWeapon")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd, ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        //
        // LEVEL 06
        //

        var attackedWithMonkWeapon =
            ValidatorsCharacter.HasAnyOfConditions(conditionWayOfTheDistantHandAttackedWithMonkWeapon);

        var flurryOfArrowsSprite =
            CustomIcons.CreateAssetReferenceSprite("FlurryOfArrows", Resources.FlurryOfArrows, 128, 64);

        var powerWayOfTheDistantHandZenArcherFlurryOfArrows = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArcherFlurryOfArrows")
            .SetGuiPresentation(Category.Feature, flurryOfArrowsSprite)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetCostPerUse(2)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetShowCasting(false)
            .SetCustomSubFeatures(new ValidatorPowerUse(
                attackedWithMonkWeapon, ValidatorsCharacter.NoShield, ValidatorsCharacter.NoArmor))
            .SetEffectDescription(new EffectDescriptionBuilder()
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionWayOfTheDistantHandZenArcherFlurryOfArrows")
                            .SetGuiPresentationNoContent(true)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetDuration(DurationType.Round, 0, false)
                            .SetSpecialDuration(true)
                            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                            .SetSpecialInterruptions(ConditionInterruption.BattleEnd,
                                ConditionInterruption.AnyBattleTurnEnd)
                            .SetFeatures(additionalActionWayOfTheDistantHandExtraAttack1,
                                additionalActionWayOfTheDistantHandExtraAttack2)
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add, true, true)
                    .Build())
                .Build())
            .AddToDB();

        var wayOfDistantHandsKiPoweredArrows = FeatureDefinitionBuilder
            .Create("WayOfTheDistantHandKiPoweredArrows")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new AddTagToWeaponAttack(TagsDefinitions.Magical,
                (mode, _, character) => IsZenArcherWeapon(character, mode.SourceDefinition as ItemDefinition)))
            .AddToDB();

        //
        // LEVEL 11
        //

        var wayOfDistantHandsZenArcherStunningArrows = FeatureDefinitionBuilder
            .Create("WayOfTheDistantHandZenArcherStunningArrows")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ZenArcherStunningArrows())
            .AddToDB();

        // UPGRADE ZEN ARROW

        var powerWayOfTheDistantHandZenArrowUpgradedTechnique = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowUpgradedTechnique")
            .SetGuiPresentation(Category.Feature, zenArrow)
            .SetActivationTime(ActivationTime.OnAttackHit)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetOverriddenPower(powerWayOfTheDistantHandZenArrowTechnique)
            .SetCustomSubFeatures(new ReactionAttackModeRestriction(
                (mode, _, _, _) => mode != null && mode.AttackTags.Contains(ZenArrowTag)
            ))
            .AddToDB();

        var powerWayOfTheDistantHandZenArrowUpgradedProne = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandZenArrowUpgradedProne")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Dexterity,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                        .SetMotionForm(MotionForm.MotionType.FallProne, 0)
                        .Build(),
                    new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionWayOfTheDistantHandZenArrowUpgradedSlow")
                            .SetGuiPresentation(Category.Condition,
                                ConditionDefinitions.ConditionEncumbered.GuiPresentation.SpriteReference)
                            .SetDuration(DurationType.Round, 1)
                            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                            .SetConditionType(ConditionType.Detrimental)
                            .SetFeatures(FeatureDefinitionMovementAffinityBuilder
                                .Create("MovementAffinityWayOfTheDistantHandUpgradedSlow")
                                .SetGuiPresentationNoContent(true)
                                .SetBaseSpeedMultiplicativeModifier(0)
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        var powerWayOfTheDistantHandUpgradedPush = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandUpgradedPush")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Strength,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 4)
                    .Build())
                .Build())
            .AddToDB();

        var powerWayOfTheDistantHandUpgradedDistract = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfTheDistantHandUpgradedDistract")
            .SetGuiPresentation(Category.Feature)
            .SetRechargeRate(RechargeRate.KiPoints)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Wisdom,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency
                )
                .SetEffectForms(
                    new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionWayOfTheDistantHandUpgradedDistract")
                            .SetGuiPresentation(Category.Condition,
                                ConditionDefinitions.ConditionDazzled.GuiPresentation.SpriteReference)
                            .SetDuration(DurationType.Round, 1)
                            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                            .SetConditionType(ConditionType.Detrimental)
                            .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                                .Create("CombatAffinityWayOfTheDistantHandUpgradedDistract")
                                .SetGuiPresentationNoContent(true)
                                .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        PowersBundleContext.RegisterPowerBundle(powerWayOfTheDistantHandZenArrowUpgradedTechnique, true,
            powerWayOfTheDistantHandZenArrowUpgradedProne,
            powerWayOfTheDistantHandUpgradedPush,
            powerWayOfTheDistantHandUpgradedDistract);

        //
        // PROGRESSION
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WayOfTheDistantHand")
            .SetOrUpdateGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.RangerMarksman.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                proficiencyWayOfTheDistantHandCombat,
                powerWayOfTheDistantHandZenArrowTechnique)
            .AddFeaturesAtLevel(6,
                wayOfDistantHandsKiPoweredArrows,
                powerWayOfTheDistantHandZenArcherFlurryOfArrows)
            .AddFeaturesAtLevel(11,
                wayOfDistantHandsZenArcherStunningArrows,
                powerWayOfTheDistantHandZenArrowUpgradedTechnique)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    // private class ExtendWeaponRange : IModifyAttackModeForWeapon
    // {
    //     public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
    //     {
    //         if (attackMode == null || attackMode.Magical || (!attackMode.Ranged && !attackMode.Thrown))
    //         {
    //             return;
    //         }
    //
    //         if (!Monk.IsMonkWeapon(character, attackMode))
    //         {
    //             return;
    //         }
    //
    //         attackMode.CloseRange = Math.Min(16, attackMode.CloseRange * 2);
    //         attackMode.MaxRange = Math.Min(32, attackMode.MaxRange * 2);
    //     }
    // }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    private static bool IsMonkWeapon(RulesetAttackMode attackMode, RulesetItem weapon, RulesetCharacter character)
    {
        return IsMonkWeapon(character, attackMode) || IsMonkWeapon(character, weapon);
    }

    private static bool IsMonkWeapon(RulesetCharacter character, RulesetAttackMode attackMode)
    {
        return attackMode is { SourceDefinition: ItemDefinition item } && IsMonkWeapon(character, item);
    }

    private static bool IsMonkWeapon(RulesetCharacter character, RulesetItem weapon)
    {
        return weapon == null || IsMonkWeapon(character, weapon.ItemDefinition);
    }

    private static bool IsMonkWeapon(RulesetActor character, ItemDefinition weapon)
    {
        if (weapon == null)
        {
            return false;
        }

        var typeDefinition = weapon.WeaponDescription?.WeaponTypeDefinition;

        if (typeDefinition == null)
        {
            return false;
        }

        return ZenArcherWeapons.Contains(typeDefinition) || IsZenArcherWeapon(character, weapon);
    }

    private static bool IsZenArcherWeapon(RulesetActor character, ItemDefinition item)
    {
        if (character == null || item == null)
        {
            return false;
        }

        var typeDefinition = item.WeaponDescription?.WeaponTypeDefinition;

        if (typeDefinition == null)
        {
            return false;
        }

        return character.HasSubFeatureOfType<ZenArcherMarker>() && ZenArcherWeapons.Contains(typeDefinition);
    }

    private static bool IsZenArrowAttack(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return mode != null
               && (mode.Ranged || mode.Thrown)
               && IsZenArcherWeapon(character, mode.SourceDefinition as ItemDefinition);
    }

    private static bool WieldsZenArcherWeapon(RulesetCharacter character)
    {
        var mainHandItem =
            character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand].EquipedItem;

        return IsZenArcherWeapon(character, mainHandItem?.ItemDefinition);
    }

    private sealed class ZenArcherMarker
    {
        // used for easier detection of Zen Archer characters to extend their Monk weapon list
    }

    private sealed class ZenArcherStunningArrows
    {
        // used for easier detection of Zen Archer characters to allow stunning strike on arrows
    }
}
