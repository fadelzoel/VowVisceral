using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using VowVisceral.UI;
using VowVisceral.Players;

namespace VowVisceral.UI
{
    public class classSelectionSystem : ModSystem
    {
        // --- UI REFERENCES ---
        internal UserInterface SelectionUserInterface;
        internal classSelectionUI SelectionUI;

        // --- TRACKING ---
        // Prevents the UI from auto-opening every single frame on first spawn.
        // Set to true once the UI has been shown for a "None" class player.
        private bool hasAutoShownThisSession = false;

        // ============================================================
        // LOAD / UNLOAD
        // ============================================================

        public override void Load()
        {
            // Client-only: servers have no UI, so skip loading entirely on them.
            if (!Main.dedServ)
            {
                SelectionUI = new classSelectionUI();
                SelectionUI.Activate();
                SelectionUserInterface = new UserInterface();
            }
        }

        public override void Unload()
        {
            // Clean up references so the mod can be reloaded without stale state.
            SelectionUI = null;
            SelectionUserInterface = null;
        }

        // ============================================================
        // SHOW / HIDE HELPERS
        // ============================================================

        /// <summary>
        /// Opens the class selection UI.
        /// </summary>
        public void ShowUI()
        {
            SelectionUserInterface?.SetState(SelectionUI);
        }

        /// <summary>
        /// Closes the class selection UI.
        /// </summary>
        public void HideUI()
        {
            SelectionUserInterface?.SetState(null);
        }

        /// <summary>
        /// Convenience check so other systems (e.g. classLockPlayer) can ask
        /// "is the UI currently open?" without touching UserInterface directly.
        /// </summary>
        public bool IsUIOpen => SelectionUserInterface?.CurrentState != null;

        // ============================================================
        // AUTO-SHOW LOGIC (runs every tick on the client)
        // ============================================================

        public override void PostUpdatePlayers()
        {
            if (Main.dedServ)
                return;

            // Only care about the local player (the one whose screen this is).
            Player localPlayer = Main.LocalPlayer;

            // Guard: LocalPlayer may be null during world load / menu transitions.
            if (localPlayer == null || !localPlayer.active)
                return;

            vvPlayer modPlayer = localPlayer.GetModPlayer<vvPlayer>();

            // Don't auto-open if the accessory is suppressing class lock,
            // OR if the player already chose a class.
            if (modPlayer.SuppressClassLock)
                return;

            if (modPlayer.ChosenClass != PlayerClass.None)
                return;

            // Don't spam the UI open — only auto-show once per session.
            // The player can still reopen it manually via a keybind or item.
            if (hasAutoShownThisSession)
                return;

            // Don't interrupt the player mid-menu (e.g. inventory, chest, etc.)
            if (Main.playerInventory || Main.gameMenu || Main.drawingPlayerChat)
                return;

            hasAutoShownThisSession = true;
            ShowUI();
        }

        // ============================================================
        // UPDATE / DRAW
        // ============================================================

        public override void UpdateUI(GameTime gameTime)
        {
            // Only update if a state is actually set, and only on client.
            if (!Main.dedServ && SelectionUserInterface?.CurrentState != null)
            {
                SelectionUserInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Insert our UI just below the vanilla resource bars so it doesn't
            // cover health/mana. FindIndex returns -1 if not found, so guard it.
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                // Insert AFTER inventory → draws on top of it
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer(
                    "VowVisceral: Class Selection UI",
                    delegate {
                        if (SelectionUserInterface?.CurrentState != null)
                        {
                            SelectionUserInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}