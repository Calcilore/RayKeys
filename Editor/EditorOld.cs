using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.Editor {
    public class EditorOld : Scene {
        private Texture2D keysTexture;
        private Texture2D notesTexture;
        private Texture2D background;
        private const int XStart = 670;
        private const int XLen = 580;
        
        public float scrollPos;
        private int scrollPosR;
        public List<Note> notes = new List<Note>();
        private int LastZoomU = 1;
        private int zoomU = 1; // thing for zoom input, not used by anything apart from setting zoom
        private float zoom = 1; // the zoom

        private Engine playTest;

        public EditorOld() {
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
            int clickY = RMouse.Y;

            float cPosU = (-clickY + 1080f) / 96f - 1;
            float cPos = (int)cPosU;
            byte cLane =  (byte)(((float)RMouse.X - XStart - 4f) / 96f);

            cPosU += scrollPosR / 96f; cPos += scrollPosR / 96f;

            foreach (Note note in notes) {
                Note n = new Note(note.time, note.lane); // copy note
                n.time *= zoom; // make it so that i dont have to think about zoom

                if (Math.Abs(cPosU - n.time - 0.5f) <= 0.5f && n.lane == cLane) {
                    notes.Remove(note);
                    return;
                }
            }
            
            // change the zoom before adding it
            cPosU /= zoom; cPos /= zoom;

            if (cPos < 0) return;
            Note na = new Note(cPos, cLane);
            notes.Add(na);
        }

        private void Update(float delta) {
            if (float.IsNaN(scrollPos)) scrollPos = 0; // one time the scroll pos was infinity and NaN, so i added this, I also added infinity to the spritefonts.
            
            scrollPosR = Math.Max(scrollPosR + (RMouse.ScrollFrame == 0 ? 0 : RMouse.ScrollFrame > 0 ? 96 : -96) * (RKeyboard.IsKeyHeld(Keys.LeftShift) ? 4 : 1), 0);
            scrollPos = ThingTools.Lerp(scrollPos, scrollPosR, 10f * delta);

            if (!AudioManager.IsPlaying()) {
                if (RMouse.LeftButtonPressed && RMouse.X > XStart && RMouse.X < XStart + XLen)
                    DoTheNoteShit();
                
                if (RKeyboard.IsKeyPressed(Keys.OemPlus)) {
                    zoomU++;
                    CalculateZoom();
                } else if (RKeyboard.IsKeyPressed(Keys.OemMinus) && zoomU > 0) {
                    zoomU--;
                    CalculateZoom();
                    //zoom = Math.Max(1, zoom - 1);
                } else if (RKeyboard.IsKeyPressed(Keys.D0)) {
                    zoomU = 1;
                    CalculateZoom();
                    //zoom = Math.Max(1, zoom - 1);
                }
            } 
            else { // if music is playing, scroll to position in song, overrides user input
                scrollPosR = (int) (AudioManager.GetBeatTime() * 96 * zoom);
                scrollPos = scrollPosR;
            }

            if (RKeyboard.IsKeyPressed(Keys.Space)) {
                if (AudioManager.IsPlaying()) {
                    AudioManager.SetPause(true);
                    scrollPos = ThingTools.RoundN(scrollPos, 96);
                    scrollPosR = (int) scrollPos;

                    Console.WriteLine($"{scrollPos}, {scrollPosR}");
                }
                else {
                    AudioManager.SetPause(false);
                    // making it start at 0 seconds causes it to lagspike, so i made it seek to a minimum of 1 millisecond
                    AudioManager.Seek(Math.Max(0.001f, scrollPos / (96 * zoom) / AudioManager.bps)); 
                }
            }
            
            if (RKeyboard.IsKeyPressed(Keys.H)) {
                Game1.Game.UpdateEvent -= Update;
                Game1.Game.UpdateEvent += PlayTestUpdate;
                Game1.Game.DrawEvent -= Draw;

                playTest = new Engine(1);
                foreach (Note note in notes) {
                    Note n = new Note(note.time / AudioManager.bps, note.lane);
                    playTest.notes.Add(n);
                }

                AudioManager.SetPause(false);
                AudioManager.Seek(0.001f); 
                
                playTest.Start();
            }
        }

        private void CalculateZoom() {
            scrollPos /= 96f;
            scrollPos = ThingTools.Round(scrollPos);
            
            if (zoomU > 0 && LastZoomU > 0) {
                zoom = zoomU;
                
                scrollPos *= (float)zoomU / LastZoomU;
            }
            else { // 1 -> 0   0 -> 1
                if (zoomU > LastZoomU) {
                    zoom = 1;
                
                    scrollPos *= 2;
                } else {
                    zoom = 0.5f;
                
                    scrollPos /= 2;
                }
            }

            scrollPos = (int) scrollPos * 96;
            scrollPosR = (int) scrollPos;

            LastZoomU = zoomU;
        }
        
        private void Draw(float delta) {
            int sp = (int) scrollPos;
            int r96p = (int) ThingTools.FloorN(scrollPos, 96);

            // background
            RRender.Draw(Align.Center, Align.Top, background, -background.Width / 2 + 48, -168 + sp - r96p, 0, 0, 676, 1344);
            
            // the keys
            for (int i = 0; i < 6; i++) {
                int lPos = (i - 3) * 96;
                RRender.Draw(Align.Center, Align.Bottom, keysTexture, lPos + 16, -96*2+16 + sp, i*64, 0, 64, 64);
            }
            
            // the numbers to make zoom easier 
            {
                float n = (float)(int)(scrollPos / 96f) / zoom - 1 / zoom;
                for (int i = 0; i < 12; i++, n += 1 / zoom) {
                    int lPos = (int) (i * -96 - 48);
                    //((float)(int)(n * 10) / 10)
                    string numText = ThingTools.Floor(n, 2).ToString();
                    RRender.DrawString(Align.Center, Align.Bottom, Align.Right, Align.Center, numText, -300, lPos + sp - r96p, 5);
                }
            }

            // the play line thingo
            RRender.DrawBlank(Align.Left, Align.Bottom, 0, -96*2-2 + 48, 1920, 4, Color.White);
            
            // the notes
            foreach (Note n in notes) {
                // if (n.dead)     // This if does nothing rn, might uncomment later when i add more features
                //     continue;

                RRender.Draw(Align.Center, Align.Bottom, notesTexture, (n.lane-3)*96 + 16, (int) (n.time * -96 * zoom) + 16 - 96*2 + sp, n.lane*64, 0, 64, 64);
                //RRender.DrawString(Align.Center, Align.Bottom, Align.Center, Align.Center, n.lane.ToString(), (n.lane-3)*96 + 16, (int) ((n.time + 1) * -96) + 16 + sp, 4);
                //RRender.DrawString(Align.Center, Align.Bottom, Align.Center, Align.Center, n.time.ToString(), (n.lane-3)*96 + 16, (int) (n.time * -96 * zoom) + 16 - 96*2 + sp, 4);
                //RRender.DrawString(Align.Center, Align.Bottom, Align.Center, Align.Center, (n.time * zoom).ToString(), (n.lane-3)*96 + 64, (int) (n.time * -96 * zoom) + 64 - 96*2 + sp, 4);
            }
            
            RRender.DrawString(Align.Left, Align.Bottom, Align.Left, Align.Bottom, $"Zoom: {ThingTools.Floor(1 / zoom, 3)}\nScroll Pos: {scrollPosR}", 5, -5, 5);
        }
        
        // PLAY TEST SECTION
        // PLAY TEST SECTION
        // PLAY TEST SECTION
        // PLAY TEST SECTION
        
        private void PlayTestUpdate(float delta) {
            if (RKeyboard.IsKeyPressed(Keys.H)) {
                Game1.Game.UpdateEvent -= PlayTestUpdate;
                Game1.Game.UpdateEvent += Update;
                Game1.Game.DrawEvent += Draw;
            }
        }
    }
}