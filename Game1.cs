using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayKeys {
    public class Game1 : Game {
        public static Game1 Me; 
        public readonly GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public float Scaling = 0;
        public RhythmManager Music = new RhythmManager();
        public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();

        private Engine engine;

        public delegate void UpdateEventD(double delta);
        public event UpdateEventD UpdateEvent;
        
        public delegate void DrawEventD(double delta);
        public event DrawEventD DrawEvent;

        public Game1() {
            Me = this;
            Graphics = new GraphicsDeviceManager(this);
            AudioManager.Initialize();

            Content.RootDirectory = "Assets";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.ApplyChanges();

            Scaling = Graphics.PreferredBackBufferHeight / 1080.0f;

            base.Initialize();
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

              Sounds.Add("hitsound" , SoundEffect.FromFile("Assets/Sounds/hitsound.wav"));
            Textures.Add("notes"    , ThingTools.LoadImage("Textures/notes.png"    ));
            Textures.Add("keys"     , ThingTools.LoadImage("Textures/keys.png"     ));
            Textures.Add("healthbar", ThingTools.LoadImage("Textures/healthbar.png"));

            engine = new Engine(new Keys[] {Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L}, speed: 10f);
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateEvent?.Invoke(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            SpriteBatch.Begin(blendState:BlendState.NonPremultiplied);
            
            DrawEvent?.Invoke(gameTime.ElapsedGameTime.TotalSeconds);

            SpriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args) {
            Music.Stop();
            base.OnExiting(sender, args);
        }
    }
}