﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

public static class SubspellSelectionModalPatcher
{
    [HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnActivate_Patch
    {
        public static bool Prefix(SubspellSelectionModal __instance, int index)
        {
            //PATCH: customizes subspell activation for upcasted elemental/fey
            return UpcastConjureElementalAndFey.CheckSubSpellActivated(__instance, index);
        }
    }

    [HarmonyPatch(typeof(SubspellSelectionModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [HarmonyPatch(new[]
    {
        typeof(SpellDefinition), typeof(RulesetCharacter), typeof(RulesetSpellRepertoire),
        typeof(SpellsByLevelBox.SpellCastEngagedHandler), typeof(int), typeof(RectTransform)
    })]
    public static class Bind_Patch
    {
        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: replaces available subspell list with additional higher level elemental/fey
            return UpcastConjureElementalAndFey.ReplaceSubSpellList(instructions);
        }
    }
}
