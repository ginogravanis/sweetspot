using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    class Kinect
    {
        /// <summary>
        /// The position the user should optimally be standing relative to the sensor.
        /// The x-coordinate corresponds to the user's position parallel to the sensor,
        /// 0 being the center of the sensor and positive coordinates going to the right of the sensor.
        /// The y-coordinate corresponds to the user's position away from the sensor,
        /// 0 being the surface of the sensor.
        /// </summary>
        public Vector2 sweetspot;

        /// <summary>
        /// The minumum interaction distance from the sweetspot measured in meters.
        /// </summary>
        public float sweetspotPerimeter = 1f;

        List<Sensor> sensors;

        public Kinect(Vector2 sweetspot)
        {
            this.sensors = new List<Sensor>();
            this.sweetspot = sweetspot;
        }

        public void Initialize()
        {
            foreach(KinectSensor candidate in KinectSensor.KinectSensors)
            {
                if (candidate.Status == KinectStatus.Connected)
                {
                    Sensor sensor = new Sensor(candidate);
                    sensor.Initialize();
                    this.sensors.Add(sensor);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
        }

        public Vector2 GetNearestViewerPosition()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (Sensor sensor in this.sensors)
            {
                positions.AddRange(sensor.GetUserPositions());
            }

            if (positions.Count == 0)
                throw new ApplicationException("No skeletons detected.");

            float shortestDistanceToSweetSpot = this.DistanceToSweetSpot(positions[0]);
            Vector2 nearestUserPosition = positions[0];
            foreach (Vector2 position in positions.Skip(1))
            {
                float newDistance = this.DistanceToSweetSpot(position);
                if (newDistance < shortestDistanceToSweetSpot)
                {
                    shortestDistanceToSweetSpot = newDistance;
                    nearestUserPosition = position;
                }
            }

            return nearestUserPosition;
        }

        public float DistanceToSweetSpot(Vector2 position)
        {
            return Math.Abs((this.sweetspot - position).Length());
        }
    }
}
