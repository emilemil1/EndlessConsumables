using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace EndlessConsumables {
    public class EndlessPlayer : ModPlayer {
        private List<int> diedWithBuffs = new List<int>();
        private HashSet<int> forcedPotions = new HashSet<int>();

        public void addForcedPotion(Item item) {
            int type = item.type;
            if (forcedPotions.Contains(type)) {
                forcedPotions.Remove(type);
                Main.NewText(item.Name + " is no longerbeing autoused.");
            } else {
                forcedPotions.Add(type);
                Main.NewText(item.Name + " is being autoused.");
            }
        }

        public override bool ConsumeAmmo(Item weapon, Item ammo) {
            bool def = base.ConsumeAmmo(weapon, ammo);
            return ItemUtils.IsAtMaxStacks(ammo) ? false : def;
        }

        public override void PreUpdateBuffs() {
            base.PreUpdateBuffs();

            foreach (int itemType in forcedPotions) {
                RefreshPotionBuff(itemType, true);
            }

            for (int i = 0; i < player.buffType.Length; i++) {
                if (player.buffTime[i] <= 60) {
                    RefreshPotionBuff(player.buffType[i]);
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) {
            bool result = base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);

            diedWithBuffs.Clear();
            diedWithBuffs.AddRange(player.buffType);

            return result;
        }

        public override void OnRespawn(Player player) {
            base.OnRespawn(player);

            diedWithBuffs.ForEach(buff => RefreshPotionBuff(buff));
            diedWithBuffs.Clear();
        }

        public override TagCompound Save() {
            return new TagCompound() {
                {
                    "forcedPotions", new List<int>(forcedPotions)
                }
            };
        }

        public override void Load(TagCompound tag)
        {
            forcedPotions = new HashSet<int>(tag.GetList<int>("forcedPotions") ?? new List<int>());
        }

        private void RefreshPotionBuff(int type, bool itemType = false) {
            RefreshPotionBuffForArray(type, player.inventory, itemType);
            RefreshPotionBuffForArray(type, player.bank.item, itemType);
            RefreshPotionBuffForArray(type, player.bank2.item, itemType);
            RefreshPotionBuffForArray(type, player.bank3.item, itemType);
        }

        private void RefreshPotionBuffForArray(int type, Item[] arr, bool itemType) {
            foreach(Item item in arr) {
                if ((itemType ? item.type == type : item.buffType == type) && ItemUtils.IsAtMaxStacks(item)) {
                    ModItem modItem = ItemLoader.GetItem(item.type);
                    if (modItem != null) {
                        modItem.UseItem(player);
                    }
                    player.AddBuff(item.buffType, item.buffTime);
                }
            }
        }
    }
}