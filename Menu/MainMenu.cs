using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.Options;
using RayKeys.Render;

namespace RayKeys.Menu {
    public class MainMenu : Scene {
        private Dictionary<string, Button> buttons = new Dictionary<string, Button>();
        private Dictionary<string, int> levelButtons = new Dictionary<string, int>(); // <string id, int yPosition (without scroll)>
        private Dictionary<string, Dictionary<string, int>> categories = new Dictionary<string, Dictionary<string, int>>();
        private string currentCategory;

        private Point scrollBounds = Point.Zero; // X is higher, Y is lower (just wanted 2 ints)
        private int scrollPos = 0; private float scrollPosL = 0f;

        private Vector2 camTPos = Vector2.Zero;

        private int smx(int x) {
            return Math.Min(RRender.resolution.X * x, 1920);
        }
        
        private int smy(int y) {
            return Math.Min(RRender.resolution.Y * y, 1080);
        }
        
        private void addNavigator(int xn, int yn, int spbt, int scbb, Align h, Align v, string id, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 2, bool drawFrame = true) {
            Button a = new Button(h, v, id, text, x, y, sizeX, sizeY, fontSize, drawFrame);
            a.ClickEvent += NavigatorPressed;
            a.Arg = new Rectangle(smx(xn), smy(yn), spbt, scbb); // rectangle because i need 4 ints in one variable
            buttons.Add(id, a);

            scrollBounds = new Point(spbt, scbb);
        }
        
        private void addOptionButton(string option, string[] values, string category, Align h = Align.Center, Align v = Align.Top, int sizeX = 1000, int sizeY = 200, int fontSize = 2, bool drawFrame = true) {
            Option optionO = OptionsManager.GetOption(option);
            string text = "Invalid Option";
            if (optionO.OptionType == OptionType.Boolean) {
                text = (bool) optionO.currentValue ? values[0] : values[1];
            } else if (optionO.OptionType == OptionType.Switcher) {
                text = (string) optionO.currentValue;
            }

            int y = categories[category].Count * 250 + 100;

            Button a = new Button(Align.Center, Align.Top, option, text, smx(-1), y, sizeX, sizeY, fontSize, drawFrame);
            a.ClickEvent += OptionChangerPressed;
            string[][] sa = new string[2][];
            sa[0] = new string[] {option};
            sa[1] = values;
            a.Arg = sa;
            buttons.Add(option, a);
            
            // categories
            categories[category].Add(option, y);
        }

        private void CreateCategory(string category) {
            categories.Add(category, new Dictionary<string, int>());
        }

        private void ChangeCategory(string category) {
            if (currentCategory != null)
                foreach (string id in categories[currentCategory].Keys) {
                    Button button = buttons[id];
                    button.Hide();
                }
            
            foreach (string id in categories[category].Keys) {
                Button button = buttons[id];
                button.Show();
            }

            currentCategory = category;
            scrollBounds = new Point(0, categories[category].Count * 250 + 100);
        }

        public MainMenu() {
            Game1.Game.DrawEvent += Draw;

            // Get all of the resolutions and put them into a list
            List<string> resolutions = new List<string>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                resolutions.Add($"{mode.Width}x{mode.Height}");
            }
            
            // Create Categories
            CreateCategory("Graphics");
            CreateCategory("Gameplay");

            // Create Category Buttons
            {
                int sPos = categories.Count * -75 + 75;
                string[] categoriesKeys = categories.Keys.ToArray();
                for (int i = 0; i < categories.Count; i++) {
                    string category = categoriesKeys[i];
                    string id = "categorybutton:" + category;
                    Button button = new Button(Align.Left, Align.Center, id, category, smx(-1) + 50, sPos + 150 * i, 300, 100, 5);
                    button.Arg = category;
                    button.ClickEvent += CategoryButtonPressed;
                    buttons.Add(id, button);
                }
            }

            // Graphics Category
            addOptionButton("limitfps"  , new string[] {"Limit FPS", "Unlimited FPS"}, "Graphics");
            addOptionButton("fpslimit"  , new string[] {"30 FPS", "60 FPS", "75 FPS", "120 FPS", "144 FPS", "165 FPS", "240 FPS", "1000 FPS"}, "Graphics");
            addOptionButton("resolution", resolutions.Distinct().ToArray(), "Graphics"); // remove dupes from list and convert to array
            addOptionButton("vsync"     , new string[] {"VSync", "No VSync"}, "Graphics");
            addOptionButton("fullscreen", new string[] {"Fullscreen", "Windowed"}, "Graphics");
            
            // Gameplay Category
            addOptionButton("downscroll", new string[] {"Downscroll", "Upscroll"}, "Gameplay");
            addOptionButton("repositiontracks", new string[] {"Reposition Tracks", "Normal Tracks"}, "Gameplay");

