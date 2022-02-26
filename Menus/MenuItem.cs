namespace RayKeys.Menus {
    public abstract class MenuItem {
        public Menu parent;
        public int Id;

        public virtual void Hide() { }

        public virtual void Show() { }
    }
}