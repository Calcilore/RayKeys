using System;
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
        
        private const float CameraMovePointUp = 360;
        private const float CameraMovePointDown = 720; // 1080 - CameraMovePointUp

        private const int LevelSelectPage = 1;
        private const int EditorSelectPage = 2;
        private const int NewLevelPage = 3;
        private const int RUSureUWTEPage = 4;
        private const int OptionsPage = 5;

        public MainMenu() {
            Logger.Info("Loading Main Menu");
            
            Game1.Game.DrawEvent += Draw;
            
            menu = new Menu();
            menu.EscapeEvent += OnEscape;
            menu.ChangeSelectionEvent += OnSelectSwitch;
            
            menu.AddPage(0, 0);         // 0 main page
            menu.AddPage(1920, 0);      // 1 level select
            menu.AddPage(0, 1080);      // 2 editor select
            menu.AddPage(0, 1080*2);    // 3 editor new level
            menu.AddPage(0, -1080);     // 4 are you sure you want to exit?
            menu.AddPage(-1920, 0);     // 5 options
            menu.AddPage(-1920, 0);     // 6 suboptions (Graphics)
            menu.AddPage(-1920, 0);     // 7 suboptions (Gameplay)
            menu.AddPage(-1920, 0);     // 8 suboptions (Controls)
            menu.AddPage(-1920, 0);     // 9 suboptions (Editor)

            // Main Page
            menu.AddLabel(0, Align.Right, Align.Top, Align.Center, Align.Top, "RayKeys!", -450, 0, 1);
            menu.AddLabel(0, Align.Right, Align.Top, Align.Center, Align.Top, "The Best 6 Key Rhythm Game!", -450, 164, 5);
            
            menu.AddPageChangeButton(0, LevelSelectPage, Align.Right, Align.Top, Align.Right, Align.Center, "Play", -16, 300);
            menu.AddPageChangeButton(0, OptionsPage, Align.Right, Align.Top, Align.Right, Align.Center, "Options", -16, 400);
            menu.AddPageChangeButton(0, EditorSelectPage, Align.Right, Align.Top, Align.Right, Align.Center, "Editor", -16, 500);
            menu.AddPageChangeButton(0, RUSureUWTEPage, Align.Right, Align.Top, Align.Right, Align.Center, "Exit", -16, 600);

            menu.AddLabel(0, Align.Left, Align.Top, Align.Left, Align.Top, "A", 64, 300);
            menu.AddLabel(0, Align.Center, Align.Top, Align.Center, Align.Top, "V", -1920/2 + 364, 300);
            menu.AddLabel(0, Align.Right, Align.Top, Align.Right, Align.Top, "C", -1920 + 664, 300);

            // Are you sure you want to exit?
            menu.AddLabel(RUSureUWTEPage, Align.Center, Align.Center, Align.Center, Align.Center, "Are you sure you\nwant to exit?", -270, 0, 2);
            
            menu.AddPageChangeNoHistoryButton(RUSureUWTEPage, 0, Align.Right, Align.Top, Align.Right, Align.Center, "Cancel", -16, 400);
            menu.AddFunctionCallButton(RUSureUWTEPage, Exit, Align.Right, Align.Top, Align.Right, Align.Center, "Exit", -16, 500);

            
            // Options
            menu.AddLabel(OptionsPage, Align.Center, Align.Top, Align.Center, Align.Top, "Options", 0, 0, 2);
            
            Logger.Info("Getting Resolutions");
            List<string> resolutions = new List<string>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                resolutions.Add($"{mode.Width}x{mode.Height}");
            }
            
            AddOptionCategory("Graphics", -150, OptionsPage + 1);
            AddOptionCategory("Gameplay", -50,  OptionsPage + 2);
            AddOptionCategory("Controls", 50,   OptionsPage + 3);
            AddOptionCategory("Editor", 150,    OptionsPage + 4);
            
            OBYPos = 200;
            AddBooleanOptionButton("limitfps", 0, "If the FPS limit should be followed");
            AddSwitcherOptionButton("fpslimit", 0, new object[]{"30 FPS", "60 FPS", "75 FPS", "120 FPS", "144 FPS", "165 FPS", "240 FPS", "1000 FPS"}, "What the FPS limit should be");
            AddSwitcherOptionButton("resolution", 0, resolutions.Distinct().ToArray(), "Change the Game's Resolution"); // remove dupes from list and convert to array
            AddBooleanOptionButton("vsync", 0, "Sync frames to monitors frames");
            AddBooleanOptionButton("fullscreen", 0, "Windowed or Fullscreen Mode");

            OBYPos = 200;
            AddBooleanOptionButton("downscroll", 1, "Do notes move up or down");

            OBYPos = 200;
            AddBooleanOptionButton("sectionScrolling", 3, "You can change sections in the editor by scrolling past the border");

            OBYPos = 200;
            for (int i = 0; ((Controls)i).ToString() != i.ToString(); i++) {
                //if (i == 6 || i == 12) OBYPos += 100;
                AddKeyOptionButton("control_" + (Controls)i, 2, "Enter to set, Escape to cancel");
            }
            
            // Play
            menu.AddLabel(LevelSelectPage, Align.Center, Align.Top, Align.Center, Align.Top, "Play", 0, 0, 2);
            
            // Editor
            menu.AddLabel(EditorSelectPage, Align.Center, Align.Top, Align.Center, Align.Top, "Editor", 0, 0, 2);
            menu.AddPageChangeButton(EditorSelectPage, NewLevelPage, Align.Center, Align.Top, Align.Center, Align.Center, "New Level", 0, 200);
            
            // Get The Levels in the folder
            Logger.Info("Getting Levels");
            DirectoryInfo levelF = new DirectoryInfo("Content/Levels/");
            DirectoryInfo[] dis = levelF.GetDirectories();
            List<string> validLevels = new List<string>();

            // Add to songs to the play menu
            for (int i = 0; i < dis.Length; i++) {
                if (!SongJsonManager.LoadJson(dis[i].Name, out JsonFileThing song)) {
                    Logger.Error("Failed to load song: " + dis[i].Name);
                    continue;
                }
                
                validLevels.Add(dis[i].Name);
                
                Button button = menu.AddButton(LevelSelectPage, Align.Left, Align.Top, Align.Left, Align.Center, song.name, 16, 200 + i * 100);
                button.ClickEvent += SongButtonPressed;
                button.args = new object[] {dis[i].Name};
                
                Button ebutton = menu.AddButton(EditorSelectPage, Align.Center, Align.Top, Align.Center, Align.Center, song.name, 0, 300 + i * 100);
                ebutton.ClickEvent += EditorButtonPressed;
                ebutton.args = new object[] {dis[i].Name};
            }
            
            Logger.Info("Starting Random level");
            // get random one for main page
            string randomLevel = validLevels[ThingTools.Rand.Next(validLevels.Count)];
            new EngineManager().Start(randomLevel, 0, 1, -300);
            
            // New Level
            menu.AddLabel(NewLevelPage, Align.Center, Align.Top, Align.Center, Align.Top, "New Level", 0, 0, 2);
            menu.AddInputField(NewLevelPage, Align.Left, Align.Top, "Name", 16, 100);
            menu.AddFunctionCallButton(NewLevelPage, CreateButtonPressed, Align.Left, Align.Top, Align.Left, Align.Top, "Create", 16, 300);
        }
        
        private void Draw(float delta) {
            foreach (OptionButton button in optionButtons[currentOptionCatagory]) {
                //RRender.DrawString(Align.Right, Align.Top, Align.Right, Align.Top, button.valueText, -100-1920, (int) button.button.pos.Y, 3);
                RRender.DrawString(Align.Left, Align.Top, button.displayName, 450-1920, (int) button.menuItem.pos.Y, 3);
            }
            
            RRender.DrawBlank(Align.Left, Align.Bottom, -1920,  -64 + (int)RRender.CameraPos.Y, 1920, 64, Color.DarkSlateGray, 0.4f);
        }

        private void AddOptionCategory(string text, int yp, int pt) {
            Button b = menu.AddPageChangeButton(OptionsPage, pt, Align.Left, Align.Center, Align.Left, Align.Center, text, 16, yp);
            optionButtons.Add(b.Id, new List<OptionButton>());
            if (startingCatagoryButtonId == -1) {
                startingCatagoryButtonId = b.Id;
                currentOptionCatagory = b.Id;
            }
        }

        private int OBYPos;

        private Button AddButtonCommon(string option, int optionCategory, string tooltip) {
            Button b = menu.AddButton(optionCategory + OptionsPage + 1, Align.Right, Align.Top, Align.Right, Align.Top, OptionsManager.GetOption(option).CurrentValue.ToString(), -100, OBYPos);
            Label l = menu.AddLabel(optionCategory + OptionsPage + 1, Align.Center, Align.Bottom, Align.Center, Align.Bottom, tooltip, 1920, -5, 4, Color.White, 0.3f, b);
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
            int lastPos = 0;
            int setCat = -1;
            int i = 0;
            
            foreach (List<OptionButton> category in optionButtons.Values) {
                foreach (OptionButton button in category) {
                    if (button.menuItem.Id == after) {
                        setCat = i;

                        if (before < after) {
                            // Buttons are added in order from top to bottom so this should work
                            Logger.Debug($"Down");

                            if (button.menuItem.pos.Y - RRender.CameraPos.Y > CameraMovePointDown) {
                                menu.tPos.Y = CameraMovePointUp + button.menuItem.pos.Y - 1080;
                            }
                        }
                        else {
                            Logger.Debug($"Up");

                            if (button.menuItem.pos.Y - RRender.CameraPos.Y < CameraMovePointUp) {
                                menu.tPos.Y = -CameraMovePointUp + button.menuItem.pos.Y;
                            }
                        }
                    }
                    
                    if (setCat == i) lastPos = (int) button.menuItem.pos.Y;
                }

                i++;
            }

            if (setCat != -1) menu.tPos.Y = Math.Max(Math.Min(menu.tPos.Y, lastPos - 1080 + 200), 0f);

            // Category Change
            if (!optionButtons.ContainsKey(after)) return;

            foreach (KeyValuePair<int, List<OptionButton>> c in optionButtons) {
                if (c.Key == after) {
                    foreach (OptionButton b in c.Value) {
                        b.menuItem.Show();
                    }
                } else {
                    foreach (OptionButton b in c.Value) {
                        b.menuItem.Hide();
                    }
                }
            }

            currentOptionCatagory = after;
        }
        
        private void OnEscape() {
            menu.ChangePage(RUSureUWTEPage);
        }
        
        private void Exit() {
            Game1.Game.Exit();
        }

        private bool CheckIfFolderExists(DirectoryInfo[] dis, string name) {
            foreach (DirectoryInfo di in dis) {
                if (di.Name.ToLower() == name)
                    return true;
            }

            return false;
        }
        
        private void CreateButtonPressed() {
            InputField nameField = (InputField) menu.pages[NewLevelPage].FocusableItems[0];
            string name = nameField.Text;
            string dir = name;
            
            // Stop user from creating 2 dirs with same name
            Logger.Info("Getting Levels");
            DirectoryInfo levelF = new DirectoryInfo("Content/Levels/");
            DirectoryInfo[] dis = levelF.GetDirectories();

            foreach (DirectoryInfo di in dis) {
                if (di.Name.ToLower() == name) {
                    Logger.Info("Level Already Exists");
                    int i = 0;
                    while (CheckIfFolderExists(dis, name + i)) {
                        i++;
                    }

                    dir = name + i;
                    break;
                }
            }
            
            Directory.CreateDirectory("Content/Levels/" + dir);
            LoadEditor(dir);
        }
        
        private void EditorButtonPressed(int id, params object[] args) {
            LoadEditor(args[0] as string);
        }

        private void LoadEditor(string path) {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new Editor.Editor(path));
        }

        private void SongButtonPressed(int id, params object[] args) {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new PlayScene((string) args[0], 1));
        }
    }
}
