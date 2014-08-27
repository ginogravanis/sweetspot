using System;
using SweetspotApp.Util;

namespace SweetspotApp.GameCore.Screens
{
    public class TrackingScreen : TaskScreen
    {
        protected static readonly TimeSpan RECORDING_INTERVAL = TimeSpan.FromMilliseconds(100);

        protected Sweetspot sweetspot;
        protected TimeSpan lastCaptureTime;

        public TrackingScreen(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(gc, cue, mapping)
        {
            this.sweetspot = sweetspot;
            lastCaptureTime = TimeSpan.FromSeconds(-1);
        }

        public override void Initialize()
        {
            base.Initialize();
            gc.Kinect.sweetspot = sweetspot;
            gc.Database.SetSweetspot(roundId, sweetspot);
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
                gc.Kinect.IsUserActive();
        }

        protected bool recordingIntervalElapsed()
        {
            return lastCaptureTime + RECORDING_INTERVAL <= elapsedTime;
        }

        protected void recordPosition()
        {
            lastCaptureTime = elapsedTime;
            gc.Database.RecordUserPosition(
                roundId,
                gc.Kinect.GetUserPosition(),
                (int)lastCaptureTime.TotalMilliseconds
                );
        }
    }
}
