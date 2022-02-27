using System;
using LibVLCSharp.Shared;
using RayKeys.Misc;

namespace RayKeys {
    public static class AudioManager {
        private static LibVLC _libVLC;

        public static Media Media;
        public static MediaPlayer MediaPlayer;

        public static float bpm;
        public static float bps;
        
        public static float FrameTime     = 0f; // progress in seconds, updates every frame
        public static float LastFrameTime = 0f; // frameTime but last frame
        public static float MusicTime     = 0f; // progress in seconds, updates about every half second, is tied to the song so will always be accurate
        public static float LastMusicTime = 0f; // musicTime but last frame
        
        public static void Initialise() {
            Logger.Info("Initialising Audio Manager");
            Core.Initialize();
            _libVLC = new LibVLC(false);
            MediaPlayer = new MediaPlayer(_libVLC);
        }

        public static void Update(float delta) {
            LastMusicTime = MusicTime;
            LastFrameTime = FrameTime;

            FrameTime += delta * MediaPlayer.Rate;
            MusicTime = MediaPlayer.Time / 1000f;

            // if music time changes, then set frametime to musictime (sync to song)
            if (Math.Abs(MusicTime - LastMusicTime) > 0.05) {
                FrameTime = MusicTime;
            }
            
            // If it is paused
            if (!MediaPlayer.IsPlaying) {
                FrameTime -= delta * MediaPlayer.Rate;
            }
        }

        public static float GetBeatTime() {
            return FrameTime * bps;
        }

        public static float GetSpeed() {
            return MediaPlayer.Rate;
        }
        
        public static int GetVolume() {
            return MediaPlayer.Volume;
        }
        
        public static void SetVolume(int volume) {
            MediaPlayer.Volume = volume;
        }

        public static bool IsPlaying() {
            return MediaPlayer.IsPlaying;
        }

        public static void Play() {
            MediaPlayer.Play();
        }
        
        public static void Stop() {
            MediaPlayer.Stop();
        }

        public static void Seek(float time) {
            MediaPlayer.Time = (long) (time * 1000);
        }

        public static void SetPause(bool pause) {
               MediaPlayer.SetPause(pause);
        }

        public static void LoadSong(string song, float bpm, float speed = 1f) {
            Logger.Info("Loading Song: " + song);
            
            AudioManager.bpm = bpm;
            AudioManager.bps = bpm / 60;
            
            Media = new Media(_libVLC, "Content/" + song);
            MediaPlayer.Media = Media;

            MediaPlayer.SetRate(speed);
        }
    }
}