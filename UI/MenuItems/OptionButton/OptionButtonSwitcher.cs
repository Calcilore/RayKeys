using RayKeys.Misc;
using RayKeys.Options;

namespace RayKeys.UI {
    public class OptionButtonSwitcher : OptionButton {
        public object[] values;
        
        public OptionButtonSwitcher(Button button, object[] values, string optionName) : base(button, optionName) {
            this.values = values;

            button.ClickEvent += OnClick;
        }
        
        private void OnClick(int id, params object[] args) {
            int cI = -1;
            for (int i = 0; i < values.Length; i++) {
                if (option.CurrentValue.ToString() == values[i].ToString()) { // without .ToString() the first time you change a boolean option it wont work
                    cI = i;
                    break;
                }
            }

            object valueTo = values[cI >= values.Length - 1 ? 0 : cI + 1];
            OptionsManager.SetOption(optionName, valueTo);
            Logger.Info($"Changing Option {option.DisplayName} to {valueTo}");

            valueText = option.CurrentValue.ToString();
        }
    }
}