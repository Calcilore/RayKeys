using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RayKeys.Misc;

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
        
        private static void AddSound(ContentManager content, ref int i, string sound) {
            Logger.Info("Loading Sound " + (Textures)i);
            
            sounds[i] = content.Load<SoundEffect>("Sounds/" + sound);
            
            i++;
        }
        
        private static void AddTex(ContentManager content, ref int i, string tex) {
            Logger.Info("Loading Texture " + (Textures)i);
            
            textures[i] = content.Load<Texture2D>("Textures/" + tex);;
            
            i++;
        }

        private static void AddTexSht(ContentManager content, ref int i, string tex, int sourceX, int sourceY, int sizeX, int sizeY) {
            Logger.Info("Loading Sprite Sheet " + (Textures)i);
            
            Texture2D texture = content.Load<Texture2D>("Textures/" + tex);

            Rectangle newBounds = new Rectangle(sourceX, sourceY, sizeX, sizeY);
            
            Texture2D croppedTexture = new Texture2D(Game1.Game.GraphicsDevice, newBounds.Width, newBounds.Height);
            Color[] data = new Color[newBounds.Width * newBounds.Height];
            texture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            croppedTexture.SetData(data);

            textures[i] = croppedTexture;
            
            i++;
        }
        
        public static void Init(ContentManager content) {
            Logger.Info("Loading Textures");
            
            int i = 0;
            
            textures = new Texture2D[29];
            
            AddTexSht(content, ref i, "notes", 0, 0, 64, 64);
            AddTexSht(content, ref i, "notes", 64, 0, 64, 64);
            AddTexSht(content, ref i, "notes", 64*2, 0, 64, 64);
            AddTexSht(content, ref i, "notes", 64*3, 0, 64, 64);
            AddTexSht(content, ref i, "notes", 64*4, 0, 64, 64);
            AddTexSht(content, ref i, "notes", 64*5, 0, 64, 64);
            
            AddTexSht(content, ref i, "keys", 0, 0, 64, 64);
            AddTexSht(content, ref i, "keys", 64, 0, 64, 64);
            AddTexSht(content, ref i, "keys", 64*2, 0, 64, 64);
            AddTexSht(content, ref i, "keys", 64*3, 0, 64, 64);
            AddTexSht(content, ref i, "keys", 64*4, 0, 64, 64);
            AddTexSht(content, ref i, "keys", 64*5, 0, 64, 64);
            AddTexSht(content, ref i, "keys", 0, 64, 64, 64);
            AddTexSht(content, ref i, "keys", 64, 64, 64, 64);
            AddTexSht(content, ref i, "keys", 64*2, 64, 64, 64);
            AddTexSht(content, ref i, "keys", 64*3, 64, 64, 64);
            AddTexSht(content, ref i, "keys", 64*4, 64, 64, 64);
            AddTexSht(content, ref i, "keys", 64*5, 64, 64, 64);
            AddTexSht(content, ref i, "keys", 0, 64*2, 64, 64);
            AddTexSht(content, ref i, "keys", 64, 64*2, 64, 64);
            AddTexSht(content, ref i, "keys", 64*2, 64*2, 64, 64);
            AddTexSht(content, ref i, "keys", 64*3, 64*2, 64, 64);
            AddTexSht(content, ref i, "keys", 64*4, 64*2, 64, 64);
            AddTexSht(content, ref i, "keys", 64*5, 64*2, 64, 64);
            
            AddTexSht(content, ref i, "healthbar", 0, 0, 400, 40);
            AddTexSht(content, ref i, "healthbar", 0, 40, 400, 40);
            
            AddTex(content, ref i, "button");
            
            AddTexSht(content, ref i, "trackeditorbg", 0, 0, 96, 96);
            AddTexSht(content, ref i, "trackeditorbg", 96, 0, 96, 96);

            Logger.Info("Loading Sounds");
            i = 0;

            sounds = new SoundEffect[1];
            
            AddSound(content, ref i, "hitsound");
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