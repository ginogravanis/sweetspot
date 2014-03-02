using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    class SensorPanel
    {
        protected Screen container;
        protected Texture2D green;
        protected Texture2D red;
        protected Rectangle bounds;
        protected Rectangle sensorRect;
        protected const int rectangleSize = 20;

        protected Sensor sensor;
        protected bool SensorActive = false;
        protected Vector2 SensorPosition;
        protected List<Vector2> userPositions;

        public SensorPanel(Screen container, Sensor sensor, Rectangle screenBounds)
        {
            this.container = container;
            this.sensor = sensor;
            bounds = screenBounds;
            SensorPosition = new Vector2(screenBounds.Center.X, 0);
            userPositions = new List<Vector2>();
            sensorRect = new Rectangle(
                screenBounds.Center.X - rectangleSize/2,
                0,
                rectangleSize,
                rectangleSize
                );
        }

        protected Rectangle makeUserRect(Vector2 position)
        {
            Vector2 screenPosition = SensorManager.WorldToScreenCoords(bounds, position);

            return new Rectangle(
                (int)screenPosition.X - rectangleSize / 2,
                (int)screenPosition.Y - rectangleSize / 2,
                rectangleSize,
                rectangleSize
                );
        }

        public void LoadContent()
        {
            green = container.Content.Load<Texture2D>("texture\\green");
            red = container.Content.Load<Texture2D>("texture\\red");
        }

        public void Update(GameTime gameTime)
        {
            if (sensor.HasActiveUsers())
            {
                SensorActive = true;
                userPositions = sensor.GetUserPositions();
            }
            else
            {
                SensorActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(red, sensorRect, Color.White);
            if (SensorActive)
            {
                foreach (Vector2 position in userPositions)
                {
                    spriteBatch.Draw(green, makeUserRect(position), Color.White);
                }
            }
        }
    }
}
