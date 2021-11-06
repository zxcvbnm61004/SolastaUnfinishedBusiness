﻿using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class RemoveIdentificationContext
    {
        internal static void Load()
        {
            if (Main.Settings.NoIdentification)
            {
                foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>())
                {
                    item.SetRequiresIdentification(false);
                }
            }
        }
    }
}
