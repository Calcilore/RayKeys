using System;
using System.Threading;
using LibVLCSharp.Shared;

namespace RayKeys {
    public class AudioManager {
        private static LibVLC _libVLC;

        private Media media;
        private MediaPlayer mediaPlayer;

        public static void Initialize() {
            Core.Initialize();
            _libVLC = new LibVLC(false);
        }
        
        public float GetTime() {
            return mediaPlayer.Time / 1000f;
        }
        
        public float GetSpeed() {
            return mediaPlayer.Rate;
        }
        
        public int GetVolume() {
            return mediaPlayer.Volume;
        }
        
        public void SetVolume(int volume) {
            mediaPlayer.Volume = volume;
        }

        public bool IsFinished() {
            return mediaPlayer.IsPlaying;
        }

        public void Stop() {
            mediaPlayer.Stop();
        }

        public void Seek(float time) {
            mediaPlayer.Time = (long) (time * 1000);
        }

        public void Replay() {
            Thread t = new Thread(() => {
                mediaPlayer.Stop();
                mediaPlayer.Play();
            });
            t.Start();
        }
        
        public void PlaySong(string song, float speed = 1f) {
            media = new Media(_libVLC, "Content/" + song);
            mediaPlayer = new MediaPlayer(media);
            
            mediaPlayer.SetRate(speed);
            mediaPlayer.Play();
        }
    }
}