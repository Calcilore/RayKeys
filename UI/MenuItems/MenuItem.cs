namespace RayKeys.UI {
    public abstract class MenuItem {
        public Menu parent;
        public int Id;
        public bool followCamera;

        public virtual void Hide() { }

        public virtual void Show() { }
    }
}