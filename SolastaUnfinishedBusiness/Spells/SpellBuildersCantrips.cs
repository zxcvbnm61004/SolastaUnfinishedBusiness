﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    private static readonly (string, IMagicEffect)[] DamagesAndEffects =
    [
        (DamageTypeAcid, AcidSplash), (DamageTypeCold, ConeOfCold), (DamageTypeFire, FireBolt),
        (DamageTypeLightning, LightningBolt), (DamageTypePoison, PoisonSpray), (DamageTypeThunder, Shatter)
    ];

    #region Acid Claws

    internal static SpellDefinition BuildAcidClaw()
    {
        const string NAME = "AcidClaws";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AcidClaws, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 1, DieType.D8)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionAcidClaws")
                                    .SetGuiPresentation(Category.Condition, ConditionAcidSpit)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetFeatures(
                                        FeatureDefinitionAttributeModifierBuilder
                                            .Create("AttributeModifierAcidClawsACDebuff")
                                            .SetGuiPresentation("ConditionAcidClaws", Category.Condition)
                                            .SetModifier(AttributeModifierOperation.Additive,
                                                AttributeDefinitions.ArmorClass, -1)
                                            .AddToDB())
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(AcidSplash)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Air Blast

    internal static SpellDefinition BuildAirBlast()
    {
        const string NAME = "AirBlast";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AirBlast, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeBludgeoning, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(WindWall)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Blade Ward

    internal static SpellDefinition BuildBladeWard()
    {
        const string NAME = "BladeWard";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BladeWard, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{NAME}")
                                .SetGuiPresentation(NAME, Category.Spell, ConditionShielded)
                                .SetFeatures(
                                    DamageAffinityBludgeoningResistance,
                                    DamageAffinitySlashingResistance,
                                    DamageAffinityPiercingResistance)
                                .AddToDB()))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerPatronHiveReactiveCarapace)
                    .SetCasterEffectParameters(GuidingBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Burst of Radiance

    internal static SpellDefinition BuildBurstOfRadiance()
    {
        const string NAME = "BurstOfRadiance";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BurstOfRadiance, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(SacredFlame)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.impactParticleReference =
            spell.EffectDescription.EffectParticleParameters.effectParticleReference;

        return spell;
    }

    #endregion

    #region Enduring Sting

    internal static SpellDefinition BuildEnduringSting()
    {
        const string NAME = "EnduringSting";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.EnduringSting, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D4)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(Bane)
                    .SetImpactEffectParameters(VenomousSpike)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Illuminating Sphere

    internal static SpellDefinition BuildIlluminatingSphere()
    {
        const string NAME = "IlluminatingSphere";

        var spell = SpellDefinitionBuilder
            .Create(Sparkle, NAME)
            .SetGuiPresentation(Category.Spell, Shine)
            .SetVocalSpellSameType(VocalSpellSemeType.Detection)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Sparkle.EffectDescription)
                    .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.Sphere, 6)
                    .SetParticleEffectParameters(SacredFlame_B)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Infestation

    internal static SpellDefinition BuildInfestation()
    {
        const string NAME = "Infestation";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Infestation, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePoison, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushRandomDirection, 1)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetCasterEffectParameters(PoisonSpray)
                    .SetImpactEffectParameters(
                        PoisonSpray.EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Lightning Lure

    internal static SpellDefinition BuildLightningLure()
    {
        const string NAME = "LightningLure";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.LightningLure, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 3, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning, 1, DieType.D8)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Mind Spike

    internal static SpellDefinition BuildMindSpike()
    {
        const string NAME = "MindSpike";

        var conditionMindSpike = ConditionDefinitionBuilder
            .Create(ConditionBaned, $"Condition{NAME}")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionBaned)
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MindSpike, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Intelligence, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionMindSpike, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Minor Life Steal

    internal static SpellDefinition BuildMinorLifesteal()
    {
        var spell = SpellDefinitionBuilder
            .Create("MinorLifesteal")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("MinorLifesteal", Resources.MinorLifesteal, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6, 0, HealFromInflictedDamage.Half)
                            .Build())
                    .SetParticleEffectParameters(VampiricTouch)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Primal Savagery

    internal static SpellDefinition BuildPrimalSavagery()
    {
        const string NAME = "PrimalSavagery";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AcidClaws, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 1, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetCasterEffectParameters(AcidSplash)
                    .SetImpactEffectParameters(AcidArrow)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Starry Wisp

    internal static SpellDefinition BuildStarryWisp()
    {
        const string NAME = "StarryWisp";

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionLightSensitive)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        ConditionInvisibleBase.cancellingConditions.Add(condition);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.StarryWisp, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeRadiant, 1, DieType.D8),
                        EffectFormBuilder.ConditionForm(condition),
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitions.ConditionInvisible, ConditionForm.ConditionOperation.Remove),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 0, 2,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetParticleEffectParameters(FaerieFire)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Sunlit Blade

    internal static SpellDefinition BuildSunlightBlade()
    {
        var conditionMarked = ConditionDefinitionBuilder
            .Create("ConditionSunlightBladeMarked")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddToDB();

        var conditionSunlightBlade = ConditionDefinitionBuilder
            .Create("ConditionSunlightBlade")
            .SetGuiPresentation(Category.Condition)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageSunlightBlade")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("SunlightBlade")
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .SetAttackModeOnly()
                    .SetDamageDice(DieType.D8, 1)
                    .SetSpecificDamageType(DamageTypeRadiant)
                    .SetAdvancement(
                        ExtraAdditionalDamageAdvancement.CharacterLevel,
                        DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 2), (11, 3), (17, 4)))
                    .SetTargetCondition(conditionMarked, AdditionalDamageTriggerCondition.TargetHasCondition)
                    .AddConditionOperation(
                        ConditionOperationDescription.ConditionOperation.Add,
                        ConditionDefinitionBuilder
                            .Create(ConditionHighlighted, "ConditionSunlightBladeHighlighted")
                            .SetSpecialInterruptions(ConditionInterruption.Attacked)
                            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                            .AddToDB())
                    .SetAddLightSource(true)
                    .SetLightSourceForm(new LightSourceForm
                    {
                        brightRange = 0,
                        dimAdditionalRange = 2,
                        lightSourceType = LightSourceType.Basic,
                        color = new Color(0.9f, 0.8f, 0.4f),
                        graphicsPrefabReference = FeatureDefinitionAdditionalDamages
                            .AdditionalDamageBrandingSmite.LightSourceForm.graphicsPrefabReference
                    })
                    .SetImpactParticleReference(DivineFavor)
                    .AddToDB())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create("SunlightBlade")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("SunlightBlade", Resources.SunlightBlade, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetIgnoreCover()
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionSunlightBlade, ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder.ConditionForm(conditionMarked))
                    .SetParticleEffectParameters(DivineFavor)
                    .Build())
            .AddToDB();

        spell.AddCustomSubFeatures(
            new AttackAfterMagicEffect(),
            new UpgradeSpellRangeBasedOnWeaponReach(spell));

        return spell;
    }

    #endregion

    #region Sword Storm

    internal static SpellDefinition BuildSwordStorm()
    {
        const string NAME = "SwordStorm";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SwordStorm, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Thorny Vines

    internal static SpellDefinition BuildThornyVines()
    {
        var spell = SpellDefinitionBuilder
            .Create("ThornyVines")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("ThornyVines", Resources.ThornyVines, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5,
                        additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypePiercing, 1, DieType.D6),
                        EffectFormBuilder.MotionForm(MotionForm.MotionType.DragToOrigin, 2))
                    .SetParticleEffectParameters(VenomousSpike)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Thunder Strike

    internal static SpellDefinition BuildThunderStrike()
    {
        const string NAME = "ThunderStrike";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Shield)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeThunder, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(Shatter)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.impactParticleReference =
            spell.EffectDescription.EffectParticleParameters.zoneParticleReference;
        spell.EffectDescription.EffectParticleParameters.zoneParticleReference = new AssetReference();

        return spell;
    }

    #endregion

    #region Wrack

    internal static SpellDefinition BuildWrack()
    {
        const string NAME = "Wrack";

        var conditionWrack = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHindered)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(
                        ActionDefinitions.Id.DisengageMain,
                        ActionDefinitions.Id.DisengageBonus,
                        ActionDefinitions.Id.DashMain,
                        ActionDefinitions.Id.DashBonus)
                    .AddToDB())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Wrack, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(InflictWounds)
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Wisdom,
                        12)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionWrack, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Booming Blade

    internal static SpellDefinition BuildBoomingBlade()
    {
        var conditionMarked = ConditionDefinitionBuilder
            .Create("ConditionBoomingBladeMarked")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddToDB();

        var conditionBoomingBladeSheathed = ConditionDefinitionBuilder
            .Create(ConditionShine, "ConditionBoomingBladeSheathed")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .AddCustomSubFeatures(new ActionFinishedByMeBoomingBladeSheathed())
            .AddToDB();

        var conditionBoomingBlade = ConditionDefinitionBuilder
            .Create("ConditionBoomingBlade")
            .SetGuiPresentation(Category.Condition)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageBoomingBlade")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("BoomingBlade")
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .SetAttackModeOnly()
                    .SetDamageDice(DieType.D8, 1)
                    .SetSpecificDamageType(DamageTypeThunder)
                    .SetAdvancement(
                        ExtraAdditionalDamageAdvancement.CharacterLevel,
                        DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 2), (11, 3), (17, 4)))
                    .AddConditionOperation(
                        ConditionOperationDescription.ConditionOperation.Add, conditionBoomingBladeSheathed)
                    .SetTargetCondition(conditionMarked, AdditionalDamageTriggerCondition.TargetHasCondition)
                    .SetImpactParticleReference(Shatter)
                    .AddToDB())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create("BoomingBlade")
            .SetGuiPresentation(Category.Spell, DivineBlade)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetIgnoreCover()
                    .SetEffectAdvancement( // this is needed for tooltip
                        EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionBoomingBlade, ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder.ConditionForm(conditionMarked))
                    .SetParticleEffectParameters(Shatter)
                    .Build())
            .AddToDB();

        spell.AddCustomSubFeatures(
            new AttackAfterMagicEffect(),
            new UpgradeSpellRangeBasedOnWeaponReach(spell));

        return spell;
    }

    private sealed class ActionFinishedByMeBoomingBladeSheathed : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction.ActionId != ActionDefinitions.Id.TacticalMove)
            {
                yield break;
            }

            HandleBoomingBladeSheathedDamage(characterAction.ActingCharacter);
        }
    }

    private static void HandleBoomingBladeSheathedDamage(GameLocationCharacter defender)
    {
        var rulesetDefender = defender.RulesetCharacter;

        if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
        {
            return;
        }

        var usableCondition =
            rulesetDefender.AllConditions.FirstOrDefault(x =>
                x.ConditionDefinition.Name == "ConditionBoomingBladeSheathed");

        if (usableCondition == null)
        {
            return;
        }

        var rulesetAttacker = EffectHelpers.GetCharacterByGuid(usableCondition.SourceGuid);

        if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
        {
            return;
        }

        var attacker = GameLocationCharacter.GetFromActor(rulesetAttacker);

        if (attacker == null)
        {
            return;
        }

        // deal damage
        var characterLevel = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
        var diceNumber = characterLevel switch
        {
            >= 17 => 4,
            >= 11 => 3,
            >= 5 => 2,
            _ => 1
        };
        var damageForm = new DamageForm
        {
            DamageType = DamageTypeThunder, DieType = DieType.D8, DiceNumber = diceNumber, BonusDamage = 0
        };
        var rolls = new List<int>();
        var damageRoll = rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);
        var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
        {
            sourceCharacter = rulesetAttacker,
            targetCharacter = rulesetDefender,
            position = defender.LocationPosition
        };

        EffectHelpers.StartVisualEffect(attacker, defender, Shatter);
        RulesetActor.InflictDamage(
            damageRoll,
            damageForm,
            damageForm.DamageType,
            applyFormsParams,
            rulesetDefender,
            false,
            rulesetAttacker.Guid,
            false,
            attacker.FindActionAttackMode(ActionDefinitions.Id.AttackMain)?.AttackTags ?? [],
            new RollInfo(damageForm.DieType, rolls, 0),
            false,
            out _);

        rulesetDefender.RemoveCondition(usableCondition);
    }

    #endregion

    #region Burning Blade

    internal static SpellDefinition BuildResonatingStrike()
    {
        var additionalDamageResonatingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageResonatingStrike")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("ResonatingStrike")
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeFire)
            .SetAdvancement(
                ExtraAdditionalDamageAdvancement.CharacterLevel,
                DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 2), (11, 3), (17, 4)))
            .SetImpactParticleReference(BurningHands_B)
            .SetAttackModeOnly()
            .AddToDB();

        var conditionResonatingStrike = ConditionDefinitionBuilder
            .Create("ConditionResonatingStrike")
            .SetGuiPresentation(Category.Condition, Gui.EmptyContent)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageResonatingStrike)
            .AddToDB();

        additionalDamageResonatingStrike.AddCustomSubFeatures(
            new PhysicalAttackFinishedByMeResonatingStrike(conditionResonatingStrike));

        var spell = SpellDefinitionBuilder
            .Create("ResonatingStrike")
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("ResonatingStrike", Resources.BurningBlade, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique, 2)
                    .SetIgnoreCover()
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionResonatingStrike,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(BurningHands_B)
                    .SetImpactEffectParameters(new AssetReference())
                    .Build())
            .AddToDB();

        spell.AddCustomSubFeatures(
            // order matters here as below also implements IFilterTargetingCharacter
            new CustomBehaviorResonatingStrike(),
            new AttackAfterMagicEffect(),
            new UpgradeSpellRangeBasedOnWeaponReach(spell));

        return spell;
    }

    private sealed class CustomBehaviorResonatingStrike : IMagicEffectFinishedByMe, IFilterTargetingCharacter
    {
        internal static GameLocationCharacter SecondTarget;
        internal static int SpellCastingModifier;

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            bool isValid;

            if (__instance.SelectionService.SelectedTargets.Count == 0)
            {
                isValid = AttackAfterMagicEffect.CanAttack(__instance.ActionParams.ActingCharacter, target);

                if (!isValid)
                {
                    __instance.actionModifier.FailureFlags.Add("Tooltip/&TargetMeleeWeaponError");
                }

                return isValid;
            }

            var firstTarget = __instance.SelectionService.SelectedTargets[0];

            isValid = firstTarget.IsWithinRange(target, 1);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&SecondTargetNotWithinRange");
            }

            return isValid;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition spell)
        {
            if (action is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            var targets = action.ActionParams.TargetCharacters;

            if (targets.Count != 2)
            {
                SecondTarget = null;
            }
            else
            {
                var rulesetCaster = actionCastSpell.ActionParams.ActingCharacter.RulesetCharacter;
                var spellCastingAbility = actionCastSpell.ActiveSpell.SpellRepertoire.SpellCastingAbility;

                SecondTarget = actionCastSpell.ActionParams.TargetCharacters[1];
                SpellCastingModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                    rulesetCaster.TryGetAttributeValue(spellCastingAbility));
            }
        }
    }

    private sealed class PhysicalAttackFinishedByMeResonatingStrike(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionResonatingStrike) : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    conditionResonatingStrike.Name,
                    out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }

            var secondDefender = CustomBehaviorResonatingStrike.SecondTarget;

            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess)
                || CustomBehaviorResonatingStrike.SecondTarget is null)
            {
                yield break;
            }

            var rolls = new List<int>();
            var diceNumber = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel) switch
            {
                >= 17 => 3,
                >= 11 => 2,
                >= 5 => 1,
                _ => 0
            };
            var damageForm = new DamageForm
            {
                DamageType = DamageTypeFire,
                DieType = DieType.D8,
                DiceNumber = diceNumber,
                BonusDamage = CustomBehaviorResonatingStrike.SpellCastingModifier
            };
            var damageRoll = rulesetCharacter.RollDamage(damageForm, 0, rollOutcome == RollOutcome.CriticalSuccess, 0,
                0, 1, false, false, false, rolls);
            var rulesetDefender = secondDefender.RulesetActor;
            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetCharacter,
                targetCharacter = rulesetDefender,
                position = secondDefender.LocationPosition
            };

            EffectHelpers.StartVisualEffect(attacker, secondDefender, BurningHands_B);
            RulesetActor.InflictDamage(
                damageRoll,
                damageForm,
                damageForm.DamageType,
                applyFormsParams,
                rulesetDefender,
                false,
                rulesetCharacter.Guid,
                false,
                attackMode.AttackTags,
                new RollInfo(damageForm.DieType, rolls, damageForm.BonusDamage),
                false,
                out _);
        }
    }

    #endregion

    #region Toll the Dead

    internal static SpellDefinition BuildTollTheDead()
    {
        const string NAME = "TollTheDead";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Bane.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(CircleOfDeath)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        spell.AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemyTollTheDead(spell));

        return spell;
    }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class MagicEffectBeforeHitConfirmedOnEnemyTollTheDead(SpellDefinition spellTollTheDead)
        : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect == null || rulesetEffect.SourceDefinition != spellTollTheDead)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            actualEffectForms[0].DamageForm.dieType = rulesetDefender.MissingHitPoints == 0 ? DieType.D8 : DieType.D12;
        }
    }

    #endregion
}
