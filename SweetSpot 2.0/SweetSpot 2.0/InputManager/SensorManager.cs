﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    public class SensorManager
    {
        /// <summary>
        /// The position the user should optimally be standing relative to the sensor.
        /// The x-coordinate corresponds to the user's position parallel to the sensor,
        /// 0 being the center of the sensor and positive coordinates going to the right of the sensor.
        /// The y-coordinate corresponds to the user's position away from the sensor,
        /// 0 being the surface of the sensor.
        /// </summary>
        public Vector2 sweetSpot { get; set; }

        /// <summary>
        /// The minumum interaction distance from the sweetspot measured in meters.
        /// </summary>
        public float sweetSpotMargin = 1f;

        TimeSpan positionSmoothingTime = TimeSpan.FromMilliseconds(50);

        List<Sensor> sensors;
        Vector2 lastViewerPosition;
        TimeSpan viewerLastSeen;
        bool viewerActive;

        public SensorManager()
        {
            sensors = new List<Sensor>();
            sweetSpot = new Vector2();
            lastViewerPosition = new Vector2();
            viewerLastSeen = TimeSpan.FromSeconds(-1);
            viewerActive = false;
            Initialize();
        }

        public void Initialize()
        {
            foreach(KinectSensor candidate in KinectSensor.KinectSensors)
            {
                if (candidate.Status == KinectStatus.Connected)
                {
                    Sensor sensor = new Sensor(candidate);
                    sensor.Initialize();
                    sensors.Add(sensor);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            try
            {
                lastViewerPosition = CalculateViewerPosition();
                viewerLastSeen = gameTime.TotalGameTime;
            }
            catch (ApplicationException)
            {
                if (viewerLastSeen > gameTime.TotalGameTime - positionSmoothingTime)
                {
                    viewerActive = true;
                }
                else
                {
                    viewerActive = false;
                }
            }
        }

        public bool IsViewerActive()
        {
            return viewerActive;
        }

        public Vector2 GetViewerPosition()
        {
            return lastViewerPosition;
        }

        private Vector2 CalculateViewerPosition()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (Sensor sensor in sensors)
            {
                positions.AddRange(sensor.GetUserPositions());
            }

            if (positions.Count == 0)
                throw new ApplicationException("No skeletons detected.");

            float shortestDistanceToSweetSpot = DistanceToSweetSpot(positions[0]);
            Vector2 nearestUserPosition = positions[0];
            foreach (Vector2 position in positions.Skip(1))
            {
                float newDistance = DistanceToSweetSpot(position);
                if (newDistance < shortestDistanceToSweetSpot)
                {
                    shortestDistanceToSweetSpot = newDistance;
                    nearestUserPosition = position;
                }
            }

            return nearestUserPosition;
        }

        public float GetDistanceFromSweetSpot()
        {
            float distanceFromSweetSpot = Math.Abs((sweetSpot - lastViewerPosition).Length());
            distanceFromSweetSpot = Math.Min(distanceFromSweetSpot, sweetSpotMargin);
            return distanceFromSweetSpot;
        }

        public float DistanceToSweetSpot(Vector2 position)
        {
            return Math.Abs((sweetSpot - position).Length());
        }
    }
}
