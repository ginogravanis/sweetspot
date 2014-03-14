using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot.ScreenManagement.Screens
{
    public class ImageScreen : Screen
    {
        public Vector2 SweetSpot { get; set; }
        public string Cue { get; set; }

        protected Texture2D image;
        protected int test;
        protected int testSubject;

        public ImageScreen(ScreenManager screenManager)
            : base(screenManager)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            image = Content.Load<Texture2D>(@"texture\testimage");
        }

        public override void Initialize()
        {
            base.Initialize();
            testSubject = screenManager.TestSubject;
            test = screenManager.Database.RecordTest(testSubject, Cue, SweetSpot);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }
    }
}
