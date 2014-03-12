using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot_2._0.Database;
using SweetSpot_2._0.Input;
using SweetSpot_2._0.ScreenManagement.Screens;

namespace SweetSpot_2._0.ScreenManagement
{
    public class ScreenManager : DrawableGameComponent
    {
        public bool Debug { get; internal set; }

        protected List<Screen> screens;
        protected List<Vector2> sweetSpots;
        protected List<Effect> effects;
        protected Texture2D image;

        public SensorManager Kinect { get; internal set; }
        public InputManager Input { get; internal set; }
        public IDatabase Database { get; internal set; }
        public SpriteBatch SpriteBatch { get; internal set; }

        public ScreenManager(Game game)
            : base(game)
        {
            Debug = false;
            screens = new List<Screen>();
            sweetSpots = new List<Vector2>();
            effects = new List<Effect>();
            Kinect = new SensorManager();
            Input = new InputManager();
            Database = new SQLiteAdapter();
        }

        public override void Initialize()
        {
            base.Initialize();
            Kinect.sweetSpot = new Vector2(0f, 2f);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            image = Game.Content.Load<Texture2D>("texture\\testimage");

            effects.Add(Game.Content.Load<Effect>("shader\\SaturationShader"));
            effects.Add(Game.Content.Load<Effect>("shader\\BrightnessShader"));
            effects.Add(Game.Content.Load<Effect>("shader\\ContrastShader"));
            effects.Add(Game.Content.Load<Effect>("shader\\PixelateShader"));
            effects.Add(Game.Content.Load<Effect>("shader\\DistortShader"));
            effects.Add(Game.Content.Load<Effect>("shader\\JitterShader"));

            AddScreen(new TransitionScreen(this, "Calibration"));
            AddScreen(new SensorCalibrationScreen(this));
        }

        protected override void UnloadContent()
        {
            foreach (Screen screen in screens)
                screen.UnloadContent();
        }

        public void AddScreen(Screen screen)
        {
            screen.LoadContent();
            screens.Add(screen);
        }

        public void GenerateTestSession(List<Vector2> sweetSpots)
        {
            this.sweetSpots = sweetSpots;
            List<Screen> testSession = new List<Screen>();
            testSession.Add(new BaselineArrowScreen(this, image));
            testSession.Add(new BaselineTextScreen(this, image));
            foreach (Effect effect in effects)
            {
                testSession.Add(new EffectScreen(this, image, effect));
            }
            testSession.Shuffle();

            for (int i = 0; i < testSession.Count; i++)
            {
                AddScreen(new TransitionScreen(this, String.Format("Test {0}/{1}", i + 1, testSession.Count)));
                AddScreen(testSession[i]);
            }
            AddScreen(new TransitionScreen(this, "Thank you for participating!"));
        }

        public void ToggleDebug()
        {
            Debug = !Debug;
        }

        public override void Update(GameTime gameTime)
        {
            if (screens[0].Finished)
                RemoveScreen();

            Input.Update(gameTime);
            Kinect.Update(gameTime);
            screens[0].Update(gameTime);
        }

        protected void RemoveScreen()
        {
            screens[0].UnloadContent();
            screens.RemoveAt(0);
            if (screens.Count == 0)
                Game.Exit();
        }

        public override void Draw(GameTime gameTime)
        {
            screens[0].Draw(gameTime);
        }
    }
}
