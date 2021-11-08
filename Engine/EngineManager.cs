using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Input;

namespace RayKeys {
    public class EngineManager {
        public static AudioManager Music = new AudioManager();
        private static List<Engine> engines = new List<Engine>();
        private static float bps;

        public static void Start(string level, float speed = 1f) {

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
            int[] players = new int[playersJ.GetArrayLength()];

            // add notes to engines
            for (int i = 0; i < players.Length; i++) {
                int xpos = playersJ[i].TryGetProperty("xpos", out JsonElement xposJ) ? 
                    xposJ.GetInt32() : 960;
                
                int cont = playersJ[i].TryGetProperty("controls", out JsonElement contJ) ? 
                    contJ.GetInt32() : 1;
                
                engines.Add(new Engine(cont, xpos, speed));
                engines[i].notes = notes[playersJ[i].GetProperty("beatmap").GetInt32() - 1];
            }

            Music.PlaySong("Levels/" + level + "/song.ogg", speed); 

            foreach (Engine engine in engines) {
                engine.Start();
            }
        }
    }
}
