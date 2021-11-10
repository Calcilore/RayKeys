using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys {
    public class ThingTools {
        public static Random rand = new Random();
        public static Vector2 cameraPos = Vector2.Zero;
        
        public static float Lerp(float firstFloat, float secondFloat, float by) {
            // line comment first line to enable second line
            return firstFloat + (secondFloat - firstFloat) * by; /*
            return firstFloat * (1 - by) + secondFloat * by; /**/
        }

        public static Texture2D LoadPNG(string location) {
            return Texture2D.FromFile(Game1.Game.GraphicsDevice, "Content/" + location);
        }

        private static Vector2 CamPos(Vector2 inp) {
            return inp + cameraPos;
        }
        
        private static Vector2 CamScale(Vector2 dr) {
            return new Vector2(dr.X * Game1.Game.Scaling, dr.Y * Game1.Game.Scaling);
        }
        
        private static Vector2 CamPosScale(Vector2 inp) {
            return CamPos(CamScale(inp));
        }

        private static Point CamPosScaleInt(Vector2 inp) {
            Vector2 cps = CamPosScale(inp);
            return new Point((int) cps.X, (int) cps.Y);
        }
        
        private static Rectangle CamPosScaleRec(Rectangle inp) {
            return new Rectangle(
                (int) (inp.X * Game1.Game.Scaling + cameraPos.X), 
                (int) (inp.Y * Game1.Game.Scaling + cameraPos.Y), 
                (int) (inp.Width * Game1.Game.Scaling), 
                (int) (inp.Height * Game1.Game.Scaling));
        }

        public static void DrawToSpriteBatch(Texture2D texture, Rectangle dr, Rectangle sr) {
            DrawToSpriteBatch(texture, dr, sr, Color.White);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, Rectangle dr, Rectangle sr, Color color) {
            Game1.Game.SpriteBatch.Draw(texture, CamPosScaleRec(dr), sr, color);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, int x, int y, int sourceX, int sourceY, int sizeX, int sizeY) {
            DrawToSpriteBatch(texture, x, y, sourceX, sourceY, sizeX, sizeY, Color.White);
        }
        
        public static void DrawToSpriteBatch(Texture2D texture, int x, int y, int sourceX, int sourceY, int sizeX, int sizeY, Color color) {
            Game1.Game.SpriteBatch.Draw(texture, 
                new Rectangle(CamPosScaleInt(new Vector2(x, y)), CamPosScaleInt(new Vector2(sizeX, sizeY))), 
                new Rectangle(sourceX, sourceY, sizeX, sizeY), color);
        }

        public static void DrawString(string text, int px, int py, int scale) {
            DrawString(text, px, py, scale, Color.White);
        }
        
        public static void DrawString(string text, int px, int py, int scale, Color color) {
            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, CamPos(new Vector2(px, py)), color);
        }
        
        public static void DrawStringCentered(string text, int px, int py, int scale) {
            DrawStringCentered(text, px, py, scale, Color.White);
        }
        
        public static void DrawStringCentered(string text, int px, int py, int scale, Color color) {
            Vector2 pos = new Vector2(px, py) - Game1.Game.Fonts[scale].MeasureString(text) / 2;
            DrawString(text, (int) pos.X, (int) pos.Y, scale, color);
            //Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, pos, color);
        }
    }
}