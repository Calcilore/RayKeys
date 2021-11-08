using System;
using Microsoft.Xna.Framework;

namespace RayKeys.Menu {
    public class MainMenu {
        private Button playButton;
        private string t = "The best 6 key Rhythm Game!";

        public void PlayButtonPressed() {
            Game1.Game.DrawEvent -= Draw;
            EngineManager.Start("1", 1.2f);
        }
        
        public MainMenu() {
            Game1.Game.DrawEvent += Draw;

            playButton = new Button("Play", 960 - 300, 400);
            playButton.ClickEvent += PlayButtonPressed;
        }

        private void Draw(float delta) {
            Console.WriteLine("Hello");
            ThingTools.DrawStringCentered("RayKeys!", 960, 64, 1);
            ThingTools.DrawStringCentered(t, 960, 164, 5);
        }
    }
}