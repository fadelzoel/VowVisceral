using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using VowVisceral.Players;

namespace VowVisceral.Items
{
    public class classLockItem : GlobalItem
    {
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (!classLockPlayer.CanUseWeaponClass(player, item))
            {
                damage = StatModifier.Default;
                damage.Flat = 1f;
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            if (player == null || !player.active)
                return;

            if (!classLockPlayer.CanUseWeaponClass(player, item))
            {
                // Rewrite the vanilla "Damage" line so the displayed number is 1
                foreach (TooltipLine line in tooltips)
                {
                    if (line.Name == "Damage")
                    {
                        line.Text = "1 damage";
                        line.OverrideColor = Color.Red;
                    }
                }

                // Add the warning line
                tooltips.Add(new TooltipLine(Mod, "ClassLocked",
                    "Locked: not your chosen class")
                {
                    OverrideColor = Color.Red
                });
            }
        }
    }
}