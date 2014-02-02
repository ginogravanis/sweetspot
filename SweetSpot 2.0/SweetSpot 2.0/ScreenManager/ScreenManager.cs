using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    public class ScreenManager : DrawableGameComponent
    {
        private Screen screen;
        public Screen Screen
        {
            get { return screen; }

            set
            {
                if (screen != null)
                    screen.UnloadContent();

                value.ScreenManager = this;
                value.LoadContent();
                screen = value;
            }
        }

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
            Texture2D image = Game.Content.Load<Texture2D>("testimage");
            Effect effect = Game.Content.Load<Effect>("SaturationShader");
            Screen = new EffectScreen(image, effect);
            Screen.LoadContent();
        }

        protected override void UnloadContent()
        {
            Screen.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            Kinect.Update(gameTime);
            Screen.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.Draw(gameTime);
        }
    }
}
