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
        Texture2D compassContent;
        Rectangle compass;
        Rectangle compassNeedle;
        int compassSize = 400;
        float compassOrientation;
        bool viewerDetected = false;

        public BaselineArrowScreen(ScreenManager screenManager, Texture2D image)
            : base(screenManager, image)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            black = content.Load<Texture2D>("texture\\black");
            arrow = content.Load<Texture2D>("texture\\arrow");
            checkMark = content.Load<Texture2D>("texture\\checkmark");
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            compass = new Rectangle(
                viewport.Width - compassSize,
                viewport.Height - compassSize,
                compassSize,
                compassSize
            );
            compassNeedle = new Rectangle(
                (int)(compass.Center.X),
                (int)(compass.Center.Y),
                (int)(0.9f * compass.Width),
                (int)(0.9f * compass.Height)
            );

            compassContent = arrow;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!ScreenManager.Kinect.IsViewerActive())
            {
                viewerDetected = false;
                return;
            }

            viewerDetected = true;
            Vector2 vectorToSweetspot = ScreenManager.Kinect.GetVectorToSweetSpot();
            compassOrientation = (float)Math.Atan2(vectorToSweetspot.Y, vectorToSweetspot.X);

            if (ScreenManager.Kinect.GetDistanceFromSweetSpot() < 0.05)
            {
                compassOrientation = 0f;
                compassContent = checkMark;
            }
            else
            {
                compassContent = arrow;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(black, compass, Color.White);
            if (viewerDetected)
                spriteBatch.Draw(compassContent, compassNeedle, null, Color.Wheat, (float)compassOrientation, new Vector2(compassContent.Bounds.Center.X, compassContent.Bounds.Center.Y), SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
