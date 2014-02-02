using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    public class ScreenManager : DrawableGameComponent
    {
        Screen screen;

        public SensorManager Kinect
        {
            get;
            internal set;
        }

        public InputManager Input
        {
            get;
            internal set;
        }

        public SpriteBatch SpriteBatch
        {
            get;
            internal set;
        }

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
            ChangeScreen(new SampleScreen());
            screen.LoadContent();
        }

        protected override void UnloadContent()
        {
            screen.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            Kinect.Update(gameTime);
            screen.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            screen.Draw(gameTime);
        }

        public void ChangeScreen(Screen newScreen)
        {
            if (screen != null)
                screen.UnloadContent();

            newScreen.ScreenManager = this;
            newScreen.LoadContent();
            screen = newScreen;
        }
    }
}
