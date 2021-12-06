using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace RayKeys.Misc {
    public class RKeyboard {
        public static Keys[] HeldKeys;
        public static Keys[] LastHeldKeys;

        public static void Update() {
            LastHeldKeys = HeldKeys;
            
            KeyboardState ks = Keyboard.GetState();
            HeldKeys = ks.GetPressedKeys();
        }

        public static bool IsKeyPressed(Keys k) {
            return HeldKeys.Contains(k) && !LastHeldKeys.Contains(k);
        }
        
        public static bool IsKeyHeld(Keys k) {
            return HeldKeys.Contains(k);
        }
    }
}