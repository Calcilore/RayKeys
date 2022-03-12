using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.UI;
using RayKeys.Misc;
using RayKeys.Options;
using RayKeys.Render;

namespace RayKeys.Editor {
    public class Editor : Scene {
        private const int XStart = 670;
        private const int XLen = 580;
        
        private float scrollPos;
        private int scrollPosR;

        private bool sectionScroll;
        private int sectionScrollAdd;

        private PauseMenu pauseMenu;
        private Menu menu;
        private bool shouldBePaused; // music
        private bool isPaused;

        private string infoText;
        private float infoTextTimer;
        private const float infoTextTimerMax = 2f;
        
        // Sections
        private int currentSection;
        private List<List<Note>> sections = new List<List<Note>>(); // list of sections, each section is a list of notes
        private List<Note> csNotes = new List<Note>();

        // Zoom
        private int zoom;
        private int backgroundSize;
        
        // Song Stats
        private float bpm = 100;
        private string levelName = "";
        private string songName = "";
        private string artist = "";

        public Editor() {
            Logger.Info("Loading Editor");
            
            Game1.Game.DrawEvent += Draw;
            Game1.Game.UpdateEvent += Update;

            AudioManager.LoadSong("Levels/1/song.ogg", 100);
            AudioManager.Play();
            AudioManager.SetPause(true);
            shouldBePaused = true;

            sectionScroll = (bool) OptionsManager.GetOption("sectionScrolling").currentValue;
            sectionScrollAdd = sectionScroll ? 1 : 0;
            
            sections.Add(new List<Note>());
            ChangeZoom(1);

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
            ((InputField) menu.pages[^1].Items[^1]).cursorPos = 3;
        }
        
        private void DoTheNoteShit() {
            int clickY = RMouse.Y;

            float cPosU = (-clickY + 1080f) / 96f - 1;
            float cPos = (int)cPosU;
            byte cLane =  (byte)(((float)RMouse.X - XStart - 4f) / 96f);

            if (cPosU < 0) cPos--; // idk why
             
            Logger.Debug($"Note Clicked At: Mouse: {RMouse.Y}, CPosU: {cPosU}, CPos: {cPos}");
            
            foreach (Note note in csNotes) {
                if (Math.Abs(cPosU - note.time - 0.5f) <= 0.5f && note.lane == cLane) {
                    Logger.Debug($"Removing note at lane {note.lane} and time {note.time}");
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
                // Scrolling
                scrollPosR = Math.Clamp(
                    scrollPosR + (RMouse.ScrollFrame == 0 ? 0 : RMouse.ScrollFrame > 0 ? 96 : -96) *
                    (RKeyboard.IsKeyHeld(Keys.LeftShift) ? 4 : 1), -sectionScrollAdd, backgroundSize * 96 + sectionScrollAdd);
                
                // Scroll to change section
                if (sectionScroll) {
                    if (scrollPosR > backgroundSize * 96) {
                        scrollPosR = 0;
                        ChangeSection(currentSection + 1);
                    } else if (scrollPosR < 0 && currentSection > 0) {
                        scrollPosR = backgroundSize * 96 * zoom;
                        ChangeSection(currentSection - 1);
                    }
                }

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
            }

            if (RKeyboard.IsKeyPressed(Keys.Space)) {
                if (AudioManager.IsPlaying()) {
                    AudioManager.SetPause(true);
                    shouldBePaused = true;
                    scrollPosR = (int) ThingTools.RoundN(scrollPos, 96);
                }
                else {
                    AudioManager.SetPause(false);
                    shouldBePaused = false;
                    // making it start at 0 seconds causes it to lagspike, so i made it seek to a minimum of 1 millisecond
                    AudioManager.Seek(Math.Max(0.001f, (scrollPos / 96 / zoom + currentSection * 16) / AudioManager.bps));
                }
            }

            if (RKeyboard.IsKeyPressed(Keys.OemCloseBrackets) && zoom < 8) {
                ChangeZoom(zoom + 1);
            }
            else if (RKeyboard.IsKeyPressed(Keys.OemOpenBrackets) && zoom > 1) {
                ChangeZoom(zoom - 1);
            }
            
            if (RKeyboard.IsKeyPressed(Keys.Right) || RKeyboard.IsKeyPressed(Keys.D)) {
                ChangeSection(currentSection + 1);
            }
            else if (RKeyboard.IsKeyPressed(Keys.Left) || RKeyboard.IsKeyPressed(Keys.A)) {
                ChangeSection(currentSection - 1);
            }
            
            if (RKeyboard.IsKeyPressed(Keys.R)) {
                scrollPosR = 0;
            }

            if (RKeyboard.IsKeyPressed(Keys.S) && RKeyboard.IsKeyHeld(Keys.LeftControl)) {
                Logger.Info("Saving Level");

                JsonFileThing jf = new JsonFileThing() {
                    name = levelName,
                    songName = songName,
                    artist = artist,
                    bpm = bpm,
                    bps = AudioManager.bps,
                    players = new List<Player>() {new Player() {beatmap = 1, controls = 1}},
                    beatmaps = new List<List<List<Note>>>() { sections }
                };
                
                SongJsonManager.SaveJson(levelName, jf);

                infoText = "Level Saved...";
                infoTextTimer = infoTextTimerMax;
            }
        }

        private void ChangeZoom(int set) {
            Logger.Info($"Changing Zoom to: {set} from {zoom}");

            backgroundSize = set * 16;
            
            foreach (Note t in csNotes) {
                t.time = t.time / zoom * set;
            }

            zoom = set;
        }
        
        private void ChangeSection(int section) {
            if (section < 0) return;

            Logger.Info($"Changing Section to: {section} from {currentSection}");

            int zoomBackup = zoom;
            ChangeZoom(1);
            sections[currentSection] = csNotes;
            currentSection = section;

            if (currentSection >= sections.Count) sections.Add(new List<Note>());
            
            csNotes = sections[currentSection];
            
            ChangeZoom(zoomBackup);
        }

        private void Draw(float delta) {
            //int r96p = (int) ThingTools.FloorN(scrollPos, 96);

            // background
            RRender.DrawTileUp(Align.Center, Align.Top, Textures.TrackEditorBG, -288, 888, 96, 96, 6, backgroundSize, Color.White);

            { // Numbers to show where you are placing your notes and how the zoom is affecting it
                for (int i = 0; i < backgroundSize; i++) {
                    string numText = ThingTools.Floor(i / (float)zoom + 1, 2).ToString();
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
            
            RRender.DrawStringNoCam(Align.Left, Align.Top, Align.Left, Align.Top, 
                "Space: Play / Pause\nClick: Place / Remove\nArrow Keys / A & D: Change Section\n[ & ]: Change Zoom\nR: Goto start of section\nCtrl+S: Save"
                , 5, 5, 5, Color.White);

            if (infoTextTimer > 0) {
                RRender.DrawStringNoCam(Align.Right, Align.Top, Align.Right, Align.Top, infoText, -5, 5, 4, Color.White);
                
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
   
            AudioManager.bpm = bpm;
            AudioManager.bps = bpm / Engine.BeatMultiplier;
        }
    }
}