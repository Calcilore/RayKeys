using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys.Render {
    public class RRender {
        public const float DefaultDepth = 0.5f;
        private const float DepthAddAmnt = -0.001f;
        
        public static Vector2 CameraPos;
        public static Point Resolution;

        public static Texture2D BlankTexture;
        private static float depthAdd = 0f; // To make draw order second priority

        public static void Initialise() {
            Resolution = new Point(1920, 1080);
            BlankTexture = new Texture2D(Game1.Game.SpriteBatch.GraphicsDevice, 1, 1);
            BlankTexture.SetData(new [] {Color.White});
        }

        public static void Draw() {
            depthAdd = 0f;
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

        
        // -=-=-=-=-=-=-=-=-
        // |    Big Draw   |
        // -=-=-=-=-=-=-=-=-
        
        public static void DrawNoCam(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int sourceX, int sourceY, int sourceXSize, int sourceYSize, Color? color = null, float depth = DefaultDepth) {
            color ??= Color.White;

            Game1.Game.SpriteBatch.Draw(
                texture,
                new Rectangle(AlPosP(h, v, x, y), new Point(sizeX, sizeY)),
                new Rectangle(sourceX, sourceY, sourceXSize, sourceYSize),
                color.Value, 0, Vector2.Zero, SpriteEffects.None, depth + depthAdd);

            depthAdd += DepthAddAmnt;
        }

        public static void Draw(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int sourceX, int sourceY, int sourceXSize, int sourceYSize, Color? color = null, float depth = DefaultDepth) {
            DrawNoCam(h, v, texture, x - (int)CameraPos.X, y - (int)CameraPos.Y, sizeX, sizeY, sourceX, sourceY, sourceXSize, sourceYSize, color, depth);
        }
        
        public static void DrawNoCam(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int sourceX, int sourceY, int sourceXSize, int sourceYSize, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawNoCam(h, v, tex, x, y, sizeX, sizeY, sourceX, sourceY, sourceXSize, sourceYSize, color, depth);
        }
        
        public static void Draw(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int sourceX, int sourceY, int sourceXSize, int sourceYSize, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            Draw(h, v, tex, x, y, sizeX, sizeY, sourceX, sourceY, sourceXSize, sourceYSize, color, depth);
        }

        // -=-=-=-=-=-=-=-=-
        // |  Smaller Draw |
        // -=-=-=-=-=-=-=-=-
        
        public static void DrawNoCam(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, Color? color = null, float depth = DefaultDepth) {
            color ??= Color.White;
            
            Game1.Game.SpriteBatch.Draw(
                texture, 
                new Rectangle(AlPosP(h, v, x, y), new Point(sizeX, sizeY)),
                new Rectangle(0, 0, sizeX, sizeY),
                color.Value, 0, Vector2.Zero, SpriteEffects.None, depth + depthAdd);

            depthAdd += DepthAddAmnt;
        }
        
        public static void Draw(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, Color? color = null, float depth = DefaultDepth) {
            DrawNoCam(h, v, texture, x - (int)CameraPos.X, y - (int)CameraPos.Y, sizeX, sizeY, color, depth);
        }
        
        public static void DrawNoCam(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawNoCam(h, v, tex, x, y, sizeX, sizeY, color, depth);
        }
        
        public static void Draw(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            Draw(h, v, tex, x, y, sizeX, sizeY, color, depth);
        }
        
        // -=-=-=-=-=-=-=-=-=-
        // |  Smallerer Draw |
        // -=-=-=-=-=-=-=-=-=-

        public static void DrawNoCam(Align h, Align v, Texture2D texture, int x, int y, Color? color = null, float depth = DefaultDepth) {
            DrawNoCam(h, v, texture, x, y, texture.Width, texture.Height, color, depth);
        }
        
        public static void Draw(Align h, Align v, Texture2D texture, int x, int y, Color? color = null, float depth = DefaultDepth) {
            Draw(h, v, texture, x, y, texture.Width, texture.Height, color, depth);
        }
        
        public static void DrawNoCam(Align h, Align v, Textures texture, int x, int y, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawNoCam(h, v, tex, x, y, tex.Width, tex.Height, color, depth);
        }
        
        public static void Draw(Align h, Align v, Textures texture, int x, int y, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            Draw(h, v, tex, x, y, tex.Width, tex.Height, color, depth);
        }
        
        // -=-=-=-=-=-=-=-
        // |  Blank Draw |
        // -=-=-=-=-=-=-=-

        public static void DrawBlankNoCam(Align h, Align v, int x, int y, int sizeX, int sizeY, Color? color = null, float depth = DefaultDepth) {
            DrawNoCam(h, v, BlankTexture, x, y, sizeX, sizeY, 0, 0, 1, 1, color, depth);
        }
        
        public static void DrawBlank(Align h, Align v, int x, int y, int sizeX, int sizeY, Color? color = null, float depth = DefaultDepth) {
            Draw(h, v, BlankTexture, x, y, sizeX, sizeY, 0, 0, 1, 1, color, depth);
        }

        // -=-=-=-=-=-=-
        // |  Strings  |
        // -=-=-=-=-=-=-
        
        public static void DrawStringNoCam(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale, Color? color = null, float depth = DefaultDepth) {
            color ??= Color.White;
            
            // Align pos
            Vector2 pos = new Vector2(px + (int) h * Resolution.X / 2, py + (int) v * Resolution.Y / 2);
            // Center Text
            Vector2 ts = Game1.Game.Fonts[scale].MeasureString(text);
            pos = AlPosV(th, tv, pos.X, pos.Y, ts.X, ts.Y);

            Game1.Game.SpriteBatch.DrawString(Game1.Game.Fonts[scale], text, pos, color.Value, 
                0, Vector2.Zero, Vector2.One, SpriteEffects.None, depth + depthAdd);

            depthAdd += DepthAddAmnt;
        }
        
        public static void DrawString(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale, Color? color = null, float depth = DefaultDepth) {
            DrawStringNoCam(h, v, th, tv, text, px - (int)CameraPos.X, py - (int)CameraPos.Y, scale, color, depth);
        }
        
        // -=-=-=-=-=-=-=-=-=-=-
        // |  Smaller Strings  |
        // -=-=-=-=-=-=-=-=-=-=-

        public static void DrawStringNoCam(Align h, Align v, string text, int px, int py, int scale, Color? color = null, float depth = DefaultDepth) {
            DrawStringNoCam(h, v, h, v, text, px, py, scale, color, depth);
        }
        
        public static void DrawString(Align h, Align v, string text, int px, int py, int scale, Color? color = null, float depth = DefaultDepth) {
            DrawString(h, v, h, v, text, px, py, scale, color, depth);
        }
        
        // -=-=-=-=-=-=-=-=-
        // |  Tiling Draw  |
        // -=-=-=-=-=-=-=-=-

        private static void DrawTileDownCommon(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color, float depth,
            Action<Align, Align, Texture2D, int, int, int, int, Color?, float> action) {

            for (int i = 0; i < tileX; i++) {
                int ytp = y;
                for (int j = 0; j < tileY; j++) {
                    action.Invoke(h, v, texture, x, ytp, sizeX, sizeY, color, depth);
                    ytp += sizeY;
                }
                x += sizeX;
            }
        }

        public static void DrawTileDownNoCam(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, float depth = DefaultDepth) {
            DrawTileDownCommon(h, v, texture, x, y, sizeX, sizeY, tileX, tileY, color, depth, DrawNoCam);
        }
        
        public static void DrawTileDown(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, float depth = DefaultDepth) {
            DrawTileDownCommon(h, v, texture, x, y, sizeX, sizeY, tileX, tileY, color, depth, Draw);
        }   
        
        public static void DrawTileDownNoCam(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawTileDownCommon(h, v, tex, x, y, sizeX, sizeY, tileX, tileY, color, depth, DrawNoCam);
        }
        
        public static void DrawTileDown(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawTileDownCommon(h, v, tex, x, y, sizeX, sizeY, tileX, tileY, color, depth, Draw);
        }
        
        private static void DrawTileUpCommon(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color, float depth,
            Action<Align, Align, Texture2D, int, int, int, int, Color?, float> action) {

            for (int i = 0; i < tileX; i++) {
                int ytp = y;
                for (int j = 0; j < tileY; j++) {
                    action.Invoke(h, v, texture, x, ytp, sizeX, sizeY, color, depth);
                    ytp -= sizeY;
                }
                x += sizeX;
            }
        }
        
        public static void DrawTileUpNoCam(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, float depth = DefaultDepth) {
            DrawTileUpCommon(h, v, texture, x, y, sizeX, sizeY, tileX, tileY, color, depth, DrawNoCam);
        }
        
        public static void DrawTileUp(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, float depth = DefaultDepth) {
            DrawTileUpCommon(h, v, texture, x, y, sizeX, sizeY, tileX, tileY, color, depth, Draw);
        }   
        
        public static void DrawTileUpNoCam(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawTileUpCommon(h, v, tex, x, y, sizeX, sizeY, tileX, tileY, color, depth, DrawNoCam);
        }
        
        public static void DrawTileUp(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, float depth = DefaultDepth) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawTileUpCommon(h, v, tex, x, y, sizeX, sizeY, tileX, tileY, color, depth, Draw);
        }
    }
}