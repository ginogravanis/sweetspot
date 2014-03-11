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

            Texture2D image = Game.Content.Load<Texture2D>("texture\\testimage");
            Effect saturation = Game.Content.Load<Effect>("shader\\SaturationShader");
            Effect brightness = Game.Content.Load<Effect>("shader\\BrightnessShader");
            Effect contrast = Game.Content.Load<Effect>("shader\\ContrastShader");
            Effect pixelate = Game.Content.Load<Effect>("shader\\PixelateShader");
            Effect distort = Game.Content.Load<Effect>("shader\\DistortShader");
            Effect jitter = Game.Content.Load<Effect>("shader\\JitterShader");

            AddScreen(new TransitionScreen(this, "Calibration"));
            AddScreen(new SensorCalibrationScreen(this));
            AddScreen(new TransitionScreen(this, "Arrow"));
            AddScreen(new BaselineArrowScreen(this, image));
            AddScreen(new TransitionScreen(this, "Text"));
            AddScreen(new BaselineTextScreen(this, image));
            AddScreen(new TransitionScreen(this, "Saturation"));
            AddScreen(new EffectScreen(this, image, saturation));
            AddScreen(new TransitionScreen(this, "Brightness"));
            AddScreen(new EffectScreen(this, image, brightness));
            AddScreen(new TransitionScreen(this, "Contrast"));
            AddScreen(new EffectScreen(this, image, contrast));
            AddScreen(new TransitionScreen(this, "Pixelate"));
            AddScreen(new EffectScreen(this, image, pixelate));
            AddScreen(new TransitionScreen(this, "Distort"));
            AddScreen(new EffectScreen(this, image, distort));
            AddScreen(new TransitionScreen(this, "Jitter"));
            AddScreen(new EffectScreen(this, image, jitter));
            AddScreen(new TransitionScreen(this, "Thank you for participating!"));
        }

        protected override void UnloadContent()
        {
            foreach (Screen screen in screens)
                screen.UnloadContent();
        }

        public void AddScreen(Screen newScreen)
        {
            newScreen.LoadContent();
            screens.Add(newScreen);
        }

        public void GenerateTestSession(List<Vector2> sweetSpots)
        {
            this.sweetSpots = sweetSpots;
        }

        public void ToggleDebug()
        {
            Debug = !Debug;
        }

        public override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            Kinect.Update(gameTime);
            screens[0].Update(gameTime);

            if (screens[0].Finished)
                RemoveScreen();

            if (screens.Count == 0)
                Game.Exit();
        }

        protected void RemoveScreen()
        {
            screens[0].UnloadContent();
            screens.RemoveAt(0);
        }

        public override void Draw(GameTime gameTime)
        {
            screens[0].Draw(gameTime);
        }
    }
}
