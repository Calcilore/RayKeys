using Microsoft.Xna.Framework;
using RayKeys.Misc;

namespace RayKeys.Render {
    public class LoadingScene : Scene {
        public static string loadText = "Loading...";
        public static bool canText = false; // prevent drawing string before font is loaded
        public static float progress = 0f;
        
        public LoadingScene() {
            Game1.DrawEvent += Draw;
            
            Stuffs.Init();
        }

        private void Draw(float delta) {
            RRender.DrawBlank(Align.Left, Align.Center, 560, 0, 800, 60, Color.White);
            RRender.DrawBlank(Align.Left, Align.Center, 570, 10, (int)(780f * progress), 40, Color.Green);
            if (canText) RRender.DrawString(Align.Center, Align.Center, loadText, 0, 80, 5);
        }
    }
}