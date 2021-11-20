using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.Render;

namespace RayKeys.Menu {
    public class Button {
        public object Arg;

        private Texture2D tex;
        private Color cColour;
        private bool pressedLastFrame;
        private int fontSize;
        private int sizeX;
        private int sizeY;
        private bool drawFrame;

        public delegate void ClickEventD(string id, object arg);
        public event ClickEventD ClickEvent;
        
        public int X; public Align Alh;
        public int Y; public Align Alv;
        public string Text;
        public string Id;

        public Button(Align h, Align v, string id, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 2, bool drawFrame = true) {
            tex = Game1.Game.Textures["button"];
            
            Game1.Game.DrawEvent += Draw;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.fontSize = fontSize;
            this.Text = text;
            this.drawFrame = drawFrame;
            this.Id = id;
            Alh = h;
            Alv = v;
            
            Point alPos = RRender.AlPosP(h, v, x, y);
            alPos = RRender.AlPosP(h, v, alPos.X, alPos.Y, -sizeX, -sizeY);
            this.X = alPos.X;
            this.Y = alPos.Y;

            cColour = Color.White;
        }

        public void Hide() {
            Game1.Game.DrawEvent -= Draw;
        }
        
        public void Show() {
            Game1.Game.DrawEvent += Draw;
        }

        private void Draw(float delta) {
            MouseState ms = Mouse.GetState();

            // funny window things
            Point pos = ((ms.Position - Game1.Game.RenderRectangle.Location).ToVector2() / Game1.Game.Scaling).ToPoint();
            pos += RRender.cameraPos.ToPoint();

            bool pressed = ms.LeftButton == ButtonState.Pressed;
            
            if ( pos.X >= X && pos.X <= X + sizeX &&
                 pos.Y >= Y && pos.Y <= Y + sizeY ) {
                cColour = pressed ? Color.Gray : Color.LightGray;

                if (!pressed && pressedLastFrame) { 
                    ClickEvent?.Invoke(Id, Arg);
                }
            }
            else {
                cColour = Color.White;
            }
            
            if (drawFrame)
                RRender.Draw(Align.None, Align.None, tex, new Rectangle(X, Y, sizeX, sizeY), new Rectangle(0, 0, 600, 200), cColour);
            
            RRender.DrawString(Align.None, Align.None, Align.Center, Align.Center, Text, X + sizeX / 2, Y + sizeY / 2, fontSize, cColour);

            pressedLastFrame = pressed;
        }
    }
}
