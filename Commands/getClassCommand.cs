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
    public class getClassCommand : ModCommand
    {
        // The command name typed in chat: /getclass
        public override string Command => "getclass";

        // Allows command execution in singleplayer and multiplayer
        public override CommandType Type => CommandType.Chat | CommandType.World;

        // Message shown when typing /help getclass
        public override string Description => "Gets the current player's class (usage: /getclass)";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Check if user provided an argument
            if (args.Length == 0)
            {
                var modPlayer = caller.Player.GetModPlayer<vvPlayer>();
                caller.Reply($"Current class: {modPlayer.ChosenClass}", Microsoft.Xna.Framework.Color.Green);
            }
        }
    }
}