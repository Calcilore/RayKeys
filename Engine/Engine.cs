using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayKeys {
    public class Engine {
        private Texture2D keysTexture;
        private Texture2D notesTexture;
        private Texture2D healthBarTexture;

        private Keys[] controls;
        private int xpos;
        private float speed;

        private bool[] keysHeld = new bool[6];
        private bool[] keysHeld2 = new bool[6];
        private bool[] keysHeldOnHit = new bool[6];
        private List<Note> notes = new List<Note>();
        
        private float frameTime = 0;
        private float frameTimePre = 0;
        private float musicTimePre = 0;

        public float health = 1f;
        
        public Engine(Keys[] controls, int xpos = 960, float speed = 1f) {
            this.controls = controls;
            this.xpos = xpos;
            this.speed = speed;

            notesTexture = Game1.Me.Textures["notes"];
            keysTexture = Game1.Me.Textures["keys"];
            healthBarTexture = Game1.Me.Textures["healthbar"];

            Game1.Me.UpdateEvent += Update;
            Game1.Me.DrawEvent += Draw;

            for (float i = 0; i < 1000; i += 0.176470589f) {
                notes.Add(new Note(3.0f + i, (byte)ThingTools.rand.Next(6)));
            }

            Game1.Me.Music.PlaySongBPM("Levels/1/song.ogg", 170, speed);
        }

        private void Update(double delta) {
            frameTime += (float)delta * speed;
            float musicTime = Game1.Me.Music.GetTime();

            if (Math.Abs(musicTime - musicTimePre) > 0.05) {
                frameTime = musicTime;
            }

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

                float np0 = n.time - frameTime;
                float npp = n.time - frameTimePre;
                if (Math.Abs(np0) < 0.1 && keysHeld2[n.lane]) {
                    n.dead = true;
                    keysHeldOnHit[n.lane] = true;

                    health += 0.05f;
                }
                
                if (np0 <= 0 && npp > 0) {
                    Game1.Me.Sounds["hitsound"].Play();
                }
                
                if (np0 < -0.3) {
                    if (!n.dead)
                        health -= 0.1f;
                    
                    notes.RemoveAt(i);
                    continue;
                }
            
                i++;
            }

            health = Math.Min(1f, health);
            if (health < 0) {
                Console.Write("Dead! ");
            }
            
            Console.WriteLine(health);

            musicTimePre = musicTime;
            frameTimePre = frameTime;
            // make keysheld2 = is key held last frame
            keysHeld2 = (bool[]) keysHeld.Clone();
        }
        
        private void Draw(double delta) {
            for (int i = 0; i < 6; i++) {
                ThingTools.DrawToSpriteBatch(keysTexture, xpos+(i-3)*96, 880, i*64, keysHeld[i] ? keysHeldOnHit[i] ? 128 : 64 : 0, 64, 64);
            }
            
            for (int i = 0; i < notes.Count; i++) {
                Note n = notes[i];
                if (n.dead)
                    continue;
                
                ThingTools.DrawToSpriteBatch(notesTexture, xpos+(n.lane-3)*96, 880 - (int)((n.time - frameTime) * 800f), n.lane*64, 0, 64, 64);
            }

            ThingTools.DrawToSpriteBatch(healthBarTexture, xpos - 200, 1040 - 10, 0, 40, 400, 40);
            ThingTools.DrawToSpriteBatch(healthBarTexture, xpos - 200, 1040 - 10, 0, 0, (int) (400 * health), 40);
        } 
    }
}
