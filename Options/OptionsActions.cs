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
            Game1.Game.TargetElapsedTime = TimeSpan.FromSeconds(1 / float.Parse((string)v));
        }

        public static void ResolutionChanged(object v) {
            string[] vs = ((string) v).Split('x');
            RRender.resolution = new Point(int.Parse(vs[0]), int.Parse(vs[1]));
            Game1.Game.Graphics.PreferredBackBufferWidth = RRender.resolution.X;
            Game1.Game.Graphics.PreferredBackBufferHeight = RRender.resolution.Y;
            Game1.Game.Graphics.ApplyChanges();
        }
        
        public static void VSyncChanged(object v) {
            Game1.Game.Graphics.SynchronizeWithVerticalRetrace = (bool)v;
            Game1.Game.Graphics.ApplyChanges();
        }
    }
}