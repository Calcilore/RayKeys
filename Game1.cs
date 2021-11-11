using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.Menu;
using RayKeys.Options;
using RayKeys.Render;

namespace RayKeys {
    public class Game1 : Game {
        public static Game1 Game; 
        public readonly GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public float Scaling = 0;
        public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();
        public SpriteFont[] Fonts = new SpriteFont[7];
        public Keys[][] Controls = new Keys[4][];
        private MainMenu mainMenu;
        private FPSCounter fpsCounter = new FPSCounter();

        public delegate void UpdateEventD(float delta);
        public event UpdateEventD UpdateEvent;
        
        public delegate void DrawEventD(float delta);
        public event DrawEventD DrawEvent;

        public Game1() {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);
            AudioManager.Initialise();
            OptionsManager.Initialise();
            RRender.resolution = new Point(1920, 1080);

            Controls[0] = new Keys[] {Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L};
            Controls[1] = new Keys[] {Keys.W, Keys.E, Keys.R, Keys.Y, Keys.U, Keys.I};
            Controls[2] = new Keys[] {Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L};
            Controls[3] = new Keys[] {Keys.W, Keys.E, Keys.R, Keys.Y, Keys.U, Keys.I};
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            
            // Graphics.PreferredBackBufferWidth = 1920;
            // Graphics.PreferredBackBufferHeight = 1080;
            // Graphics.SynchronizeWithVerticalRetrace = false;
            //Graphics.IsFullScreen = true;
            //IsFixedTimeStep = false;
            //Graphics.ApplyChanges();
    
            Scaling = Graphics.PreferredBackBufferHeight / 1080.0f;

            base.Initialize();
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            for (int i = 0; i < Fonts.Length; i++) {
                Fonts[i] = Content.Load<SpriteFont>("Fonts/Font" + i);
            }

            Sounds.Add("hitsound" , Content.Load<SoundEffect>("Sounds/hitsound" ));
            Textures.Add("notes"    , Content.Load<Texture2D>("Textures/notes"    ));
            Textures.Add("keys"     , Content.Load<Texture2D>("Textures/keys"     ));
            Textures.Add("healthbar", Content.Load<Texture2D>("Textures/healthbar"));
            Textures.Add("button", Content.Load<Texture2D>("Textures/button"));

            mainMenu = new MainMenu();
            //EngineManager.Start("1");
        }

        protected override void Update(GameTime gameTime) {

            fpsCounter.Update(gameTime);
            UpdateEvent?.Invoke((float) gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(/*blendState:BlendState.NonPremultiplied*/);

            Button.cursorType = false;
            DrawEvent?.Invoke((float) gameTime.ElapsedGameTime.TotalSeconds);
            Mouse.SetCursor( Button.cursorType ? MouseCursor.Hand : MouseCursor.Arrow);

            fpsCounter.DrawFps();

            SpriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args) {
            base.OnExiting(sender, args);
        }
    }
}
