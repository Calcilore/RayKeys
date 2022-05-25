using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.UI {
    public class Menu {
        public delegate void EscapeEventD();
        public event EscapeEventD EscapeEvent;
        public event EscapeEventD AllEscapeEvent;
        
        public delegate void ChangeSelectionEventD(int beforeId, int afterId);
        public event ChangeSelectionEventD ChangeSelectionEvent;
        public event ChangeSelectionEventD ChangePageEvent;
        
        public List<Page> pages;

        public int CurrentPage { get; private set; }
        public int CurrentSelection { get; private set; }
        private bool changeSel = false; 
        private int oldPage; private int oldSel;
        public Vector2 tPos;

        private List<int> history = new List<int>();
        
        private int currentI;

        private void ChangeSelectionPage(int selection, int page) {
            Logger.Debug($"Changing Selection to {selection} from {CurrentSelection}");
            
            //pages[CurrentPage].FocusableItems[CurrentSelection].UnFocus();
            oldPage = CurrentPage; oldSel = CurrentSelection;
            changeSel = true;

            CurrentPage = page;
            CurrentSelection = selection;
        }
        
        public void ChangePageNoHistory(int pageId) {
            tPos = pages[pageId].pos.ToVector2();

            ChangePageEvent?.Invoke(CurrentPage, pageId);

            ChangeSelectionPage(0, pageId);
        }

        public void AddToHistory(int pageId) {
            history.Add(pageId);
        }
        
        public void DeleteHistoryNoPageChange() {
            history.RemoveAt(history.Count - 1);
        }
        
        public void DeleteHistory() {
            ChangePageNoHistory(history[^1]);
            DeleteHistoryNoPageChange();
        }

        public void ChangePage(int pageId) {
            AddToHistory(CurrentPage);
            ChangePageNoHistory(pageId);
        }

        private void PageChangeEventListener(int id, params object[] args) {
            int pageId = (int) args[0];

            if ((bool)args[1]) ChangePage(pageId);
            else      ChangePageNoHistory(pageId);
        }

        private void FunctionCallButtonEventListener(int id, params object[] args) {
            Action action = (Action) args[0];
            action.Invoke();
        }
        
        private void FunctionCallIFEventListener(int id, string text, params object[] args) {
            Action<string> action = (Action<string>) args[0];
            action.Invoke(text);
        }
        
        private void FunctionCallSwitcherEventListener(int id, float swPos, params object[] args) {
            Action<float> action = (Action<float>) args[0];
            action.Invoke(swPos);
        }
        
        public Menu() {
            pages = new List<Page>();

            Game1.UpdateEvent += Update;
        }

        private void Update(float delta) {
            if (changeSel) {
                pages[oldPage].FocusableItems[oldSel].UnFocus();
                pages[CurrentPage].FocusableItems[CurrentSelection].Focus();
                
                ChangeSelectionEvent?.Invoke(pages[oldPage].FocusableItems[oldSel].Id, pages[CurrentPage].FocusableItems[CurrentSelection].Id);
                
                changeSel = false;
            }

            if (!pages[CurrentPage].followCamera) {
                RRender.CameraPos.X = ThingTools.Lerp(RRender.CameraPos.X, tPos.X, 10 * delta);
                RRender.CameraPos.Y = ThingTools.Lerp(RRender.CameraPos.Y, tPos.Y, 10 * delta);    
            }

            if (RKeyboard.IsKeyPressed(Keys.Escape)) {
                AllEscapeEvent?.Invoke();
                
                if (history.Count == 0) {
                    EscapeEvent?.Invoke();
                }
                else {
                    DeleteHistory();
                }
            }
            
            if (RKeyboard.IsKeyPressed(Keys.Down)) {
                if (CurrentSelection >= pages[CurrentPage].FocusableItems.Count - 1) ChangeSelectionPage(0, CurrentPage);
                else                                                                 ChangeSelectionPage(CurrentSelection + 1, CurrentPage);
            }
            else if (RKeyboard.IsKeyPressed(Keys.Up)) {
                if (CurrentSelection <= 0) ChangeSelectionPage(pages[CurrentPage].FocusableItems.Count - 1, CurrentPage);
                else                       ChangeSelectionPage(CurrentSelection - 1, CurrentPage);
            }
        }

        public void AddPage(int x, int y, bool followCamera = false) {
            pages.Add(new Page(x, y, followCamera));
        }

        private void AddCommon(int page, FocusableMenuItem mi) {
            currentI++;
            pages[page].FocusableItems.Add(mi);
            
            if (!pages[CurrentPage].FocusableItems[CurrentSelection].IsFocused) pages[CurrentPage].FocusableItems[CurrentSelection].Focus();
        } 

        public Button AddButton(int page, Align h, Align v, Align hT, Align vT, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            x += pages[page].pos.X; y += pages[page].pos.Y;
            
            Button button = new Button(this, pages[page].followCamera, h, v, hT, vT, currentI, text, x, y, sizeX, sizeY, fontSize);
            
            AddCommon(page, button);
            return button;
        }
        
        public Button AddPageChangeButton(int page, int pageTo, Align h, Align v, Align hT, Align vT, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Button button = AddButton(page, h, v, hT, vT, text, x, y, sizeX, sizeY, fontSize);
            button.ClickEvent += PageChangeEventListener;
            button.args = new object[] {pageTo, true};

            return button;
        }
        
        public Button AddPageChangeNoHistoryButton(int page, int pageTo, Align h, Align v, Align hT, Align vT, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Button button = AddButton(page, h, v, hT, vT, text, x, y, sizeX, sizeY, fontSize);
            button.ClickEvent += PageChangeEventListener;
            button.args = new object[] {pageTo, false};

            return button;
        }
        
        public Button AddFunctionCallButton(int page, Action func, Align h, Align v, Align hT, Align vT, string text, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 3) {
            Button button = AddButton(page, h, v, hT, vT, text, x, y, sizeX, sizeY, fontSize);
            button.ClickEvent += FunctionCallButtonEventListener;
            button.args = new object[] {func};

            return button;
        }

        public InputField AddInputField(int page, Align h, Align v, string label, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 4) {
            x += pages[page].pos.X; y += pages[page].pos.Y;
            
            InputField inputField = new InputField(this, pages[page].followCamera, h, v, currentI, label, x, y, sizeX, sizeY, fontSize);
            
            AddCommon(page, inputField);
            return inputField;
        }

        public InputField AddFunctionCallInputField(int page, Action<string> func, Align h, Align v, string label, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 4) {
            InputField inp = AddInputField(page, h, v, label, x, y, sizeX, sizeY, fontSize);
            inp.EnterEvent += FunctionCallIFEventListener;
            inp.args = new object[] {func};

            return inp;
        }

        public Slider AddSlider(int page, Align h, Align v, string label, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 4) {
            x += pages[page].pos.X; y += pages[page].pos.Y;
            
            Slider slider = new Slider(this, pages[page].followCamera, h, v, currentI, label, x, y, sizeX, sizeY, fontSize);
            
            AddCommon(page, slider);
            return slider;
        }

        public Slider AddFunctionCallSlider(int page, Action<float> func, Align h, Align v, string label, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 4) {
            Slider slider = AddSlider(page, h, v, label, x, y, sizeX, sizeY, fontSize);
            slider.EnterEvent += FunctionCallSwitcherEventListener;
            slider.args = new object[] {func};

            return slider;
        }
        
        public OtherFocusLabel AddOtherFocusLabel(int page, Align h, Align v, Align hT, Align vT, string text, FocusableMenuItem other, int x, int y, int fontSize = 4, Color? color = null, float drawdepth = RRender.DefaultDepth) {
            x += pages[page].pos.X; y += pages[page].pos.Y;

            color ??= Color.White;
            OtherFocusLabel button = new OtherFocusLabel(this, pages[page].followCamera, h, v, hT, vT, currentI, text, other, x, y, fontSize, color.Value, drawdepth);
            
            AddCommon(page, button);
            return button;
        }

        public Label AddLabel(int page, Align h, Align v, Align hT, Align vT, string text, int x, int y, int fontSize, Color color, float drawDepth = RRender.DefaultDepth, FocusableMenuItem itemParent = null) {
            x += pages[page].pos.X; y += pages[page].pos.Y;

            Label label = new Label(this, pages[page].followCamera, h, v, hT, vT, currentI, text, x, y, fontSize, color, drawDepth);
            currentI++;

            pages[page].UnFocusableItems.Add(label);

            if (itemParent != null) {
                itemParent.Child = label;
                label.Hide();
            }
            
            return label;
        }
        
        public Label AddLabel(int page, Align h, Align v, Align hT, Align vT, string text, int x, int y, int fontSize = 3) {
            return AddLabel(page, h, v, hT, vT, text, x, y, fontSize, Color.White);
        }
    }
}