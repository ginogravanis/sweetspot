﻿using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    public class Sensor
    {
        KinectSensor sensor;

        public Sensor(KinectSensor kinect)
        {
            sensor = kinect;
        }

        ~Sensor()
        {
            sensor.Stop();
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

            sensor.SkeletonStream.Enable(smoothingParameters);
            sensor.Start();
        }

        public List<Vector2> GetUserPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (Skeleton skeleton in GetRawSkeletonData())
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
            using (SkeletonFrame frame = sensor.SkeletonStream.OpenNextFrame(0))
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
