using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.Editor {
    public class Editor : Scene {
        private const int XStart = 670;
        private const int XLen = 580;
        
        private float scrollPos;
        private int scrollPosR;
        private int barPos;
        
        // Sections
        private int currentSection;
        private List<List<Note>> notes = new List<List<Note>>();
        private List<Note> csNotes = new List<Note>();

        // Zoom
        private int zoom;
        private int backgroundSize;

        public Editor() {
            Game1.Game.DrawEvent += Draw;
            Game1.Game.UpdateEvent += Update;

            AudioManager.LoadSong("Levels/1/song.ogg", 340);
            AudioManager.Play();
            AudioManager.SetPause(true);
            
            notes.Add(new List<Note>());
            ChangeZoom(true);
        }
        
        private void DoTheNoteShit() {
            int clickY = RMouse.Y;

            float cPosU = (-clickY + 1080f) / 96f - 1;
            float cPos = (int)cPosU;
            byte cLane =  (byte)(((float)RMouse.X - XStart - 4f) / 96f);

            if (cPosU < 0) cPos--; // idk why
             
            Console.WriteLine($"{RMouse.Y} {cPosU} {cPos}");
            
            foreach (Note note in csNotes) {
                if (Math.Abs(cPosU - note.time - 0.5f) <= 0.5f && note.lane == cLane) {
                    csNotes.Remove(note);
                    return;
                }
            }

            if (cPos < -1) return;
            Note na = new Note(cPos, cLane);
            csNotes.Add(na); 
        }

        private void Update(float delta) {
            RRender.CameraPos.Y = -scrollPos;

            if (!AudioManager.IsPlaying()) {
                scrollPosR = Math.Clamp(
                        scrollPosR + (RMouse.ScrollFrame == 0 ? 0 : RMouse.ScrollFrame > 0 ? 96 : -96) *
                        (RKeyboard.IsKeyHeld(Keys.LeftShift) ? 4 : 1), 0, backgroundSize * 96);
                scrollPos = ThingTools.Lerp(scrollPos, scrollPosR, 10f * delta);
                
                if (RMouse.LeftButtonPressed && RMouse.X > XStart && RMouse.X < XStart + XLen)
                    DoTheNoteShit();
            }
            else {
                // if music is playing, scroll to position in song, overrides user input
                scrollPosR = (int) ((AudioManager.GetBeatTime() - currentSection * 16) * 96);
                if (scrollPosR > 16 * 96) {
                    scrollPosR = 0;
                    ChangeSection(currentSection + 1);
                }

                scrollPosR *= zoom;
                scrollPos = scrollPosR;
                
                barPos = -scrollPosR - 96;
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
                    AudioManager.Seek(Math.Max(0.001f, (scrollPos / 96 / zoom + currentSection * 16) / AudioManager.bps));
                }
            }

            if (RKeyboard.IsKeyPressed(Keys.OemCloseBrackets) && zoom < 8) {
                ChangeZoom(true);
            }
            else if (RKeyboard.IsKeyPressed(Keys.OemOpenBrackets) && zoom > 1) {
                ChangeZoom(false);
            }
            
            if (RKeyboard.IsKeyPressed(Keys.Right) && currentSection < 16) {
                ChangeSection(currentSection + 1);
            }
            else if (RKeyboard.IsKeyPressed(Keys.Left) && currentSection > 0) {
                ChangeSection(currentSection - 1);
            }
        }

        private void ChangeZoom(bool up) {
            zoom += up ? 1 : -1;
            Console.WriteLine("Changing Zoom to: " + zoom);

            backgroundSize = zoom * 16;

            float mul = up ? 2f : 0.5f;
            for (int i = 0; i < csNotes.Count; i++) {
                csNotes[i].time *= mul;
            }
        }
        
        private void ChangeSection(int section) {
            Console.WriteLine($"Changing Section to: {currentSection} from {section}");
            
            notes[currentSection] = csNotes;
            currentSection = section;

            if (currentSection >= notes.Count) notes.Add(new List<Note>());
            
            csNotes = notes[currentSection];
        }

        private void Draw(float delta) {
            //int r96p = (int) ThingTools.FloorN(scrollPos, 96);

            // background
            RRender.DrawTileUp(Align.Center, Align.Top, Textures.TrackEditorBG, -288, 888, 96, 96, 6, backgroundSize, Color.White);

            { // Numbers to show where you are placing your notes and how the zoom is affecting it
                for (int i = 0; i < backgroundSize; i++) {
                    string numText = ThingTools.Floor((i + 0f) / (float)Math.Pow(2, zoom - 1) + 1, 2).ToString();
                    RRender.DrawString(Align.Center, Align.Bottom, Align.Right, Align.Center, numText, -300, i * -96 - 144, 5, Color.White);
                }
            }
            
            // the play line thingo
            RRender.DrawBlank(Align.Left, Align.Bottom, 0, (int) RRender.CameraPos.Y - 96, 1920, 4, new Color(255, 255, 255, 0));
            
            // the notes
            foreach (Note n in csNotes) {
                // if (n.dead)     // This if does nothing rn, might uncomment later when i add more features
                //     continue;
                
                RRender.Draw(Align.Center, Align.Bottom, Stuffs.GetTexture((int)Textures.Note1 + n.lane), (n.lane-3)*96 + 16, (int) (n.time * -96) + 16 - 96*2, 64, 64, Color.White);
                //RRender.DrawString(Align.Center, Align.Bottom, Align.Center, Align.Center, n.lane.ToString(), (n.lane-3)*96 + 16, (int) ((n.time + 1) * -96) + 16 + sp, 4);
                //RRender.DrawString(Align.Center, Align.Bottom, Align.Center, Align.Center, n.time.ToString(), (n.lane-3)*96 + 16, (int) (n.time * -96) + 16 - 96*2 + sp, 4);
            }
            
            RRender.DrawStringNoCam(Align.Left, Align.Bottom, Align.Left, Align.Bottom, $"Zoom: {zoom}\nSection: {currentSection + 1}", 5, -5, 5, Color.White); 
        }
    }
}