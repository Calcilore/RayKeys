using System;
using Microsoft.Xna.Framework;
using RayKeys.Render;

namespace RayKeys.Options {
    public class OptionsActions {
        public static void LimitFPSChanged(object v) {
            Console.WriteLine(v);
            Game1.Game.IsFixedTimeStep = (bool) v;
        }

        public static void FPSLimitChanged(object v) {
            string fpsS = (string) v;
            float fps = float.Parse(fpsS[..fpsS.IndexOf(" ")]);
            Game1.Game.TargetElapsedTime = TimeSpan.FromSeconds(1 / fps);
        }

        public static void ResolutionChanged(object v) {
            string[] vs = ((string) v).Split('x');
            Point a = new Point(int.Parse(vs[0]), int.Parse(vs[1]));
            //RRender.resolution = a;
            Game1.Game.Graphics.PreferredBackBufferWidth = a.X;
            Game1.Game.Graphics.PreferredBackBufferHeight = a.Y;
            Game1.Game.Scaling = Math.Min(a.X / 1920.0f, a.Y / 1080.0f);
            Game1.Game.Graphics.ApplyChanges();
            Game1.Game.RedoRenderPos();
        }
        
        public static void VSyncChanged(object v) {
            Game1.Game.Graphics.SynchronizeWithVerticalRetrace = (bool)v;
            Game1.Game.Graphics.ApplyChanges();
        }
        
        public static void FullscreenChanged(object v) {
            Game1.Game.Graphics.IsFullScreen = (bool)v;
            Game1.Game.Graphics.ApplyChanges();
        }
    }
}