using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    class Sensor
    {
        KinectSensor sensor;

        public Sensor(KinectSensor sensor)
        {
            this.sensor = sensor;
        }
        ~Sensor()
        {
            this.sensor.Stop();
        }

        public void Initialize()
        {
            TransformSmoothParameters smoothingParameters = new TransformSmoothParameters();
            {
                smoothingParameters.Smoothing = 0.5f;
                smoothingParameters.Correction = 0.5f;
                smoothingParameters.Prediction = 0.5f;
                smoothingParameters.JitterRadius = 0.05f;
                smoothingParameters.MaxDeviationRadius = 0.04f;
            };

            this.sensor.SkeletonStream.Enable(smoothingParameters);
            this.sensor.Start();
        }

        public List<Vector2> GetUserPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (Skeleton skeleton in this.GetRawSkeletonData())
            {
                if (skeleton.TrackingState != SkeletonTrackingState.NotTracked)
                {
                    positions.Add(new Vector2(skeleton.Position.X, skeleton.Position.Z));
                }
            }
            return positions;
        }

        private Skeleton[] GetRawSkeletonData()
        {
            using (SkeletonFrame frame = this.sensor.SkeletonStream.OpenNextFrame(0))
            {
                Skeleton[] skeletons;
                if (null != frame)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
                else
                {
                    skeletons = new Skeleton[0];
                }
                return skeletons;
            }
        }
    }
}
