using Terraria;
using Terraria.ModLoader;
using VowVisceral.Players;

namespace VowVisceral.Players;

public class classLockPlayer : ModPlayer
{
    public static bool CanUseWeaponClass(Player player, Item item)
    {
        var data = player.GetModPlayer<vvPlayer>();

        if (data.SuppressClassLock)
            return true;

        if (data.ChosenClass == PlayerClass.None)
            return true;

        if (item.damage <= 0 || item.DamageType == DamageClass.Default)
            return true;

        return data.ChosenClass switch
        {
            PlayerClass.Melee => item.CountsAsClass(DamageClass.Melee),
            PlayerClass.Ranged => item.CountsAsClass(DamageClass.Ranged),
            PlayerClass.Magic => item.CountsAsClass(DamageClass.Magic),
            PlayerClass.Summon => item.CountsAsClass(DamageClass.Summon),
            _ => true
        };
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        Item held = Player.HeldItem;
        if (held == null || held.IsAir)
            return;

        if (!CanUseWeaponClass(Player, held))
        {
            // Nuke all multipliers, then force exactly 1 damage.
            modifiers.FinalDamage.Base = 1f;
            modifiers.FinalDamage *= 0f;
            modifiers.FinalDamage.Flat += 1f;

            // also kill crits so it's always exactly 1
            modifiers.DisableCrit();
        }
    }
}