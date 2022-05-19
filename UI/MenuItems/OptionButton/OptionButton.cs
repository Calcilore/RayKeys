using RayKeys.Options;

namespace RayKeys.UI {
    public class OptionButton {
        public FocusableMenuItem menuItem;
        public string optionName;
        public Option option;
        public string valueText;
        public string displayName;

        public OptionButton(FocusableMenuItem menuItem, string optionName) {
            this.menuItem = menuItem;
            this.optionName = optionName;
            this.option = OptionsManager.GetOption(optionName);
            this.displayName = option.DisplayName;

            valueText = option.CurrentValue.ToString();
        }
    }
}