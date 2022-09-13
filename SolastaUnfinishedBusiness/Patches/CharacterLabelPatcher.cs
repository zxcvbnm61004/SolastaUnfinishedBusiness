﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterLabelPatcher
{
    //PATCH: allows certain conditions to report on hero label
    [HarmonyPatch(typeof(CharacterLabel), "ConditionAdded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameConsole_ConditionAdded
    {
        internal static void Prefix(CharacterLabel __instance, RulesetActor character, RulesetCondition condition)
        {
            if (Global.CharacterLabelEnabledConditions.Contains(condition.ConditionDefinition))
            {
                __instance.DisplayConditionLabel(character, condition, false);
            }
        }
    }

    //PATCH: allows certain conditions to report on hero label
    [HarmonyPatch(typeof(CharacterLabel), "ConditionRemoved")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameConsole_ConditionRemoved
    {
        internal static void Prefix(CharacterLabel __instance, RulesetActor character, RulesetCondition condition)
        {
            if (Global.CharacterLabelEnabledConditions.Contains(condition.ConditionDefinition))
            {
                __instance.DisplayConditionLabel(character, condition, true);
            }
        }
    }
}
