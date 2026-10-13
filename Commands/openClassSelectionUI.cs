using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VowVisceral.Players;
using VowVisceral.UI;
using VowVisceral.Commands;

namespace VowVisceral.Commands
{
    public class openClassSelectionUI : ModCommand
    {
        public override string Command => "openclassselection";
        public override CommandType Type => CommandType.Chat | CommandType.World;
        public override string Description => "Opens the class selection UI (usage: /openclassselection)";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var modSystem = ModContent.GetInstance<classSelectionSystem>();
            modSystem.ShowUI();
            caller.Reply("Class selection UI opened.", Microsoft.Xna.Framework.Color.Green);
        }
    }
}
