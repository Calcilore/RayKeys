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

        public static Texture2D LoadPNG(string location) {
            return Texture2D.FromFile(Game1.Game.GraphicsDevice, "Content/" + location);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, Rectangle dr, Rectangle sr) {
            Game1.Game.SpriteBatch.Draw(texture, 
                new Rectangle((int) (dr.X * Game1.Game.Scaling), (int) (dr.Y * Game1.Game.Scaling), (int) (dr.Width * Game1.Game.Scaling), (int) (dr.Height * Game1.Game.Scaling)), 
                sr, Color.White);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, Rectangle dr, Rectangle sr, Color color) {
            Game1.Game.SpriteBatch.Draw(texture, 
                new Rectangle((int) (dr.X * Game1.Game.Scaling), (int) (dr.Y * Game1.Game.Scaling), (int) (dr.Width * Game1.Game.Scaling), (int) (dr.Height * Game1.Game.Scaling)), 
                sr, color);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, int x, int y, int sourceX, int sourceY, int sizeX, int sizeY) {
            Game1.Game.SpriteBatch.Draw(texture, 
                new Rectangle((int) (x * Game1.Game.Scaling), (int) (y * Game1.Game.Scaling), (int) (sizeX * Game1.Game.Scaling), (int) (sizeY * Game1.Game.Scaling)), 
                new Rectangle(sourceX, sourceY, sizeX, sizeY), Color.White);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, int x, int y, int sourceX, int sourceY, int sizeX, int sizeY, Color color) {
            Game1.Game.SpriteBatch.Draw(texture, 
                new Rectangle((int) (x * Game1.Game.Scaling), (int) (y * Game1.Game.Scaling), (int) (sizeX * Game1.Game.Scaling), (int) (sizeY * Game1.Game.Scaling)), 
                new Rectangle(sourceX, sourceY, sizeX, sizeY), color);
        }

        public static void DrawString(string text, int px, int py, int scale) {
            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, new Vector2(px, py), Color.White);
        }
        
        public static void DrawString(string text, int px, int py, int scale, Color color) {
            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, new Vector2(px, py), color);
        }
        
        public static void DrawStringCentered(string text, int px, int py, int scale) {
            Vector2 pos = new Vector2(px, py) - Game1.Game.Fonts[scale].MeasureString(text) / 2;
            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, pos, Color.White);
        }
        
        public static void DrawStringCentered(string text, int px, int py, int scale, Color color) {
            Vector2 pos = new Vector2(px, py) - Game1.Game.Fonts[scale].MeasureString(text) / 2;
            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, pos, color);
        }
    }
}