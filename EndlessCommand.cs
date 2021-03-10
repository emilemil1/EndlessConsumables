using Terraria.ModLoader;
using Terraria;
using System;
using System.Collections.Generic;

namespace EndlessConsumables {
    public class EndlessCommand : ModCommand {
        public override CommandType Type{ get; }
        public override string Command{ get; }
        public EndlessCommand() {
            Type = CommandType.Chat;
            Command = "endless";
        }

        public override void Action(CommandCaller caller, string input, string[] args) {
            EndlessPlayer player = caller.Player.GetModPlayer<EndlessPlayer>();
            string itemName = String.Join(" ", args);
            Item item = FindItemFromNameAndArray(itemName, player.player.inventory)
                ?? FindItemFromNameAndArray(itemName, player.player.bank.item)
                ?? FindItemFromNameAndArray(itemName, player.player.bank2.item)
                ?? FindItemFromNameAndArray(itemName, player.player.bank3.item);

            if (item == null) {
                caller.Reply("Could not find the item " + itemName + " in your inventory or portable storage.");
                return;
            }

            if (item.consumable) {
                player.addForcedPotion(item);
                return;
            }
            caller.Reply("Could not find a valid action for " + item.Name);
        }

        private Item FindItemFromNameAndArray(string itemName, Item[] arr) {
            string itemNameLower = itemName.ToLower();
            foreach(Item item in arr) {
                if (item.Name.ToLower() == itemNameLower) {
                    return item;
                }
            }
            return null;
        }
    }

}