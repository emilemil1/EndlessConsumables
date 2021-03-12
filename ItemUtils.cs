using Terraria;
using Terraria.ModLoader;

namespace EndlessConsumables
{
    public class ItemUtils {
        public static bool IsAtMaxStacks(Item item) {
            if (!item.consumable) return true;

            return item.stack >= item.maxStack;
        }
    }
}