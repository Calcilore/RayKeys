using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RayKeys.Misc;
using RayKeys.Options;

namespace RayKeys {
    public class EngineManager {
        private List<Engine> engines = new List<Engine>();
        private float bps;
        
        public static string GetName(string level) {
            string fileS = File.ReadAllText("Content/Levels/" + level + "/song.json");
            using JsonDocument doc = JsonDocument.Parse(fileS);
            JsonElement root = doc.RootElement;
            
            return root.GetProperty("name").GetString();
        }

        public EngineManager() { }

        public EngineManager(string level, float speed = 1f) {
            Start(level, speed:speed);
        }

        public void Start(string level, float countdownTimer = 3f, float speed = 1f, int forceXPos = -1) {

            Logger.Info("Starting Engine Manager");
            
            engines = new List<Engine>();

            JsonFileThing rawLevel = SongJsonManager.LoadJson(level);
            bps = rawLevel.bps;
            
            // add notes to engines

            for (int i = 0; i < rawLevel.players.Count; i++) {
                Player rawLevelPlayer = rawLevel.players[i];

                // 960 = 1920 / 2
                int xpos = 960 / rawLevel.players.Count * (i*2+1) - 960;

                if (forceXPos != -1) {
                    if (rawLevelPlayer.controls == 0) continue;
                    xpos = forceXPos;
                    rawLevelPlayer.controls = 0;
                }

                engines.Add(new Engine(rawLevelPlayer.controls, xpos, countdownTimer, speed));
                List<Note> notes = new List<Note>();
                
                float sAdd = 0;
                foreach (List<Note> section in rawLevel.beatmaps[rawLevelPlayer.beatmap]) {
                    foreach (Note note in section) {
                        notes.Add(new Note((note.time + sAdd) / rawLevel.bps, note.lane));
                    }
                    
                    sAdd += 16;
                }

                engines[^1].notes = notes;

                if (forceXPos != -1) break;
            }

            AudioManager.LoadSong("Levels/" + level + "/song.ogg", bps / Engine.BeatMultiplier, speed); 
            AudioManager.Play();
            AudioManager.SetPause(true);

            foreach (Engine engine in engines) {
                engine.Start();
            }
        }

        public void Pause() {
            foreach (Engine engine in engines) {
                engine.Pause();
            }
        }
        
        public void UnPause() {
            foreach (Engine engine in engines) {
                engine.UnPause();
            }
        }
    }
}
