using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Options;
using RayKeys.UI;

namespace RayKeys.Render {
    public static class Stuffs {
        private static Texture2D[] textures;
        private static SoundEffect[] sounds;
        private static Keys[] controls;

        public static Texture2D GetTexture(int id) {
            return textures[id];
        }
        
        public static Texture2D GetTexture(Textures id) {
            return GetTexture((int) id);
        }
        
        public static SoundEffect GetSound(int id) {
            return sounds[id];
        }
        
        public static SoundEffect GetSound(Sounds id) {
            return GetSound((int) id);
        }

        public static Keys GetControl(int id) {
            return controls[id];
        }

        public static Keys GetControl(Controls id) {
            return GetControl((int) id);
        }

        private static void LogAndText(string info) {
            Logger.Info(info);
            LoadingScene.loadText = info + "...";
        }
        
        private static void AddSound(ContentManager content, int i, string sound) {
            LogAndText("Loading Sound " + (Textures)i);
            
            sounds[i] = content.Load<SoundEffect>("Sounds/" + sound);
        }
        
        private static void AddTex(ContentManager content, int i, string tex) {
            LogAndText("Loading Texture " + (Textures)i);
            
            textures[i] = content.Load<Texture2D>("Textures/" + tex);;
        }

        private static void AddTexSht(ContentManager content, int i, string tex, int sourceX, int sourceY, int sizeX, int sizeY) {
            LogAndText("Loading Sprite Sheet " + (Textures)i);
            
            Texture2D texture = content.Load<Texture2D>("Textures/" + tex);

            Rectangle newBounds = new Rectangle(sourceX, sourceY, sizeX, sizeY);
            
            Texture2D croppedTexture = new Texture2D(Game1.Game.GraphicsDevice, newBounds.Width, newBounds.Height);
            Color[] data = new Color[newBounds.Width * newBounds.Height];
            texture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            croppedTexture.SetData(data);

            textures[i] = croppedTexture;
        }

        private static List<List<object>> things = new List<List<object>>();
        private static List<Action<string, object>> thingsFuncThing = new List<Action<string, object>>();
        private static int thingsIndex = 0;
        private static int assetIndex = 0;
        private static ContentManager content;
        private static JsonElement root;

        public static void ControlChanged(string k, object v) {
            Enum.TryParse(k[(k.IndexOf('_') + 1)..], out Controls i);
            controls[(int)i] = (Keys) v;
        }
        
        private static void Update(float delta) {
            List<object> thing = things[thingsIndex];

            switch (thing[0]) {
                case "reseti":
                    assetIndex = -1;
                    break;

                case "sheet":
                    AddTexSht(content, assetIndex, (string) thing[1], (int) thing[2], (int) thing[3], (int) thing[4],
                        (int) thing[5]);
                    break;

                case "tex":
                    AddTex(content, assetIndex, (string) thing[1]);
                    break;

                case "sound":
                    AddSound(content, assetIndex, (string) thing[1]);
                    break;

                case "option":
                    LogAndText("Loading Option: " + thing[2]);
                    OptionsManager.AddOption((string) thing[1], (string) thing[2], (OptionType) thing[3], thing[4], root, thingsFuncThing[assetIndex]);
                    break;
                
                case "control":
                    LogAndText("Loading Button: " + thing[2]);
                    OptionsManager.AddOption("control_" + (Controls) assetIndex, (string) thing[2], OptionType.Key, thing[1], root, ControlChanged);
                    break;
                
                case "fonts":
                    LogAndText("Loading Fonts");
                    for (int i = 0; i < RRender.Fonts.Length; i++) {
                        RRender.Fonts[i] = content.Load<SpriteFont>("Fonts/Font" + i);
                    }

                    LoadingScene.canText = true;
                    break;
                
                case "libvlc":
                    LogAndText("Loading AudioManager");
                    AudioManager.Initialise();
                    Game1.StaticUpdateEvent += AudioManager.Update;
                    break;
                
                case "thingtools":
                    LogAndText("Loading Utilities");
                    ThingTools.Init();
                    break;

                case "end":
                    //thingsIndex = 0; assetIndex = 0; /*
                    Game1.Game.IsFixedTimeStep = (bool) OptionsManager.GetOption("limitfps").CurrentValue;
                    Game1.Graphics.ApplyChanges();
                    Game1.Game.PrepareLoadScene();
                    Game1.Game.LoadScene(new MainMenu()); /* */
                    return;
            }

            LoadingScene.progress = thingsIndex / (float) (things.Count - 1);
            
            thingsIndex++;
            assetIndex++;
        }

        public static void AddOptionThing(string a, string e, OptionType b, object c, Action<string, object> d = null) {
            things.Add(new List<object> {"option", a, e, b, c});
            thingsFuncThing.Add(d);
        }

