using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayKeys.Menu {
    public class Button {
        private Texture2D tex;
        private Color cColour;
        private bool pressedLastFrame;
        
        public delegate void ClickEventD();
        public event ClickEventD ClickEvent;
        
        public int x;
        public int y;
        public string text;

        public Button(string text, int x, int y) {
            tex = Game1.Game.Textures["button"];
            
            Game1.Game.DrawEvent += Draw;
            this.x = x;
            this.y = y;
            this.text = text;

            cColour = Color.White;
        }

        private void Draw(float delta) {
            MouseState ms = Mouse.GetState();
            bool pressed = ms.LeftButton == ButtonState.Pressed;
            
            if ( ms.X >= x && ms.X <= x + 600 &&
                 ms.Y >= y && ms.Y <= y + 200 ) {
                Mouse.SetCursor(MouseCursor.Hand);
                cColour = pressed ? Color.Gray : Color.LightGray;

                if (!pressed && pressedLastFrame) {
                    ClickEvent?.Invoke();
                }
            }
            else {
                Mouse.SetCursor(MouseCursor.Arrow);
                cColour = Color.White;
            }
            
            
            ThingTools.DrawToSpriteBatch(tex, x, y, 0, 0, 600, 200, cColour);
            ThingTools.DrawStringCentered(text, x + 300, y + 100, 2, cColour);

            pressedLastFrame = pressed;
        }
    }
}