using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.UI;
using RayKeys.Misc;
using RayKeys.Render;

namespace RayKeys.Editor {
    public class Editor : Scene {
        private const int XStart = 670;
        private const int XLen = 580;
        
        private float scrollPos;
        private int scrollPosR;
        private int barPos;

        private PauseMenu pauseMenu;
        private Menu menu;
        private bool shouldBePaused; // music
        private bool isPaused;

        private string infoText;
        private float infoTextTimer;
        private const float infoTextTimerMax = 2f;
        
        // Sections
        private int currentSection;
        private List<List<Note>> notes = new List<List<Note>>();
        private List<Note> csNotes = new List<Note>();

        // Zoom
        private int zoom;
        private int backgroundSize;
        
        // Song Stats
        private string file;
        
        private float bpm = 100;
        private string levelName = "";
        private string songName = "";
        private string artist = "";

        public Editor() {
            Logger.Info("Loading Editor");
            
            Game1.Game.DrawEvent += Draw;
            Game1.Game.UpdateEvent += Update;

            AudioManager.LoadSong("Levels/1/song.ogg", 100*2);
            AudioManager.Play();
            AudioManager.SetPause(true);
            shouldBePaused = true;
            
            notes.Add(new List<Note>());
            ChangeZoom(true);

            pauseMenu = new PauseMenu(-50);
            ((Button) pauseMenu.menu.pages[0].Items[0]).args = new object[] {2, true};
            pauseMenu.PauseEvent += OnPause;
            pauseMenu.UnPauseEvent += OnUnPause;
            
            pauseMenu.AddFunctionCallButton(ExitButton, "Exit");

            menu = pauseMenu.menu;
            menu.AddPage(0, 0, true);
            menu.AddFunctionCallInputField(2, OnLevelNameChange, Align.Right, Align.Top,  "Level Name", -16, 0);
            menu.AddFunctionCallInputField(2, OnSongNameChange, Align.Right, Align.Top,  "Song Name", -16, 150);
            menu.AddFunctionCallInputField(2, OnArtistNameChange, Align.Right, Align.Top,  "Artist", -16, 300);
            menu.AddFunctionCallInputField(2, OnBPMChange, Align.Right, Align.Top,  "BPM", -16, 450);
            ((InputField) menu.pages[^1].Items[^1]).Text = "100";
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
            Logger.Debug($"Placing note at lane {cLane} and time {cPos}");
            Note na = new Note(cPos, cLane);
            csNotes.Add(na); 
        }

