﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.FightingStyles;

namespace SolastaUnfinishedBusiness.Patches;

public static class GameLocationCharacterPatcher
{
    [HarmonyPatch(typeof(GameLocationCharacter), "StartBattleTurn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class StartBattleTurn_Patch
    {
        public static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat turn started event
            CharacterBattleListenersPatch.OnCharacterTurnStarted(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "EndBattleTurn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EndBattleTurn_Patch
    {
        public static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat turn ended event
            CharacterBattleListenersPatch.OnCharacterTurnEnded(__instance);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "StartBattle")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class StartBattle_Patch
    {
        public static void Postfix(GameLocationCharacter __instance, bool surprise)
        {
            //PATCH: acts as a callback for the character's combat started event
            //while there already is callback for this event it doesn't have character or surprise flag arguments
            CharacterBattleListenersPatch.OnCharacterBattleStarted(__instance, surprise);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "EndBattle")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EndBattle_Patch
    {
        public static void Postfix(GameLocationCharacter __instance)
        {
            //PATCH: acts as a callback for the character's combat ended event
            //while there already is callback for this event it doesn't have character argument
            CharacterBattleListenersPatch.OnCharacterBattleEnded(__instance);
        }
    }

#if false
    [HarmonyPatch(typeof(GameLocationCharacter), "AttackOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class AttackOn_Patch
    {
        public static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            //PATCH: support for `IOnAttackHitEffect` - calls before attack handlers
            var character = __instance.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            var features = character.GetSubFeaturesByType<IOnAttackHitEffect>();

            foreach (var effect in features)
            {
                effect.BeforeOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }
#endif

    [HarmonyPatch(typeof(GameLocationCharacter), "AttackImpactOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class AttackImpactOn_Patch
    {
        public static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            //PATCH: support for `IOnAttackHitEffect` - calls after attack handlers
            var character = __instance.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            var features = character.GetSubFeaturesByType<IOnAttackHitEffect>();

            foreach (var effect in features)
            {
                effect.AfterOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }

    // Yes the actual game typos this it is "OnPower" and not the expected "OnePower"
    [HarmonyPatch(typeof(GameLocationCharacter), "CanUseAtLeastOnPower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class CanUseAtLeastOnPower_Patch
    {
        // This makes it so that if a character only has powers that take longer than an action to activate the "Use Power" button is available.
        // But only not during a battle.
        public static void Postfix(
            GameLocationCharacter __instance,
            ActionDefinitions.ActionType actionType,
            ref bool __result,
            bool accountDelegatedPowers)
        {
            var rulesetCharacter = __instance.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                return;
            }

            var battleInProgress = Gui.Battle != null;

            if (__result)
            {
                //PATCH: hide use power button if character has no valid powers
                if (!rulesetCharacter.UsablePowers.Any(rulesetUsablePower =>
                        IsActionValid(rulesetCharacter, rulesetUsablePower, actionType)
                        && CanUsePower(rulesetCharacter, rulesetUsablePower, accountDelegatedPowers)
                    ))
                {
                    __result = false;
                }
            }
            //PATCH: force show power use button during exploration if it has at least one usable power
            else
            {
                if (!battleInProgress
                    && actionType == ActionDefinitions.ActionType.Main
                    && rulesetCharacter.UsablePowers.Any(rulesetUsablePower =>
                        CanUsePower(rulesetCharacter, rulesetUsablePower, accountDelegatedPowers)))

                {
                    __result = true;
                }
            }
        }

        private static bool IsActionValid(
            RulesetCharacter character,
            RulesetUsablePower power,
            ActionDefinitions.ActionType actionType)
        {
            if (Gui.Battle == null) { return true; }

            var time = power.powerDefinition.ActivationTime;
            if (!ActionDefinitions.CastingTimeToActionDefinition.TryGetValue(time, out var type)) { return false; }

            return actionType == type || !PowerVisibilityModifier.IsPowerHidden(character, power, actionType);
        }

        private static bool CanUsePower(RulesetCharacter character, RulesetUsablePower usablePower,
            bool accountDelegatedPowers)
        {
            var power = usablePower.PowerDefinition;
            return (accountDelegatedPowers || !power.DelegatedToAction)
                   && !power.GuiPresentation.Hidden
                   && character.CanUsePower(power, false);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "GetActionStatus")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class GetActionStatus_Patch
    {
        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for Pugilist Fighting Style
            // Removes check that makes `ShoveBonus` action unavailable if character has no shield
            return instructions.RemoveShieldRequiredForBonusPush();
        }

        public static void Postfix(ref GameLocationCharacter __instance, ActionDefinitions.Id actionId,
            ActionDefinitions.ActionScope scope, ref ActionDefinitions.ActionStatus __result)
        {
            //PATCH: support for `IReplaceAttackWithCantrip` - allows `CastMain` action if character used attack
            ReplaceAttackWithCantrip.AllowCastDuringMainAttack(__instance, actionId, scope, ref __result);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "RefreshActionPerformances")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class RefreshActionPerformances_Patch
    {
        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                //PATCH: Support for `IDefinitionApplicationValidator`
                .ValidateActionPerformanceProviders()
                //PATCH: Support for `IDefinitionApplicationValidator`
                .ValidateAdditionalActionProviders();
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "HandleActionExecution")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class HandleActionExecution_Patch
    {
        public static void Postfix(
            GameLocationCharacter __instance,
            CharacterActionParams actionParams,
            ActionDefinitions.ActionScope scope)
        {
            //PATCH: support for `IReplaceAttackWithCantrip` - counts cantrip casting as 1 main attack
            ReplaceAttackWithCantrip.AllowAttacksAfterCantrip(__instance, actionParams, scope);
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "GetActionAvailableIterations")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class GetActionAvailableIterations_Patch
    {
        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //replaces calls to FindExtraActionAttackModes to custom method which supports forced attack modes for offhand attacks
            return ExtraAttacksOnActionPanel.ReplaceFindExtraActionAttackModesInLocationCharacter(instructions);
        }
    }
}
