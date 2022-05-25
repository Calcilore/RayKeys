using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.UI {
    public class Switcher : FocusableMenuItem {
        public Color Color;
        private int fontSize;
        private int sizeX;
        private int sizeY;

        public delegate void ClickEventD(int id, params object[] args);
        public event ClickEventD ClickEvent;

        public object[] args;
        public string Label;

        public Switcher(Menu parent, bool followCamera, Align h, Align v, int id, string label, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Game1.UpdateEvent += Update;

            this.parent = parent;
            this.followCamera = followCamera;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.fontSize = fontSize;
            this.Label = label;
            this.Id = id;
            Alh = h;
            Alv = v;

            tPos.X = x - sizeX / 2; // idk why it needs this
            tPos.Y = y;
            
            tPosFoc = tPos + new Vector2(h == Align.Right ? -64 : 64, 0);
            pos = tPos;

            Color = Color.White;
        }

        private void Update(float delta) {
            if (RKeyboard.IsKeyPressed(Keys.Enter) && IsFocused) {
                ClickEvent?.Invoke(Id, args);
            }
        }
        
        protected override void Draw(float delta) {
            base.Draw(delta);

            Vector2 finalPos = pos;
            if (followCamera) finalPos += RRender.CameraPos;
            
            RRender.DrawString(Alh, Alv, Align.Center, Align.Center, Label, (int)finalPos.X + sizeX / 2, (int)finalPos.Y + sizeY / 2, fontSize, Color);
        }
    }
}