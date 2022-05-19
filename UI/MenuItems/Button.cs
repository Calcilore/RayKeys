using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.UI {
    public class Button : FocusableMenuItem {
        public Color Color;
        private int fontSize;
        private int sizeX;
        private int sizeY;

        public delegate void ClickEventD(int id, params object[] args);
        public event ClickEventD ClickEvent;

        public object[] args;
        public Align AlhT;
        public Align AlvT;
        
        public string Label;

        public Button(Menu parent, bool followCamera, Align h, Align v, Align hT, Align vT, int id, string label, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Game1.Game.UpdateEvent += Update;

            this.parent = parent;
            this.followCamera = followCamera;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.fontSize = fontSize;
            this.Label = label;
            this.Id = id;
            Alh = h;
            Alv = v;
            AlhT = hT;
            AlvT = vT;

            tPos.X = x;
            tPos.Y = y;
            
            tPosFoc = tPos + new Vector2(hT == Align.Right ? -64 : 64, 0);
            pos = tPos;

            Color = Color.White;
        }

        private void Update(float delta) {
            if (RKeyboard.IsKeyPressed(Keys.Enter) && IsFocused) {
                ClickEvent?.Invoke(Id, args);
            }
        }
        
        protected override void Draw(float delta) {
            Vector2 finalPos = pos;
            if (followCamera) finalPos += RRender.CameraPos;
            
            RRender.DrawString(Alh, Alv, AlhT, AlvT, Label, (int)finalPos.X, (int)finalPos.Y, fontSize, Color);
            
            base.Draw(delta);
        }

        protected override Vector2 CalculateOffset() {
            int hah = AlvT switch {
                Align.Center => 0,
                Align.Bottom => Stuffs.GetTexture(Textures.Arrow).Height,
                /* Top */ _  => Stuffs.GetTexture(Textures.Arrow).Height / 2
            };
            
            int tLen = (int) RRender.MeasureString(fontSize, Label).X;

            return Alh switch {
                Align.Left  => new Vector2(tLen, hah),
                Align.Right => new Vector2(-tLen, hah),
                /*Center*/_ => new Vector2(-tLen / 2f, hah),
            };
        }
    }
}