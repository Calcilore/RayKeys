using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RayKeys.UI {
    public class Page {
        public List<FocusableMenuItem> FocusableItems;
        public List<MenuItem> UnFocusableItems;
        public bool followCamera;
        public Point pos;

        public Page(int x, int y, bool followCamera = false) {
            FocusableItems = new List<FocusableMenuItem>();
            UnFocusableItems = new List<MenuItem>();
            pos = new Point(x, y);
            this.followCamera = followCamera;
        }

        public void HideItems() {
            foreach (FocusableMenuItem item in FocusableItems) {
                item.Hide();
            }
            
            foreach (MenuItem item in UnFocusableItems) {
                item.Hide();
            }
        }

        public void ShowItems() {
            foreach (FocusableMenuItem item in FocusableItems) {
                item.Show();
            }
            
            foreach (MenuItem item in UnFocusableItems) {
                item.Show();
            }
        }
    }
}