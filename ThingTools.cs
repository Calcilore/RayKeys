using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys {
    public class ThingTools {
        public static Random rand = new Random();
        
        public static float Lerp(float firstFloat, float secondFloat, float by) {
            // line comment first line to enable second line
            return firstFloat + (secondFloat - firstFloat) * by; /*
            return firstFloat * (1 - by) + secondFloat * by; /**/
        }
    }
}