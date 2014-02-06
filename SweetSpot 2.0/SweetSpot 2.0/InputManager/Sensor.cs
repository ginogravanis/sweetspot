﻿using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    public class Sensor
    {
        KinectSensor sensor;

        Skeleton[] rawSkeletonData =  new Skeleton[0];

        public Matrix TransformationMatrix { get; set; }

        public Sensor(KinectSensor kinect)
        {
            sensor = kinect;
            TransformationMatrix = Matrix.Identity;
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

        public void Update()
        {
            rawSkeletonData = GetRawSkeletonData();
        }

        public List<Vector2> GetUserPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (Skeleton skeleton in GetActiveUsers())
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
            return GetActiveUsers().Count;
        }

        private List<Skeleton> GetActiveUsers()
        {
            List<Skeleton> activeUsers = new List<Skeleton>();
            foreach (Skeleton skeleton in rawSkeletonData)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.NotTracked)
                {
                    activeUsers.Add(skeleton);
                }
            }
            return activeUsers;
        }

        private Skeleton[] GetRawSkeletonData()
        {
            using (SkeletonFrame frame = sensor.SkeletonStream.OpenNextFrame(20))
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
