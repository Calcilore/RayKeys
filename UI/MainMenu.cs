using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RayKeys.Misc;
using RayKeys.Options;
using RayKeys.Render;

namespace RayKeys.UI {
    public class MainMenu : Scene {
        private Menu menu;
        
        private int startingCatagoryButtonId = -1;
        private Dictionary<int, List<OptionButton>> optionButtons = new Dictionary<int, List<OptionButton>>();
        private int currentOptionCatagory = -1;
        
        private const float CameraMovePointUp = 800;
        private const float CameraMovePointDown = 280; // 1080 - CameraMovePointUp

        public MainMenu() {
            Logger.Info("Loading Main Menu");
            
            Game1.Game.DrawEvent += Draw;
            
            menu = new Menu();
            menu.EscapeEvent += OnEscape;
            menu.ChangeSelectionEvent += OnSelectSwitch;
            
            menu.AddPage(0, 0);     // 0 main page
            menu.AddPage(1920, 0);  // 1 level select
            menu.AddPage(-1920, 0); // 2 options
            menu.AddPage(0, -1080); // 3 are you sure you want to exit?
            menu.AddPage(-1920, 0); // 4 suboptions (Graphics)
            menu.AddPage(-1920, 0); // 5 suboptions (Gameplay)
            menu.AddPage(-1920, 0); // 6 suboptions (Controls)
            menu.AddPage(-1920, 0); // 7 suboptions (Editor)

            // Main Page
            menu.AddLabel(0, Align.Right, Align.Top, Align.Center, Align.Top, "RayKeys!", -450, 0, 1);
            menu.AddLabel(0, Align.Right, Align.Top, Align.Center, Align.Top, "The Best 6 Key Rhythm Game!", -450, 164, 5);
            
            menu.AddPageChangeButton(0, 1, Align.Right, Align.Top, Align.Right, Align.Center, "Play", -16, 300);
            menu.AddPageChangeButton(0, 2, Align.Right, Align.Top, Align.Right, Align.Center, "Options", -16, 400);
            menu.AddFunctionCallButton(0, Editor, Align.Right, Align.Top, Align.Right, Align.Center, "Editor", -16, 500);
            menu.AddPageChangeButton(0, 3, Align.Right, Align.Top, Align.Right, Align.Center, "Exit", -16, 600);

            // Are you sure you want to exit?
            menu.AddLabel(3, Align.Center, Align.Center, Align.Center, Align.Center, "Are you sure you\nwant to exit?", -270, 0, 2);
            
            menu.AddPageChangeNoHistoryButton(3, 0, Align.Right, Align.Top, Align.Right, Align.Center, "Cancel", -16, 400);
            menu.AddFunctionCallButton(3, Exit, Align.Right, Align.Top, Align.Right, Align.Center, "Exit", -16, 500);

            
            // Options
            menu.AddLabel(2, Align.Center, Align.Top, Align.Center, Align.Top, "Options", 0, 0, 2);
            
            Logger.Info("Getting Resolutions");
            List<string> resolutions = new List<string>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                resolutions.Add($"{mode.Width}x{mode.Height}");
            }
            
            AddOptionCategory("Graphics", -150, 4);
            AddOptionCategory("Gameplay", -50, 5);
            AddOptionCategory("Controls", 50, 6);
            AddOptionCategory("Editor", 150, 7);
            
            AddBooleanOptionButton("limitfps", 0, "If the FPS limit should be followed");
            AddSwitcherOptionButton("fpslimit", 0, new object[]{"30 FPS", "60 FPS", "75 FPS", "120 FPS", "144 FPS", "165 FPS", "240 FPS", "1000 FPS"}, "What the FPS limit should be");
            AddSwitcherOptionButton("resolution", 0, resolutions.Distinct().ToArray(), "Change the Game's Resolution"); // remove dupes from list and convert to array
            AddBooleanOptionButton("vsync", 0, "Sync frames to monitors frames");
            AddBooleanOptionButton("fullscreen", 0, "Windowed or Fullscreen Mode");

            OBYPos = 100;
            AddBooleanOptionButton("downscroll", 1, "Do notes move up or down");

            OBYPos = 100;
            AddBooleanOptionButton("sectionScrolling", 3, "You can change sections in the editor by scrolling past the border");

            OBYPos = 100;
            for (int i = 0; ((Controls)i).ToString() != i.ToString(); i++) {
                if (i == 6 || i == 12) OBYPos += 100;
                AddKeyOptionButton("control_" + (Controls)i, 2, "Enter to set, Escape to cancel");
            }
            
            // Play
            menu.AddLabel(1, Align.Center, Align.Top, Align.Center, Align.Top, "Play", 0, 0, 2);
            
            // Get The Levels in the folder
            Logger.Info("Getting Levels");
            DirectoryInfo levelF = new DirectoryInfo("Content/Levels/");
            DirectoryInfo[] dis = levelF.GetDirectories();

