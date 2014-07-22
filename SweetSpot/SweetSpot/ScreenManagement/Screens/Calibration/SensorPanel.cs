using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Input;

namespace SweetSpot.ScreenManagement.Screens
{
    class SensorPanel
    {
        protected SensorCalibrationScreen container;
        protected Texture2D green;
        protected Texture2D red;
        protected Texture2D blue;
        protected Texture2D white;
        protected Rectangle bounds;
        protected Rectangle sensorRect;
        protected const int MARKER_SIZE = 20;

        protected Sensor sensor;
        protected bool activeUsers = false;
        protected Vector2 SensorPosition;
        protected List<Vector2> userPositions;
        protected Calibrator calibrator;

        public SensorPanel(SensorCalibrationScreen container, Sensor sensor, Rectangle screenBounds)
        {
            this.container = container;
            this.sensor = sensor;
            bounds = screenBounds;
            SensorPosition = new Vector2(screenBounds.Center.X, 0);
            userPositions = new List<Vector2>();
            calibrator = new Calibrator();
            sensorRect = new Rectangle(
                screenBounds.Center.X - MARKER_SIZE/2,
                0,
                MARKER_SIZE,
                MARKER_SIZE
                );
        }

        protected Rectangle makePositionMarker(Vector2 position)
        {
            Vector2 screenPosition = SensorManager.WorldToScreenCoords(bounds, position);

            return new Rectangle(
                (int)screenPosition.X - MARKER_SIZE / 2,
                (int)screenPosition.Y - MARKER_SIZE / 2,
                MARKER_SIZE,
                MARKER_SIZE
                );
        }

        public void LoadContent(ContentManager content)
        {
            green = content.Load<Texture2D>("texture\\green");
            red = content.Load<Texture2D>("texture\\red");
            blue = content.Load<Texture2D>("texture\\blue");
            white = content.Load<Texture2D>("texture\\white");
        }

        public void Update(GameTime gameTime)
        {
            activeUsers = sensor.HasActiveUsers();
            if (activeUsers)
                userPositions = sensor.GetUserPositions();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(red, sensorRect, Color.White);

            foreach (Vector2 point in calibrator.GetReferencePoints())
            {
                spriteBatch.Draw(white, makePositionMarker(point), Color.White);
            }

            foreach (Vector2 sweetspot in container.sweetspotBounds.GetPoints())
            {
                spriteBatch.Draw(blue, makePositionMarker(sweetspot), Color.White);
            }

            if (activeUsers)
            {
                foreach (Vector2 position in userPositions)
                {
                    spriteBatch.Draw(green, makePositionMarker(position), Color.White);
                }
            }
        }

        public bool HasEnoughReferencePoints()
        {
            return calibrator.HasEnoughReferencePoints();
        }

        public void CaptureReferencePoint()
        {
            calibrator.AddPoint(sensor.GetUserPositions().First<Vector2>());
        }

        public Calibrator GetCalibrator()
        {
            return calibrator;
        }

        public bool HasActiveUsers()
        {
            return activeUsers;
        }

        public void Calibrate(Tuple<float, Vector3> calibration)
        {
            sensor.Calibrate(calibration.Item1, calibration.Item2);
        }

        public string GetSensorID()
        {
            return sensor.GetDeviceID();
        }
    }
}
