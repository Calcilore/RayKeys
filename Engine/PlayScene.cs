using RayKeys.Menus;

namespace RayKeys {
    public class PlayScene : Scene {
        private EngineManager em;
        private PauseMenu menu;

        private string level;
        private float speed;
        
        public PlayScene(string level, float speed) {
            ConstructorThing(level, speed);
        }

        private void ConstructorThing(string level, float speed) {
            this.level = level;
            this.speed = speed;

            em = new EngineManager(level, speed);

            menu = new PauseMenu(-100);
            menu.PauseEvent += OnPause;
            menu.UnPauseEvent += OnUnPause;
            
            menu.AddFunctionCallButton(RestartButton, "Restart");
            menu.AddFunctionCallButton(ExitButton, "Exit");
        }

        private void OnPause() {
            em.Pause();
        }

        private void OnUnPause() {
            em.UnPause();
        }

        private void RestartButton() {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new PlayScene(level, speed));
        }
        
        private void ExitButton() {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new MainMenu());
        }
    }
}