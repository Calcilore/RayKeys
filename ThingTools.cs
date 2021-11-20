using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayKeys {
    public class ThingTools {
        public static Random rand = new Random();
        private static int lastScroll;
        private static int currentScroll;

        public static void ThingToolsUpdate(float delta) {
            lastScroll = currentScroll;
            currentScroll = Mouse.GetState().ScrollWheelValue;
        }

        public static float Lerp(float firstFloat, float secondFloat, float by) {
            // line comment first line to enable second line
            return firstFloat + (secondFloat - firstFloat) * by; /*
            return firstFloat * (1 - by) + secondFloat * by; /**/
        }

        public static int GetScrollFrame() {
            return currentScroll - lastScroll;
        }
    }
}