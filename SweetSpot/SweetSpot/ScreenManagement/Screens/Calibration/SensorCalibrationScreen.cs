﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SweetSpot.Input;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement.Screens
{
    class SensorCalibrationScreen : Screen
    {
        Texture2D white;
        Rectangle separator;
        SensorPanel leftSensorPanel;
        SensorPanel rightSensorPanel;
        public ConvexHull sweetSpotBounds;
        protected bool unsavedSweetspotBounds = false;

        public SensorCalibrationScreen(ScreenManager screenManager)
            : base(screenManager)
        {
            sweetSpotBounds = new ConvexHull();
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
            loadSweetSpotBounds();
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
                        leftSensorPanel.Calibrate(calibration1);
                        rightSensorPanel.Calibrate(calibration2);
                    }
                    catch (InvalidOperationException) { }
                }
            }

            if (screenManager.Input.IsGamePadButtonPressed(Buttons.X) || screenManager.Input.IsKeyPressed(Keys.F5))
            {
                captureSweetSpot();
            }

            if (screenManager.Input.IsGamePadButtonPressed(Buttons.LeftTrigger) || screenManager.Input.IsKeyPressed(Keys.F6))
            {
                clearSweetSpots();
            }

            if (screenManager.Input.IsGamePadButtonPressed(Buttons.LeftShoulder) || screenManager.Input.IsKeyPressed(Keys.F9))
            {
                screenManager.Kinect.ResetSensorTilt();
            }
        }

        protected void captureSweetSpot()
        {
            if (screenManager.Kinect.IsViewerActive())
            {
                sweetSpotBounds.Add(screenManager.Kinect.GetViewerPosition());
                unsavedSweetspotBounds = true;
            }
        }

        protected void clearSweetSpots()
        {
            sweetSpotBounds.Clear();
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

        public override void SkipAction(GameTime gameTime)
        {
            base.SkipAction(gameTime);
            if (unsavedSweetspotBounds)
                screenManager.Database.SaveSweetSpotBounds(sweetSpotBounds.GetPoints());
        }

        protected void loadSweetSpotBounds()
        {
            var bounds = screenManager.Database.LoadSweetSpotBounds();
            foreach (var point in bounds)
                sweetSpotBounds.Add(point);
        }
    }
}
