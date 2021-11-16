using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RayKeys.Options;
using RayKeys.Render;

namespace RayKeys.Menu {
    public class MainMenu : Scene {
        private Dictionary<string, Button> buttons = new Dictionary<string, Button>();

        private Vector2 camTPos = Vector2.Zero;

        private int smx(int x) {
            return Math.Min(RRender.resolution.X * x, 1920);
        }
        
        private int smy(int y) {
            return Math.Min(RRender.resolution.Y * y, 1080);
        }
        
        private void addNavigator(int xn, int yn, Align h, Align v, string id, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 2, bool drawFrame = true) {
            Button a = new Button(h, v, id, text, x, y, sizeX, sizeY, fontSize, drawFrame);
            a.ClickEvent += NavigatorPressed;
            a.Arg = new Vector2(smx(xn), smy(yn));
            buttons.Add(id, a);
        }
        
        private void addOptionButton(string option, string[] values, Align h, Align v, string id, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 2, bool drawFrame = true) {
            Option optionO = OptionsManager.GetOption(option);
            string text = "Invalid Option";
            if (optionO.OptionType == OptionType.Boolean) {
                text = (bool) optionO.currentValue ? values[0] : values[1];
            } else if (optionO.OptionType == OptionType.Switcher) {
                text = (string) optionO.currentValue;
            }
            
            Button a = new Button(h, v, id, text, x, y, sizeX, sizeY, fontSize, drawFrame);
            a.ClickEvent += OptionChangerPressed;
            string[][] sa = new string[2][];
            sa[0] = new string[] {option};
            sa[1] = values;
            a.Arg = sa;
            buttons.Add(id, a);
        }

        public MainMenu() {
            Game1.Game.DrawEvent += Draw;

            addNavigator(1, 0, Align.Center, Align.Top, "play", "Play", 0, 350);
            addNavigator(0, 0, Align.Right, Align.Top, "backplay", "Back", 200, 0, 175, 100, 4, false);
            addNavigator(-1, 0, Align.Center, Align.Top, "options", "Options", 0, 600);
            addNavigator(0, 0, Align.Left, Align.Top, "backoptions", "Back", -200, 0, 175, 100, 4, false);

            
            // TODO: make the resolutions change the position of buttons when thing do
            List<string> resolutions = new List<string>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                resolutions.Add($"{mode.Width}x{mode.Height}");
            }

            addOptionButton("limitfps"  , new string[] {"Limit FPS", "Unlimited FPS"}, Align.Center, Align.Top, "limitfps"  , smx(-1) - 450, 100, sizeX: 800);
            addOptionButton("fpslimit"  , new string[] {"30", "60", "75", "120", "144", "165", "240", "1000"}, Align.Center, Align.Top, "fpslimit", smx(-1) + 450, 100, sizeX: 800);
            addOptionButton("resolution", resolutions.Distinct().ToArray(), Align.Center, Align.Top, "resolution", smx(-1) - 450, 350, sizeX: 800);
            addOptionButton("vsync"     , new string[] {"VSync", "No VSync"}, Align.Center, Align.Top, "vsync", smx(-1) + 450, 350, sizeX: 800);
            addOptionButton("fullscreen", new string[] {"Fullscreen", "Windowed"}, Align.Center, Align.Top, "fullscreen", smx(-1) - 450, 600, sizeX: 800);
            addOptionButton("downscroll", new string[] {"Downscroll", "Upscroll"}, Align.Center, Align.Top, "downscroll", smx(-1) + 450, 600, sizeX: 800);

            // Get The Levels in the folder
            DirectoryInfo levelF = new DirectoryInfo("Content/Levels/");
            DirectoryInfo[] dis = levelF.GetDirectories();
            for (int i = 0; i < dis.Length; i++) {
                Button levelB = new Button(Align.Center, Align.Top, "levelselect" + i, EngineManager.GetName(dis[i].Name), smx(1), 100 + (i * 100), 1200, 100, 3, false);
                levelB.ClickEvent += SongButtonPressed;
                levelB.Arg = dis[i].Name;
                buttons.Add("levelselect" + i, levelB);
            }

            levelF.GetDirectories();
        }

        private void Draw(float delta) {
            RRender.cameraPos = new Vector2(
                ThingTools.Lerp(RRender.cameraPos.X, camTPos.X, 10f * delta),
                ThingTools.Lerp(RRender.cameraPos.Y, camTPos.Y, 10f * delta));
            
            RRender.DrawString(Align.Center, Align.Top, "RayKeys!", 0, 0, 1);
            RRender.DrawString(Align.Center, Align.Top, "The Best 6 Key Rhythm Game!", 0, 164, 5);
        }
        
        private void NavigatorPressed(string id, object arg) {
            camTPos = (Vector2) arg;
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

            buttons[id].text = valueTo;
        }

        private void SongButtonPressed(string id, object arg) {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new EngineManager((string) arg));
        }
    }
}
