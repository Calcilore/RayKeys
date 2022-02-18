using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys.Render {
    public class RRender {
        public static Vector2 CameraPos;
        public static Point Resolution;

        public static Texture2D BlankTexture;

        public static void Initialise() {
            Resolution = new Point(1920, 1080);
            BlankTexture = new Texture2D(Game1.Game.SpriteBatch.GraphicsDevice, 1, 1);
            BlankTexture.SetData(new [] {Color.White});
        }
        
        public static Texture2D LoadPNG(string location) {
            return Texture2D.FromFile(Game1.Game.GraphicsDevice, "Content/" + location);
        }

        public static Point AlPosP(Align h, Align v, int x, int y) {
            return new Point(x + (int) h * Resolution.X / 2, y + (int)v * Resolution.Y / 2);
        }
        
        public static Point AlPosP(Align h, Align v, int x, int y, int rx, int ry) {
            return new Point(x + (int) h * rx / 2, y + (int)v * ry / 2);
        }
        
        public static Vector2 AlPosV(Align h, Align v, float x, float y) {
            return new Vector2(x - (float)(int) h * Resolution.X / 2, y - (float)(int)v * Resolution.Y / 2);
        }
        
        public static Vector2 AlPosV(Align h, Align v, float x, float y, float rx, float ry) {
            return new Vector2(x - (float)(int) h * rx / 2, y - (float)(int)v * ry / 2);
        }
        
        public static void Draw(Align h, Align v, Texture2D texture, Rectangle dr, Rectangle sr, Color color) {
            Game1.Game.SpriteBatch.Draw(
                texture, 
                new Rectangle(AlPosP(h, v, dr.X, dr.Y) - CameraPos.ToPoint(), new Point(dr.Width, dr.Height)),
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
        
        public static void DrawBlank(Align h, Align v, int x, int y, int sizeX, int sizeY, Color color) {
            Draw(h, v, BlankTexture, new Rectangle(x, y, sizeX, sizeY), new Rectangle(0, 0, 1, 1), color);
        }

        public static void DrawString(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale, Color color) {
            // Align pos
            Vector2 pos = new Vector2(px + (int) h * Resolution.X / 2, py + (int) v * Resolution.Y / 2);
            // Center Text
            Vector2 ts = Game1.Game.Fonts[scale].MeasureString(text);
            pos = AlPosV(th, tv, pos.X, pos.Y, ts.X, ts.Y);
            pos -= CameraPos;

            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, pos, color);
        }
        
        public static void DrawStringNoCam(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale, Color color) {
            // Align pos
            Vector2 pos = new Vector2(px + (int) h * Resolution.X / 2, py + (int) v * Resolution.Y / 2);
            // Center Text
            Vector2 ts = Game1.Game.Fonts[scale].MeasureString(text);
            pos = AlPosV(th, tv, pos.X, pos.Y, ts.X, ts.Y);

            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, pos, color);
        }
        
        public static void DrawString(Align h, Align v, string text, int px, int py, int scale, Color color) {
            DrawString(h, v, h, v, text, px, py, scale, color);
        }
        
        public static void DrawTile(Align h, Align v, Texture2D texture, int x, int y, int sourceX, int sourceY, int sizeX, int sizeY, int tileX, int tileY, Color color) {
            for (int i = 0; i < tileX; i++) {
                int ytp = y;
                for (int j = 0; j < tileY; j++) {
                    Draw(h, v, texture, new Rectangle(x, ytp, sizeX, sizeY), new Rectangle(sourceX, sourceY, sizeX, sizeY), color);
                    ytp += sizeY;
                }
                x += sizeX;
            }
        }
        
        public static void DrawTileUp(Align h, Align v, Texture2D texture, int x, int y, int sourceX, int sourceY, int sizeX, int sizeY, int tileX, int tileY, Color color) {
            for (int i = 0; i < tileX; i++) {
                int ytp = y;
                for (int j = 0; j < tileY; j++) {
                    Draw(h, v, texture, new Rectangle(x, ytp, sizeX, sizeY), new Rectangle(sourceX, sourceY, sizeX, sizeY), color);
                    ytp -= sizeY;
                }
                x += sizeX;
            }
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