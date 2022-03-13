using System;
using System.Text.Json;

namespace RayKeys {
    public static class ThingTools {
        public static Random Rand = new Random();
        public static JsonSerializerOptions jsonSOptions;
        public static JsonDocumentOptions jsonDOptions;

        public static void Init() {
            jsonSOptions = new JsonSerializerOptions {
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            jsonDOptions = new JsonDocumentOptions {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
        }
        
        public static float Lerp(float firstFloat, float secondFloat, float by) {
            // line comment first line to enable second line
            return firstFloat + (secondFloat - firstFloat) * by; /*
            return firstFloat * (1 - by) + secondFloat * by; /**/
        }

        public static float Floor(float n, int di = 0) {
            int m = (int) Math.Pow(10, di);
            return (float) (int) (n * m) / m;
        }

        public static float FloorN(float n, int di) {
            return (float) (int) (n / di) * di;
        }
        
        public static float Round(float n, int di = 0) {
            int m = (int) Math.Pow(10, di);
            return (float) (Math.Round(n * m) / m);
        }

        public static float RoundN(float n, int di) {
            return (float) (Math.Round(n / di) * di);
        }
    }
}