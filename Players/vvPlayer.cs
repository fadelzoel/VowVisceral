using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace VowVisceral.Players
{
    public enum PlayerClass
    {
        None,
        Melee,
        Ranged,
        Magic,
        Summon
    }

    public class vvPlayer : ModPlayer
    {
        // --- CLASS DATA ---
        public PlayerClass ChosenClass = PlayerClass.None;

        // --- EXP & PROGRESSION DATA ---
        public int CurrentLevel = 1;
        public int CurrentEXP = 0;
        public int SkillPoints = 0;

        // --- TEMPORARY / FRAME-BASED FLAGS ---
        public bool SuppressClassLock = false;

        public override void ResetEffects()
        {
            SuppressClassLock = false;
        }

        // --- SAVE DATA (Writes to character file on exit) ---
        public override void SaveData(TagCompound tag)
        {
            tag["ChosenClass"] = (int)ChosenClass;
            tag["CurrentLevel"] = CurrentLevel;
            tag["CurrentEXP"] = CurrentEXP;
            tag["SkillPoints"] = SkillPoints;
        }

        // --- LOAD DATA (Reads from character file on load) ---
        public override void LoadData(TagCompound tag)
        {
            // Read ChosenClass (defaults to None if missing)
            if (tag.ContainsKey("ChosenClass"))
            {
                ChosenClass = (PlayerClass)tag.GetInt("ChosenClass");
            }
            else
            {
                ChosenClass = PlayerClass.None;
            }

            // Read Level (defaults to 1 for old/new characters)
            if (tag.ContainsKey("CurrentLevel"))
            {
                CurrentLevel = tag.GetInt("CurrentLevel");
            }
            else
            {
                CurrentLevel = 1;
            }

            // Read EXP (defaults to 0 if missing)
            if (tag.ContainsKey("CurrentEXP"))
            {
                CurrentEXP = tag.GetInt("CurrentEXP");
            }
            else
            {
                CurrentEXP = 0;
            }

            // Read Skill Points (defaults to 0 if missing)
            if (tag.ContainsKey("SkillPoints"))
            {
                SkillPoints = tag.GetInt("SkillPoints");
            }
            else
            {
                SkillPoints = 0;
            }
        }
    }
}
