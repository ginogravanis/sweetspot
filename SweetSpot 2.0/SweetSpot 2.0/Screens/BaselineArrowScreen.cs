using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    class BaselineArrowScreen : ImageScreen
    {
        Texture2D black;
        Texture2D arrow;
        Texture2D checkMark;
        Rectangle compass;
        Rectangle arrowRect;
        Rectangle checkMarkRect;
        int compassSize = 405;
        float compassOrientation;
        bool viewerDetected = false;
        bool targetReached = false;
        Effect perspectiveShader;

        public BaselineArrowScreen(ScreenManager screenManager, Texture2D image)
            : base(screenManager, image)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            black = Content.Load<Texture2D>("texture\\black");
            arrow = Content.Load<Texture2D>("texture\\arrow");
            checkMark = Content.Load<Texture2D>("texture\\checkmark");
            perspectiveShader = Content.Load<Effect>("shader\\PerspectiveShader");
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            compass = new Rectangle(
                viewport.Width - compassSize,
                viewport.Height - compassSize,
                compassSize,
                compassSize
            );
            arrowRect = new Rectangle(
                compass.Left + (compassSize - arrow.Bounds.Width) / 2,
                compass.Bottom - arrow.Bounds.Height / 3,
                arrow.Bounds.Width,
                arrow.Bounds.Height / 3
            );
            checkMarkRect = new Rectangle(
                compass.Left + (compassSize - checkMark.Bounds.Width) / 2,
                compass.Top + (compassSize - checkMark.Bounds.Height) / 2,
                checkMark.Bounds.Width,
                checkMark.Bounds.Height
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

            targetReached = screenManager.Kinect.GetDistanceFromSweetSpot() < 0.05;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = screenManager.SpriteBatch;
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
        }
    }
}
