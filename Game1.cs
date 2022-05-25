using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.UI;
using RayKeys.Misc;
using RayKeys.Options;
using RayKeys.Render;

namespace RayKeys {
    public class Game1 : Game {
        public static Game1 Game; 
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch;
        public static float Scaling;
        public static Rectangle RenderRectangle { get; private set; }

        private FPSCounter fpsCounter = new FPSCounter();
        private RenderTarget2D renderTarget;

        public delegate void UpdateEventD(float delta);
        public static event UpdateEventD UpdateEvent;
        public static event UpdateEventD StaticUpdateEvent; // an update event that will never clear
        public static event UpdateEventD DrawEvent;

        public Game1(LogLevel logLevel) {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            Logger.Init(logLevel);
        }

        protected override void Initialize() {
            Scaling = Graphics.PreferredBackBufferHeight / 1080.0f;
            renderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            StaticUpdateEvent += RKeyboard.Update;
            StaticUpdateEvent += RMouse.Update;

            base.Initialize();
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RRender.Initialise();

            PrepareLoadScene();
            LoadScene(new LoadingScene());
        }

        protected override void Update(GameTime gameTime) {
            float delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            
            fpsCounter.Update(gameTime);
            StaticUpdateEvent?.Invoke(delta);
            UpdateEvent?.Invoke(delta);

            base.Update(gameTime);
        }

        public void PrepareLoadScene() {
            UpdateEvent = null;
            DrawEvent = null;
        }

        public void LoadScene(Scene scene) {
            RRender.CameraPos = Vector2.Zero;
        }

        public void RedoRenderPos() {
            Point thing = new Point((int) (1920 * Scaling), (int) (1080 * Scaling));
            RenderRectangle = new Rectangle((Graphics.PreferredBackBufferWidth - thing.X) / 2,
                (Graphics.PreferredBackBufferHeight - thing.Y) / 2, thing.X, thing.Y);
        }
        
        protected override void Draw(GameTime gameTime) {
            RRender.Draw();
            
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.SetRenderTarget(renderTarget);
            SpriteBatch.Begin(SpriteSortMode.BackToFront);  

            DrawEvent?.Invoke((float) gameTime.ElapsedGameTime.TotalSeconds);

            fpsCounter.DrawFps();
            
            SpriteBatch.End();
            SpriteBatch.Begin();
            
            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Draw(renderTarget, RenderRectangle, new Rectangle(0, 0, 1920, 1080), Color.White);
            
            SpriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args) {
            base.OnExiting(sender, args);
        }
    }
}
