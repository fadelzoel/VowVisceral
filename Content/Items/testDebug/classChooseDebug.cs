using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VowVisceral.Players;
using VowVisceral.UI;

namespace VowVisceral.Content.Items.testDebug
{
    public class classChooseDebug : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Gray;
            Item.value = 0;
            Item.consumable = true;
        }

        // Allow right-clicking this item from hotbar or inventory
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            var modPlayer = player.GetModPlayer<classLockPlayer>();

            // if right click, nothing happens, just return true to prevent the left click action from happening
            if (player.altFunctionUse == 2)
            {
               
                return true;
            }

            // LEFT CLICK: Quickly wipe class back to None
            ModContent.GetInstance<classSelectionSystem>().ShowUI();
            Main.NewText("[Debug] Opening Class Selection Menu...", 255, 255, 100);

            return true;
        }
    }
}