        public static void Init() {
            Logger.Info("Loading Textures");

            Game1.UpdateEvent += Update;
            Game1.Game.IsFixedTimeStep = false;
            Game1.Graphics.ApplyChanges();

            content = Game1.Game.Content;
            root = OptionsManager.GetJson();

            textures = new Texture2D[30];
            sounds = new SoundEffect[1];
            controls = new Keys[18];

            things.Add(new List<object>() {"fonts"});
            
            things.Add(new List<object>() {"thingtools"});
            things.Add(new List<object>() {"libvlc"});

            things.Add(new List<object> {"reseti"});
            things.Add(new List<object> {"sheet", "notes", 0, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64 * 2, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64 * 3, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64 * 4, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64 * 5, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 0, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 2, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 3, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 4, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 5, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 0, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 2, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 3, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 4, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 5, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 0, 64 * 2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64, 64 * 2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 2, 64 * 2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 3, 64 * 2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 4, 64 * 2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64 * 5, 64 * 2, 64, 64});
            things.Add(new List<object> {"sheet", "healthbar", 0, 0, 400, 40});
            things.Add(new List<object> {"sheet", "healthbar", 0, 40, 400, 40});
            things.Add(new List<object> {"tex"  , "button"});
            things.Add(new List<object> {"sheet", "trackeditorbg", 0, 0, 96, 96});
            things.Add(new List<object> {"sheet", "trackeditorbg", 96, 0, 96, 96});
            things.Add(new List<object> {"tex"  , "arrow"});

            things.Add(new List<object> {"reseti"});
            things.Add(new List<object> {"sound", "hitsound"});
            
            things.Add(new List<object> {"reseti"});
            AddOptionThing("limitfps",         "Do Limit FPS",      OptionType.Boolean,  false,       OptionsActions.LimitFPSChanged   );
            AddOptionThing("fpslimit",         "FPS Limit",         OptionType.Switcher, "60 FPS",    OptionsActions.FPSLimitChanged   );
            AddOptionThing("resolution",       "Resolution",        OptionType.Switcher, "1920x1080", OptionsActions.ResolutionChanged );
            AddOptionThing("vsync",            "VSync",             OptionType.Boolean,  false,       OptionsActions.VSyncChanged      );
            AddOptionThing("fullscreen",       "Fullscreen",        OptionType.Boolean,  false,       OptionsActions.FullscreenChanged );
            AddOptionThing("downscroll",       "Downscroll",        OptionType.Boolean,  true         );
            AddOptionThing("sectionScrolling", "Section Scrolling", OptionType.Boolean,  false        );

            things.Add(new List<object> {"reseti"});
            things.Add(new List<object> {"control", Keys.S, "P1 Key 1"});
            things.Add(new List<object> {"control", Keys.D, "P1 Key 2"});
            things.Add(new List<object> {"control", Keys.F, "P1 Key 3"});
            things.Add(new List<object> {"control", Keys.J, "P1 Key 4"});
            things.Add(new List<object> {"control", Keys.K, "P1 Key 5"});
            things.Add(new List<object> {"control", Keys.L, "P1 Key 6"});
            
            things.Add(new List<object> {"control", Keys.W, "P2 Key 1"});
            things.Add(new List<object> {"control", Keys.E, "P2 Key 2"});
            things.Add(new List<object> {"control", Keys.R, "P2 Key 3"});
            things.Add(new List<object> {"control", Keys.U, "P2 Key 4"});
            things.Add(new List<object> {"control", Keys.I, "P2 Key 5"});
            things.Add(new List<object> {"control", Keys.O, "P2 Key 6"});
            
            things.Add(new List<object> {"control", Keys.Z, "P3 Key 1"});
            things.Add(new List<object> {"control", Keys.X, "P3 Key 2"});
            things.Add(new List<object> {"control", Keys.C, "P3 Key 3"});
            things.Add(new List<object> {"control", Keys.N, "P3 Key 4"});
            things.Add(new List<object> {"control", Keys.M, "P3 Key 5"});
            things.Add(new List<object> {"control", Keys.OemComma, "P3 Key 6"});

            things.Add(new List<object> {"end"});
        }
    }

    public enum Controls {
        P1Key1,
        P1Key2,
        P1Key3,
        P1Key4,
        P1Key5,
        P1Key6,
        
        P2Key1,
        P2Key2,
        P2Key3,
        P2Key4,
        P2Key5,
        P2Key6,
        
        P3Key1,
        P3Key2,
        P3Key3,
        P3Key4,
        P3Key5,
        P3Key6,
    }

    public enum Textures {
        Note1,
        Note2,
        Note3,
        Note4,
        Note5,
        Note6,
        
        Keys1,
        Keys2,
        Keys3,
        Keys4,
        Keys5,
        Keys6,
        Keys1Held,
        Keys2Held,
        Keys3Held,
        Keys4Held,
        Keys5Held,
        Keys6Held,
        Keys1Color,
        Keys2Color,
        Keys3Color,
        Keys4Color,
        Keys5Color,
        Keys6Color,

        HealthBar,
        HealthBarBackground,

        Button,
        
        TrackEditorBG,
        TrackEditorBGOther,
        
        Arrow
    }

    public enum Sounds {
        Hitsound
    }
}