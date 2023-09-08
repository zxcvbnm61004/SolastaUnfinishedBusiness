﻿using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class SorcerousFieldManipulator : AbstractSubclass
{
    private const string Name = "SorcerousFieldManipulator";

    public SorcerousFieldManipulator()
    {
        // LEVEL 01

        var autoPreparedSpellsFieldManipulator = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Origin")
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, Sleep)
            .AddPreparedSpellGroup(3, HoldPerson)
            .AddPreparedSpellGroup(5, Counterspell)
            .AddPreparedSpellGroup(7, Banishment)
            .AddPreparedSpellGroup(9, HoldMonster)
            .AddPreparedSpellGroup(11, GlobeOfInvulnerability)
            .AddToDB();

        PowerSorcerousFieldManipulatorDisplacement = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Displacement")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Position)
                    .InviteOptionalAlly()
                    .SetParticleEffectParameters(Banishment)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Charisma,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination, 12)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(new CustomBehaviorDisplacement(), PushesOrDragFromEffectPoint.Marker)
            .AddToDB();

        // LEVEL 06

        MagicAffinityHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ArcaneManipulation")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(1)
            .AddToDB();

        // LEVEL 14

        var proficiencyMentalResistance = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}MentalResistance")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.SavingThrow,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom)
            .AddToDB();

        // LEVEL 18

        var powerForcefulStepApply = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForcefulStepApply")
            .SetGuiPresentation($"Power{Name}ForcefulStep", Category.Feature, hidden: true)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 2, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(EldritchBlast)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Strength,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 3, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        var effectDescriptionForcefulStep = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
            .SetParticleEffectParameters(PowerMelekTeleport)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                    .Build())
            .Build();

        var powerForcefulStepFixed = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForcefulStepFixed")
            .SetGuiPresentation($"Power{Name}ForcefulStep", Category.Feature, PowerMonkStepOfTheWindDash)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest, 1, 3)
            .SetEffectDescription(effectDescriptionForcefulStep)
            .AddToDB();

        powerForcefulStepFixed.SetCustomSubFeatures(
            new PowerUseValidityForcefulStepFixed(powerForcefulStepFixed),
            new ActionFinishedByMeForcefulStep(powerForcefulStepApply));

        var powerForcefulStepPoints = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForcefulStepPoints")
            .SetGuiPresentation($"Power{Name}ForcefulStep", Category.Feature, PowerMonkStepOfTheWindDash)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.SorceryPoints, 4)
            .SetEffectDescription(effectDescriptionForcefulStep)
            .SetCustomSubFeatures(new ActionFinishedByMeForcefulStep(powerForcefulStepApply))
            .AddToDB();

        powerForcefulStepPoints.SetCustomSubFeatures(
            new PowerUseValidityForcefulStepPoints(powerForcefulStepFixed));

        var featureSetForcefulStep = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ForcefulStep")
            .SetGuiPresentation($"Power{Name}ForcefulStep", Category.Feature)
            .AddFeatureSet(powerForcefulStepFixed, powerForcefulStepPoints, powerForcefulStepApply)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.SorcererFieldManipulator, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsFieldManipulator,
                PowerSorcerousFieldManipulatorDisplacement)
            .AddFeaturesAtLevel(6,
                MagicAffinityHeightened)
            .AddFeaturesAtLevel(14,
                proficiencyMentalResistance)
            .AddFeaturesAtLevel(18,
                featureSetForcefulStep)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Sorcerer;

    internal static FeatureDefinitionPower PowerSorcerousFieldManipulatorDisplacement { get; private set; }

    private static FeatureDefinitionMagicAffinity MagicAffinityHeightened { get; set; }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        foreach (var spellDefinition in SpellListDefinitions.SpellListAllSpells
                     .SpellsByLevel
                     .SelectMany(x => x.Spells)
                     // don't use the constant as it has a typo
                     .Where(x => x.SchoolOfMagic is "SchoolEnchantment" or SchoolAbjuration or SchoolIllusion))
        {
            if (spellDefinition.SpellsBundle)
            {
                foreach (var spellInBundle in spellDefinition.SubspellsList)
                {
                    MagicAffinityHeightened.WarListSpells.Add(spellInBundle.Name);
                }
            }
            else
            {
                MagicAffinityHeightened.WarListSpells.Add(spellDefinition.Name);
            }
        }
    }

    //
    // Displacement
    //

    private sealed class CustomBehaviorDisplacement : IUsePowerInitiatedByMe, IUsePowerFinishedByMe
    {
        public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power)
        {
            if (power != PowerSorcerousFieldManipulatorDisplacement)
            {
                yield break;
            }

            var rulesetEffect = action.ActionParams.RulesetEffect;

            // bring back power target type to position
            rulesetEffect.EffectDescription.targetType = TargetType.Position;
        }

        public IEnumerator OnUsePowerInitiatedByMe(CharacterAction characterAction, FeatureDefinitionPower power)
        {
            if (power != PowerSorcerousFieldManipulatorDisplacement)
            {
                yield break;
            }

            var rulesetEffect = characterAction.ActionParams.RulesetEffect;
            var actionParams = characterAction.ActionParams;

            actionParams.Positions.SetRange(
                GetFinalPosition(actionParams.TargetCharacters[0], actionParams.Positions[0]));

            // make target type individuals unique to trigger the game and only teleport targets
            rulesetEffect.EffectDescription.targetType = TargetType.IndividualsUnique;
        }

        private static int3 GetFinalPosition(GameLocationCharacter target, int3 position)
        {
            const string ERROR = "DISPLACEMENT: aborted as cannot place character on destination";

            var gameLocationPositioningService =
                ServiceRepository.GetService<IGameLocationPositioningService>() as GameLocationPositioningManager;

            //fall back to target original position
            if (gameLocationPositioningService == null)
            {
                return target.LocationPosition;
            }

            var xCoord = new[] { 0, -1, 1, -2, 2 };
            var yCoord = new[] { 0, -1, 1, -2, 2 };
            var canPlaceCharacter = false;
            var finalPosition = int3.zero;

            foreach (var x in xCoord)
            {
                foreach (var y in yCoord)
                {
                    finalPosition = position + new int3(x, 0, y);

                    canPlaceCharacter = gameLocationPositioningService.CanPlaceCharacterImpl(
                        target, target.RulesetCharacter.SizeParams, finalPosition, CellHelpers.PlacementMode.Station);

                    if (canPlaceCharacter)
                    {
                        break;
                    }
                }

                if (canPlaceCharacter)
                {
                    break;
                }
            }

            if (canPlaceCharacter)
            {
                return finalPosition;
            }

            //fall back to target original position
            finalPosition = target.LocationPosition;

            Gui.GuiService.ShowAlert(ERROR, Gui.ColorFailure);

            return finalPosition;
        }
    }

    //
    // Forceful Step Fixed
    //

    private sealed class PowerUseValidityForcefulStepFixed : IPowerUseValidity
    {
        private readonly FeatureDefinitionPower _powerForcefulStepFixed;

        public PowerUseValidityForcefulStepFixed(FeatureDefinitionPower powerForcefulStepFixed)
        {
            _powerForcefulStepFixed = powerForcefulStepFixed;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var usablePower = UsablePowersProvider.Get(_powerForcefulStepFixed, character);

            return usablePower.RemainingUses > 0;
        }
    }

    //
    // Forceful Step Points
    //

    private sealed class PowerUseValidityForcefulStepPoints : IPowerUseValidity
    {
        private readonly FeatureDefinitionPower _powerForcefulStepFixed;

        public PowerUseValidityForcefulStepPoints(FeatureDefinitionPower powerForcefulStepFixed)
        {
            _powerForcefulStepFixed = powerForcefulStepFixed;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var usablePower = UsablePowersProvider.Get(_powerForcefulStepFixed, character);

            return usablePower.RemainingUses == 0;
        }
    }

    //
    // Forceful Step Apply
    //

    private sealed class ActionFinishedByMeForcefulStep : IUsePowerFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerApply;

        public ActionFinishedByMeForcefulStep(FeatureDefinitionPower powerApply)
        {
            _powerApply = powerApply;
        }

        public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power)
        {
            if (power.Name != $"Power{Name}ForcefulStepFixed" && power.Name != $"Power{Name}ForcefulStepPoints")
            {
                yield break;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerApply, rulesetAttacker);
            var effectPower = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource();

            foreach (var gameLocationTarget in gameLocationBattleService.Battle.EnemyContenders
                         .Where(x =>
                             x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                             gameLocationBattleService.IsWithinXCells(action.ActingCharacter, x, 2))
                         .ToList()) // avoid changing enumerator
            {
                EffectHelpers.StartVisualEffect(action.ActingCharacter, gameLocationTarget, EldritchBlast);
                effectPower.ApplyEffectOnCharacter(
                    gameLocationTarget.RulesetCharacter, true, gameLocationTarget.LocationPosition);
            }
        }
    }
}
