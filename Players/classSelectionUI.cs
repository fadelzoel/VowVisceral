using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using VowVisceral.Players;

namespace VowVisceral.UI
{
    public class classSelectionUI : UIState
    {
        // ============================================================
        // LAYOUT CONSTANTS
        // ============================================================
        private const float PanelWidthPercent = 0.75f;
        private const float PanelHeightPercent = 0.75f;

        private const float HeaderHeight = 56f;     // space reserved for the title
        private const float OuterMargin = 16f;      // gap between main panel edge and content
        private const float ColumnGap = 12f;        // gap between left list and right column

        private const float LeftPanelWidth = 300f;
        private const float CardHeight = 64f;
        private const float CardSpacing = 6f;

        private const float ImageFraction = 0.40f;  // share of the right column used by the image box
        private const float BoxGap = 10f;           // gap between image box / desc box / button

        private const float ConfirmButtonW = 240f;
        private const float ConfirmButtonH = 44f;

        private const float ScrollbarWidth = 20f;

        // ============================================================
        // COLORS
        // ============================================================
        private static readonly Color PanelBg = new Color(20, 25, 45) * 0.98f;
        private static readonly Color PanelBorder = new Color(89, 116, 213);
        private static readonly Color DarkBg = new Color(12, 16, 30);
        private static readonly Color BoxBg = new Color(25, 32, 55);
        private static readonly Color DimBorder = new Color(60, 80, 150);
        private static readonly Color BrightBorder = new Color(140, 170, 255);

        private static readonly Color CardNormal = new Color(30, 40, 70) * 0.9f;
        private static readonly Color CardHover = new Color(45, 60, 100);
        private static readonly Color CardSelected = new Color(55, 80, 145);

        private static readonly Color ButtonNormal = new Color(40, 60, 110);
        private static readonly Color ButtonHover = new Color(55, 80, 145);
        private static readonly Color ButtonDisabled = new Color(30, 38, 62);

        // ============================================================
        // REFERENCES
        // ============================================================
        private UIPanel mainPanel;
        private UIPanel leftPanel;
        private UIPanel rightImageBox;
        private UIPanel rightDescBox;

        private UIList classList;
        private UIScrollbar listScrollbar;

        private UIText descTitle;
        private WrappedTextBlock descBody;
        private UIList descList;
        private UIScrollbar descScrollbar;

        private UIPanel confirmButton;
        private UIText confirmText;

        private ClassEntry previewedClass;
        private readonly Dictionary<ClassEntry, UIPanel> cards = new Dictionary<ClassEntry, UIPanel>();

        // ============================================================
        // CLASS DATA
        // ============================================================
        public static List<ClassEntry> AvailableClasses = new List<ClassEntry>
        {
            new ClassEntry("Melee", PlayerClass.Melee, null, null,
                "The frontline bruiser. High defense, high melee damage, and " +
                "access to swords, spears, flails, and yoyos.\n\n" +
                "Great for players who like to get close and personal, tanking " +
                "hits while dealing steady damage."),

            new ClassEntry("Ranged", PlayerClass.Ranged, null, null,
                "The sharpshooter. Bows, guns, and thrown weapons. Fragile but " +
                "deadly at a distance.\n\n" +
                "Rewards positioning and ammo management."),

            new ClassEntry("Magic", PlayerClass.Magic, null, null,
                "The arcane caster. Staves, wands, and tomes that consume mana. " +
                "High burst damage, low sustained defense.\n\n" +
                "Requires mana management and rewards burst windows."),

            new ClassEntry("Summoner", PlayerClass.Summon, null, null,
                "The commander. Whips and summon staves. Let your minions do " +
                "the fighting while you stay back.\n\n" +
                "Low personal damage, high passive damage."),
        };

