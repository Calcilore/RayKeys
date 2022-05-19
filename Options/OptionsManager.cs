using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;

namespace RayKeys.Options {
    public static class OptionsManager {
        private static Dictionary<string, Option> Options = new Dictionary<string, Option>();
        private static JsonDocument doc;

        public static JsonElement GetJson() {
            Directory.CreateDirectory("Save");
            string fileS = File.Exists("Save/save.json") ? File.ReadAllText("Save/save.json") : "{}";
            doc = JsonDocument.Parse(fileS);
            return doc.RootElement;
        }
        
        public static void AddOption(string id, string displayName, OptionType optionType, object value, JsonElement configJ, Action<string, object> changedFunc = null) {
            Option o = new Option(id, displayName, optionType, value, changedFunc);
            if (configJ.TryGetProperty(id, out configJ)) {
                string gS = configJ.GetString();

                switch (optionType) {
                    case OptionType.Boolean:
                        o.CurrentValue = gS == "True";
                        break;
                    
                    case OptionType.Switcher:
                        o.CurrentValue = gS;
                        break;
                    
                    case OptionType.Key:
                        if (Enum.TryParse(gS, out Keys ot)) {
                            o.CurrentValue = ot;
                        }
                        else {
                            Logger.Error($"Invalid Control for {o.DisplayName}, using default value");
                            o.CurrentValue = value;
                        }
                        break;
                    
                    default:
                        Logger.Error("Invalid Option Type for " + o.DisplayName);
                        break;
                }
            }
            
            o.ChangedEvent?.Invoke(id, o.CurrentValue);
            Options.Add(id, o);
        }

        public static Option GetOption(string id) {
            return Options[id];
        }
        
        public static void SetOption(string k, object v) {
            Options[k].CurrentValue = v;
            Options[k].ChangedEvent?.Invoke(k, v);

            Dictionary<string, string> thing = new Dictionary<string, string>();
            foreach ((string key, Option value) in Options) {
                thing.Add(key, value.CurrentValue.ToString());
            }
            
            string json = JsonSerializer.Serialize(thing, ThingTools.JsonSOptions);
            File.WriteAllText("Save/save.json", json);
        }
    }
}