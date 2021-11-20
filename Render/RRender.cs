using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys.Render {
    public class RRender {
        public static Vector2 cameraPos;
        public static Point resolution;
        
        public static Texture2D LoadPNG(string location) {
            return Texture2D.FromFile(Game1.Game.GraphicsDevice, "Content/" + location);
        }

        public static Point AlPosP(Align h, Align v, int x, int y) {
            return new Point(x + (int) h * resolution.X / 2, y + (int)v * resolution.Y / 2);
        }
        
        public static Point AlPosP(Align h, Align v, int x, int y, int rx, int ry) {
            return new Point(x + (int) h * rx / 2, y + (int)v * ry / 2);
        }
        
        public static Vector2 AlPosV(Align h, Align v, float x, float y) {
            return new Vector2(x - (float)(int) h * resolution.X / 2, y - (float)(int)v * resolution.Y / 2);
        }
        
        public static Vector2 AlPosV(Align h, Align v, float x, float y, float rx, float ry) {
            return new Vector2(x - (float)(int) h * rx / 2, y - (float)(int)v * ry / 2);
        }
        
        public static void Draw(Align h, Align v, Texture2D texture, Rectangle dr, Rectangle sr, Color color) {
            Game1.Game.SpriteBatch.Draw(
                texture, 
                new Rectangle(AlPosP(h, v, dr.X, dr.Y) - cameraPos.ToPoint(), new Point(dr.Width, dr.Height)),
                   sr,
                color);
        }
        
        public static void DrawNoCam(Align h, Align v, Texture2D texture, Rectangle dr, Rectangle sr, Color color) {
            Game1.Game.SpriteBatch.Draw(
                texture, 
                new Rectangle(AlPosP(h, v, dr.X, dr.Y), new Point(dr.Width, dr.Height)),
                sr,
                color);
        }

        public static void Draw(Align h, Align v, Texture2D texture, int x, int y, int sourceX, int sourceY, int sizeX, int sizeY, Color color) {
            Draw(h, v, texture, new Rectangle(x, y, sizeX, sizeY), new Rectangle(sourceX, sourceY, sizeX, sizeY), color);
        }

        public static void DrawString(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale, Color color) {
            // Align pos
            Vector2 pos = new Vector2(px + (int) h * resolution.X / 2, py + (int) v * resolution.Y / 2);
            // Center Text
            Vector2 ts = Game1.Game.Fonts[scale].MeasureString(text);
            pos = AlPosV(th, tv, pos.X, pos.Y, ts.X, ts.Y);
            pos -= cameraPos;

            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, pos, color);
        }
        
        public static void DrawStringNoCam(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale, Color color) {
            // Align pos
            Vector2 pos = new Vector2(px + (int) h * resolution.X / 2, py + (int) v * resolution.Y / 2);
            // Center Text
            Vector2 ts = Game1.Game.Fonts[scale].MeasureString(text);
            pos = AlPosV(th, tv, pos.X, pos.Y, ts.X, ts.Y);

            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, pos, color);
        }
        
        public static void DrawString(Align h, Align v, string text, int px, int py, int scale, Color color) {
            DrawString(h, v, h, v, text, px, py, scale, color);
        }
        
        // ---------------
        // |  No Colour  |
        // |   Aliases   |
        // ---------------
        
        public static void Draw(Align h, Align v, Texture2D texture, Rectangle dr, Rectangle sr) 
        { Draw(h, v, texture, dr, sr, Color.White); }
        
        public static void Draw(Align h, Align v, Texture2D texture, int x, int y, int sourceX, int sourceY, int sizeX, int sizeY) 
        { Draw(h, v, texture, x, y, sourceX, sourceY, sizeX, sizeY, Color.White); }
        
        public static void DrawString(Align h, Align v, string text, int px, int py, int scale) 
        { DrawString(h, v, h, v, text, px, py, scale, Color.White); }
        
        public static void DrawString(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale) 
        { DrawString(h, v, th, tv, text, px, py, scale, Color.White); }
    }
}