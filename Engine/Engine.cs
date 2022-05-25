using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Options;
using RayKeys.Render;

namespace RayKeys {
    /*
     *        NN
     *           NN
     *
     *     NN <-- Notes
     *
     *
     *  [] [] [] [] [] []  <-- Keys
     */
    public class Engine {
        public static float BeatMultiplier = 30f;
        
        private Action<Note, float, float> NoteHandler; // it might be more efficient to use this stupid action instead of 1 if statement in a for loop

        private int controls;
        private bool autoPlay;
        private float[] autoPlayKeyTimer = new float[6];
        private int xpos;
        private float speed;
        private int downscrollMul;
        private bool downscroll;
        private float countdownTimer;

        private bool[] keysHeld = new bool[6];      // currently held keys
        private bool[] keysHeld2 = new bool[6];     // keys held last frame and keys that were tapped (changes during the frame)
        private bool[] keysHeldOnHit = new bool[6]; // when you hit a note the keys change to the 

        public List<Note> notes = new List<Note>();
        public float health = 1f;
        private float healthD = 0f; // Health Display (health with ease)

        private bool shouldBePaused; // music
        private bool isPaused;

        private string rating = "";
        private float ratingTimer = 0f;
        private const float ratingTimerMax = 1f;
        
        public Engine(int controls, int xpos = 0, float countdownTimer = 3f, float speed = 1f) {
            controls--;
            autoPlay = controls == -1;
            this.controls = controls * 6;

            if (autoPlay) NoteHandler = AutoPlayNoteHandling;
            else          NoteHandler = NoAutoPlayNoteHandling;

            this.xpos = xpos;
            this.speed = speed;
            this.countdownTimer = countdownTimer + .001f;
            
            Game1.DrawEvent += Draw;

            downscroll = (bool) OptionsManager.GetOption("downscroll").CurrentValue;
            downscrollMul = downscroll ? 1 : -1;
            
            AudioManager.SetPause(true);
            shouldBePaused = true;
        }

        public void Start() {
            Game1.UpdateEvent += Update;
        }

        public void Pause() {
            AudioManager.SetPause(true);
            this.isPaused = true;
        }

        public void UnPause() {
            AudioManager.SetPause(shouldBePaused);
            this.isPaused = false;
        }

        private void NoAutoPlayNoteHandling(Note n, float np0, float npp) {
            if (keysHeld2[n.lane]) {
                float time = Math.Abs(np0);

                if (time < 0.15) { // if hit note
                    // rating non-specific things
                    n.dead = true;
                    keysHeldOnHit[n.lane] = true;

                    ratingTimer = ratingTimerMax;
                    
                    if (time < 0.03) { // rating specific things
                        rating = "Gaming";
                        health += 0.06f;
                    } else if (time < 0.06) {
                        rating = "Good";
                        health += 0.04f;
                    } else if (time < 0.1) {
                        rating = "Ok";
                        health += 0.02f;
                    } else {
                        rating = "Bad";
                        health += 0.01f;
                    }
                }
            }
            
            if (Math.Abs(np0) < 0.1 && keysHeld2[n.lane]) { // player hit note
                n.dead = true;
                keysHeldOnHit[n.lane] = true;

                health += 0.05f;
            }
                
            if (np0 < 0 && npp >= 0) { // hitsounds
                Stuffs.GetSound(Sounds.Hitsound).Play();
            }
        } 
        
        private void AutoPlayNoteHandling(Note n, float np0, float npp) {
            if (!n.dead && np0 < 0.3) {
                keysHeldOnHit[n.lane] = false;
            }
                    
            if (np0 <= 0 && !n.dead) { // when player should hit note
                n.dead = true;
                keysHeldOnHit[n.lane] = true;
                autoPlayKeyTimer[n.lane] = (float)ThingTools.Rand.NextDouble() * 0.1f + 0.2f;
            }
        }

        private bool CountdownCompareThing(float a, float b) {
            return (int)(a - b) != (int)a;
        }
        
        private float GetFrameTime() {
            return countdownTimer > 0 ? -countdownTimer : AudioManager.FrameTime;
        }
        
