using System;
using Microsoft.Xna.Framework;

namespace SweetSpot.ScreenManagement.Screens
{
    public class TrackingScreen : TaskScreen
    {
        protected Vector2 sweetspot;
        protected TimeSpan recordingIntervall = TimeSpan.FromMilliseconds(100);
        protected TimeSpan lastPositionCaptured;

        public TrackingScreen(ScreenManager sm, string cue, Mapping mapping, Vector2 sweetspot)
            : base(sm, cue, mapping)
        {
            this.sweetspot = sweetspot;
            lastPositionCaptured = TimeSpan.FromSeconds(-1);
        }

        public override void Initialize()
        {
            base.Initialize();
            sm.Kinect.sweetspot = sweetspot;
            sm.Database.SetSweetspot(roundID, sweetspot);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (shouldRecordPosition())
                recordPosition();
        }

        protected bool shouldRecordPosition()
        {
            return !taskCompleted &&
                recordingIntervalElapsed() &&
                sm.Kinect.IsViewerActive();
        }

        protected bool recordingIntervalElapsed()
        {
            return lastPositionCaptured + recordingIntervall <= elapsedTime;
        }

        protected void recordPosition()
        {
            lastPositionCaptured = elapsedTime;
            sm.Database.RecordUserPosition(
                roundID,
                sm.Kinect.GetViewerPosition(),
                (int)lastPositionCaptured.TotalMilliseconds
                );
        }
    }
}
