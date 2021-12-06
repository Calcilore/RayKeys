using Microsoft.Xna.Framework.Graphics;
using RayKeys.Render;

namespace RayKeys.Editor {
    public class TrackEditor {
        private Texture2D keysTexture;
        private Texture2D notesTexture;
        private Texture2D background;
        
        public TrackEditor() {
            Game1.Game.DrawEvent += Draw;
            Game1.Game.UpdateEvent += Update;
            
            notesTexture = Game1.Game.Textures["notes"];
            keysTexture = Game1.Game.Textures["keys"];
            background = Game1.Game.Textures["trackeditorbg"];
        }
        
        private void Update(float delta) {
            
        }

        private void Draw(float delta) {
            RRender.Draw(Align.Center, Align.Bottom, background, -306, -1080, 0, 0, 580, 977);
            for (int i = 0; i < 6; i++) {
                int lPos = (i - 3) * 96;
                RRender.Draw(Align.Center, Align.Bottom, keysTexture, lPos, -200, i*64, 0, 64, 64);
            }
        }
    }
}