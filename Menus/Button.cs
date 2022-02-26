using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.Menus {
    public class Button : MenuItem {
        private Color cColour;
        private int fontSize;
        private int sizeX;
        private int sizeY;

        public delegate void ClickEventD(int id, params object[] args);
        public event ClickEventD ClickEvent;

        public object[] args;
        public Align Alh;
        public Align Alv;
        public Align AlhT;
        public Align AlvT;
        public string Text;

        private Vector2 tPos;
        private Vector2 tPosFoc;
        private Vector2 pos;

        private bool isSubbed;
        private bool isFocused;

        public Button(Menu parent, Align h, Align v, Align hT, Align vT, int id, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Game1.Game.UpdateEvent += Update;
            Game1.Game.DrawEvent += Draw;
            isSubbed = true;

            this.parent = parent;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.fontSize = fontSize;
            this.Text = text;
            this.Id = id;
            Alh = h;
            Alv = v;
            AlhT = hT;
            AlvT = vT;

            tPos.X = x - sizeX / 2; // idk why it needs this
            tPos.Y = y;
            
            tPosFoc = tPos + new Vector2(hT == Align.Right ? -64 : 64, 0);
            pos = tPos;

            cColour = Color.White;
        }

        public override void Hide() {
            if (isSubbed) Game1.Game.DrawEvent -= Draw;
            isSubbed = false;
        }
        
        public override void Show() {
            if (!isSubbed) Game1.Game.DrawEvent += Draw;
            isSubbed = true;
        }

        private void Update(float delta) {
            isFocused = parent.CurrentId == Id;
            
            if (RKeyboard.IsKeyPressed(Keys.Enter) && isFocused) {
                ClickEvent?.Invoke(Id, args);
            }
        }
        
        private void Draw(float delta) {
            pos.X = ThingTools.Lerp(pos.X, isFocused ? tPosFoc.X : tPos.X, 10 * delta);

            RRender.DrawString(Alh, Alv, AlhT, AlvT, Text, (int)pos.X + sizeX / 2, (int)pos.Y + sizeY / 2, fontSize, cColour);
        }
    }
}