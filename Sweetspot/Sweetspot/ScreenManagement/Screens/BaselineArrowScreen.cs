using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class BaselineArrowScreen : TrackingScreen
    {
        protected static readonly int COMPASS_WIDTH = 405;
        protected static readonly int COMPASS_HEIGHT = 200;
        protected static readonly float FADE_TIME = 200;    // in ms

        protected Texture2D black;
        protected Texture2D green;
        protected Texture2D red;
        protected Texture2D arrow;
        protected Texture2D checkMark;
        protected Rectangle compass;
        protected Rectangle arrowRect;
        protected Rectangle checkMarkRect;
        protected float compassOrientation;
        protected bool userDetected = false;
        protected bool targetReached = false;
        protected Effect perspectiveShader;
        protected float alpha = 0;

        public BaselineArrowScreen(GameController sm, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(sm, cue, mapping, sweetspot)
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
            Viewport viewport = sm.GraphicsDevice.Viewport;
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
            if (!sm.Kinect.IsUserActive())
            {
                userDetected = false;
                return;
            }

            userDetected = true;
            Vector2 userPosition = sm.Kinect.GetUserPosition();
            Vector2 vectorToSweetspot = sm.Kinect.sweetspot.GetVectorToSweetspot(userPosition);
            compassOrientation = (float)(2*Math.PI - Math.Atan2(vectorToSweetspot.Y, vectorToSweetspot.X));

            targetReached = sm.Kinect.sweetspot.GetDistanceFromSweetspot(userPosition) == 0;
            if (targetReached)
                alpha = Math.Min(alpha + (gameTime.ElapsedGameTime.Milliseconds / FADE_TIME), 1);
            else
                alpha = Math.Max(alpha - (gameTime.ElapsedGameTime.Milliseconds / FADE_TIME), 0);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch spriteBatch = sm.SpriteBatch;
            Viewport viewport = sm.GraphicsDevice.Viewport;

            sm.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White * alpha);
            spriteBatch.Draw(black, compass, Color.White);
            spriteBatch.End();
            if (userDetected)
            {
                if (TaskCompleted || targetReached)
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

            if (sm.Debug)
            {
                // Overlay
                Vector2 sweetspotPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, sm.Kinect.sweetspot.Position);
                Vector2 userPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, sm.Kinect.GetUserPosition());
                Rectangle sweetspot = new Rectangle((int)sweetspotPosition.X - 10, (int)sweetspotPosition.Y - 10, 20, 20);
                Rectangle userRect = new Rectangle((int)userPosition.X - 15, (int)userPosition.Y - 15, 30, 30);
                spriteBatch.Begin();
                spriteBatch.Draw(green, sweetspot, Color.White);
                spriteBatch.Draw(red, userRect, Color.White);
                spriteBatch.End();
            }
        }
    }
}
