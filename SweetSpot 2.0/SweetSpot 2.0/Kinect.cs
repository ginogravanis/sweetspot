using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    class Kinect : Microsoft.Xna.Framework.GameComponent
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

        TimeSpan positionSmoothingTime = new TimeSpan(0, 0, 0, 0, 50);

        List<Sensor> sensors;
        Vector2 lastViewerPosition;
        TimeSpan viewerLastSeen;
        bool viewerActive;

        public Kinect(Game game)
            : base(game)
        {
            this.sensors = new List<Sensor>();
            this.sweetSpot = new Vector2();
            this.lastViewerPosition = new Vector2();
            this.viewerLastSeen = new TimeSpan(-1, 0, 0);
            this.viewerActive = false;
        }

        public override void Initialize()
        {
            base.Initialize();
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            try
            {
                this.lastViewerPosition = this.CalculateViewerPosition();
                this.viewerLastSeen = gameTime.TotalGameTime;
            }
            catch (ApplicationException)
            {
                if (this.viewerLastSeen > gameTime.TotalGameTime - this.positionSmoothingTime)
                {
                    this.viewerActive = true;
                }
                else
                {
                    this.viewerActive = false;
                }
            }
        }

        public bool IsViewerActive()
        {
            return this.viewerActive;
        }

        public Vector2 GetViewerPosition()
        {
            return this.lastViewerPosition;
        }

        private Vector2 CalculateViewerPosition()
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

        public float GetDistanceFromSweetSpot()
        {
            float distanceFromSweetSpot = Math.Abs((this.sweetSpot - this.lastViewerPosition).Length());
            distanceFromSweetSpot = Math.Min(distanceFromSweetSpot, this.sweetSpotMargin);
            return distanceFromSweetSpot;
        }

        public float DistanceToSweetSpot(Vector2 position)
        {
            return Math.Abs((this.sweetSpot - position).Length());
        }
    }
}