        private void Update(float delta) {
            if (isPaused) return;

            // Countdown (let controls exist beforehand)
            if (countdownTimer > 0) {
                if (CountdownCompareThing(countdownTimer, delta))
                    Stuffs.GetSound(Sounds.Hitsound).Play();

                if (countdownTimer - delta < 0) {
                    AudioManager.Seek(0.001f);
                    AudioManager.SetPause(false);
                    shouldBePaused = false;
                }
                
                countdownTimer -= delta;
            }
            
            if (!autoPlay) {
                KeyboardState ks = Keyboard.GetState();
                for (int i = 0; i < 6; i++) {
                    Keys k = Stuffs.GetControl(controls + i);

                    keysHeld[i] = ks.IsKeyDown(k);
                
                    keysHeldOnHit[i] = keysHeldOnHit[i] && ks.IsKeyDown(k);
                
                    // make keysheld2 = is key pressed (from keys held last frame)
                    keysHeld2[i] = ks.IsKeyDown(k) && !keysHeld2[i];
                }
            } 
            else {
                for (int i = 0; i < 6; i++) {
                    autoPlayKeyTimer[i] -= delta;
                    keysHeldOnHit[i] = autoPlayKeyTimer[i] > 0 && keysHeldOnHit[i];
                    keysHeld[i] = keysHeldOnHit[i];
                    keysHeld2[i] = true;
                }
            }

            float frameTime = GetFrameTime();
            for (int i = 0; i < notes.Count;) {
                Note n = notes[i];

                // IMPORTANT: npp is incorrect during countdown
                // IMPORTANT: npp is incorrect during countdown
                // IMPORTANT: npp is incorrect during countdown
                // IMPORTANT: npp is incorrect during countdown
                // IMPORTANT: npp is incorrect during countdown
                // IMPORTANT: npp is incorrect during countdown
                float np0 = n.time - frameTime;                  // current pos of note (in seconds away from passing keys)
                float npp = n.time - AudioManager.LastFrameTime; // pos of note last frame 

                NoteHandler.Invoke(n, np0, npp);
                
                // TODO: make scroll speed and make this use scroll speed
                if (np0 < -0.3) {  // delete note after it has passed the screen
                    if (!n.dead)
                        health -= 0.1f;
                    
                    notes.RemoveAt(i);
                    continue;
                }
            
                i++;
            }

            // cap health at 1
            health = Math.Min(1f, health);
            if (health < 0 && !autoPlay) {
                // TODO: make death work
            }
            
            // make keysheld2 = is key held last frame
            keysHeld2 = (bool[]) keysHeld.Clone();
        }
        
        private void Draw(float delta) {
            Align vAl = downscroll ? Align.Bottom : Align.Top;
            float frameTime = GetFrameTime();
            
            for (int i = 0; i < 6; i++) {
                RRender.Draw(Align.Center, vAl, Stuffs.GetTexture((int)Textures.Keys1 + i + (keysHeld[i] ? keysHeldOnHit[i] ? 12 : 6 : 0)), xpos+(i-3)*96, (-200 * downscrollMul), 64, 64, Color.White);
            }

            foreach (Note n in notes) {
                if (n.dead)
                    continue;

                RRender.Draw(Align.Center, vAl, Stuffs.GetTexture((int)Textures.Note1 + n.lane) , xpos+(n.lane-3)*96, (-200 * downscrollMul) - (int)((n.time - frameTime) * 800f * downscrollMul), 64, 64, Color.White);
            }

            if (!autoPlay) {
                healthD = ThingTools.Lerp(healthD, health, 10f * delta);

                RRender.Draw(Align.Center, vAl, Textures.HealthBarBackground, xpos - 200, -90 * downscrollMul, 400, 40, Color.White);
                RRender.Draw(Align.Center, vAl, Textures.HealthBar, xpos - 200, -90 * downscrollMul, (int) (400 * healthD), 40, Color.White, 0.4f);
                
                if (ratingTimer > 0f) {
                    RRender.DrawString(Align.Center, vAl, Align.Center, vAl, rating, xpos, -10 * downscrollMul, 4);
                    ratingTimer -= delta;
                }
            }
        } 
    }
}
