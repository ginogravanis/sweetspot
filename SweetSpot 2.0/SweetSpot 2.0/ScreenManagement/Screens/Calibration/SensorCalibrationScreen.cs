using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SweetSpot_2._0.Input;

namespace SweetSpot_2._0.ScreenManagement.Screens
{
    class SensorCalibrationScreen : Screen
    {
        Texture2D white;
        Rectangle separator;
        SensorPanel leftSensorPanel;
        SensorPanel rightSensorPanel;
        List<Vector2> sweetspots;

        public SensorCalibrationScreen(ScreenManager screenManager)
            : base(screenManager)
        {
            sweetspots = new List<Vector2>();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            white = Content.Load<Texture2D>("texture\\white");
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            separator = new Rectangle(
                viewport.Width/2,
                0,
                1,
                viewport.Height
                );
            leftSensorPanel = new SensorPanel(
                this,
                screenManager.Kinect.GetSensor(SensorName.One),
                new Rectangle(
                    0,
                    0,
                    viewport.Width / 2,
                    viewport.Height
                ));
            rightSensorPanel = new SensorPanel(
                this,
                screenManager.Kinect.GetSensor(SensorName.Two),
                new Rectangle(
                    viewport.Width / 2,
                    0,
                    viewport.Width / 2,
                    viewport.Height
                ));
            leftSensorPanel.LoadContent(Content);
            rightSensorPanel.LoadContent(Content);
            loadCalibration();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            leftSensorPanel.Update(gameTime);
            rightSensorPanel.Update(gameTime);

            if (screenManager.Input.IsGamePadButtonPressed(Buttons.B) || screenManager.Input.IsKeyPressed(Keys.F1))
            {
                if (leftSensorPanel.HasActiveUsers() && rightSensorPanel.HasActiveUsers())
                {
                    leftSensorPanel.CaptureReferencePoint();
                    rightSensorPanel.CaptureReferencePoint();
                }
            }

            if (screenManager.Input.IsGamePadButtonPressed(Buttons.Y) || screenManager.Input.IsKeyPressed(Keys.F2))
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
                        leftSensorPanel.Calibrate(calibration1.Item1, calibration1.Item2);
                        rightSensorPanel.Calibrate(calibration2.Item1, calibration2.Item2);
                        screenManager.Database.SaveCalibration(leftSensorPanel.GetSensorID(), calibration1.Item1, calibration1.Item2);
                        screenManager.Database.SaveCalibration(rightSensorPanel.GetSensorID(), calibration2.Item1, calibration2.Item2);
                    }
                    catch (InvalidOperationException) { }
                }
            }

            if (screenManager.Input.IsGamePadButtonPressed(Buttons.X) || screenManager.Input.IsKeyPressed(Keys.F5))
            {
                captureSweetSpot();
            }
        }

        protected void captureSweetSpot()
        {
            if (screenManager.Kinect.IsViewerActive())
            {
                addSweetSpot(screenManager.Kinect.GetViewerPosition());
            }
        }

        protected void addSweetSpot(Vector2 position)
        {
            int id = sweetspots.Count;
            sweetspots.Add(position);
            screenManager.Database.SaveSweetSpot(id, position);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            graphics.Clear(Color.Black);
            spriteBatch.Begin();
            leftSensorPanel.Draw(spriteBatch);
            rightSensorPanel.Draw(spriteBatch);
            spriteBatch.Draw(white, separator, Color.White);
            spriteBatch.End();
        }

        protected void loadCalibration()
        {
            string leftSensorID = leftSensorPanel.GetSensorID();
            string rightSensorID = rightSensorPanel.GetSensorID();

            if (screenManager.Database.HasCalibrationDataFor(leftSensorID)
                && screenManager.Database.HasCalibrationDataFor(rightSensorID))
            {
                leftSensorPanel.Calibrate(screenManager.Database.GetCalibration(leftSensorID));
                rightSensorPanel.Calibrate(screenManager.Database.GetCalibration(rightSensorID));
            }
        }
    }
}
