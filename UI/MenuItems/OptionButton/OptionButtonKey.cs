using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Options;

namespace RayKeys.UI {
    public class OptionButtonKey : OptionButton {
        private bool isListening;

        public OptionButtonKey(OtherFocusLabel otl, string optionName) : base(otl, optionName) {
            ((Button)otl.Other).ClickEvent += OnClick;
            isListening = false;
        }
        
        private void OnClick(int id, params object[] args) {
            if (isListening) return;
            
            isListening = true;
            Menu menu = menuItem.parent;
            
            menu.AddToHistory(menu.CurrentPage);
            menu.EscapeEvent += OnEscape;
            Game1.Game.UpdateEvent += Update;

            ((Button)menuItem.Other).Color = Color.SlateGray;
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
            ((Button)menuItem.Other).Color = Color.White;

            menuItem.parent.EscapeEvent -= OnEscape;
            Game1.Game.UpdateEvent -= Update;
        }

        private void OnEscape() {
            FinishClick();
        }
        
        private void Update(float delta) {
            if (RKeyboard.PressedKeys.Length > 0) {
                menuItem.parent.DeleteHistoryNoPageChange();
                RegisterClick(RKeyboard.PressedKeys[0]);
                FinishClick();
                ((Button)menuItem.Other).Label = valueText;
            }
        }
    }
}