            // Hide all categories
            foreach (Dictionary<string, int> category in categories.Values) {
                foreach ((string id, int pos) in category) {
                    Button button = buttons[id];
                    button.Hide();
                }
            }
            
            ChangeCategory("Graphics");

            // Get The Levels in the folder
            DirectoryInfo levelF = new DirectoryInfo("Content/Levels/");
            DirectoryInfo[] dis = levelF.GetDirectories();
            for (int i = 0; i < dis.Length; i++) {
                Button levelB = new Button(Align.Center, Align.Top, "levelselect" + i, EngineManager.GetName(dis[i].Name), smx(1), 100 + (i * 100), 1200, 100, 3, false);
                levelB.ClickEvent += SongButtonPressed;
                levelB.Arg = dis[i].Name;
                buttons.Add("levelselect" + i, levelB);
                levelButtons.Add(levelB.Id, levelB.Y);
            }

            // Add Navigators
            addNavigator(1, 0, 0, (dis.Length * 100) + 200, Align.Center, Align.Top, "play", "Play", 0, 350);
            addNavigator(0, 0, 0, 0, Align.Right, Align.Top, "backplay", "Back", 200, 0, 175, 100, 4, false);
            addNavigator(-1, 0, 0, categories[currentCategory].Count * 250 + 100, Align.Center, Align.Top, "options", "Options", 0, 600);
            addNavigator(0, 0, 0, 0, Align.Left, Align.Top, "backoptions", "Back", -200, 0, 175, 100, 4, false);
        }

        private void Draw(float delta) {
            scrollPos = Math.Min(scrollPos, scrollBounds.Y - 1080); // Do the bottom first so that if it breaks both, it will place down
            scrollPos = Math.Max(scrollPos, scrollBounds.X);
            scrollPos -= ThingTools.GetScrollFrame();

            scrollPosL = ThingTools.Lerp(scrollPosL, scrollPos, 10f * delta);

            foreach ((string key, int value) in levelButtons) {
                Button button = buttons[key];
                //button.Y = (int)ThingTools.Lerp(button.Y, value - scrollPos, 10f * delta);
                button.Y = (int) (value - scrollPosL);
            }

            foreach ( Dictionary<string, int> category in categories.Values) {
                foreach ((string key, int value) in category) {
                    Button button = buttons[key];
                    //button.Y = (int)ThingTools.Lerp(button.Y, value - scrollPos, 10f * delta);
                    button.Y = (int) (value - scrollPosL);
                }
            }

            for (float i = 0; i < 1; i += 0.01f) {
                RRender.Draw(Align.Left, Align.Center, Game1.Game.Textures["notes"], 0 + (int)(i * 300), (int)ThingTools.Lerp(0, 100, i), 0, 0, 3, 3);
            }

            RRender.cameraPos = new Vector2(
                ThingTools.Lerp(RRender.cameraPos.X, camTPos.X, 10f * delta),
                ThingTools.Lerp(RRender.cameraPos.Y, camTPos.Y, 10f * delta));
            
            RRender.DrawString(Align.Center, Align.Top, "RayKeys!", 0, 0, 1);
            RRender.DrawString(Align.Center, Align.Top, "The Best 6 Key Rhythm Game!", 0, 164, 5);
        }
        
        private void NavigatorPressed(string id, object arg) {
            Rectangle ar = (Rectangle) arg;
            camTPos = new Vector2(ar.X, ar.Y);
            scrollBounds = new Point(ar.Width, ar.Height);
        }
        
        private void CategoryButtonPressed(string id, object arg) {
            ChangeCategory((string) arg);
        }

        private void OptionChangerPressed(string id, object arg) {
            string option = ((string[][]) arg)[0][0];
            string[] values = ((string[][]) arg)[1];
            Option cOption = OptionsManager.GetOption(option);

            string valueTo;

            if (cOption.OptionType == OptionType.Boolean) {
                OptionsManager.SetOption(option, !(bool) cOption.currentValue);
                valueTo = !(bool) cOption.currentValue ? values[1] : values[0];
            } else if (cOption.OptionType == OptionType.Switcher) {
                int cI = -1;
                for (int i = 0; i < values.Length; i++) {
                    if ((string) cOption.currentValue == values[i]) {
                        cI = i;
                        break;
                    }
                }
                
                valueTo = values[cI >= values.Length - 1 ? 0 : cI + 1];
                OptionsManager.SetOption(option, valueTo);
            } else return;

            buttons[id].Text = valueTo;
        }

        private void SongButtonPressed(string id, object arg) {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new EngineManager((string) arg));
        }
    }
}
