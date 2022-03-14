using Microsoft.Xna.Framework;

namespace RayKeys.Render {
    public class LoadingScene : Scene {
        public static string loadText = "Loading...";
        public static float progress = 0f;
        
        public LoadingScene() {
            Game1.Game.DrawEvent += Draw;
            
            Stuffs.Init();
        }

        public void Draw(float delta) {
            RRender.DrawBlank(Align.Left, Align.Center, 560, 0, 800, 60, Color.White);
            RRender.DrawBlank(Align.Left, Align.Center, 570, 10, (int)(780f * progress), 40, Color.Green);
            RRender.DrawString(Align.Center, Align.Center, loadText, 0, 80, 5);
        }
    }
}