using System;
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
        public readonly GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public float Scaling;
        public SpriteFont[] Fonts = new SpriteFont[7];
        public Rectangle RenderRectangle { get; private set; }

        private Scene currentScene;
        private FPSCounter fpsCounter = new FPSCounter();
        private RenderTarget2D test;

        public delegate void UpdateEventD(float delta);
        public event UpdateEventD UpdateEvent;
        
        public delegate void DrawEventD(float delta);
        public event DrawEventD DrawEvent;

        public Game1(LogLevel logLevel) {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            Logger.Init(logLevel);
        }

        protected override void Initialize() {
            Scaling = Graphics.PreferredBackBufferHeight / 1080.0f;
            
            AudioManager.Initialise();
            ThingTools.Init();

            test = new RenderTarget2D(GraphicsDevice, 1920, 1080, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            base.Initialize();
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            RRender.Initialise();

            for (int i = 0; i < Fonts.Length; i++) {
                Fonts[i] = Content.Load<SpriteFont>("Fonts/Font" + i);
            }

            PrepareLoadScene();
            LoadScene(new LoadingScene());
        }

        protected override void Update(GameTime gameTime) {

            float delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            
            RKeyboard.Update();
            RMouse.Update();
            AudioManager.Update(delta);
            fpsCounter.Update(gameTime);
            UpdateEvent?.Invoke(delta);

            base.Update(gameTime);
        }

        public void PrepareLoadScene() {
            UpdateEvent = null;
            DrawEvent = null;
        }

        public void LoadScene(Scene scene) {
            RRender.CameraPos = Vector2.Zero;
            currentScene = scene;
        }

        public void RedoRenderPos() {
            Point thing = new Point((int) (1920 * Scaling), (int) (1080 * Scaling));
            RenderRectangle = new Rectangle((Graphics.PreferredBackBufferWidth - thing.X) / 2,
                (Graphics.PreferredBackBufferHeight - thing.Y) / 2, thing.X, thing.Y);
        }
        
        protected override void Draw(GameTime gameTime) {
            RRender.Draw();
            
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.SetRenderTarget(test);
            SpriteBatch.Begin(SpriteSortMode.BackToFront);  

            DrawEvent?.Invoke((float) gameTime.ElapsedGameTime.TotalSeconds);

            fpsCounter.DrawFps();
            
            SpriteBatch.End();
            SpriteBatch.Begin();
            
            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Draw(test, RenderRectangle, new Rectangle(0, 0, 1920, 1080), Color.White);
            
            SpriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args) {
            base.OnExiting(sender, args);
        }
    }
}
