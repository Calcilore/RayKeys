using System;
using RayKeys.Render;

namespace RayKeys.Menus {
    public class PauseMenu {
        public Menu menu;
        public delegate void Event();
        public event Event PauseEvent;
        public event Event UnPauseEvent;

        public int OBYPos;

        public PauseMenu(int startPos) {
            OBYPos = startPos;

            menu = new Menu();
            menu.ChangeSelectionEvent += OnSwitch;
            menu.EscapeEvent += OnEscape;
            
            // Make page 1 the game so escape will go to pause menu
            menu.AddPage(0, 0); // 0 the game
            menu.AddPage(0, 0); // 1 pause menu

            menu.AddPageChangeButton(0, 1, Align.Left, Align.Top, Align.Left, Align.Top, "", 0, 0).Hide();

            menu.AddPageChangeButton(1, 0, Align.Left, Align.Center, Align.Left, Align.Center, "Resume", 16, OBYPos).Hide();
            OBYPos += 100;
        }

        public void AddFunctionCallButton(Action func, string name) {
            Button b = menu.AddFunctionCallButton(1, func, Align.Left, Align.Center, Align.Left, Align.Center, name, 16, OBYPos);
            if (menu.CurrentPage == 0) b.Hide(); else b.Show();
            
            OBYPos += 100;
        }
        
        public Button AddButton(string name) {
            Button b = menu.AddButton(1, Align.Left, Align.Center, Align.Left, Align.Center, name, 16, OBYPos);
            if (menu.CurrentPage == 0) b.Hide(); else b.Show();
            
            OBYPos += 100;

            return b;
        }

        private void OnSwitch(int before, int after) {
            if (after == 0) {
                if (before == 0) return;
                
                Console.WriteLine("Unpause");
                menu.pages[1].HideItems();
                UnPauseEvent?.Invoke();
                return;
            }
            
            if (before != 0) return;
            Console.WriteLine("Pause");
            menu.pages[1].ShowItems();
            PauseEvent?.Invoke();
        }

        private void OnEscape() {
            menu.ChangePage(1);
        }
    }
}