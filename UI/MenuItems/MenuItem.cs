using Microsoft.Xna.Framework;
using RayKeys.Misc;

namespace RayKeys.UI {
    public abstract class MenuItem {
        public Menu parent;
        public int Id;
        public bool followCamera;
        private bool isSubbed;

        protected MenuItem() {
            Game1.DrawEvent += Draw;
            isSubbed = true;
        }

        protected virtual void Draw(float delta) {}

        public virtual void Hide() {
            if (isSubbed) Game1.DrawEvent -= Draw;
            isSubbed = false;
        }

        public virtual void Show() {
            if (!isSubbed) Game1.DrawEvent += Draw;
            isSubbed = true;
        }
    }
}