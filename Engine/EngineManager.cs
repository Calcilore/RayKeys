using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace RayKeys {
    public class EngineManager {
        public static RhythmManager Music = new RhythmManager();

        private static List<Engine> engines = new List<Engine>();

        public static void addEngine(Keys[] controls, int xpos = 960, float speed = 1f) {
            engines.Add(new Engine(new Keys[] {Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L}, xpos, speed));
        }

        public static void Start() {
            Music.PlaySongBPM("Levels/1/song.ogg", 170);

            foreach (Engine engine in engines) {
                engine.Start();
            }
        }
    }
}