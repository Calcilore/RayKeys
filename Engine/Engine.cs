using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private Action<Note, float, float> NoteHandler; // it might be more efficient to use this stupid action instead of 1 if statement in a for loop
        private Texture2D keysTexture;
        private Texture2D notesTexture;
        private Texture2D healthBarTexture;

        private Keys[] controls;
        private bool autoPlay;
        private float[] autoPlayKeyTimer = new float[6];
        private int xpos;
        private float speed;
        private int downscrollMul;
        private bool downscroll;

        private bool[] keysHeld = new bool[6];      // currently held keys
        private bool[] keysHeld2 = new bool[6];     // keys held last frame and keys that were tapped (changes during the frame)
        private bool[] keysHeldOnHit = new bool[6]; // when you hit a note the keys change to the 

        public List<Note> notes = new List<Note>();
        public float health = 1f;
        private float healthD = 0f;
        
        public Engine(int controls, int xpos = 960, float speed = 1f) {
            controls--;
            if (controls == -1) {
                autoPlay = true;
            } else {
                this.controls = Game1.Game.Controls[controls];
            }

            if (autoPlay) NoteHandler = AutoPlayNoteHandling;
            else          NoteHandler = NoAutoPlayNoteHandling;

            this.xpos = xpos;
            this.speed = speed;
            
            Game1.Game.DrawEvent += Draw;

            notesTexture = Game1.Game.Textures["notes"];
            keysTexture = Game1.Game.Textures["keys"];
            healthBarTexture = Game1.Game.Textures["healthbar"];

            downscroll = (bool) OptionsManager.GetOption("downscroll").currentValue;
            downscrollMul = downscroll ? 1 : -1;
        }

        public void Start() {
            Game1.Game.UpdateEvent += Update;
        }

        private void NoAutoPlayNoteHandling(Note n, float np0, float npp) {
            if (Math.Abs(np0) < 0.1 && keysHeld2[n.lane]) { // player hit note
                n.dead = true;
                keysHeldOnHit[n.lane] = true;

                health += 0.05f;
            }
                
            if (np0 <= 0 && npp > 0) { // hitsounds
                Game1.Game.Sounds["hitsound"].Play();
            }
        } 
        
        private void AutoPlayNoteHandling(Note n, float np0, float npp) {
            if (!n.dead && np0 < 0.3) {
                keysHeldOnHit[n.lane] = false;
            }
                    
            if (np0 <= 0 && npp > 0) { // when player should hit note
                n.dead = true;
                keysHeldOnHit[n.lane] = true;
                autoPlayKeyTimer[n.lane] = (float)ThingTools.Rand.NextDouble() * 0.1f + 0.2f;
            }

        } 
        
        private void Update(float delta) {
            if (!autoPlay) {
                KeyboardState ks = Keyboard.GetState();
                for (int i = 0; i < 6; i++) {
                    keysHeld[i] = ks.IsKeyDown(controls[i]);
                
                    keysHeldOnHit[i] = keysHeldOnHit[i] && ks.IsKeyDown(controls[i]);
                
                    // make keysheld2 = is key pressed (from keys held last frame)
                    keysHeld2[i] = ks.IsKeyDown(controls[i]) && !keysHeld2[i];
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
            
            for (int i = 0; i < notes.Count;) {
                Note n = notes[i];

                float np0 = n.time - AudioManager.FrameTime;     // current pos of note (in seconds away from passing keys)
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
            
            for (int i = 0; i < 6; i++) {
                RRender.Draw(Align.Center, vAl, keysTexture, xpos+(i-3)*96, (-200 * downscrollMul), i*64, keysHeld[i] ? keysHeldOnHit[i] ? 128 : 64 : 0, 64, 64);
            }

            foreach (Note n in notes) {
                if (n.dead)
                    continue;

                RRender.Draw(Align.Center, vAl, notesTexture, xpos+(n.lane-3)*96, (-200 * downscrollMul) - (int)((n.time - AudioManager.FrameTime) * 800f * downscrollMul), n.lane*64, 0, 64, 64);
            }

            if (!autoPlay) {
                healthD = ThingTools.Lerp(healthD, health, 10f * delta);
            
                RRender.Draw(Align.Center, vAl, healthBarTexture, xpos - 200, -90 * downscrollMul, 0, 40, 400, 40);
                RRender.Draw(Align.Center, vAl, healthBarTexture, xpos - 200, -90 * downscrollMul, 0, 0, (int) (400 * healthD), 40);
            }
        } 
    }
}
