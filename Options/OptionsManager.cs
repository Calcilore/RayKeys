using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RayKeys.Misc;

namespace RayKeys.Options {
    public static class OptionsManager {
        private static Dictionary<string, Option> Options = new Dictionary<string, Option>();
        private static JsonDocument doc;

        private static JsonElement GetJson() {
            Directory.CreateDirectory("Save");
            string fileS = File.Exists("Save/save.json") ? File.ReadAllText("Save/save.json") : "{}";
            doc = JsonDocument.Parse(fileS);
            return doc.RootElement;
        }
        
        private static void AddOption(string id, OptionType optionType, object value, JsonElement configJ, Action<object> changedFunc = null) {
            Logger.Info("Initialising " + id);
            
            Option o = new Option(id, optionType, value, changedFunc);
            if (configJ.TryGetProperty(id, out configJ)) {
                string gS = configJ.GetString();

                o.currentValue = optionType switch {
                    OptionType.Boolean => gS == "True",
                    OptionType.Switcher => gS,
                    _ => o.currentValue
                };
            }
            
            o.changedEvent?.Invoke(o.currentValue);
            Options.Add(id, o);
        }
        
        public static void Initialise() {
            Logger.Info("Initialising Options...");
            JsonElement root = GetJson();

            AddOption("limitfps", OptionType.Boolean, false, root, OptionsActions.LimitFPSChanged);
            AddOption("fpslimit", OptionType.Switcher, "60 FPS", root, OptionsActions.FPSLimitChanged);
            AddOption("resolution", OptionType.Switcher, "1920x1080", root, OptionsActions.ResolutionChanged);
            AddOption("vsync", OptionType.Boolean, false, root, OptionsActions.VSyncChanged);
            AddOption("fullscreen", OptionType.Boolean, false, root, OptionsActions.FullscreenChanged);
            AddOption("downscroll", OptionType.Boolean, true, root);
            AddOption("repositiontracks", OptionType.Boolean, false, root);
            AddOption("sectionScrolling", OptionType.Boolean, false, root);
            
            Logger.Info("Initialising All Options!");
        }
        
        public static Option GetOption(string id) {
            return Options[id];
        }
        
        public static void SetOption(string k, object v) {
            Options[k].currentValue = v;
            Options[k].changedEvent?.Invoke(v);

            Dictionary<string, string> thing = new Dictionary<string, string>();
            foreach (KeyValuePair<string, Option> op in Options) {
                thing.Add(op.Key, op.Value.currentValue.ToString());
            }
            
            string json = JsonSerializer.Serialize(thing, ThingTools.jsonSOptions);
            File.WriteAllText("Save/save.json", json);
        }
    }
}