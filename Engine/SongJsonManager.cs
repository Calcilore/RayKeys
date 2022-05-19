using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RayKeys.Misc;

namespace RayKeys {
    public static class SongJsonManager {
        public static bool LoadJson(string path, out JsonFileThing json) {
            try {
                JsonFileThing jf = new JsonFileThing();

                // Read Json
                string fileS = File.ReadAllText("Content/Levels/" + path + "/song.json");
                using JsonDocument doc = JsonDocument.Parse(fileS, ThingTools.JsonDOptions);
                JsonElement root = doc.RootElement;
            
                // Get BPS
                jf.bpm = (float)root.GetProperty("bpm").GetDouble();
                jf.bps = jf.bpm / Engine.BeatMultiplier;
            
                // Get Info
                jf.name = root.GetProperty("name").GetString();
                jf.songName = root.GetProperty("songName").GetString();
                jf.artist = root.GetProperty("artist").GetString();

                // Get notes
                JsonElement beatmaps = root.GetProperty("beatmaps");
                int bmLen = beatmaps.GetArrayLength();

                jf.beatmaps = new List<List<List<Note>>>();
                for (int i = 0; i < bmLen; i++) {
                    jf.beatmaps.Add(new List<List<Note>>());
                    for (int j = 0; j < beatmaps[i].GetArrayLength(); j++) {
                        jf.beatmaps[i].Add(new List<Note>());
                        for (int k = 0; k < beatmaps[i][j].GetArrayLength(); k++) {
                            jf.beatmaps[i][j].Add(new Note((float) beatmaps[i][j][k].GetProperty("time").GetDouble(), beatmaps[i][j][k].GetProperty("lane").GetByte()));
                        }
                    }
                }
            
                // Get which engine for which beatmap
                JsonElement playersJ = root.GetProperty("players");

                // Get Players
                jf.players = new List<Player>();
                for (int i = 0; i < playersJ.GetArrayLength(); i++) {
                    jf.players.Add(new Player {
                        controls = playersJ[i].TryGetProperty("controls", out JsonElement contJ) ? contJ.GetInt32() : 1,
                        beatmap = playersJ[i].GetProperty("beatmap").GetInt32() - 1
                    });
                }

                json = jf;
                return true;
            }
            catch (Exception e) {
                Logger.Error(e.Message);
                json = new JsonFileThing();
                return false;
            }
        }

        public static void SaveJson(string path, JsonFileThing data) {
            Dictionary<string, object> json = new Dictionary<string, object>();
            
            json.Add("name", data.name);
            json.Add("songName", data.songName);
            json.Add("artist", data.artist);
            json.Add("bpm", data.bpm);
            List<Dictionary<string, int>> players = new List<Dictionary<string, int>>();
            List<List<List<Dictionary<string, object>>>> beatmaps = new List<List<List<Dictionary<string, object>>>>();

            for (int i = 0; i < data.players.Count; i++) {
                players.Add(new Dictionary<string, int> {{"beatmap", data.players[i].beatmap}, {"controls", data.players[i].controls}});
                
                beatmaps.Add(new List<List<Dictionary<string, object>>>());
                for (int j = 0; j < data.beatmaps[i].Count; j++) {
                    beatmaps[i].Add(new List<Dictionary<string, object>>());
                    for (int k = 0; k < data.beatmaps[i][j].Count; k++) {
                        beatmaps[i][j].Add(new Dictionary<string, object> { {"lane", data.beatmaps[i][j][k].lane}, {"time", data.beatmaps[i][j][k].time} });
                    }
                }
            }

            json.Add("players", players);
            json.Add("beatmaps", beatmaps);
            
            string jsonS = JsonSerializer.Serialize(json, ThingTools.JsonSOptions);
            File.WriteAllText("Content/Levels/" + path + "/song.json", jsonS);
        }
    }
    
    public struct JsonFileThing {
        public string name;
        public string songName;
        public string artist;
        public float bpm;
        public float bps;
        public List<Player> players;

        public List<List<List<Note>>> beatmaps;
    }
        
    public struct Player {
        public int beatmap;
        public int controls;
    }
}