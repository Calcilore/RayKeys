using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RayKeys.UI {
    public class Page {
        public List<MenuItem> Items;
        public bool followCamera;
        public Point pos;

        public Page(int x, int y, bool followCamera = false) {
            Items = new List<MenuItem>();
            pos = new Point(x, y);
            this.followCamera = followCamera;
        }

        public void HideItems() {
            foreach (MenuItem item in Items) {
                item.Hide();
            }
        }

        public void ShowItems() {
            foreach (MenuItem item in Items) {
                item.Show();
            }
        }
    }
}