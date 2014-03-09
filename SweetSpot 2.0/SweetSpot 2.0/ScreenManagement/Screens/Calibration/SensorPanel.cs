using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot_2._0.Input;

namespace SweetSpot_2._0.ScreenManagement.Screens
{
    class SensorPanel
    {
        protected Screen container;
        protected Texture2D green;
        protected Texture2D red;
        protected Texture2D blue;
        protected Texture2D white;
        protected Rectangle bounds;
        protected Rectangle sensorRect;
        protected const int rectangleSize = 20;

        protected Sensor sensor;
        protected bool activeUsers = false;
        protected Vector2 SensorPosition;
        protected List<Vector2> userPositions;
        protected Calibrator calibrator;

        public SensorPanel(Screen container, Sensor sensor, Rectangle screenBounds)
        {
            this.container = container;
            this.sensor = sensor;
            bounds = screenBounds;
            SensorPosition = new Vector2(screenBounds.Center.X, 0);
            userPositions = new List<Vector2>();
            calibrator = new Calibrator();
            sensorRect = new Rectangle(
                screenBounds.Center.X - rectangleSize/2,
                0,
                rectangleSize,
                rectangleSize
                );
        }

        protected Rectangle makePositionMarker(Vector2 position)
        {
            Vector2 screenPosition = SensorManager.WorldToScreenCoords(bounds, position);

            return new Rectangle(
                (int)screenPosition.X - rectangleSize / 2,
                (int)screenPosition.Y - rectangleSize / 2,
                rectangleSize,
                rectangleSize
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

        public void SetTransformation(float axisTilt, Vector3 offset)
        {
            sensor.Calibrate(axisTilt, offset);
        }

        public string GetSensorID()
        {
            return sensor.GetDeviceID();
        }
    }
}
