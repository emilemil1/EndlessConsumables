using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace EndlessConsumables
{
    public class EndlessPlayer : ModPlayer
    {
        private List<int> diedWithBuffs;
        public HashSet<int> autoUses;
        private int autoUseCooldown = 300;

        public override void Initialize()
        {
            diedWithBuffs = new List<int>();
            autoUses = new HashSet<int>();
        }

        public void ToggleAutoUse(Item item)
        {
            int type = item.type;
            if (autoUses.Contains(type))
            {
                autoUses.Remove(type);
                Main.NewText(item.Name + " is no longer being autoused.");
            }
            else
            {
                autoUses.Add(type);
                Main.NewText(item.Name + " is now being autoused.");
            }
        }

        public override void PreUpdate()
        {
            autoUseCooldown--;
            if (autoUseCooldown != 0) return;
            autoUseCooldown = 300;

            foreach (int itemType in autoUses)
            {
                RefreshPotionBuff(itemType, true);
            }
        }

        public override bool ConsumeAmmo(Item weapon, Item ammo)
        {
            bool def = base.ConsumeAmmo(weapon, ammo);
            return Utils.IsAtMaxStacks(ammo) ? false : def;
        }

        public override void PreUpdateBuffs()
        {
            for (int i = 0; i < player.buffType.Length; i++)
            {
                if (player.buffTime[i] <= 60)
                {
                    RefreshPotionBuff(player.buffType[i]);
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool result = base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);

            diedWithBuffs.Clear();
            diedWithBuffs.AddRange(player.buffType);

            return result;
        }

        public override void OnRespawn(Player player)
        {
            diedWithBuffs.ForEach(buff => RefreshPotionBuff(buff));
            diedWithBuffs.Clear();
        }

        private void RefreshPotionBuff(int type, bool itemType = false)
        {
            RefreshPotionBuffForArray(type, player.inventory, itemType);
            RefreshPotionBuffForArray(type, player.bank.item, itemType);
            RefreshPotionBuffForArray(type, player.bank2.item, itemType);
            RefreshPotionBuffForArray(type, player.bank3.item, itemType);
        }

        private void RefreshPotionBuffForArray(int type, Item[] arr, bool itemType)
        {
            foreach (Item item in arr)
            {
                if (item.consumable == false) continue;

                if ((itemType ? item.type == type : item.buffType == type) && Utils.IsAtMaxStacks(item))
                {
                    ModItem modItem = ItemLoader.GetItem(item.type);
                    if (modItem != null)
                    {
                        modItem.UseItem(player);
                    }
                    player.AddBuff(item.buffType, item.buffTime);
                }
            }
        }

        public override TagCompound Save()
        {
            TagCompound tagCompound = new TagCompound();
            tagCompound.Add("autoreuse", new List<int>(autoUses));
            return tagCompound;
        }

        public override void Load(TagCompound tag)
        {
            autoUses = new HashSet<int>(tag.GetList<int>("autoreuse"));
        }
    }
}