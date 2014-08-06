using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SweetspotApp.Input;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    class CalibrationScreen : Screen
    {
        Texture2D white;
        Rectangle separator;
        SensorPanel leftSensorPanel;
        SensorPanel rightSensorPanel;
        public SweetspotBounds sweetspotBounds;
        protected bool unsavedSweetspotBounds = false;

        public CalibrationScreen(GameController gc)
            : base(gc)
        {
            sweetspotBounds = new SweetspotBounds(gc.Database);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            white = Content.Load<Texture2D>("texture\\white");
            Viewport viewport = gc.GraphicsDevice.Viewport;
            separator = new Rectangle(
                viewport.Width/2,
                0,
                1,
                viewport.Height
                );
            leftSensorPanel = new SensorPanel(
                this,
                gc.Kinect.GetSensor(SensorName.One),
                new Rectangle(
                    0,
                    0,
                    viewport.Width / 2,
                    viewport.Height
                ));
            rightSensorPanel = new SensorPanel(
                this,
                gc.Kinect.GetSensor(SensorName.Two),
                new Rectangle(
                    viewport.Width / 2,
                    0,
                    viewport.Width / 2,
                    viewport.Height
                ));
            leftSensorPanel.LoadContent(Content);
            rightSensorPanel.LoadContent(Content);
            loadSweetspotBounds();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            leftSensorPanel.Update(gameTime);
            rightSensorPanel.Update(gameTime);

            if (gc.Input.IsGamePadButtonPressed(Buttons.B) || gc.Input.IsKeyPressed(Keys.F1))
            {
                if (leftSensorPanel.HasActiveUsers() && rightSensorPanel.HasActiveUsers())
                {
                    leftSensorPanel.CaptureReferencePoint();
                    rightSensorPanel.CaptureReferencePoint();
                }
            }

            if (gc.Input.IsGamePadButtonPressed(Buttons.Y) || gc.Input.IsKeyPressed(Keys.F2))
            {
                if (leftSensorPanel.HasEnoughReferencePoints() && rightSensorPanel.HasEnoughReferencePoints())
                {
                    Calibrator calibrator1 = leftSensorPanel.GetCalibrator();
                    Calibrator calibrator2 = rightSensorPanel.GetCalibrator();

                    try
                    {
                        Tuple<Tuple<float, Vector3>, Tuple<float, Vector3>> calibrations = Calibrator.Calibrate(calibrator1, calibrator2);
                        Tuple<float, Vector3> calibration1 = calibrations.Item1;
                        Tuple<float, Vector3> calibration2 = calibrations.Item2;
                        leftSensorPanel.Calibrate(calibration1);
                        rightSensorPanel.Calibrate(calibration2);
                    }
                    catch (InvalidOperationException) { }
                }
            }

            if (gc.Input.IsGamePadButtonPressed(Buttons.X) || gc.Input.IsKeyPressed(Keys.F5))
                captureSweetspot();

            if (gc.Input.IsGamePadButtonPressed(Buttons.LeftTrigger) || gc.Input.IsKeyPressed(Keys.F6))
                clearSweetspots();

            if (gc.Input.IsGamePadButtonPressed(Buttons.LeftShoulder) || gc.Input.IsKeyPressed(Keys.F9))
                gc.Kinect.ResetSensorTilt();
        }

        protected void captureSweetspot()
        {
            if (gc.Kinect.IsUserActive())
            {
                sweetspotBounds.Add(gc.Kinect.GetUserPosition());
                unsavedSweetspotBounds = true;
            }
        }

        protected void clearSweetspots()
        {
            sweetspotBounds.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice graphics = gc.GraphicsDevice;
            graphics.Clear(Color.Black);
            spriteBatch.Begin();
            leftSensorPanel.Draw(spriteBatch);
            rightSensorPanel.Draw(spriteBatch);
            spriteBatch.Draw(white, separator, Color.White);
            spriteBatch.End();
        }

        public override void NextScreen()
        {
            base.NextScreen();
            if (unsavedSweetspotBounds)
                gc.Database.SaveSweetspotBounds(sweetspotBounds.GetPoints());
        }

        protected void loadSweetspotBounds()
        {
            var bounds = gc.Database.LoadSweetspotBounds();
            foreach (var point in bounds)
                sweetspotBounds.Add(point);
        }
    }
}
