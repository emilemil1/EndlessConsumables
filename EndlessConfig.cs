using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace EndlessConsumables {
    [Label("Server Config")]
    public class EndlessConfig : ModConfig {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(0)]
        [Label("Potions Max")]
        [Tooltip("Make potions endless at this amount. 0 to disable.")]
        public int PotionsMax { get; set; }
    }
}