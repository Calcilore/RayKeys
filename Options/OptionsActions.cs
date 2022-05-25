using System;
using Microsoft.Xna.Framework;
using RayKeys.Render;

namespace RayKeys.Options {
    public static class OptionsActions {
        public static void LimitFPSChanged(string k, object v) {
            Game1.Game.IsFixedTimeStep = (bool) v;
            Game1.Graphics.ApplyChanges();
        }

        public static void FPSLimitChanged(string k, object v) {
            string fpsS = (string) v;
            float fps = float.Parse(fpsS[..fpsS.IndexOf(" ")]);
            Game1.Game.TargetElapsedTime = TimeSpan.FromSeconds(1 / fps);
        }

        public static void ResolutionChanged(string k, object v) {
            string[] vs = ((string) v).Split('x');
            Point a = new Point(int.Parse(vs[0]), int.Parse(vs[1]));
            //RRender.resolution = a;
            Game1.Graphics.PreferredBackBufferWidth = a.X;
            Game1.Graphics.PreferredBackBufferHeight = a.Y;
            Game1.Scaling = Math.Min(a.X / 1920.0f, a.Y / 1080.0f);
            Game1.Graphics.ApplyChanges();
            Game1.Game.RedoRenderPos();
        }
        
        public static void VSyncChanged(string k, object v) {
            Game1.Graphics.SynchronizeWithVerticalRetrace = (bool)v;
            Game1.Graphics.ApplyChanges();
        }
        
        public static void FullscreenChanged(string k, object v) {
            Game1.Graphics.IsFullScreen = (bool)v;
            Game1.Graphics.ApplyChanges();
        }
    }
}