using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RayKeys.Menu {
    public class Page {
        public List<MenuItem> Items;
        public Point pos;

        public Page(int x, int y) {
            Items = new List<MenuItem>();
            pos = new Point(x, y);
        }
    }
}