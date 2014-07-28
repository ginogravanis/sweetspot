using System;
using Microsoft.Xna.Framework;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class TrackingScreen : TaskScreen
    {
        protected Sweetspot sweetspot;
        protected TimeSpan recordingIntervall = TimeSpan.FromMilliseconds(100);
        protected TimeSpan lastPositionCaptured;

        public TrackingScreen(ScreenManager sm, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(sm, cue, mapping)
        {
            this.sweetspot = sweetspot;
            lastPositionCaptured = TimeSpan.FromSeconds(-1);
        }

        public override void Initialize()
        {
            base.Initialize();
            sm.Kinect.sweetspot = sweetspot;
            sm.Database.SetSweetspot(roundId, sweetspot);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (shouldRecordPosition())
                recordPosition();
        }

        protected bool shouldRecordPosition()
        {
            return !TaskCompleted &&
                recordingIntervalElapsed() &&
                sm.Kinect.IsUserActive();
        }

        protected bool recordingIntervalElapsed()
        {
            return lastPositionCaptured + recordingIntervall <= elapsedTime;
        }

        protected void recordPosition()
        {
            lastPositionCaptured = elapsedTime;
            sm.Database.RecordUserPosition(
                roundId,
                sm.Kinect.GetUserPosition(),
                (int)lastPositionCaptured.TotalMilliseconds
                );
        }
    }
}
