using RayKeys.Options;

namespace RayKeys.Menu {
    public class OptionButton {
        public Button button;
        public object[] values;
        public string optionName;
        public Option option;
        public string valueText;

        public OptionButton(Button button, object[] values, string optionName) {
            this.button = button;
            this.values = values;
            this.optionName = optionName;
            this.option = OptionsManager.GetOption(optionName);

            valueText = option.currentValue.ToString();
        }
    }
}