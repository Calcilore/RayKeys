using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RayKeys.Menus {
    public class Page {
        public List<MenuItem> Items;
        public Point pos;

        public Page(int x, int y) {
            Items = new List<MenuItem>();
            pos = new Point(x, y);
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