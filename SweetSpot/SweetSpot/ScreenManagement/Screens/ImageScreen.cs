using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement.Screens
{
    public class ImageScreen : Screen
    {
        protected Texture2D image;
        protected string cue;
        protected Vector2 sweetSpot;
        protected int test;
        protected int testSubject;
        protected TimeSpan lastPositionCaptured;
        protected TimeSpan recordingIntervall = TimeSpan.FromMilliseconds(100);

        public ImageScreen(ScreenManager screenManager, string cue, Vector2 sweetSpot)
            : base(screenManager)
        {
            this.cue = cue;
            this.sweetSpot = sweetSpot;
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
            testSubject = screenManager.TestSubject;
            test = screenManager.Database.RecordTest(testSubject, cue, sweetSpot);
            screenManager.Kinect.sweetSpot = sweetSpot;
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
