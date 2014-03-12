using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetSpot.Input
{
    public class Sensor
    {
        protected KinectSensor sensor;
        protected List<Skeleton> activeUsers;
        protected TimeSpan lastSensorUpdate;
        protected TimeSpan positionSmoothingTime;
        protected Matrix TransformationMatrix;

        public Sensor(KinectSensor kinect)
        {
            sensor = kinect;
            activeUsers = new List<Skeleton>();
            positionSmoothingTime = TimeSpan.FromMilliseconds(50);
            TransformationMatrix = Matrix.Identity;
            lastSensorUpdate = TimeSpan.FromSeconds(-1);
        }

        ~Sensor()
        {
            sensor.Stop();
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

            sensor.SkeletonStream.Enable(smoothingParameters);
            sensor.Start();
        }

        public void Update(GameTime gameTime)
        {
            Skeleton[] rawSkeletons = GetRawSkeletonData();
            List<Skeleton> trackableSkeletons = GetTrackableSkeletons(rawSkeletons);

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
            using (SkeletonFrame frame = sensor.SkeletonStream.OpenNextFrame(10))
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
        }

        public string GetDeviceID()
        {
            return sensor.DeviceConnectionId;
        }
    }
}
