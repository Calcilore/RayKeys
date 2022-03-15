using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Options;

namespace RayKeys.UI {
    public class OptionButtonKey : OptionButton {
        private bool isListening;

        public OptionButtonKey(Button button, string optionName) : base(button, optionName) {
            this.button.ClickEvent += OnClick;
            isListening = false;
        }
        
        private void OnClick(int id, params object[] args) {
            if (isListening) return;
            
            isListening = true;
            Menu menu = button.parent;
            
            menu.AddToHistory(menu.CurrentPage);
            menu.EscapeEvent += OnEscape;
            Game1.Game.UpdateEvent += Update;

            button.Colour = Color.SlateGray;
        }

        private void RegisterClick(Keys key) {
            OptionsManager.SetOption(optionName, key);
            Logger.Info($"Changing Control {option.DisplayName} to {key}");
            
            valueText = option.CurrentValue.ToString();
        }
        
        private void FinishClick() {
            if (!isListening) {
                Logger.Error("Finish Click was called when it shouldn't have");
                return;
            }
            
            isListening = false;
            button.Colour = Color.White;
            
            button.parent.EscapeEvent -= OnEscape;
            Game1.Game.UpdateEvent -= Update;
        }

        private void OnEscape() {
            FinishClick();
        }
        
        private void Update(float delta) {
            if (RKeyboard.PressedKeys.Length > 0) {
                button.parent.DeleteHistoryNoPageChange();
                RegisterClick(RKeyboard.PressedKeys[0]);
                FinishClick();
            }
        }
    }
}