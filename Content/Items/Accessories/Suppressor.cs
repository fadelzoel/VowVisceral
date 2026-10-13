using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VowVisceral.Content.Buffs;

namespace VowVisceral.Content.Items.Accessories
{
    public class Suppressor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<ClassSuppressedBuff>(), 10);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "SuppressorDesc",
                "Suppresses class lock while equipped")
            {
                OverrideColor = Color.Cyan
            });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 5)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}