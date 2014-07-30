using System;
using Microsoft.Xna.Framework;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class TrackingScreen : TaskScreen
    {
        protected static readonly TimeSpan RECORDING_INTERVAL = TimeSpan.FromMilliseconds(100);

        protected Sweetspot sweetspot;
        protected TimeSpan lastCaptureTime;

        public TrackingScreen(ScreenManager sm, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(sm, cue, mapping)
        {
            this.sweetspot = sweetspot;
            lastCaptureTime = TimeSpan.FromSeconds(-1);
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
            return lastCaptureTime + RECORDING_INTERVAL <= elapsedTime;
        }

        protected void recordPosition()
        {
            lastCaptureTime = elapsedTime;
            sm.Database.RecordUserPosition(
                roundId,
                sm.Kinect.GetUserPosition(),
                (int)lastCaptureTime.TotalMilliseconds
                );
        }
    }
}
