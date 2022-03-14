namespace RayKeys.UI {
    public class FocusableMenuItem : MenuItem {
        public MenuItem Child;
        public bool IsFocused = false;

        public virtual void Focus() {
            Child?.Show();
            IsFocused = true;
        }
        
        public virtual void UnFocus() {
            Child?.Hide();
            IsFocused = false;
        }
    }
}