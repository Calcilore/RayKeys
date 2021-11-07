using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys {
    public class ThingTools {
        public static Random rand = new Random();
        
        public static float Lerp(float firstFloat, float secondFloat, float by) {
            // line comment first line to enable second line
            return firstFloat + (secondFloat - firstFloat) * by; /*
            return firstFloat * (1 - by) + secondFloat * by; /**/
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, Rectangle dr, Rectangle sr) {
            Game1.Game.SpriteBatch.Draw(texture, 
                new Rectangle((int) (dr.X * Game1.Game.Scaling), (int) (dr.Y * Game1.Game.Scaling), (int) (dr.Width * Game1.Game.Scaling), (int) (dr.Height * Game1.Game.Scaling)), 
                sr, Color.White);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, int x, int y, int sx, int sy, int ssx, int ssy) {
            Game1.Game.SpriteBatch.Draw(texture, 
                new Rectangle((int) (x * Game1.Game.Scaling), (int) (y * Game1.Game.Scaling), (int) (ssx * Game1.Game.Scaling), (int) (ssy * Game1.Game.Scaling)), 
                new Rectangle(sx, sy, ssx, ssy), Color.White);
        }
    }
}