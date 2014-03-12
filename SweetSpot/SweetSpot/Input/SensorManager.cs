using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetSpot.Input
{
    public enum SensorName { One, Two }

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
        /// The minumum interaction distance from the sweetspot in meters.
        /// </summary>
        public float sweetSpotMargin = 1f;

        TimeSpan positionSmoothingTime = TimeSpan.FromMilliseconds(50);

        List<Sensor> sensors;
        Vector2 lastViewerPosition;
        TimeSpan viewerLastSeen;
        bool viewerActive;

        public static int MaxSensorCount = 2;
        public static float sensorRange = 5.0f;

        public SensorManager()
        {
            sensors = new List<Sensor>();
            sweetSpot = new Vector2();
            lastViewerPosition = new Vector2();
            viewerLastSeen = TimeSpan.FromSeconds(-1);
            viewerActive = false;
            initializeSensors();
        }

        protected void initializeSensors()
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

            int maxSensors = SensorManager.MaxSensorCount;
            if (sensors.Count > maxSensors)
            {
                sensors.RemoveRange(maxSensors, sensors.Count - maxSensors);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Sensor sensor in sensors)
            {
                sensor.Update(gameTime);
            }

            if (viewerPositionAvailable())
            {
                lastViewerPosition = calculateViewerPosition();
                viewerLastSeen = gameTime.TotalGameTime;
                viewerActive = true;
            }
            else if (viewerRecentlySeen(gameTime))
            {
                viewerActive = true;
            }
            else
            {
                viewerActive = false;
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

        protected bool viewerPositionAvailable()
        {
            foreach (Sensor sensor in sensors)
            {
                if (sensor.HasActiveUsers())
                    return true;
            }
            return false;
        }

        protected bool viewerRecentlySeen(GameTime gameTime)
        {
            return viewerLastSeen > gameTime.TotalGameTime - positionSmoothingTime;
        }

        protected Vector2 calculateViewerPosition()
        {
            if (sensors.Count == 2
                && sensors[0].GetActiveUserCount() == 1
                && sensors[1].GetActiveUserCount() == 1)
            {
                Vector2 position1 = sensors[0].GetUserPositions().First<Vector2>();
                Vector2 position2 = sensors[1].GetUserPositions().First<Vector2>();
                return Calibrator.CalculateMidpoint(position1, position2);
            }
            else
            {
                List<Vector2> positions = new List<Vector2>();
                foreach (Sensor sensor in sensors)
                {
                    positions.AddRange(sensor.GetUserPositions());
                }
                return getNearestViewer(positions);
            }
        }

        protected Vector2 getNearestViewer(List<Vector2> positions)
        {
            if (positions.Count == 0)
                throw new ApplicationException("User position not available.");

            float shortestDistanceToSweetSpot = distanceToSweetSpot(positions[0]);
            Vector2 nearestUserPosition = positions[0];
            foreach (Vector2 position in positions.Skip(1))
            {
                float newDistance = distanceToSweetSpot(position);
                if (newDistance < shortestDistanceToSweetSpot)
                {
                    shortestDistanceToSweetSpot = newDistance;
                    nearestUserPosition = position;
                }
            }

            return nearestUserPosition;
        }

        public Vector2 GetVectorToSweetSpot()
        {
            return vectorToSweetSpot(lastViewerPosition);
        }

        protected Vector2 vectorToSweetSpot(Vector2 position)
        {
            return sweetSpot - position;
        }

        public float GetDistanceFromSweetSpot()
        {
            return distanceToSweetSpot(lastViewerPosition);
        }

        protected float distanceToSweetSpot(Vector2 position)
        {
            return Math.Abs((sweetSpot - position).Length());
        }

        public Sensor GetSensor(SensorName name)
        {
            if (sensors.Count == 0)
                throw new ApplicationException("No sensor connected!");

            Sensor sensorOne = sensors.First<Sensor>();
            Sensor sensorTwo = sensors.Last<Sensor>();
            Sensor result = sensorOne;

            if (sensors.Count > 1 && name == SensorName.Two)
            {
                result = sensorTwo;
            }

            return result;
        }

        public static Vector2 WorldToScreenCoords(Rectangle bounds, Vector2 position)
        {
            float x = bounds.Left + (bounds.Width / 2) + ((bounds.Width / 2f) * position.X / (sensorRange/2f));
            float y = bounds.Top + bounds.Height * (position.Y / sensorRange);
            return new Vector2((int)Math.Round(x), (int)Math.Round(y));
        }
    }
}
