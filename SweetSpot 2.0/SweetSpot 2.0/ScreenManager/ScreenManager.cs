using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    public class ScreenManager : DrawableGameComponent
    {
        protected List<Screen> screens = new List<Screen>();

        public SensorManager Kinect { get; internal set; }

        public InputManager Input { get; internal set; }

        public SpriteBatch SpriteBatch { get; internal set; }

        public ScreenManager(Game game)
            :base(game)
        {
            Kinect = new SensorManager();
            Input = new InputManager();
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
            Effect saturation= Game.Content.Load<Effect>("shader\\SaturationShader");
            Effect brightness = Game.Content.Load<Effect>("shader\\BrightnessShader");
            Effect contrast = Game.Content.Load<Effect>("shader\\ContrastShader");
            Effect sepia = Game.Content.Load<Effect>("shader\\SepiaShader");
            Effect pixelate = Game.Content.Load<Effect>("shader\\PixelateShader");
            Effect distort = Game.Content.Load<Effect>("shader\\DistortShader");
            AddScreen(new TransitionScreen(this, "Saturation Shader"));
            AddScreen(new DebugEffectScreen(this, image, saturation));
            AddScreen(new TransitionScreen(this, "Brightness Shader"));
            AddScreen(new DebugEffectScreen(this, image, brightness));
            AddScreen(new TransitionScreen(this, "Contrast Shader"));
            AddScreen(new DebugEffectScreen(this, image, contrast));
            AddScreen(new TransitionScreen(this, "Pixelate Shader"));
            AddScreen(new DebugEffectScreen(this, image, pixelate));
            AddScreen(new TransitionScreen(this, "Distort Shader"));
            AddScreen(new DebugEffectScreen(this, image, distort));
            AddScreen(new TransitionScreen(this, "Sepia Shader"));
            AddScreen(new DebugEffectScreen(this, image, sepia));
            AddScreen(new TransitionScreen(this, "The End"));
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
