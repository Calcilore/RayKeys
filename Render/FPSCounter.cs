using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayKeys.Render {
    public class FPSCounter
    {
        private float frames = 0;
        private float last = 0;
        private float now = 0;
        public float msgFrequency = .3f; 
        public float msg = 1;
        public float fps = 1;

        public void Update(GameTime gameTime) {
            now = (float) gameTime.TotalGameTime.TotalSeconds;
            float elapsed = now - last;
            if (elapsed > msgFrequency) {
                msg = (int)(frames / elapsed);
                //Console.WriteLine(msg);
                frames = 0;
                last = now;
            }
        }

        public void DrawFps() {
            RRender.DrawStringNoCam(Align.Right, Align.Top, Align.Right, Align.Top, "FPS: " + msg, -10, 10, 6, Color.White);
            frames++;
        }
    }
}