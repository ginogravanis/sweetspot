using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    class SensorCalibrationScreen : Screen
    {
        Texture2D white;
        Rectangle separator;
        Sensor sensorOne;
        Sensor sensorTwo;
        SensorPanel sensorPanelOne;
        SensorPanel sensorPanelTwo;

        public SensorCalibrationScreen(ScreenManager screenManager)
            : base(screenManager)
        {
            sensorOne = screenManager.Kinect.GetSensor(SensorName.One);
            sensorTwo = screenManager.Kinect.GetSensor(SensorName.Two);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            white = Content.Load<Texture2D>("texture\\white");
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            separator = new Rectangle(
                viewport.Width/2,
                0,
                1,
                viewport.Height
                );
            sensorPanelOne = new SensorPanel(
                this,
                sensorOne,
                new Rectangle(
                    0,
                    0,
                    viewport.Width / 2,
                    viewport.Height
                ));
            sensorPanelTwo = new SensorPanel(
                this,
                sensorTwo,
                new Rectangle(
                    viewport.Width / 2,
                    0,
                    viewport.Width / 2,
                    viewport.Height
                ));
            sensorPanelOne.LoadContent();
            sensorPanelTwo.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            sensorPanelOne.Update(gameTime);
            sensorPanelTwo.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            graphics.Clear(Color.Black);
            spriteBatch.Begin();
            sensorPanelOne.Draw(spriteBatch);
            sensorPanelTwo.Draw(spriteBatch);
            spriteBatch.Draw(white, separator, Color.White);
            spriteBatch.End();
        }

        protected void swapPanels()
        {
            SensorPanel temp = sensorPanelOne;
            sensorPanelOne = sensorPanelTwo;
            sensorPanelTwo = temp;
        }
    }
}
