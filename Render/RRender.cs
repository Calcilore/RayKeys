using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys.Render {
    public static class RRender {
        public const float DefaultDepth = 0.5f;
        private const float DepthAddAmnt = -0.001f;
        
        public static Vector2 CameraPos;
        public static Point Resolution;

        public static Texture2D BlankTexture;
        public static SpriteFont[] Fonts = new SpriteFont[7];
        private static float depthAdd = 0f; // To make draw order second priority

        public static void Initialise() {
            Resolution = new Point(1920, 1080);
            BlankTexture = new Texture2D(Game1.SpriteBatch.GraphicsDevice, 1, 1);
            BlankTexture.SetData(new [] {Color.White});
        }

        public static void Draw() {
            depthAdd = 0f;
        }

        public static Point AlPosP(Align h, Align v, int x, int y)
            => new Point(x + (int) h * Resolution.X / 2, y + (int)v * Resolution.Y / 2);

        public static Point AlPosP(Align h, Align v, int x, int y, int rx, int ry)
            => new Point(x + (int) h * rx / 2, y + (int)v * ry / 2);

        public static Vector2 AlPosV(Align h, Align v, float x, float y)
            => new Vector2(x - (float)(int) h * Resolution.X / 2, y - (float)(int)v * Resolution.Y / 2);

        public static Vector2 AlPosV(Align h, Align v, float x, float y, float rx, float ry)
            => new Vector2(x - (float)(int) h * rx / 2, y - (float)(int)v * ry / 2);

        public static Vector2 MeasureString(int fontSize, string text) {
            return Fonts[fontSize].MeasureString(text);
        }
        
        // -=-=-=-=-=-=-=-=-
        // |    Big Draw   |
        // -=-=-=-=-=-=-=-=-
        
        public static void DrawNoCam(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int sourceX, int sourceY, int sourceXSize, int sourceYSize, 
            Color? color = null, float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Game1.SpriteBatch.Draw(
                texture,
                new Rectangle(AlPosP(h, v, x, y), new Point(sizeX, sizeY)),
                new Rectangle(sourceX, sourceY, sourceXSize, sourceYSize),
                color.GetValueOrDefault(Color.White), rotation, origin.GetValueOrDefault(Vector2.Zero), spriteEffects, depth + depthAdd);

            depthAdd += DepthAddAmnt;
        }

        public static void Draw(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int sourceX, int sourceY, int sourceXSize, int sourceYSize, 
            Color? color = null, float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
           => DrawNoCam(h, v, texture, x - (int)CameraPos.X, y - (int)CameraPos.Y, sizeX, sizeY, sourceX, sourceY, sourceXSize, sourceYSize, color, depth, rotation, origin, spriteEffects);
        
        public static void DrawNoCam(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int sourceX, int sourceY, int sourceXSize, int sourceYSize, 
            Color? color = null, float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawNoCam(h, v, tex, x, y, sizeX, sizeY, sourceX, sourceY, sourceXSize, sourceYSize, color, depth, rotation, origin, spriteEffects);
        }
        
        public static void Draw(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int sourceX, int sourceY, int sourceXSize, int sourceYSize, 
            Color? color = null, float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            Draw(h, v, tex, x, y, sizeX, sizeY, sourceX, sourceY, sourceXSize, sourceYSize, color, depth, rotation, origin, spriteEffects);
        }

        // -=-=-=-=-=-=-=-=-
        // |  Smaller Draw |
        // -=-=-=-=-=-=-=-=-
        
        public static void DrawNoCam(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, 
            Color? color = null, float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Game1.SpriteBatch.Draw(
                texture, 
                new Rectangle(AlPosP(h, v, x, y), new Point(sizeX, sizeY)),
                new Rectangle(0, 0, sizeX, sizeY),
                color.GetValueOrDefault(Color.White), rotation, origin.GetValueOrDefault(Vector2.Zero), spriteEffects, depth + depthAdd);

            depthAdd += DepthAddAmnt;
        }
        
        public static void Draw(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) 
            => DrawNoCam(h, v, texture, x - (int)CameraPos.X, y - (int)CameraPos.Y, sizeX, sizeY, color, depth, rotation, origin, spriteEffects);

        public static void DrawNoCam(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawNoCam(h, v, tex, x, y, sizeX, sizeY, color, depth, rotation, origin, spriteEffects);
        }
        
        public static void Draw(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            Draw(h, v, tex, x, y, sizeX, sizeY, color, depth, rotation, origin, spriteEffects);
        }
        
        // -=-=-=-=-=-=-=-=-=-
        // |  Smallerer Draw |
        // -=-=-=-=-=-=-=-=-=-

        public static void DrawNoCam(Align h, Align v, Texture2D texture, int x, int y, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawNoCam(h, v, texture, x, y, texture.Width, texture.Height, color, depth, rotation, origin, spriteEffects);

        public static void Draw(Align h, Align v, Texture2D texture, int x, int y, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => Draw(h, v, texture, x, y, texture.Width, texture.Height, color, depth, rotation, origin, spriteEffects);

        public static void DrawNoCam(Align h, Align v, Textures texture, int x, int y, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawNoCam(h, v, tex, x, y, tex.Width, tex.Height, color, depth, rotation, origin, spriteEffects);
        }
        
        public static void Draw(Align h, Align v, Textures texture, int x, int y, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            Draw(h, v, tex, x, y, tex.Width, tex.Height, color, depth, rotation, origin, spriteEffects);
        }
        
        // -=-=-=-=-=-=-=-
        // |  Blank Draw |
        // -=-=-=-=-=-=-=-

        public static void DrawBlankNoCam(Align h, Align v, int x, int y, int sizeX, int sizeY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawNoCam(h, v, BlankTexture, x, y, sizeX, sizeY, 0, 0, 1, 1, color, depth, rotation, origin, spriteEffects);

        public static void DrawBlank(Align h, Align v, int x, int y, int sizeX, int sizeY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => Draw(h, v, BlankTexture, x, y, sizeX, sizeY, 0, 0, 1, 1, color, depth, rotation, origin, spriteEffects);

        // -=-=-=-=-=-=-
        // |  Strings  |
        // -=-=-=-=-=-=-
        
        public static void DrawStringNoCam(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            // Align pos
            Vector2 pos = new Vector2(px + (int) h * Resolution.X / 2, py + (int) v * Resolution.Y / 2);
            // Center Text
            Vector2 ts = Fonts[scale].MeasureString(text);
            pos = AlPosV(th, tv, pos.X, pos.Y, ts.X, ts.Y);

            Game1.SpriteBatch.DrawString(Fonts[scale], text, pos, color.GetValueOrDefault(Color.White), 
                rotation, origin.GetValueOrDefault(Vector2.Zero), Vector2.One, spriteEffects, depth + depthAdd);

            depthAdd += DepthAddAmnt;
        }
        
        public static void DrawString(Align h, Align v, Align th, Align tv, string text, int px, int py, int scale, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawStringNoCam(h, v, th, tv, text, px - (int)CameraPos.X, py - (int)CameraPos.Y, scale, color, depth, rotation, origin, spriteEffects);

        // -=-=-=-=-=-=-=-=-=-=-
        // |  Smaller Strings  |
        // -=-=-=-=-=-=-=-=-=-=-

        public static void DrawStringNoCam(Align h, Align v, string text, int px, int py, int scale, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawStringNoCam(h, v, h, v, text, px, py, scale, color, depth, rotation, origin, spriteEffects);

        public static void DrawString(Align h, Align v, string text, int px, int py, int scale, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawString(h, v, h, v, text, px, py, scale, color, depth, rotation, origin, spriteEffects);

        // -=-=-=-=-=-=-=-=-
        // |  Tiling Draw  |
        // -=-=-=-=-=-=-=-=-

        private static void DrawTileDownCommon(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color, float depth
            , float rotation, Vector2? origin, SpriteEffects spriteEffects,
            Action<Align, Align, Texture2D, int, int, int, int, Color?, float, float, Vector2?, SpriteEffects> action) {

            for (int i = 0; i < tileX; i++) {
                int ytp = y;
                for (int j = 0; j < tileY; j++) {
                    action.Invoke(h, v, texture, x, ytp, sizeX, sizeY, color, depth, rotation, origin, spriteEffects);
                    ytp += sizeY;
                }
                x += sizeX;
            }
        }

        public static void DrawTileDownNoCam(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawTileDownCommon(h, v, texture, x, y, sizeX, sizeY, tileX, tileY, color, depth, rotation, origin, spriteEffects, DrawNoCam);

        public static void DrawTileDown(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawTileDownCommon(h, v, texture, x, y, sizeX, sizeY, tileX, tileY, color, depth, rotation, origin, spriteEffects, Draw);
        
        
        public static void DrawTileDownNoCam(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawTileDownCommon(h, v, tex, x, y, sizeX, sizeY, tileX, tileY, color, depth, rotation, origin, spriteEffects, DrawNoCam);
        }
        
        public static void DrawTileDown(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawTileDownCommon(h, v, tex, x, y, sizeX, sizeY, tileX, tileY, color, depth, rotation, origin, spriteEffects, Draw);
        }
        
        private static void DrawTileUpCommon(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color, float depth
            , float rotation, Vector2? origin, SpriteEffects spriteEffects,
            Action<Align, Align, Texture2D, int, int, int, int, Color?, float, float, Vector2?, SpriteEffects> action) {

            for (int i = 0; i < tileX; i++) {
                int ytp = y;
                for (int j = 0; j < tileY; j++) {
                    action.Invoke(h, v, texture, x, ytp, sizeX, sizeY, color, depth, rotation, origin, spriteEffects);
                    ytp -= sizeY;
                }
                x += sizeX;
            }
        }
        
        public static void DrawTileUpNoCam(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, 
            Color? color = null, float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawTileUpCommon(h, v, texture, x, y, sizeX, sizeY, tileX, tileY, color, depth, rotation, origin, spriteEffects, DrawNoCam);

        public static void DrawTileUp(Align h, Align v, Texture2D texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None)
            => DrawTileUpCommon(h, v, texture, x, y, sizeX, sizeY, tileX, tileY, color, depth, rotation, origin, spriteEffects, Draw);

        public static void DrawTileUpNoCam(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawTileUpCommon(h, v, tex, x, y, sizeX, sizeY, tileX, tileY, color, depth, rotation, origin, spriteEffects, DrawNoCam);
        }
        
        public static void DrawTileUp(Align h, Align v, Textures texture, int x, int y, int sizeX, int sizeY, int tileX, int tileY, Color? color = null, 
            float depth = DefaultDepth, float rotation = 0f, Vector2? origin = null, SpriteEffects spriteEffects = SpriteEffects.None) {
            Texture2D tex = Stuffs.GetTexture(texture);
            DrawTileUpCommon(h, v, tex, x, y, sizeX, sizeY, tileX, tileY, color, depth, rotation, origin, spriteEffects, Draw);
        }
    }
}