        // ============================================================
        // INITIALIZE
        // ============================================================
        public override void OnInitialize()
        {
            // ---------- MAIN PANEL ----------
            // UIPanel has 12px default padding which silently shifts every child.
            // We zero it and use explicit margins instead so the math is predictable.
            mainPanel = new UIPanel();
            mainPanel.SetPadding(0f);
            mainPanel.Width.Set(0f, PanelWidthPercent);
            mainPanel.Height.Set(0f, PanelHeightPercent);
            mainPanel.MinWidth.Set(760f, 0f);
            mainPanel.MaxWidth.Set(1200f, 0f);
            mainPanel.MinHeight.Set(480f, 0f);
            mainPanel.MaxHeight.Set(720f, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = PanelBg;
            mainPanel.BorderColor = PanelBorder;
            Append(mainPanel);

            // ---------- TITLE ----------
            UIText title = new UIText("Choose Your Class", 0.85f, true);
            title.HAlign = 0.5f;
            title.Top.Set(14f, 0f);
            mainPanel.Append(title);

            // ---------- BODY (everything under the title) ----------
            UIElement body = new UIElement();
            body.Left.Set(OuterMargin, 0f);
            body.Top.Set(HeaderHeight, 0f);
            body.Width.Set(-OuterMargin * 2f, 1f);
            body.Height.Set(-HeaderHeight - OuterMargin, 1f);
            mainPanel.Append(body);

            // ---------- LEFT: CLASS LIST ----------
            leftPanel = new UIPanel();
            leftPanel.SetPadding(0f);
            leftPanel.Width.Set(LeftPanelWidth, 0f);
            leftPanel.Height.Set(0f, 1f);
            leftPanel.BackgroundColor = DarkBg;
            leftPanel.BorderColor = DimBorder;
            body.Append(leftPanel);

            listScrollbar = new UIScrollbar();
            listScrollbar.HAlign = 1f;
            listScrollbar.Left.Set(-6f, 0f);
            listScrollbar.Top.Set(6f, 0f);
            listScrollbar.Height.Set(-12f, 1f);
            leftPanel.Append(listScrollbar);

            classList = new UIList();
            classList.Left.Set(6f, 0f);
            classList.Top.Set(6f, 0f);
            classList.Width.Set(-(6f + ScrollbarWidth + 6f + 4f), 1f);
            classList.Height.Set(-12f, 1f);
            classList.ListPadding = CardSpacing;
            classList.ManualSortMethod = _ => { };
            classList.SetScrollbar(listScrollbar);
            leftPanel.Append(classList);

            // ---------- RIGHT COLUMN ----------
            UIElement rightColumn = new UIElement();
            rightColumn.Left.Set(LeftPanelWidth + ColumnGap, 0f);
            rightColumn.Width.Set(-(LeftPanelWidth + ColumnGap), 1f);
            rightColumn.Height.Set(0f, 1f);
            body.Append(rightColumn);

            // Space at the bottom of the column for the confirm button + a gap.
            float reserved = ConfirmButtonH + BoxGap;

            // ---- Image box (top 40% of what's left after the button row) ----
            rightImageBox = new UIPanel();
            rightImageBox.SetPadding(10f);
            rightImageBox.Top.Set(0f, 0f);
            rightImageBox.Width.Set(0f, 1f);
            rightImageBox.Height.Set(-(reserved * ImageFraction + BoxGap / 2f), ImageFraction);
            rightImageBox.BackgroundColor = BoxBg;
            rightImageBox.BorderColor = DimBorder;
            rightColumn.Append(rightImageBox);

            UIText imagePlaceholder = new UIText("Class Image", 1f, false);
            imagePlaceholder.HAlign = 0.5f;
            imagePlaceholder.VAlign = 0.5f;
            imagePlaceholder.TextColor = new Color(100, 120, 180);
            rightImageBox.Append(imagePlaceholder);

            // ---- Description box (remaining 60%) ----
            // Bottom edge works out to exactly (columnHeight - reserved).
            rightDescBox = new UIPanel();
            rightDescBox.SetPadding(0f);
            rightDescBox.Top.Set(-(reserved * ImageFraction) + BoxGap / 2f, ImageFraction);
            rightDescBox.Width.Set(0f, 1f);
            rightDescBox.Height.Set(-(reserved * (1f - ImageFraction) + BoxGap / 2f), 1f - ImageFraction);
            rightDescBox.BackgroundColor = BoxBg;
            rightDescBox.BorderColor = DimBorder;
            rightColumn.Append(rightDescBox);

            descTitle = new UIText("Select a Class", 0.8f, true);
            descTitle.Left.Set(14f, 0f);
            descTitle.Top.Set(12f, 0f);
            rightDescBox.Append(descTitle);

            const float descListTop = 52f;
            const float descListBottomMargin = 10f;

            descScrollbar = new UIScrollbar();
            descScrollbar.HAlign = 1f;
            descScrollbar.Left.Set(-6f, 0f);
            descScrollbar.Top.Set(descListTop, 0f);
            descScrollbar.Height.Set(-(descListTop + descListBottomMargin), 1f);
            rightDescBox.Append(descScrollbar);

            descList = new UIList();
            descList.Left.Set(14f, 0f);
            descList.Top.Set(descListTop, 0f);
            descList.Width.Set(-(14f + ScrollbarWidth + 6f + 6f), 1f);
            descList.Height.Set(-(descListTop + descListBottomMargin), 1f);
            descList.SetScrollbar(descScrollbar);
            rightDescBox.Append(descList);

            descBody = new WrappedTextBlock(0.95f);
            descBody.SetText("Pick a class from the left to see details here.");
            descBody.HeightChanged = () => descList.Recalculate();
            descList.Add(descBody);

            // ---- Confirm button (its own row at the bottom of the column) ----
            confirmButton = new UIPanel();
            confirmButton.SetPadding(0f);
            confirmButton.Width.Set(ConfirmButtonW, 0f);
            confirmButton.Height.Set(ConfirmButtonH, 0f);
            confirmButton.HAlign = 1f;
            confirmButton.VAlign = 1f;
            confirmButton.BackgroundColor = ButtonDisabled;
            confirmButton.BorderColor = DimBorder;
            rightColumn.Append(confirmButton);

            // Normal font here (not the huge "Death" font) so it always fits the button.
            confirmText = new UIText("Choose a Class", 1.0f, false);
            confirmText.HAlign = 0.5f;
            confirmText.VAlign = 0.5f;
            confirmText.TextColor = Color.Gray;
            confirmButton.Append(confirmText);

            confirmButton.OnLeftClick += (evt, el) => ConfirmSelection();
            confirmButton.OnMouseOver += (evt, el) =>
            {
                if (previewedClass != null)
                    confirmButton.BackgroundColor = ButtonHover;
            };
            confirmButton.OnMouseOut += (evt, el) =>
                confirmButton.BackgroundColor = previewedClass != null ? ButtonNormal : ButtonDisabled;

            BuildClassList();
        }

        // ============================================================
        // UPDATE
        // ============================================================
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Stop clicks on the UI from also swinging your weapon / using items.
            if (mainPanel.ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
        }

        // ============================================================
        // BUILD CLASS CARDS
        // ============================================================
        private void BuildClassList()
        {
            classList.Clear();
            cards.Clear();

            foreach (ClassEntry entry in AvailableClasses)
                classList.Add(BuildClassCard(entry));

            classList.Recalculate();
        }

        private static Asset<Texture2D> LoadTexture(string path)
        {
            if (string.IsNullOrEmpty(path) || !ModContent.HasAsset(path))
                return null;
            return ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
        }

        private UIElement BuildClassCard(ClassEntry entry)
        {
            UIPanel card = new UIPanel();
            card.SetPadding(0f);
            card.Width.Set(0f, 1f);
            card.Height.Set(CardHeight, 0f);
            card.BackgroundColor = CardNormal;
            card.BorderColor = DimBorder;
            cards[entry] = card;

            // Icon (or blue placeholder square)
            const float iconSize = 40f;
            const float iconLeft = 12f;

            Asset<Texture2D> icon = LoadTexture(entry.TexturePath);
            UIElement iconSlot;
            if (icon != null)
            {
                UIImage img = new UIImage(icon);
                img.ScaleToFit = true;
                iconSlot = img;
            }
            else
            {
                UIPanel blue = new UIPanel();
                blue.SetPadding(0f);
                blue.BackgroundColor = new Color(63, 82, 151);
                blue.BorderColor = PanelBorder;
                iconSlot = blue;
            }
            iconSlot.Width.Set(iconSize, 0f);
            iconSlot.Height.Set(iconSize, 0f);
            iconSlot.Left.Set(iconLeft, 0f);
            iconSlot.VAlign = 0.5f;
            iconSlot.IgnoresMouseInteraction = true;
            card.Append(iconSlot);

            // Name (normal font, vertically centered in the card)
            UIText nameText = new UIText(entry.DisplayName, 1.15f, false);
            nameText.Left.Set(iconLeft + iconSize + 14f, 0f);
            nameText.VAlign = 0.5f;
            nameText.IgnoresMouseInteraction = true;
            card.Append(nameText);

            card.OnLeftClick += (evt, el) => PreviewClass(entry);
            card.OnMouseOver += (evt, el) => ApplyCardColors(entry, true);
            card.OnMouseOut += (evt, el) => ApplyCardColors(entry, false);

            return card;
        }

        private void ApplyCardColors(ClassEntry entry, bool hovered)
        {
            if (!cards.TryGetValue(entry, out UIPanel card))
                return;

            bool selected = entry == previewedClass;
            card.BackgroundColor = selected ? CardSelected : (hovered ? CardHover : CardNormal);
            card.BorderColor = selected ? BrightBorder : DimBorder;
        }

        // ============================================================
        // PREVIEW
        // ============================================================
        private void PreviewClass(ClassEntry entry)
        {
            ClassEntry old = previewedClass;
            previewedClass = entry;

            if (old != null) ApplyCardColors(old, false);
            ApplyCardColors(entry, true);

            descTitle.SetText(entry.DisplayName);
            descBody.SetText(entry.Description);
            descScrollbar.ViewPosition = 0f;
            descList.Recalculate();

            confirmText.SetText($"Choose {entry.DisplayName}");
            confirmText.TextColor = Color.White;
            confirmButton.BackgroundColor = ButtonNormal;
            confirmButton.BorderColor = PanelBorder;

            // Swap image box content
            rightImageBox.RemoveAllChildren();
            Asset<Texture2D> bigImage = LoadTexture(entry.BigImagePath);

            if (bigImage != null)
            {
                UIImage image = new UIImage(bigImage);
                image.Width.Set(0f, 1f);
                image.Height.Set(0f, 1f);
                image.HAlign = 0.5f;
                image.VAlign = 0.5f;
                image.ScaleToFit = true;
                rightImageBox.Append(image);
            }
            else
            {
                UIText placeholder = new UIText(entry.DisplayName + " Image", 1f, false);
                placeholder.HAlign = 0.5f;
                placeholder.VAlign = 0.5f;
                placeholder.TextColor = new Color(100, 120, 180);
                rightImageBox.Append(placeholder);
            }

            rightImageBox.Recalculate();
        }

        // ============================================================
        // CONFIRM
        // ============================================================
        private void ConfirmSelection()
        {
            if (previewedClass == null)
            {
                Main.NewText("Pick a class first.", 255, 200, 100);
                return;
            }

            Player player = Main.LocalPlayer;
            vvPlayer modPlayer = player.GetModPlayer<vvPlayer>();

            if (modPlayer.SuppressClassLock)
            {
                Main.NewText("Class lock is suppressed by an accessory.", 255, 200, 100);
                return;
            }

            modPlayer.ChosenClass = previewedClass.ClassType;
            Main.NewText($"You have chosen the {previewedClass.DisplayName} class!", 50, 255, 130);

            ModContent.GetInstance<classSelectionSystem>().HideUI();
        }

        // ============================================================
        // DATA
        // ============================================================
        public class ClassEntry
        {
            public string DisplayName;
            public PlayerClass ClassType;
            public string TexturePath;
            public string BigImagePath;
            public string Description;

            public ClassEntry(string displayName, PlayerClass classType,
                              string texturePath, string bigImagePath,
                              string description)
            {
                DisplayName = displayName;
                ClassType = classType;
                TexturePath = texturePath;
                BigImagePath = bigImagePath;
                Description = description;
            }
        }

        public static void RegisterClass(string displayName, PlayerClass classType,
                                         string texturePath, string bigImagePath,
                                         string description)
        {
            AvailableClasses.Add(new ClassEntry(displayName, classType,
                texturePath, bigImagePath, description));
        }

        // ============================================================
        // WRAPPED TEXT ELEMENT
        // UIText's built-in wrapping measures itself before it has a real width,
        // so inside a UIList the height comes out wrong and the text overlaps or
        // gets cut off. This element re-wraps whenever its real width is known
        // (or changes, e.g. on resize) and reports its true height to the list.
        // ============================================================
        private class WrappedTextBlock : UIElement
        {
            private const float LineHeight = 22f;   // per line at scale 1

            private string text = "";
            private List<string> lines = new List<string>();
            private float lastWidth = -1f;
            private readonly float textScale;

            public Color TextColor = Color.White;
            public Action HeightChanged;

            public WrappedTextBlock(float scale)
            {
                textScale = scale;
                Width.Set(0f, 1f);
                Height.Set(LineHeight * scale, 0f);
            }

            public void SetText(string newText)
            {
                text = newText ?? "";
                lastWidth = -1f; // force a re-wrap on the next Update
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);

                float width = GetInnerDimensions().Width;
                if (width < 1f || width == lastWidth)
                    return;

                lastWidth = width;
                Rewrap(width);
            }

            private void Rewrap(float maxWidth)
            {
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                lines = Wrap(text, font, maxWidth - 4f, textScale);

                Height.Set(lines.Count * LineHeight * textScale + 8f, 0f);

                if (HeightChanged != null)
                    HeightChanged();
                else
                    Recalculate();
            }

            private static List<string> Wrap(string input, DynamicSpriteFont font, float maxWidth, float scale)
            {
                List<string> result = new List<string>();

                foreach (string paragraph in input.Split('\n'))
                {
                    if (paragraph.Length == 0)
                    {
                        result.Add(""); // blank line between paragraphs
                        continue;
                    }

                    string line = "";
                    foreach (string word in paragraph.Split(' '))
                    {
                        string test = line.Length == 0 ? word : line + " " + word;
                        if (line.Length > 0 && font.MeasureString(test).X * scale > maxWidth)
                        {
                            result.Add(line);
                            line = word;
                        }
                        else
                        {
                            line = test;
                        }
                    }
                    result.Add(line);
                }

                return result;
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                Vector2 pos = GetInnerDimensions().Position();
                float lineStep = LineHeight * textScale;

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Length == 0)
                        continue;

                    Utils.DrawBorderString(spriteBatch, lines[i],
                        pos + new Vector2(0f, i * lineStep), TextColor, textScale);
                }
            }
        }
    }
}