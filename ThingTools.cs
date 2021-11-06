using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys {
    public class ThingTools {
        public static Random rand = new Random();
        
        public static Texture2D LoadImage(string path) {
            return Texture2D.FromFile(Game1.Me.Graphics.GraphicsDevice, "Assets/" + path);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, Rectangle dr, Rectangle sr) {
            Game1.Me.SpriteBatch.Draw(texture, 
                new Rectangle((int) (dr.X * Game1.Me.Scaling), (int) (dr.Y * Game1.Me.Scaling), (int) (dr.Width * Game1.Me.Scaling), (int) (dr.Height * Game1.Me.Scaling)), 
                sr, Color.White);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, int x, int y, int sx, int sy, int ssx, int ssy) {
            Game1.Me.SpriteBatch.Draw(texture, 
                new Rectangle((int) (x * Game1.Me.Scaling), (int) (y * Game1.Me.Scaling), (int) (ssx * Game1.Me.Scaling), (int) (ssy * Game1.Me.Scaling)), 
                new Rectangle(sx, sy, ssx, ssy), Color.White);
        }
    }
}