using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Input;

namespace SweetSpot.ScreenManagement.Screens
{
    class BaselineArrowScreen : ImageScreen
    {
        const int COMPASS_WIDTH = 405;
        const int COMPASS_HEIGHT = 200;

        Texture2D black;
        Texture2D green;
        Texture2D red;
        Texture2D arrow;
        Texture2D checkMark;
        Rectangle compass;
        Rectangle arrowRect;
        Rectangle checkMarkRect;
        float compassOrientation;
        bool viewerDetected = false;
        bool targetReached = false;
        Effect perspectiveShader;

        public BaselineArrowScreen(ScreenManager screenManager, string cue, Vector2 sweetSpot)
            : base(screenManager, cue, sweetSpot)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            black = Content.Load<Texture2D>("texture\\black");
            green = Content.Load<Texture2D>("texture\\green");
            red = Content.Load<Texture2D>("texture\\red");
            arrow = Content.Load<Texture2D>("texture\\arrow");
            checkMark = Content.Load<Texture2D>("texture\\checkmark");
            perspectiveShader = Content.Load<Effect>("shader\\PerspectiveShader");
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            compass = new Rectangle(
                viewport.Width - COMPASS_WIDTH,
                viewport.Height - COMPASS_HEIGHT,
                COMPASS_WIDTH,
                COMPASS_HEIGHT
            );
            arrowRect = new Rectangle(
                compass.Left + (COMPASS_WIDTH - arrow.Bounds.Width) / 2,
                compass.Bottom - arrow.Bounds.Height / 3,
                arrow.Bounds.Width,
                arrow.Bounds.Height / 3
            );
            checkMarkRect = new Rectangle(
                compass.Left + (COMPASS_WIDTH - checkMark.Bounds.Width / 2) / 2,
                compass.Top + (COMPASS_WIDTH - checkMark.Bounds.Height) / 2,
                checkMark.Bounds.Width / 2,
                checkMark.Bounds.Height / 2
            );
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!screenManager.Kinect.IsViewerActive())
            {
                viewerDetected = false;
                return;
            }

            viewerDetected = true;
            Vector2 vectorToSweetspot = screenManager.Kinect.GetVectorToSweetSpot();
            compassOrientation = (float)(2*Math.PI - Math.Atan2(vectorToSweetspot.Y, vectorToSweetspot.X));

            targetReached = screenManager.Kinect.GetDistanceFromSweetSpot() == 0;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(black, compass, Color.White);
            spriteBatch.End();
            if (viewerDetected)
            {
                if (targetReached)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(checkMark, checkMarkRect, Color.White);
                    spriteBatch.End();
                }
                else
                {
                    perspectiveShader.Parameters["s"].SetValue((float)Math.Sin(compassOrientation));
                    perspectiveShader.Parameters["c"].SetValue((float)Math.Cos(compassOrientation));
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, perspectiveShader);
                    spriteBatch.Draw(arrow, arrowRect, Color.White);
                    spriteBatch.End();
                }
            }

            if (screenManager.Debug)
            {
                // Overlay
                Vector2 sweetSpotPosition = SensorManager.WorldToScreenCoords(viewport.Bounds, screenManager.Kinect.sweetSpot);
                Vector2 viewerPosition = SensorManager.WorldToScreenCoords(viewport.Bounds, screenManager.Kinect.GetViewerPosition());
                Rectangle sweetSpot = new Rectangle((int)sweetSpotPosition.X - 10, (int)sweetSpotPosition.Y - 10, 20, 20);
                Rectangle viewer = new Rectangle((int)viewerPosition.X - 15, (int)viewerPosition.Y - 15, 30, 30);
                spriteBatch.Begin();
                spriteBatch.Draw(green, sweetSpot, Color.White);
                spriteBatch.Draw(red, viewer, Color.White);
                spriteBatch.End();
            }
        }
    }
}
