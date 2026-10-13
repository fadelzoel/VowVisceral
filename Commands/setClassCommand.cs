using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VowVisceral.Players;

namespace VowVisceral.Commands
{
    public class setClassCommand : ModCommand
    {
        // The command name typed in chat: /setclass
        public override string Command => "setclass";

        // Allows command execution in singleplayer and multiplayer
        public override CommandType Type => CommandType.Chat | CommandType.World;

        // Message shown when typing /help setclass
        public override string Description => "Sets the current player's class for debugging (usage: /setclass <melee|ranged|magic|summon|none>)";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Check if user provided an argument
            if (args.Length == 0)
            {
                caller.Reply("Please specify a class: melee, ranged, magic, summon, or none.", Microsoft.Xna.Framework.Color.Red);
                return;
            }

            var modPlayer = caller.Player.GetModPlayer<vvPlayer>();

            // Parse argument (case-insensitive)
            string choice = args[0].ToLower();

            switch (choice)
            {
                case "melee":
                    modPlayer.ChosenClass = PlayerClass.Melee;
                    break;
                case "ranged":
                    modPlayer.ChosenClass = PlayerClass.Ranged;
                    break;
                case "magic":
                    modPlayer.ChosenClass = PlayerClass.Magic;
                    break;
                case "summon":
                    modPlayer.ChosenClass = PlayerClass.Summon;
                    break;
                case "none":
                    modPlayer.ChosenClass = PlayerClass.None;
                    break;
                default:
                    caller.Reply($"Invalid class '{args[0]}'. Options: melee, ranged, magic, summon, none.", Microsoft.Xna.Framework.Color.Red);
                    return;
            }

            caller.Reply($"Class successfully set to: {modPlayer.ChosenClass}", Microsoft.Xna.Framework.Color.Green);
        }
    }
}