        private void Update(float delta) {
            if (isPaused) return;
            
            RRender.CameraPos.Y = -scrollPos;

            if (!AudioManager.IsPlaying()) {
                scrollPosR = Math.Clamp(
                        scrollPosR + (RMouse.ScrollFrame == 0 ? 0 : RMouse.ScrollFrame > 0 ? 96 : -96) *
                        (RKeyboard.IsKeyHeld(Keys.LeftShift) ? 4 : 1), 0, backgroundSize * 96);

                // Scroll to change section
                // scrollPosR = Math.Clamp(
                //     scrollPosR + (RMouse.ScrollFrame == 0 ? 0 : RMouse.ScrollFrame > 0 ? 96 : -96) *
                //     (RKeyboard.IsKeyHeld(Keys.LeftShift) ? 4 : 1), -1, backgroundSize * 96 + 1);
                //
                // if (scrollPosR > backgroundSize * 96) {
                //     scrollPosR = 0;
                //     ChangeSection(currentSection + 1);
                // } else if (scrollPosR < 0) {
                //     scrollPosR = backgroundSize * 96 * zoom;
                //     ChangeSection(currentSection - 1);
                // }

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
                    shouldBePaused = true;
                    scrollPos = ThingTools.RoundN(scrollPos, 96);
                    scrollPosR = (int) scrollPos;

                    Console.WriteLine($"{scrollPos}, {scrollPosR}");
                }
                else {
                    AudioManager.SetPause(false);
                    shouldBePaused = false;
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
            
            if (RKeyboard.IsKeyPressed(Keys.Right)) {
                ChangeSection(currentSection + 1);
            }
            else if (RKeyboard.IsKeyPressed(Keys.Left)) {
                ChangeSection(currentSection - 1);
            }

            if (RKeyboard.IsKeyPressed(Keys.S) && RKeyboard.IsKeyHeld(Keys.LeftControl)) {
                Logger.Info("Saving Level");
                
                Dictionary<string, object> thing = new Dictionary<string, object>();
                
                thing.Add("name", levelName);
                thing.Add("songName", songName);
                thing.Add("artist", artist);
                thing.Add("bpm", bpm / 2);
                thing.Add("players", new List<Dictionary<string, int>>() {new Dictionary<string, int>() {{"beatmap", 1}, {"controls", 1}}});

                List<Dictionary<string, float>> dick = new List<Dictionary<string, float>>();
                float sAdd = 0;
                foreach (List<Note> section in notes) {
                    foreach (Note note in section) {
                        dick.Add(new Dictionary<string, float>() {{"lane", note.lane}, {"time", (note.time + sAdd)/2}});
                    }

                    sAdd += 16;
                }
                
                thing.Add("beatmaps", new List<List<Dictionary<string, float>>>() {dick});
                
                string json = JsonSerializer.Serialize(thing);
                Directory.CreateDirectory($"Content/Levels/{levelName}/");
                File.WriteAllText($"Content/Levels/{levelName}/song.json", json);

                infoText = "Level Saved...";
                infoTextTimer = infoTextTimerMax;
            }
        }

        private void ChangeZoom(bool up) {
            zoom += up ? 1 : -1;
            Logger.Info("Changing Zoom to: " + zoom);

            backgroundSize = zoom * 16;

            float mul = up ? 2f : 0.5f;
            for (int i = 0; i < csNotes.Count; i++) {
                csNotes[i].time *= mul;
            }
        }
        
        private void ChangeSection(int section) {
            if (section < 0) return;

            Logger.Info($"Changing Section to: {currentSection} from {section}");
            
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
            RRender.DrawBlank(Align.Left, Align.Bottom, 0, (int) RRender.CameraPos.Y - 96, 1920, 4, Color.White * 0.5f);
            
            // the notes
            foreach (Note n in csNotes) {
                // if (n.dead)     // This if does nothing rn, might uncomment later when i add more features
                //     continue;
                
                RRender.Draw(Align.Center, Align.Bottom, Stuffs.GetTexture((int)Textures.Note1 + n.lane), (n.lane-3)*96 + 16, (int) (n.time * -96) + 16 - 96*2, 64, 64, Color.White);
                // RRender.DrawString(Align.Center, Align.Bottom, Align.Left, Align.Bottom, n.lane.ToString(), (n.lane-3)*96 + 16, (int) (n.time * -96 - 96*1.5f), 4); // Debugs
                // RRender.DrawString(Align.Center, Align.Bottom, Align.Left, Align.Top   , n.time.ToString(), (n.lane-3)*96 + 64, (int) (n.time * -96 - 96*1.5f), 4);
            }
            
            RRender.DrawStringNoCam(Align.Left, Align.Bottom, Align.Left, Align.Bottom, $"Zoom: {zoom}\nSection: {currentSection + 1}", 5, -5, 5, Color.White);

            if (infoTextTimer > 0) {
                RRender.DrawStringNoCam(Align.Left, Align.Top, Align.Left, Align.Top, infoText, 5, 5, 4, Color.White);
                
                infoTextTimer -= delta;
            }
        }

        private void ExitButton() {
            Game1.Game.PrepareLoadScene();
            Game1.Game.LoadScene(new MainMenu());
        }

        private void OnPause() {
            isPaused = true;
            AudioManager.SetPause(true);
        }

        private void OnUnPause() {
            isPaused = false;
            AudioManager.SetPause(shouldBePaused);
        }

        private void OnLevelNameChange(string text) {
            levelName = text;
        }
        
        private void OnSongNameChange(string text) {
            songName = text;
        }
        
        private void OnArtistNameChange(string text) {
            artist = text;
        }
        
        private void OnBPMChange(string text) {
            if (!float.TryParse(text, out bpm)) return;
            bpm *= 2;

            AudioManager.bpm = bpm;
            AudioManager.bps = bpm / 60f;
        }
    }
}