using Terraria.ModLoader;
using Terraria;
using System;
using System.Collections.Generic;

namespace EndlessConsumables
{
    public class EndlessCommand : ModCommand
    {
        public override CommandType Type => CommandType.World;
        public override string Command => "endless";
        public override string Usage => "/endless [amount] [item name]";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            EndlessPlayer player = caller.Player.GetModPlayer<EndlessPlayer>();
            int amount = 0;
            string itemName;
            int.TryParse(args[0], out amount);
            if (amount <= 0)
            {
                itemName = String.Join(" ", new List<string>(args));
            }
            else
            {
                itemName = String.Join(" ", new List<string>(args).GetRange(1, args.Length - 1).ToArray());
            }
            Item item = Utils.FindItemFromNameAndArray(itemName, player.player.inventory)
                ?? Utils.FindItemFromNameAndArray(itemName, player.player.bank.item)
                ?? Utils.FindItemFromNameAndArray(itemName, player.player.bank2.item)
                ?? Utils.FindItemFromNameAndArray(itemName, player.player.bank3.item);

            if (item == null)
            {
                caller.Reply("Could not find the item " + itemName + " in your inventory or portable storage.");
                return;
            }

            if (item.consumable)
            {
                if (amount <= 0) amount = item.maxStack;
                ModContent.GetInstance<EndlessWorld>().ToggleCustomEndlessItem(item, amount);
                return;
            }
            caller.Reply("Could not find a valid action for " + item.Name);
        }
    }

    public class AutouseCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "autouse";
        public override string Usage => "/autouse [item name]";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            EndlessPlayer player = caller.Player.GetModPlayer<EndlessPlayer>();
            string itemName = String.Join(" ", new List<string>(args));
            Item item = Utils.FindItemFromNameAndArray(itemName, player.player.inventory)
                ?? Utils.FindItemFromNameAndArray(itemName, player.player.bank.item)
                ?? Utils.FindItemFromNameAndArray(itemName, player.player.bank2.item)
                ?? Utils.FindItemFromNameAndArray(itemName, player.player.bank3.item);

            if (item == null)
            {
                caller.Reply("Could not find the item " + itemName + " in your inventory or portable storage.");
                return;
            }

            if (item.consumable)
            {
                player.ToggleAutoUse(item);
                return;
            }
            caller.Reply("Could not find a valid action for " + item.Name);
        }
    }
}