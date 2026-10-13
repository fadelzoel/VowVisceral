using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VowVisceral.Players;
using VowVisceral;

namespace VowVisceral.Content.Buffs
{
    public class ClassSuppressedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;   // hide the timer
            Main.buffNoSave[Type] = true;          // don't persist on save/load
            
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Re-assert the flag every tick the buff is active.
            // This doubles as a safety net — even if something clear
            player.GetModPlayer<vvPlayer>().SuppressClassLock = true;
        }
    }
}