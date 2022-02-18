﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RayKeys.Menu;
using RayKeys.Misc;
using RayKeys.Options;
using RayKeys.Render;

namespace RayKeys {
    public class Game1 : Game {
        public static Game1 Game; 
        public readonly GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public float Scaling;
        // public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        // public Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();
        public SpriteFont[] Fonts = new SpriteFont[7];
        public Keys[][] Controls = new Keys[4][];
        public Rectangle RenderRectangle { get; private set; }

        private Scene currentScene;
        //private MainMenu mainMenu;
        private FPSCounter fpsCounter = new FPSCounter();
        private RenderTarget2D test;

        public delegate void UpdateEventD(float delta);
        public event UpdateEventD UpdateEvent;
        
        public delegate void DrawEventD(float delta);
        public event DrawEventD DrawEvent;

        public Game1() {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            Scaling = Graphics.PreferredBackBufferHeight / 1080.0f;
            
            AudioManager.Initialise();
            OptionsManager.Initialise();

            test = new RenderTarget2D(GraphicsDevice, 1920, 1080, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            Controls[0] = new Keys[] {Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L};
            Controls[1] = new Keys[] {Keys.W, Keys.E, Keys.R, Keys.U, Keys.I, Keys.O};
            Controls[2] = new Keys[] {Keys.Z, Keys.X, Keys.C, Keys.N, Keys.M, Keys.OemComma};
            Controls[3] = new Keys[] {Keys.D3, Keys.D4, Keys.D5, Keys.D8, Keys.D9, Keys.D0};

            base.Initialize();
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            RRender.Initialise();
            
            // Textures.Add("notes"    , Content.Load<Texture2D>("Textures/notes"    ));
            // Textures.Add("keys"     , Content.Load<Texture2D>("Textures/keys"     ));
            // Textures.Add("healthbar", Content.Load<Texture2D>("Textures/healthbar"));
            // Textures.Add("button", Content.Load<Texture2D>("Textures/button"));
            // Textures.Add("trackeditorbg", Content.Load<Texture2D>("Textures/trackeditorbg"));

            for (int i = 0; i < Fonts.Length; i++) {
                Fonts[i] = Content.Load<SpriteFont>("Fonts/Font" + i);
            }

            Stuffs.Init(Content);

            //Sounds.Add("hitsound" , Content.Load<SoundEffect>("Sounds/hitsound" ));
            

            PrepareLoadScene();
            LoadScene(new MainMenu());
            //EngineManager.Start("1");
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
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.SetRenderTarget(test);
            SpriteBatch.Begin();  

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
