using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using RayKeys.Render;

namespace RayKeys.Menu {
    public class MainMenu {
        private Button playButton;
        private Button optionsButton;
        private Button aaButton;
        private string t = "The best 6 key Rhythm Game!";
        private Vector2 camTPos = new Vector2(1920, 0);
        private string[] levels;

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

            playButton = new Button(Align.Center, Align.Top, "Play", 0, 400);
            playButton.ClickEvent += PlayButtonPressed;

            optionsButton = new Button(Align.Center, Align.Top, "Options", 0, 650);
            optionsButton.ClickEvent += OptionsButtonPressed;
            
            aaButton = new Button(Align.Center, Align.Top, "1 idk", smx(1), 700, 600);
            aaButton.ClickEvent += AAButtonPressed;

            // Get The Levels in the folder
            DirectoryInfo levelF = new DirectoryInfo("Content/Levels/");
            DirectoryInfo[] dis = levelF.GetDirectories();
            levels = new string[dis.Length];
            for (int i = 0; i < dis.Length; i++) {
                levels[i] = dis[i].Name;
            }

            levelF.GetDirectories();
        }

        private void Draw(float delta) {
            RRender.cameraPos = new Vector2(
                ThingTools.Lerp(RRender.cameraPos.X, camTPos.X, 1f * delta),
                ThingTools.Lerp(RRender.cameraPos.Y, camTPos.Y, 1f * delta));
            
            RRender.DrawString(Align.Center, Align.Top, "RayKeys!", 0, 0, 1);
            RRender.DrawString(Align.Center, Align.Top, t, 0, 164, 5);

            foreach (string level in levels) {
                RRender.DrawString(Align.Center, Align.Top, level, smx(1), 100, 4);
            }
        }
        
        public void PlayButtonPressed() {
            camTPos = new Vector2(smx(1), 0);
        }

        public void OptionsButtonPressed() {
            camTPos = new Vector2(smx(-1), 0);
        }

        public void AAButtonPressed() {
            EngineManager.Start("1");
            camTPos = Vector2.Zero;
        }
    }
}
