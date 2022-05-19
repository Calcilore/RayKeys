using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RayKeys.Render;

namespace RayKeys.UI {
    public class FocusableMenuItem : MenuItem {
        public MenuItem Child;
        public bool IsFocused = false;
        
        public Align Alh;
        public Align Alv;
        
        protected Vector2 tPos;
        protected Vector2 tPosFoc;
        public Vector2 pos;

        public virtual void Focus() {
            Child?.Show();
            IsFocused = true;
        }
        
        public virtual void UnFocus() {
            Child?.Hide();
            IsFocused = false;
        }

        protected virtual Vector2 CalculateOffset() {
            return Vector2.Zero;
        }
        
        protected virtual void DrawArrow() {
            Vector2 fPos = pos + CalculateOffset() - new Vector2(0f, Stuffs.GetTexture(Textures.Arrow).Height / 2f);
            
            switch (Alh) {
                case Align.Left:
                    RRender.Draw(Alh, Alv, Textures.Arrow, (int) fPos.X, (int) fPos.Y);
                    break;
                
                case Align.Right:
                    RRender.Draw(Alh, Alv, Textures.Arrow, (int) fPos.X - 64, (int) fPos.Y, 
                        spriteEffects: SpriteEffects.FlipHorizontally);
                    break;
                
                case Align.Center:
                    RRender.Draw(Alh, Alv, Textures.Arrow, (int) fPos.X - 64, (int) fPos.Y, 
                        spriteEffects: SpriteEffects.FlipHorizontally);
                    break;
            }
        }

        protected override void Draw(float delta) {
            base.Draw(delta);

            if (IsFocused) {
                DrawArrow();
            }
        }
    }
}