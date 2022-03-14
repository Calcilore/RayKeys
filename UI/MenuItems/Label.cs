using Microsoft.Xna.Framework;
using RayKeys.Render;

namespace RayKeys.UI {
    public class Label : MenuItem {
        public Point Pos;
        
        public Align Alh;
        public Align Alv;
        public Align AlhT;
        public Align AlvT;

        public string Text;
        public int FontSize;
        public Color Color;

        public Label(Menu parent, bool followCamera, Align h, Align v, Align hT, Align vT, int id, string text, int x, int y, int fontSize, Color color) {
            Game1.Game.DrawEvent += Draw;
            
            this.parent = parent;
            this.followCamera = followCamera;
            this.Alh = h; this.Alv = v;
            this.AlhT = hT; this.AlvT = vT;
            this.Id = id;
            this.Text = text;
            Pos.X = x; Pos.Y = y;
            this.FontSize = fontSize;
            this.Color = color;
        }

        public override void Hide() {
            Game1.Game.DrawEvent -= Draw;
        }
        
        public override void Show() {
            Game1.Game.DrawEvent += Draw;
        }

        private void Draw(float delta) {
            RRender.DrawString(Alh, Alv, AlhT, AlvT, Text, Pos.X, Pos.Y, FontSize);
        }
    }
}