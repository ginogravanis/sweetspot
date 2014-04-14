using System;
using Microsoft.Xna.Framework;

namespace SweetSpot.ScreenManagement.Screens
{
    public class TrackingScreen : TaskScreen
    {
        protected Vector2 sweetSpot;
        protected TimeSpan recordingIntervall = TimeSpan.FromMilliseconds(100);
        protected TimeSpan lastPositionCaptured;

        public TrackingScreen(ScreenManager screenManager, string cue, Vector2 sweetSpot)
            : base(screenManager, cue)
        {
            this.sweetSpot = sweetSpot;
            lastPositionCaptured = TimeSpan.FromSeconds(-1);
        }

        public override void Initialize()
        {
            base.Initialize();
            screenManager.Kinect.sweetSpot = sweetSpot;
        }

        protected override int initializeTest()
        {
            return screenManager.Database.RecordTest(testSubject, cue, sweetSpot);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (!taskCompleted && recordingIntervalElapsed() && screenManager.Kinect.IsViewerActive())
                recordPosition();
        }

        protected bool recordingIntervalElapsed()
        {
            return lastPositionCaptured + recordingIntervall <= elapsedTime;
        }

        protected void recordPosition()
        {
            lastPositionCaptured = elapsedTime;
            screenManager.Database.RecordUserPosition(test, screenManager.Kinect.GetViewerPosition(), (int)lastPositionCaptured.TotalMilliseconds);
        }
    }
}
