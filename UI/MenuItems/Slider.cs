using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.UI {
    public class Slider : FocusableMenuItem {
        private int fontSize;
        private int sizeX;
        private int sizeY;

        public delegate void EnterEventD(int id, float sliderPos, params object[] args);
        public event EnterEventD EnterEvent;

        public object[] args;
        public string Label;

        public float sliderPos;
        public int cursorPos;

        public Slider(Menu parent, bool followCamera, Align h, Align v, int id, string label, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 4) {
            Game1.UpdateEvent += Update;
            
            this.parent = parent;
            this.followCamera = followCamera;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.fontSize = fontSize;
            this.Label = label;
            this.Id = id;
            this.sliderPos = 0.5f;
            Alh = h;
            Alv = v;

            tPos.X = x - sizeX / 2 - (h == Align.Right ? sizeX : 0); // idk why it needs this
            tPos.Y = y;
            
            tPosFoc = tPos + new Vector2(h == Align.Right ? -64 : 64, 0);
            pos = tPos;
        }

        private void Update(float delta) {
            if (IsFocused) {
                if (RKeyboard.IsKeyHeld(Keys.Left))
                    sliderPos -= delta * 0.3f;

                if (RKeyboard.IsKeyHeld(Keys.Right))
                    sliderPos += delta * 0.3f;
                
                sliderPos = Math.Clamp(sliderPos, 0f, 1f);
            }
        }

        protected override void Draw(float delta) {
            base.Draw(delta);
            
            Point finalPos = pos.ToPoint();
            finalPos.X += sizeX / 2;
            finalPos.Y += sizeY / 2;
            if (followCamera) finalPos += RRender.CameraPos.ToPoint();
            
            RRender.DrawString(Alh, Alv, Align.Left, Alv, Label, finalPos.X, finalPos.Y, fontSize);
            //RRender.DrawString(Alh, Alv, Align.Left, Alv, Text, finalPos.X, finalPos.Y + 50, fontSize);
            RRender.DrawBlank(Alh, Alv, finalPos.X + (int)(sliderPos * sizeX) - 3, finalPos.Y + 20, 6, 26, Color.White);
            
            RRender.DrawBlank(Alh, Alv, finalPos.X - 10, finalPos.Y + 20, 10, 26, Color.White);
            RRender.DrawBlank(Alh, Alv, finalPos.X + sizeX, finalPos.Y + 20, 10, 26, Color.White);
            RRender.DrawBlank(Alh, Alv, finalPos.X, finalPos.Y + 30, sizeX, 6, Color.White);
        }

        public override void UnFocus() {
            EnterEvent?.Invoke(Id, sliderPos, args);
            base.UnFocus();
        }
    }
}