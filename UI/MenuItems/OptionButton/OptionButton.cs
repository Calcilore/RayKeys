using RayKeys.Options;

namespace RayKeys.UI {
    public class OptionButton {
        public OtherFocusLabel menuItem;
        public string optionName;
        public Option option;
        public string valueText;
        public string displayName;

        public OptionButton(OtherFocusLabel menuItem, string optionName) {
            this.menuItem = menuItem;
            this.optionName = optionName;
            this.option = OptionsManager.GetOption(optionName);
            this.displayName = option.DisplayName;

            valueText = option.CurrentValue.ToString();
            menuItem.Text = displayName;
        }

        public void Hide() {
            menuItem.Hide();
            menuItem.Other.Hide();
        }

        public void Show() {
            menuItem.Show();
            menuItem.Other.Show();
        }
    }
}