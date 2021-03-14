using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace EndlessConsumables
{
    public class EndlessGlobalItem : GlobalItem
    {
        private const string TOOLTIPNAME = "EndlessConsumable_Tooltip";
        private const string TOOLTIPTEXT = " (Endless)";
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!item.consumable) return;

            bool isSet = tooltips[0].text.Contains(TOOLTIPTEXT);

            if (Utils.IsAtMaxStacks(item) && !isSet)
            {
                tooltips[0].text += TOOLTIPTEXT;
            }
            else
            {
                tooltips[0].text.Replace(TOOLTIPTEXT, "");
            }
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            bool def = base.ConsumeItem(item, player);

            return Utils.IsAtMaxStacks(item) ? false : def;
        }
    }
}