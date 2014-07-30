using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetspotApp.Input

{
    public class Kinect
    {
        protected static readonly int BUFFER_TIME = 3;
        protected static readonly TimeSpan BUFFER_INTERVAL = TimeSpan.FromMilliseconds(200);
                
        public bool IsCalibrated { get; protected set; }

        protected KinectSensor sensor;
        protected List<Skeleton> activeUsers;
        protected TimeSpan lastSensorUpdate;
        protected TimeSpan positionSmoothingTime;
        protected Matrix TransformationMatrix;
        protected ICalibrationProvider calibrationProvider;
        protected TimeSpan lastBufferTime;
        protected TimeSpan elapsedTime;

        protected WriteableBitmap colorBitmap;
        protected DepthImagePixel[] depthPixels;
        protected byte[] colorPixels;
        protected int counter = 0;
        protected LinkedList<Tuple<WriteableBitmap, DateTime>> buffer = new LinkedList<Tuple<WriteableBitmap, DateTime>>();
        protected LinkedList<Tuple<WriteableBitmap, DateTime>> expiredBuffer = new LinkedList<Tuple<WriteableBitmap, DateTime>>();
        protected String[] timestamp = new String[0];

        public Kinect(KinectSensor kinect, ICalibrationProvider calibrationProvider)
        {
            elapsedTime = TimeSpan.FromSeconds(0);
            sensor = kinect;
            activeUsers = new List<Skeleton>();
            positionSmoothingTime = TimeSpan.FromMilliseconds(200);
            TransformationMatrix = Matrix.Identity;
            lastSensorUpdate = TimeSpan.FromSeconds(-1);
            this.calibrationProvider = calibrationProvider;
            IsCalibrated = false;
            lastBufferTime = TimeSpan.FromSeconds(-1);
        }

        ~Kinect()
        {
            sensor.Stop();
        }

        public override string ToString()
        {
            return GetDeviceId().Replace('\\', '_').Replace('&', '_');
        }

        public void Initialize()
        {
            TransformSmoothParameters smoothingParameters = new TransformSmoothParameters();
            {
                smoothingParameters.Smoothing = 0.75f;
                smoothingParameters.Correction = 0.2f;
                smoothingParameters.Prediction = 0.5f;
                smoothingParameters.JitterRadius = 0.01f;
                smoothingParameters.MaxDeviationRadius = 0.04f;
            };

            if (calibrationProvider.HasCalibrationDataFor(GetDeviceId()))
            {
                var calibration = calibrationProvider.LoadCalibration(GetDeviceId());
                Calibrate(calibration.Item1, calibration.Item2);
                IsCalibrated = true;
            }

            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            depthPixels = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
            colorPixels = new byte[sensor.DepthStream.FramePixelDataLength * sizeof(int)];
            colorBitmap = new WriteableBitmap(sensor.DepthStream.FrameWidth, sensor.DepthStream.FrameHeight, 96.0, 96.0, System.Windows.Media.PixelFormats.Bgr32, null);
            sensor.DepthFrameReady += SensorDepthFrameReady;

            sensor.SkeletonStream.Enable(smoothingParameters);
            sensor.Start();
        }

        public void Update(GameTime gameTime)
        {
            Skeleton[] rawSkeletons = GetRawSkeletonData();
            List<Skeleton> trackableSkeletons = GetTrackableSkeletons(rawSkeletons);
            elapsedTime += gameTime.ElapsedGameTime;

            if (trackableSkeletons.Count > 0)
            {
                activeUsers = trackableSkeletons;
                lastSensorUpdate = gameTime.TotalGameTime;
            }
            else
            {
                if (sensorRecentlyUpdated(gameTime))
                {
                    // user has been seen within position smoothing time
                    // use old skeleton data
                }
                else
                {
                    activeUsers = trackableSkeletons;
                    lastSensorUpdate = gameTime.TotalGameTime;
                }
            }
        }

        protected bool sensorRecentlyUpdated(GameTime gameTime)
        {
            return lastSensorUpdate > gameTime.TotalGameTime - positionSmoothingTime;
        }

        protected Skeleton[] GetRawSkeletonData()
        {
            using (SkeletonFrame frame = sensor.SkeletonStream.OpenNextFrame(0))
            {
                Skeleton[] skeletons;
                if (null == frame)
                {
                    skeletons = new Skeleton[0];
                }
                else
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
                return skeletons;
            }
        }

        protected List<Skeleton> GetTrackableSkeletons(Skeleton[] candidates)
        {
            List<Skeleton> trackableSkeletons = new List<Skeleton>();
            foreach (Skeleton skeleton in candidates)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.NotTracked)
                {
                    trackableSkeletons.Add(skeleton);
                }
            }
            return trackableSkeletons;
        }

        public List<Vector2> GetUserPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (Skeleton skeleton in activeUsers)
            {
                Vector2 rawPosition = new Vector2(skeleton.Position.X, skeleton.Position.Z);
                Vector2 adjustedPosition = Vector2.Transform(rawPosition, TransformationMatrix);
                positions.Add(adjustedPosition);
            }
            return positions;
        }

        public bool HasActiveUsers()
        {
            return GetActiveUserCount() > 0;
        }

        public int GetActiveUserCount()
        {
            return activeUsers.Count;
        }

        public void Calibrate(float axisTilt, Vector3 offset)
        {
            Matrix rotation = Matrix.CreateRotationZ(axisTilt);
            Matrix translation = Matrix.CreateTranslation(offset);
            TransformationMatrix = rotation * translation;
            calibrationProvider.SaveCalibration(GetDeviceId(), axisTilt, offset);
        }

        public string GetDeviceId()
        {
            return sensor.UniqueKinectId;
        }

        public void ResetSensorTilt()
        {
            try
            {
                sensor.ElevationAngle = 0;
            }
            catch (InvalidOperationException)
            {
                Logger.Log("Can't change elevation angle, sensor not responding.");
            }
        }

        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    depthFrame.CopyDepthImagePixelDataTo(depthPixels);

                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;

                    int colorPixelIndex = 0;

                    for (int i = 0; i < depthPixels.Length; ++i)
                    {
                        short depth = depthPixels[i].Depth;
                        byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                        colorPixels[colorPixelIndex++] = intensity;
                        colorPixels[colorPixelIndex++] = intensity;                  
                        colorPixels[colorPixelIndex++] = intensity;
                        ++colorPixelIndex;
                    }

                    colorBitmap.WritePixels(
                        new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight),
                        colorPixels,
                        colorBitmap.PixelWidth * sizeof(int),
                        0);

                    if (isRecordingIntervalElapsed())
                    {
                        bufferFrame(colorBitmap);
                        lastBufferTime = elapsedTime;
                    }
                }
            }
        }

        protected bool isRecordingIntervalElapsed()
        {
            return lastBufferTime + BUFFER_INTERVAL <= elapsedTime;
        }

        protected void bufferFrame(WriteableBitmap frame)
        {
            WriteableBitmap currentFrame = frame.Clone();
            buffer.AddLast(Tuple.Create(currentFrame, DateTime.Now));

            while (isFirstFrameExpired())
            {
                expiredBuffer.AddLast(buffer.First.Value);
                buffer.RemoveFirst();
            }
        }

        public IEnumerable<Tuple<WriteableBitmap, DateTime>> GetNextFrames()
        {
            var nextFrames = new LinkedList<Tuple<WriteableBitmap, DateTime>>();
            foreach (var frame in expiredBuffer)
                nextFrames.AddLast(frame);
            expiredBuffer.Clear();
            return nextFrames;
        }

        public IEnumerable<Tuple<WriteableBitmap, DateTime>> GetRemainingFrames()
        {
            var remainingFrames = buffer;
            buffer.Clear();
            return remainingFrames;
        }

        protected bool isFirstFrameExpired()
        {
            DateTime timeNow = DateTime.Now;
            DateTime bufferTime = buffer.First.Value.Item2;

            var diffInSeconds = (timeNow - bufferTime).TotalSeconds;

            return diffInSeconds > BUFFER_TIME;
        }
    }
}
