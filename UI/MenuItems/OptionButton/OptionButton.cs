using RayKeys.Options;

namespace RayKeys.UI {
    public class OptionButton {
        public Button button;
        public string optionName;
        public Option option;
        public string valueText;

        public OptionButton(Button button, string optionName) {
            this.button = button;
            this.optionName = optionName;
            this.option = OptionsManager.GetOption(optionName);

            valueText = option.CurrentValue.ToString();
        }
    }
}