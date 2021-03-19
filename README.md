TODO

# Endless Consumables

A simple convenience mod for tModLoader.

## Table of Contents

1. [Usage](#usage)
2. [Commands](#commands)
3. [Change Log](#commands)

## Usage

Simply reach maximum stacks and your item will be marked as "(endless)", at which point it will no longer consume stacks when used.

Additionally any endless buff potions will be automatically refreshed, as long as the potion is either in your inventory or portable storage (except Void Vault).

## Commands

For the most part this mod should just work, though it relies on certain conventions that mods sometimes don't follow when they add their own consumables. For this reason there are a few commands that can help you make these items work:

### /endless [amount] [consumable name]

This command is used to lower how many stacks are required for an item to turn endless. If the amount isn't specified it will default to whatever the maximum number of stacks are.

Repeat the command (with any value for amount) to revert.

### /autouse [buff potion name]

By default the mod tries to determine which consumables are buff potions by looking to see if an item.buffType property is present. This is the case for all vanilla buff potions, but not necessarily for those added by mods. This command will force those potions to be consumed every 5 seconds, if they are endless.

Repeat the command to revert.

## Change Log

### v0.1
- Mod created
### v0.1.1
- GitHub connected
### v0.1.2
- /endless command reworked
- /autouse command added
### v0.1.3
- /endless command can disable consumables