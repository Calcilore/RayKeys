using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayKeys {
    /*
     *     NN
     *        NN
     *
     *  NN <-- Notes
     *
     *
     *  [] [] [] []  <-- Keys
     */
    public class Engine {
        private Texture2D keysTexture;
        private Texture2D notesTexture;
        private Texture2D healthBarTexture;

        private Keys[] controls;
        private int xpos;
        private float speed;

        private bool[] keysHeld = new bool[6];      // currently held keys
        private bool[] keysHeld2 = new bool[6];     // keys held last frame and keys that were tapped (changes during the frame)
        private bool[] keysHeldOnHit = new bool[6]; // when you hit a note the keys change to the 
        private List<Note> notes = new List<Note>();
        
        private float frameTime    = 0; // progress in seconds, updates every frams
        private float frameTimePre = 0; // frameTime but last frame
                   // musicTime    = 0; // progress in seconds, updates about every half second, is tied to the song so will always be accurate
        private float musicTimePre = 0; // musicTime but last frame

        public float health = 1f;
        private float healthD = 0f;
        
        public Engine(Keys[] controls, int xpos = 960, float speed = 1f) {
            this.controls = controls;
            this.xpos = xpos;
            this.speed = speed;
            
            Game1.Game.DrawEvent += Draw;

            notesTexture = Game1.Game.Textures["notes"];
            keysTexture = Game1.Game.Textures["keys"];
            healthBarTexture = Game1.Game.Textures["healthbar"];

            // temporary random charting
            for (float i = 0; i < 1000; i += 0.176470589f) {
                notes.Add(new Note(3.0f + i, (byte)ThingTools.rand.Next(6)));
            }
        }

        public void Start() {
            Game1.Game.UpdateEvent += Update;
        }

        private void Update(float delta) {
            frameTime += delta * speed;
            float musicTime = EngineManager.Music.GetTime();
            
            // if music time changes, then set frametime to musictime (sync to song)
            if (Math.Abs(musicTime - musicTimePre) > 0.05) {
                frameTime = musicTime;
            }

            // for time for audio to start
            if (musicTime == 0f) {
                frameTime = 0f;
            }
            
            KeyboardState ks = Keyboard.GetState();
            for (int i = 0; i < 6; i++) {
                keysHeld[i] = ks.IsKeyDown(controls[i]);
                
                keysHeldOnHit[i] = keysHeldOnHit[i] && ks.IsKeyDown(controls[i]);
                
                // make keysheld2 = is key pressed (from keys held last frame)
                keysHeld2[i] = ks.IsKeyDown(controls[i]) && !keysHeld2[i];
            }

            for (int i = 0; i < notes.Count;) {
                Note n = notes[i];

                float np0 = n.time - frameTime;    // current pos of note (in seconds away from passing keys)
                float npp = n.time - frameTimePre; // pos of note last frame
                if (Math.Abs(np0) < 0.1 && keysHeld2[n.lane]) { // player hit note
                    n.dead = true;
                    keysHeldOnHit[n.lane] = true;

                    health += 0.05f;
                }
                
                if (np0 <= 0 && npp > 0) { // hitsounds
                    Game1.Game.Sounds["hitsound"].Play();
                }
                
                
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
            if (health < 0) {
                
            }

            musicTimePre = musicTime;
            frameTimePre = frameTime;
            // make keysheld2 = is key held last frame
            keysHeld2 = (bool[]) keysHeld.Clone();
        }
        
        private void Draw(float delta) {
            for (int i = 0; i < 6; i++) {
                ThingTools.DrawToSpriteBatch(keysTexture, xpos+(i-3)*96, 880, i*64, keysHeld[i] ? keysHeldOnHit[i] ? 128 : 64 : 0, 64, 64);
            }
            
            for (int i = 0; i < notes.Count; i++) {
                Note n = notes[i];
                if (n.dead)
                    continue;
                
                ThingTools.DrawToSpriteBatch(notesTexture, xpos+(n.lane-3)*96, 880 - (int)((n.time - frameTime) * 800f), n.lane*64, 0, 64, 64);
            }
            
            healthD = ThingTools.Lerp(healthD, health, 10f * delta);
            
            ThingTools.DrawToSpriteBatch(healthBarTexture, xpos - 200, 1040 - 10, 0, 40, 400, 40);
            ThingTools.DrawToSpriteBatch(healthBarTexture, xpos - 200, 1040 - 10, 0, 0, (int) (400 * healthD), 40);
        } 
    }
}