            // Add to songs to the play menu
            for (int i = 0; i < dis.Length; i++) {
                Button button = menu.AddButton(1, Align.Left, Align.Top, Align.Left, Align.Center, SongJsonManager.LoadJson(dis[i].Name).name, 16, 100 + i * 100);
                button.ClickEvent += SongButtonPressed;
                button.args = new object[] {dis[i].Name};
            }
            
            Logger.Info("Starting Random level");
            // get random one for main page
            string randomLevel = dis[ThingTools.Rand.Next(dis.Length)].Name;
            new EngineManager().Start(randomLevel, 0, 1, -300);
        }
        
        private void Draw(float delta) {
            foreach (OptionButton button in optionButtons[currentOptionCatagory]) {
                RRender.DrawString(Align.Right, Align.Top, Align.Right, Align.Top, button.valueText, -100-1920, (int) button.button.pos.Y + 100, 3);
            }
            
            RRender.DrawBlank(Align.Left, Align.Bottom, -1920,  -64 + (int)RRender.CameraPos.Y, 1920, 64, Color.DarkSlateGray, 0.4f);
        }

        private void AddOptionCategory(string text, int yp, int pt) {
            Button b = menu.AddPageChangeButton(2, pt, Align.Left, Align.Center, Align.Left, Align.Center, text, 16, yp);
            optionButtons.Add(b.Id, new List<OptionButton>());
            if (startingCatagoryButtonId == -1) {
                startingCatagoryButtonId = b.Id;
                currentOptionCatagory = b.Id;
            }
        }

        private int OBYPos = 100;

        private Button AddButtonCommon(string option, int optionCategory, string tooltip) {
            Button b = menu.AddButton(optionCategory + 4, Align.Left, Align.Top, Align.Left, Align.Top, OptionsManager.GetOption(option).DisplayName, 450, OBYPos);
            Label l = menu.AddLabel(optionCategory + 4, Align.Center, Align.Bottom, Align.Center, Align.Bottom, tooltip, 1920, -5, 4, Color.White, 0.3f, b);
            l.followCamera = true;
            b.Hide();
            
            OBYPos += 100;
            return b;
        }
        
        private void AddSwitcherOptionButton(string option, int optionCategory, object[] values, string tooltip) {
            Button b = AddButtonCommon(option, optionCategory, tooltip);

            optionButtons[startingCatagoryButtonId + optionCategory].Add(new OptionButtonSwitcher(b, values, option));
        }
        
        private void AddKeyOptionButton(string option, int optionCategory, string tooltip) {
            Button b = AddButtonCommon(option, optionCategory, tooltip);

            optionButtons[startingCatagoryButtonId + optionCategory].Add(new OptionButtonKey(b, option));
        }
        
        private void AddBooleanOptionButton(string option, int optionCategory, string tooltip) {
            AddSwitcherOptionButton(option, optionCategory, new object[] {true, false}, tooltip);
        }

        private void OnSelectSwitch(int before, int after) {
            // Camera Movement
            foreach (List<OptionButton> category in optionButtons.Values) {
                foreach (OptionButton button in category) {
                    if (button.button.Id == after) {
                        if (before < after) { // Buttons are added in order from top to bottom so this should work
                            Logger.Debug($"Down, {button.button.pos.Y} < {CameraMovePointUp}");
                            
                            if (button.button.pos.Y > RRender.CameraPos.Y + CameraMovePointUp) 
                                 menu.tPos.Y = button.button.pos.Y - CameraMovePointUp;
                            else ;
                        }
                        else {
                            Logger.Debug($"Up, {button.button.pos.Y} < {RRender.CameraPos.Y} + {CameraMovePointDown} = {button.button.pos.Y} < {RRender.CameraPos.Y + CameraMovePointDown}");
                            
                            if (button.button.pos.Y < RRender.CameraPos.Y + CameraMovePointDown) 
                                 menu.tPos.Y = button.button.pos.Y - 100;
                            else ;
                        }
                    }
                }
            }
            
            // Category Change
            if (!optionButtons.ContainsKey(after)) return;

            foreach (KeyValuePair<int, List<OptionButton>> c in optionButtons) {
                if (c.Key == after) {
                    foreach (OptionButton b in c.Value) {
                        b.button.Show();
                    }
                } else {
                    foreach (OptionButton b in c.Value) {
                        b.button.Hide();
                    }
                }
            }

            currentOptionCatagory = after;
        }
        
        private void OnEscape() {
            menu.ChangePage(3);
        }
        
        private void Exit() {
            Game1.Game.Exit();
        }
        
        private void Editor() {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new Editor.Editor());
        }

        private void SongButtonPressed(int Id, params object[] args) {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new PlayScene((string) args[0], 1));
        }
    }
}
