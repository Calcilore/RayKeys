using Microsoft.Xna.Framework;
using RayKeys.Render;

namespace RayKeys.UI {
    public class OtherFocusLabel : FocusableMenuItem {
        public Align Alh;
        public Align Alv;
        public Align AlhT;
        public Align AlvT;

        public string Text;
        public int FontSize;
        public Color Color;
        public float DrawDepth;

        public FocusableMenuItem Other { get; }

        public OtherFocusLabel(Menu parent, bool followCamera, Align h, Align v, Align hT, Align vT, int id, string text, FocusableMenuItem other, int x, int y, int fontSize, Color color, float drawDepth = RRender.DefaultDepth) {
            this.parent = parent;
            this.followCamera = followCamera;
            this.Alh = h; this.Alv = v;
            this.AlhT = hT; this.AlvT = vT;
            this.Id = id;
            this.Text = text;
            this.FontSize = fontSize;
            this.Color = color;
            this.DrawDepth = drawDepth;
            this.Other = other;
            other.hideFocus = true;

            this.tPos.X = x;
            this.tPos.Y = y;
            
            tPosFoc = tPos + new Vector2(hT == Align.Right ? -64 : 64, 0);
            pos = tPos;
            
            Color = Color.White;
        }

        public override void Focus() {
            Other.Focus();
            base.Focus();
        }

        public override void UnFocus() {
           Other.UnFocus();
           base.UnFocus();
        }

        protected override void Draw(float delta) {
            Vector2 finalPos = pos;
            if (followCamera) finalPos += RRender.CameraPos;
            
            RRender.DrawString(Alh, Alv, AlhT, AlvT, Text, (int)finalPos.X, (int)finalPos.Y, FontSize, Color);
            
            base.Draw(delta);
        }
        
        protected override Vector2 CalculateOffset() {
            int hah = AlvT switch {
                Align.Center => 0,
                Align.Bottom => Stuffs.GetTexture(Textures.Arrow).Height,
                /* Top */ _  => Stuffs.GetTexture(Textures.Arrow).Height / 2
            };
            
            int tLen = (int) RRender.MeasureString(FontSize, Text).X;

            return Alh switch {
                Align.Left  => new Vector2(tLen, hah),
                Align.Right => new Vector2(-tLen, hah),
                /*Center*/_ => new Vector2(-tLen / 2f, hah),
            };
        }
    }
}