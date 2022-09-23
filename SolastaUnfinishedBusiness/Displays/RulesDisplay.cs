﻿using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class RulesDisplay
{
    internal static void DisplayRules()
    {
        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&SRD"));
        UI.Label("");

        var toggle = Main.Settings.UseOfficialAdvantageDisadvantageRules;
        if (UI.Toggle(Gui.Localize("ModUi/&UseOfficialAdvantageDisadvantageRules"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseOfficialAdvantageDisadvantageRules = toggle;
        }

        // UI.Label("");
        //
        // toggle = Main.Settings.UseMoreRestrictiveAcStacking;
        // if (UI.Toggle(Gui.Localize("ModUi/&UseMoreRestrictiveAcStacking"), ref toggle, UI.AutoWidth()))
        // {
        //     Main.Settings.UseMoreRestrictiveAcStacking = toggle;
        // }
        // ModUi/&UseMoreRestrictiveAcStacking=AC granting clothes and <color=#D89555>Barkskin</color> won't stack with <color=#D89555>Unarmored Defense</color> or similar features

        UI.Label("");

        toggle = Main.Settings.AddBleedingToLesserRestoration;
        if (UI.Toggle(Gui.Localize("ModUi/&AddBleedingToLesserRestoration"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddBleedingToLesserRestoration = toggle;
            SrdAndHouseRulesContext.AddBleedingToRestoration();
        }

        toggle = Main.Settings.BlindedConditionDontAllowAttackOfOpportunity;
        if (UI.Toggle(Gui.Localize("ModUi/&BlindedConditionDontAllowAttackOfOpportunity"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.BlindedConditionDontAllowAttackOfOpportunity = toggle;
            SrdAndHouseRulesContext.ApplyConditionBlindedShouldNotAllowOpportunityAttack();
        }

        UI.Label("");

        toggle = Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowTargetingSelectionWhenCastingChainLightningSpell"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell = toggle;
            SrdAndHouseRulesContext.AllowTargetingSelectionWhenCastingChainLightningSpell();
        }

        toggle = Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove;
        if (UI.Toggle(Gui.Localize("ModUi/&BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove = toggle;
        }

        toggle = Main.Settings.EnableUpcastConjureElementalAndFey;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableUpcastConjureElementalAndFey"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableUpcastConjureElementalAndFey = toggle;
            Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey = false;
            ConjurationsContext.SwitchEnableUpcastConjureElementalAndFey();
        }

        if (Main.Settings.EnableUpcastConjureElementalAndFey)
        {
            toggle = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey;
            if (UI.Toggle(Gui.Localize("ModUi/&OnlyShowMostPowerfulUpcastConjuredElementalOrFey"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey = toggle;
            }
        }

        toggle = Main.Settings.FixSorcererTwinnedLogic;
        if (UI.Toggle(Gui.Localize("ModUi/&FixSorcererTwinnedLogic"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixSorcererTwinnedLogic = toggle;
        }

        toggle = Main.Settings.FullyControlConjurations;
        if (UI.Toggle(Gui.Localize("ModUi/&FullyControlConjurations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FullyControlConjurations = toggle;
            ConjurationsContext.SwitchFullyControlConjurations();
        }

        UI.Label("");

        toggle = Main.Settings.ApplySrdWeightToFoodRations;
        if (UI.Toggle(Gui.Localize("ModUi/&ApplySRDWeightToFoodRations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ApplySrdWeightToFoodRations = toggle;
            SrdAndHouseRulesContext.ApplySrdWeightToFoodRations();
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&House"));
        UI.Label("");

        toggle = Main.Settings.ChangeSleetStormToCube;
        if (UI.Toggle(Gui.Localize("ModUi/&ChangeSleetStormToCube"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ChangeSleetStormToCube = toggle;
            SrdAndHouseRulesContext.UseCubeOnSleetStorm();
        }

        toggle = Main.Settings.UseHeightOneCylinderEffect;
        if (UI.Toggle(Gui.Localize("ModUi/&UseHeightOneCylinderEffect"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.UseHeightOneCylinderEffect = toggle;
            SrdAndHouseRulesContext.UseHeightOneCylinderEffect();
        }

        toggle = Main.Settings.RemoveConcentrationRequirementsFromAnySpell;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveConcentrationRequirementsFromAnySpell"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveConcentrationRequirementsFromAnySpell = toggle;
        }

        toggle = Main.Settings.RemoveHumanoidFilterOnHideousLaughter;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveHumanoidFilterOnHideousLaughter"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveHumanoidFilterOnHideousLaughter = toggle;
        }

        toggle = Main.Settings.RemoveRecurringEffectOnEntangle;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveRecurringEffectOnEntangle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveRecurringEffectOnEntangle = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.AllowAnyClassToWearSylvanArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowAnyClassToWearSylvanArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowAnyClassToWearSylvanArmor = toggle;
            ItemOptionsContext.SwitchUniversalSylvanArmorAndLightbringer();
        }

        toggle = Main.Settings.AllowDruidToWearMetalArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDruidToWearMetalArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowDruidToWearMetalArmor = toggle;
            ItemOptionsContext.SwitchDruidAllowMetalArmor();
        }

        toggle = Main.Settings.DisableAutoEquip;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableAutoEquip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableAutoEquip = toggle;
        }

        toggle = Main.Settings.MakeAllMagicStaveArcaneFoci;
        if (UI.Toggle(Gui.Localize("ModUi/&MakeAllMagicStaveArcaneFoci"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.MakeAllMagicStaveArcaneFoci = toggle;
            ItemOptionsContext.SwitchMagicStaffFoci();
        }

        UI.Label("");

        var intValue = Main.Settings.IncreaseSenseNormalVision;
        UI.Label(Gui.Localize("ModUi/&IncreaseSenseNormalVision"));
        if (UI.Slider(Gui.Localize("ModUi/&IncreaseSenseNormalVisionHelp"), ref intValue,
                SrdAndHouseRulesContext.DefaultVisionRange, SrdAndHouseRulesContext.MaxVisionRange,
                SrdAndHouseRulesContext.DefaultVisionRange, "", UI.AutoWidth()))
        {
            Main.Settings.IncreaseSenseNormalVision = intValue;
        }

        UI.Label("");

        toggle = Main.Settings.AddPickPocketableLoot;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPickPocketableLoot"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPickPocketableLoot = toggle;
            if (toggle)
            {
                PickPocketContext.Load();
            }
        }

        toggle = Main.Settings.AllowStackedMaterialComponent;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowStackedMaterialComponent"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AllowStackedMaterialComponent = toggle;
        }

        toggle = Main.Settings.ScaleMerchantPricesCorrectly;
        if (UI.Toggle(Gui.Localize("ModUi/&ScaleMerchantPricesCorrectly"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ScaleMerchantPricesCorrectly = toggle;
        }

        UI.Label("");
    }
}
