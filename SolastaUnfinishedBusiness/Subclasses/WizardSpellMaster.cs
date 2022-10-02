﻿using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardSpellMaster : AbstractSubclass
{
    internal const string PowerSpellMasterBonusRecoveryName = "PowerSpellMasterBonusRecovery";

    internal WizardSpellMaster()
    {
        var magicAffinitySpellMasterPrepared = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterPrepared")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.ProficiencyBonus)
            .AddToDB();

        var magicAffinitySpellMasterExtraPrepared = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterExtraPrepared")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.SpellcastingAbilityBonus)
            .AddToDB();

        var magicAffinitySpellMasterKnowledge = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterKnowledge")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(1f, 1f, 1, RuleDefinitions.AdvantageType.None,
                RuleDefinitions.PreparedSpellsModifier.None)
            .AddToDB();

        var magicAffinitySpellMasterScriber = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellMasterScriber")
            .SetGuiPresentation(Category.Feature)
            .SetSpellLearnAndPrepModifiers(0.25f, 0.25f, 0, RuleDefinitions.AdvantageType.Advantage,
                RuleDefinitions.PreparedSpellsModifier.None)
            .AddToDB();

        var pointPoolSpellMasterBonusCantrips = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolSpellMasterBonusCantrips")
            .SetGuiPresentation(Category.Feature)
            .SetPool(HeroDefinitions.PointsPoolType.Cantrip, 2)
            .OnlyUniqueChoices()
            .AddToDB();

        var savingThrowAffinitySpellMasterSpellResistance = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("SavingThrowAffinitySpellMasterSpellResistance")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(
                RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Charisma
            )
            .AddToDB();

        var effectParticleParameters = new EffectParticleParameters();

        effectParticleParameters.Copy(PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);

        var magicAffinitySpellMasterRecovery = FeatureDefinitionPowerBuilder
            .Create(PowerSpellMasterBonusRecoveryName)
            .SetGuiPresentation("MagicAffinitySpellMasterRecovery", Category.Feature,
                PowerWizardArcaneRecovery.GuiPresentation.SpriteReference)
            .Configure(1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.Rest,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Intelligence,
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Self, 0, 0, 0, 0)
                    .SetCreatedByCharacter()
                    .AddEffectForm(EffectFormBuilder.Create().SetSpellForm(9).Build())
                    .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
                    .SetParticleEffectParameters(effectParticleParameters)
                    .Build())
            .AddToDB();

        _ = RestActivityDefinitionBuilder
            .Create("SpellMasterArcaneDepth")
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RuleDefinitions.RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                FunctorDefinitions.FunctorUsePower,
                magicAffinitySpellMasterRecovery.Name)
            .SetGuiPresentation("MagicAffinitySpellMasterRecovery", Category.Feature,
                PowerWizardArcaneRecovery.GuiPresentation.SpriteReference)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardSpellMaster")
            .SetGuiPresentation(Category.Subclass,
                DomainInsight.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2,
                magicAffinitySpellMasterPrepared,
                magicAffinitySpellMasterKnowledge,
                magicAffinitySpellMasterRecovery)
            .AddFeaturesAtLevel(6,
                magicAffinitySpellMasterScriber,
                pointPoolSpellMasterBonusCantrips)
            .AddFeaturesAtLevel(10,
                magicAffinitySpellMasterExtraPrepared)
            .AddFeaturesAtLevel(14,
                savingThrowAffinitySpellMasterSpellResistance)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
}
