using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Database;

namespace SweetSpot.ScreenManagement.Screens
{
    public class ImageScreen : Screen
    {
        public Vector2 SweetSpot { get; set; }
        public string Cue { get; set; }

        protected Texture2D image;
        protected int test;
        protected int testSubject;
        protected TimeSpan lastPositionCaptured;
        protected TimeSpan recordingIntervall = TimeSpan.FromMilliseconds(100);

        public ImageScreen(ScreenManager screenManager)
            : base(screenManager)
        {
            lastPositionCaptured = TimeSpan.FromSeconds(-1);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            image = Content.Load<Texture2D>(@"texture\testimage");
        }

        public override void Initialize()
        {
            base.Initialize();
            SweetSpot = new Vector2(0, 2);
            Cue = "dummy";
            testSubject = screenManager.TestSubject;
            test = screenManager.Database.RecordTest(testSubject, Cue, SweetSpot);
            screenManager.Kinect.sweetSpot = SweetSpot;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (screenManager.Kinect.IsViewerActive() && recordingIntervalElapsed(gameTime))
                recordPosition(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }

        protected bool recordingIntervalElapsed(GameTime gameTime)
        {
            return lastPositionCaptured + recordingIntervall <= gameTime.TotalGameTime;
        }

        protected void recordPosition(GameTime gameTime)
        {
            lastPositionCaptured = gameTime.TotalGameTime;
            screenManager.Database.RecordUserPosition(test, screenManager.Kinect.GetViewerPosition(), (int)gameTime.TotalGameTime.TotalMilliseconds);
        }
    }
}
