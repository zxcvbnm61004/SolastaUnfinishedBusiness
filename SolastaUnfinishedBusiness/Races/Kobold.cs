﻿using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;

namespace SolastaUnfinishedBusiness.Races;

internal static class KoboldRaceBuilder
{
    internal static CharacterRaceDefinition RaceKobold { get; } = BuildKobold();

    [NotNull]
    private static CharacterRaceDefinition BuildKobold()
    {
        var proficiencyKoboldLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyKoboldLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Draconic")
            .AddToDB();

        var raceKobold = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceKobold")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                proficiencyKoboldLanguages)
            .AddToDB();

        raceKobold.SubRaces.SetRange(new List<CharacterRaceDefinition>
        {
            BuildDarkKobold(raceKobold), BuildDraconicKobold(raceKobold)
        });
        raceKobold.GuiPresentation.sortOrder = Elf.GuiPresentation.sortOrder + 1;
        RacesContext.RaceScaleMap[raceKobold] = -0.04f / -0.06f;
        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceKobold.name);

        return raceKobold;
    }

    [NotNull]
    private static CharacterRaceDefinition BuildDarkKobold(CharacterRaceDefinition characterRaceDefinition)
    {
        var darkKoboldSpriteReference = Dragonborn.GuiPresentation.SpriteReference;

        var attributeModifierDarkKoboldDexterityAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierDarkKoboldDexterityAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 2)
            .AddToDB();

        var abilityCheckAffinityDarkKoboldLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityDarkKoboldLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        abilityCheckAffinityDarkKoboldLightSensitivity.AffinityGroups[0].lightingContext = LightingContext.BrightLight;

        var darkKoboldCombatAffinityLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(CombatAffinitySensitiveToLight, "CombatAffinityDarkKoboldLightSensitivity")
            .SetOrUpdateGuiPresentation("LightAffinityDarkKoboldLightSensitivity", Category.Feature)
            .SetMyAttackAdvantage(AdvantageType.None)
            .SetMyAttackModifierSign(AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(DieType.D4)
            .AddToDB();

        var conditionDarkKoboldLightSensitive = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionLightSensitive, "ConditionDarkKoboldLightSensitive")
            .SetOrUpdateGuiPresentation("LightAffinityDarkKoboldLightSensitivity", Category.Feature)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(abilityCheckAffinityDarkKoboldLightSensitivity, darkKoboldCombatAffinityLightSensitivity)
            .AddToDB();

        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(conditionDarkKoboldLightSensitive);

        var lightAffinityDarkKoboldLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityDarkKoboldLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(
                new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = conditionDarkKoboldLightSensitive
                })
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create(TrueStrike.EffectDescription)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
            .SetDurationData(DurationType.Round, 1)
            .Build();

        var conditionDarkKoboldGrovelCowerAndBeg = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionDarkKoboldGrovelCowerAndBeg")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetDuration(DurationType.Round, 1)
            .ClearSpecialInterruptions()
            .AddToDB();

        effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = conditionDarkKoboldGrovelCowerAndBeg;

        var powerDarkKoboldGrovelCowerAndBeg = FeatureDefinitionPowerBuilder
            .Create("PowerDarkKoboldGrovelCowerAndBeg")
            .SetGuiPresentation(Category.Feature, Aid)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(effectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var darkKoboldRacePresentation = Dragonborn.RacePresentation.DeepCopy();

        var raceDarkKobold = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, "RaceDarkKobold")
            .SetGuiPresentation(Category.Race, darkKoboldSpriteReference)
            .SetRacePresentation(darkKoboldRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierDarkKoboldDexterityAbilityScoreIncrease,
                powerDarkKoboldGrovelCowerAndBeg,
                CombatAffinityPackTactics,
                lightAffinityDarkKoboldLightSensitivity)
            .AddToDB();

        return raceDarkKobold;
    }

    private static CharacterRaceDefinition BuildDraconicKobold(CharacterRaceDefinition characterRaceDefinition)
    {
        var draconicKoboldSpriteReference = Dragonborn.GuiPresentation.SpriteReference;

        var effectDescription = EffectDescriptionBuilder
            .Create(TrueStrike.EffectDescription)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
            .SetDurationData(DurationType.Round, 1)
            .Build();

        var conditionDraconicKoboldDraconicCry = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionDraconicKoboldDraconicCry")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetDuration(DurationType.Round, 1)
            .ClearSpecialInterruptions()
            .AddToDB();

        effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = conditionDraconicKoboldDraconicCry;

        var powerDraconicKoboldDraconicCry = FeatureDefinitionPowerBuilder
            .Create("PowerDraconicKoboldDraconicCry")
            .SetGuiPresentation(Category.Feature, Aid)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(effectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var spellListDraconicKoboldMagic = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListSorcerer, "SpellListDraconicKoboldMagic")
            .SetGuiPresentationNoContent()
            .ClearSpells()
            .SetSpellsAtLevel(0, SpellListDefinitions.SpellListSorcerer.SpellsByLevel[0].Spells.ToArray())
            .FinalizeSpells()
            .AddToDB();

        var castSpellDraconicKoboldMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellDraconicKoboldMagic")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(spellListDraconicKoboldMagic)
            .AddToDB();

        var draconicKoboldRacePresentation = Dragonborn.RacePresentation.DeepCopy();

        var raceDraconicKobold = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, "RaceDraconicKobold")
            .SetGuiPresentation(Category.Race, draconicKoboldSpriteReference)
            .SetRacePresentation(draconicKoboldRacePresentation)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionFeatureSets.FeatureSetHalfElfAbilityScoreIncrease,
                powerDraconicKoboldDraconicCry,
                castSpellDraconicKoboldMagic)
            .AddToDB();

        return raceDraconicKobold;
    }
}
