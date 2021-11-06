using System;

namespace RayKeys {
    public class RhythmManager : AudioManager {
        public float bps;
        
        public double GetBeatTime() {
            return GetTime() * bps;
        }

        [Obsolete("You have to specify bps in a RhythmManager", true)]
        public new void PlaySong(string son, float speed = 1f) { }
        
        public void PlaySongBPM(string song, float bpm, float speed = 1f) {
            bps = bpm / 60f;
            base.PlaySong(song, speed);
        }
        
        public void PlaySongBPS(string song, float bps, float speed = 1f) {
            this.bps = bps;
            base.PlaySong(song, speed);
        }
    }
}
