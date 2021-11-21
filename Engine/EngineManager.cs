using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RayKeys.Options;

namespace RayKeys {
    public class EngineManager : Scene {
        public static AudioManager Music = new AudioManager();
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
            Start(level, speed);
        }

        public void Start(string level, float speed = 1f) {

            engines = new List<Engine>();
            //Music.Stop();

            // Read Json
            string fileS = File.ReadAllText("Content/Levels/" + level + "/song.json");
            using JsonDocument doc = JsonDocument.Parse(fileS);
            JsonElement root = doc.RootElement;
            
            // Get BPS
            bps = (float) (root.GetProperty("bpm").GetDouble() / 60d);
            Console.WriteLine(bps);

            // Get notes
            JsonElement beatmaps = root.GetProperty("beatmaps");
            List<Note>[] notes = new List<Note>[beatmaps.GetArrayLength()];

            for (int i = 0; i < notes.Length; i++) {
                notes[i] = new List<Note>();
                for (int j = 0; j < beatmaps[i].GetArrayLength(); j++) {
                    notes[i].Add(new Note((float) beatmaps[i][j].GetProperty("time").GetDouble() / bps + 3, beatmaps[i][j].GetProperty("lane").GetByte()));
                }
            }
            
            // Get which engine for which beatmap
            JsonElement playersJ = root.GetProperty("players");

            // add notes to engines
            bool recenterTracks = (bool) OptionsManager.GetOption("repositiontracks").currentValue;
            float recenterLen = playersJ.GetArrayLength() - 1;
            int[] controls = new int[playersJ.GetArrayLength()];

            for (int i = 0; i < playersJ.GetArrayLength(); i++) {
                controls[i] = playersJ[i].TryGetProperty("controls", out JsonElement contJ) ? contJ.GetInt32() : 1;
                if (controls[i] == 0)
                    recenterLen--;
            }
            
            recenterLen /= 2;
            if (recenterLen == 0) {
                recenterLen = 1;
            }

            for (int i = 0; i < playersJ.GetArrayLength(); i++) {
                int xpos;
                if (!recenterTracks)
                     xpos = playersJ[i].TryGetProperty("xpos", out JsonElement xposJ) ? 
                        xposJ.GetInt32() : 960;
                else {
                    if (controls[i] == 0) continue;
                    xpos = (int)((i - recenterLen) * 608f);
                    Console.WriteLine($"Engine {i}: ({i} - {recenterLen}) * 608 = {xpos}");
                }

                engines.Add(new Engine(controls[i], xpos, speed));
                engines[^1].notes = notes[playersJ[i].GetProperty("beatmap").GetInt32() - 1].ToArray().ToList();
            }

            Music.PlaySong("Levels/" + level + "/song.ogg", speed); 

            foreach (Engine engine in engines) {
                engine.Start();
            }
        }
    }
}
