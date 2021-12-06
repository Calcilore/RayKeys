using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.Editor {
    public class Editor : Scene {
        private Texture2D keysTexture;
        private Texture2D notesTexture;
        private Texture2D background;
        private const int XStart = 670;
        private const int XLen = 580;
        
        public float scrollPos;
        private int scrollPosR;
        public List<Note> notes = new List<Note>();
        private int zoom = 1; 

        public Editor() {
            Game1.Game.DrawEvent += Draw;
            Game1.Game.UpdateEvent += Update;

            notesTexture = Game1.Game.Textures["notes"];
            keysTexture = Game1.Game.Textures["keys"];
            background = Game1.Game.Textures["trackeditorbg"];
            
            AudioManager.LoadSong("Levels/1/song.ogg", 340);
            AudioManager.Play();
            AudioManager.SetPause(true);
        }

        private void DoTheNoteShit() {
            // TODO: make the zoom work!!!!!!!!!!!!
            int clickY = RMouse.Y;

            float cPosU = (-clickY + 1080f) / 96f;
            float cPos = (int)cPosU;
            byte cLane =  (byte)(((float)RMouse.X - XStart - 4f) / 96f);

            cPosU += scrollPosR / 96f; cPos += scrollPosR / 96f;
            cPosU /= zoom; cPos /= zoom;
            cPosU -= 1f / zoom; cPos -= 1f / zoom;

            Console.WriteLine($"{cPosU}, {cPos}, {scrollPosR}");

            foreach (Note note in notes) {
                if (Math.Abs(cPosU - note.time - 0.5f / zoom) <= 0.5f / zoom && note.lane == cLane) {
                    notes.Remove(note);
                    return;
                }
            }
                    
            //if (cPos < 1) return;
            Note n = new Note(cPos, cLane);
            notes.Add(n);
        }

        private void Update(float delta) {
            scrollPosR += (RMouse.ScrollFrame == 0 ? 0 : RMouse.ScrollFrame > 0 ? 96 : -96) * (RKeyboard.IsKeyHeld(Keys.LeftShift) ? 4 : 1);
            scrollPos = ThingTools.Lerp(scrollPos, scrollPosR, 10f * delta);

            if (!AudioManager.IsPlaying() && RMouse.LeftButtonPressed && RMouse.X > XStart && RMouse.X < XStart + XLen) {
                DoTheNoteShit();
            }
            
            if (AudioManager.IsPlaying()) { // if music is playing, scroll to position in song, overrides user input
                scrollPosR = (int) (AudioManager.GetBeatTime() * 96);
                scrollPos = scrollPosR;
            }

            if (RKeyboard.IsKeyPressed(Keys.Space)) {
                if (AudioManager.IsPlaying()) {
                    AudioManager.SetPause(true);
                    scrollPosR = (int) (scrollPosR / 96f) * 96;
                    scrollPos = scrollPosR;
                    
                    Console.WriteLine($"{scrollPos}, {scrollPosR}");
                }
                else {
                    AudioManager.SetPause(false);
                    // making it start at 0 seconds causes it to lagspike, so i made it seek to a minimum of 1 millisecond
                    AudioManager.Seek(Math.Max(0.001f, scrollPos / 96 / AudioManager.bps)); 
                }
            }
        }
        
        private void Draw(float delta) {
            int sp = (int) scrollPos;

            // background
            RRender.Draw(Align.Center, Align.Top, background, -background.Width / 2, -168 + sp - (int)(scrollPos / 96f) * 96, 0, 0, 580, 1344);
            
            // the keys
            for (int i = 0; i < 6; i++) {
                int lPos = (i - 3) * 96;
                RRender.Draw(Align.Center, Align.Bottom, keysTexture, lPos + 16, -96*2+16 + sp, i*64, 0, 64, 64);
            }

            // the play line thingo
            RRender.DrawBlank(Align.Left, Align.Bottom, 0, -96*2-2 + 48, 1920, 4, Color.White);
            
            // the notes
            foreach (Note n in notes) {
                if (n.dead)
                    continue;

                RRender.Draw(Align.Center, Align.Bottom, notesTexture, (n.lane-3)*96 + 16, (int) ((n.time + 1) * -96 * zoom) + 16 + sp, n.lane*64, 0, 64, 64);
                //RRender.DrawString(Align.Center, Align.Bottom, Align.Center, Align.Center, n.lane.ToString(), (n.lane-3)*96 + 16, (int) ((n.time + 1) * -96) + 16 + sp, 4);
                //RRender.DrawString(Align.Center, Align.Bottom, Align.Center, Align.Center, n.time.ToString(), (n.lane-3)*96 + 16, (int) ((n.time + 1) * -96) + 64 + sp, 4);
            }
        }
    }
}