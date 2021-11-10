using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using RayKeys.Render;

namespace RayKeys.Menu {
    public class MainMenu {
        private Button playButton;
        private Button optionsButton;
        private Button[] levelButtons;
        private string t = "The best 6 key Rhythm Game!";
        private Vector2 camTPos = new Vector2(1920, 0);

        private Vector2 sm(int x, int y) {
            return new Vector2(RRender.resolution.X * x, RRender.resolution.Y * y);
        }
        
        private int smx(int x) {
            return RRender.resolution.X * x;
        }
        
        private int smy(int y) {
            return RRender.resolution.Y * y;
        }
        
        public MainMenu() {
            Game1.Game.DrawEvent += Draw;

            playButton = new Button(Align.Center, Align.Top, "play", "Play", 0, 400);
            playButton.ClickEvent += PlayButtonPressed;

            optionsButton = new Button(Align.Center, Align.Top, "options", "Options", 0, 650);
            optionsButton.ClickEvent += OptionsButtonPressed;

            // Get The Levels in the folder
            DirectoryInfo levelF = new DirectoryInfo("Content/Levels/");
            DirectoryInfo[] dis = levelF.GetDirectories();
            levelButtons = new Button[dis.Length];
            for (int i = 0; i < dis.Length; i++) {
                levelButtons[i] = new Button(Align.Center, Align.Top, dis[i].Name, dis[i].Name, smx(1), 100 + (i * 100), 1200, 100, 3, false);
                levelButtons[i].ClickEvent += SongButtonPressed;
            }

            levelF.GetDirectories();
        }

        private void Draw(float delta) {
            RRender.cameraPos = new Vector2(
                ThingTools.Lerp(RRender.cameraPos.X, camTPos.X, 2f * delta),
                ThingTools.Lerp(RRender.cameraPos.Y, camTPos.Y, 2f * delta));
            
            RRender.DrawString(Align.Center, Align.Top, "RayKeys!", 0, 0, 1);
            RRender.DrawString(Align.Center, Align.Top, t, 0, 164, 5);
        }
        
        public void PlayButtonPressed(Button b) {
            camTPos = new Vector2(smx(1), 0);
        }

        public void OptionsButtonPressed(Button b) {
            camTPos = new Vector2(smx(-1), 0);
        }

        public void SongButtonPressed(Button b) {
            camTPos = Vector2.Zero;
            RRender.cameraPos = Vector2.Zero;
            foreach (Button bu in levelButtons) { bu.Delete(); }
            playButton.Delete(); optionsButton.Delete();
            Game1.Game.DrawEvent -= Draw;

            EngineManager.Start(b.id);
        }
    }
}
