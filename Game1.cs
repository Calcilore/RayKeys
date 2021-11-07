using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayKeys {
    public class Game1 : Game {
        public static Game1 Game; 
        public readonly GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public float Scaling = 0;
        public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();
        public SpriteFont Font;

        public delegate void UpdateEventD(float delta);
        public event UpdateEventD UpdateEvent;
        
        public delegate void DrawEventD(float delta);
        public event DrawEventD DrawEvent;

        public Game1() {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);
            AudioManager.Initialize();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            Graphics.ApplyChanges();

            Scaling = Graphics.PreferredBackBufferHeight / 1080.0f;

            base.Initialize();
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Font = Content.Load<SpriteFont>("Font");

              Sounds.Add("hitsound" , Content.Load<SoundEffect>("Sounds/hitsound" ));
            Textures.Add("notes"    , Content.Load<Texture2D>("Textures/notes"    ));
            Textures.Add("keys"     , Content.Load<Texture2D>("Textures/keys"     ));
            Textures.Add("healthbar", Content.Load<Texture2D>("Textures/healthbar"));

            EngineManager.addEngine(new [] {Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L} );
            EngineManager.Start();
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateEvent?.Invoke((float) gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(/*blendState:BlendState.NonPremultiplied*/);
            
            DrawEvent?.Invoke((float) gameTime.ElapsedGameTime.TotalSeconds);

            string fpst = "FPS: " + Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);
            SpriteBatch.DrawString(Font, fpst, new Vector2(1910 - Font.MeasureString(fpst).X, 10), Color.White);

            SpriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args) {
            base.OnExiting(sender, args);
        }
    }
}