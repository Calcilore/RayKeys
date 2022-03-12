using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            menu.AddPage(-1920, 0); // 6 suboptions (Editor)

            // Main Page
            menu.AddPageChangeButton(0, 1, Align.Right, Align.Top, Align.Right, Align.Center, "Play", -16, 300);
            menu.AddPageChangeButton(0, 2, Align.Right, Align.Top, Align.Right, Align.Center, "Options", -16, 400);
            menu.AddFunctionCallButton(0, Editor, Align.Right, Align.Top, Align.Right, Align.Center, "Editor", -16, 500);
            menu.AddPageChangeButton(0, 3, Align.Right, Align.Top, Align.Right, Align.Center, "Exit", -16, 600);
            //menu.AddInputField(0, Align.Right, Align.Top, "Test", -16, 700);

            // Are you sure you want to exit?
            menu.AddPageChangeNoHistoryButton(3, 0, Align.Right, Align.Top, Align.Right, Align.Center, "Cancel", -16, 400);
            menu.AddFunctionCallButton(3, Exit, Align.Right, Align.Top, Align.Right, Align.Center, "Exit", -16, 500);

            Logger.Info("Getting Resolutions");
            List<string> resolutions = new List<string>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                resolutions.Add($"{mode.Width}x{mode.Height}");
            }
            
            AddOptionCategory("Graphics", -100, 4);
            AddOptionCategory("Gameplay", 0, 5);
            AddOptionCategory("Editor", 100, 6);
            
            AddBooleanOptionButton("limitfps", 0, "Do Limit FPS");
            AddSwitcherOptionButton("fpslimit", 0, "FPS Limit", new object[]{"30 FPS", "60 FPS", "75 FPS", "120 FPS", "144 FPS", "165 FPS", "240 FPS", "1000 FPS"});
            AddSwitcherOptionButton("resolution", 0, "Resolution", resolutions.Distinct().ToArray()); // remove dupes from list and convert to array
            AddBooleanOptionButton("vsync", 0, "VSync");
            AddBooleanOptionButton("fullscreen", 0, "Fullscreen");

            OBYPos = 100;
            AddBooleanOptionButton("downscroll", 1, "Downscroll");
            AddBooleanOptionButton("repositiontracks", 1, "Reposition Tracks");
            
            OBYPos = 100;
            AddBooleanOptionButton("sectionScrolling", 2, "Section Scrolling");

            // Get The Levels in the folder
            Logger.Info("Getting Levels");
            DirectoryInfo levelF = new DirectoryInfo("Content/Levels/");
            DirectoryInfo[] dis = levelF.GetDirectories();

            // Add to songs to the play menu
            for (int i = 0; i < dis.Length; i++) {
                Button button = menu.AddButton(1, Align.Left, Align.Top, Align.Left, Align.Center, EngineManager.GetName(dis[i].Name), 16, 100 + i * 100);
                button.ClickEvent += SongButtonPressed;
                button.args = new object[] {dis[i].Name};
            }
            
            Logger.Info("Starting Random level");
            // get random one for main page
            string randomLevel = dis[ThingTools.Rand.Next(dis.Length)].Name;
            new EngineManager().Start(randomLevel, 0, 1, -300);
        }
        
        private void Draw(float delta) {
            RRender.DrawString(Align.Right, Align.Top, Align.Center, Align.Top, "RayKeys!", -450, 0, 1);
            RRender.DrawString(Align.Right, Align.Top, Align.Center, Align.Top, "The Best 6 Key Rhythm Game!", -450, 164, 5);
            
            RRender.DrawString(Align.Center, Align.Top, Align.Center, Align.Top, "Options", -1920, 0, 2);
            RRender.DrawString(Align.Center, Align.Top, Align.Center, Align.Top, "Play", 1920, 0, 2);
            RRender.DrawString(Align.Center, Align.Center, Align.Center, Align.Center, "Are you sure you\nwant to exit?", -270, -1080, 2);

            OBYPos = 200;
            foreach (OptionButton button in optionButtons[currentOptionCatagory]) {
                RRender.DrawString(Align.Right, Align.Top, Align.Right, Align.Top, button.valueText, -100-1920, OBYPos, 3);
                OBYPos += 100;
            }
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
        
        private void AddButtonCommon(string option, int optionCategory, string displayName, object[] values) {
            Button b = menu.AddButton(optionCategory + 4, Align.Left, Align.Top, Align.Left, Align.Top, displayName, 450, OBYPos);
            b.Hide();

            int index = startingCatagoryButtonId + optionCategory;
            b.args = new object[] {index, optionButtons[index].Count};
            b.ClickEvent += OnOptionButtonClick;
            
            optionButtons[index].Add(new OptionButton(b, values, option));
            OBYPos += 100;
        }
        
        private void AddBooleanOptionButton(string option, int optionCategory, string displayName) {
            AddButtonCommon(option, optionCategory, displayName, new object[] {true, false});
        }
        
        private void AddSwitcherOptionButton(string option, int optionCategory, string displayName, object[] values) {
            AddButtonCommon(option, optionCategory, displayName, values);
        }

        private void OnOptionButtonClick(int id, params object[] args) {
            int[] ii = new int[] {(int) args[0], (int) args[1]};
            OptionButton optionB = optionButtons[ii[0]][ii[1]];
            Option option = optionB.option;

            int cI = -1;
            for (int i = 0; i < optionB.values.Length; i++) {
                if (option.currentValue.ToString() == optionB.values[i].ToString()) { // without .ToString() the first time you change a boolean option it wont work
                    cI = i;
                    break;
                }
            }

            object valueTo = optionB.values[cI >= optionB.values.Length - 1 ? 0 : cI + 1];
            OptionsManager.SetOption(optionB.optionName, valueTo);
            Logger.Info($"Changing Option {optionB.optionName} to {valueTo}");

            optionB.valueText = option.currentValue.ToString();
        }

        private void OnSelectSwitch(int before, int after) {
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
