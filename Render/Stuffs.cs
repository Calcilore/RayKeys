using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RayKeys.Misc;
using RayKeys.Options;
using RayKeys.UI;

namespace RayKeys.Render {
    public static class Stuffs {
        private static Texture2D[] textures;
        private static SoundEffect[] sounds;

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
        private static int thingsIndex = 0;
        private static int assetIndex = 0;
        private static ContentManager content;

        private static void Update(float delta) {
            List<object> thing = things[thingsIndex];

            switch (thing[0]) {
                case "reseti":
                    assetIndex = -1;
                    break;
                
                case "sheet":
                    AddTexSht(content, assetIndex, (string) thing[1], (int) thing[2], (int) thing[3], (int) thing[4], (int) thing[5]);
                    break;
            
                case "tex":
                    AddTex(content, assetIndex, (string) thing[1]);
                    break;
            
                case "sound":
                    AddSound(content, assetIndex, (string) thing[1]);
                    break;
            
                case "end":
                    //thingsIndex = -1; /*
                    Game1.Game.IsFixedTimeStep = (bool) OptionsManager.GetOption("limitfps").currentValue;
                    Game1.Game.Graphics.ApplyChanges();
                    Game1.Game.PrepareLoadScene();
                    Game1.Game.LoadScene(new MainMenu());/* */
                    return;
            }

            LoadingScene.progress = thingsIndex / (float) (things.Count - 1);

            thingsIndex++;
            assetIndex++;
        }
        
        public static void Init() {
            Logger.Info("Loading Textures");

            Game1.Game.UpdateEvent += Update;
            Game1.Game.IsFixedTimeStep = false;
            Game1.Game.Graphics.ApplyChanges();

            content = Game1.Game.Content;

            textures = new Texture2D[29];
            sounds = new SoundEffect[1];
            
            things.Add(new List<object> {"reseti"});
            
            things.Add(new List<object> {"sheet", "notes", 0, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64*2, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64*3, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64*4, 0, 64, 64});
            things.Add(new List<object> {"sheet", "notes", 64*5, 0, 64, 64});
            
            things.Add(new List<object> {"sheet", "keys", 0, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*2, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*3, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*4, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*5, 0, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 0, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*2, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*3, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*4, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*5, 64, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 0, 64*2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64, 64*2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*2, 64*2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*3, 64*2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*4, 64*2, 64, 64});
            things.Add(new List<object> {"sheet", "keys", 64*5, 64*2, 64, 64});
            things.Add(new List<object> {"sheet", "healthbar", 0, 0, 400, 40});
            things.Add(new List<object> {"sheet", "healthbar", 0, 40, 400, 40});
            things.Add(new List<object> {"tex", "button"});
            things.Add(new List<object> {"sheet", "trackeditorbg", 0, 0, 96, 96});
            things.Add(new List<object> {"sheet", "trackeditorbg", 96, 0, 96, 96});
            
            things.Add(new List<object> {"reseti"});
            
            things.Add(new List<object> {"sound", "hitsound"});
            
            things.Add(new List<object> {"end"});
        }
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
        TrackEditorBGOther
    }

    public enum Sounds {
        Hitsound
    }
}