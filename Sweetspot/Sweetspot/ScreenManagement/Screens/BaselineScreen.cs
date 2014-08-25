using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class BaselineScreen : TaskGUI
    {
        protected static readonly float FADE_TIME = 200;    // in ms
        protected static readonly int COMPASS_WIDTH = 405;
        protected static readonly int COMPASS_HEIGHT = 200;
        protected static readonly int TIMER_HEIGHT = 50;

        protected bool userDetected = false;
        protected bool targetReached = false;
        protected float alpha = 1;

        protected Texture2D arrow;
        protected Texture2D checkMark;
        protected Rectangle compass;
        protected Rectangle arrowRect;
        protected Rectangle checkMarkRect;
        protected float compassOrientation;
        protected Effect perspectiveShader;
        protected Rectangle timer;

        public BaselineScreen(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            :base(gc, cue, mapping, sweetspot)
        {
            int screenHeight = gc.GraphicsDevice.Viewport.Height;
            int screenWidth = gc.GraphicsDevice.Viewport.Width;
            timer = new Rectangle(0, screenHeight - TIMER_HEIGHT, screenWidth, TIMER_HEIGHT);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            arrow = Content.Load<Texture2D>("texture\\arrow");
            checkMark = Content.Load<Texture2D>("texture\\checkmark");
            perspectiveShader = Content.Load<Effect>("shader\\PerspectiveShader");

            Viewport viewport = gc.GraphicsDevice.Viewport;
            compass = new Rectangle(
                viewport.Width - COMPASS_WIDTH,
                viewport.Height - COMPASS_HEIGHT - timer.Height,
                COMPASS_WIDTH,
                COMPASS_HEIGHT
            );
            arrowRect = new Rectangle(
                compass.Left + (COMPASS_WIDTH - arrow.Bounds.Width) / 2,
                compass.Bottom - arrow.Bounds.Height / 3 - timer.Height,
                arrow.Bounds.Width,
                arrow.Bounds.Height / 3
            );
            checkMarkRect = new Rectangle(
                compass.Left + (COMPASS_WIDTH - checkMark.Bounds.Width / 2) / 2,
                compass.Top + (COMPASS_WIDTH - checkMark.Bounds.Height) / 2 - timer.Height,
                checkMark.Bounds.Width / 2,
                checkMark.Bounds.Height / 2
            );
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!gc.Kinect.IsUserActive())
            {
                userDetected = false;
                return;
            }
            userDetected = true;

            Vector2 userPosition = gc.Kinect.GetUserPosition();
            Vector2 vectorToSweetspot = gc.Kinect.sweetspot.GetVectorToSweetspot(userPosition);
            compassOrientation = (float)(2 * Math.PI - Math.Atan2(vectorToSweetspot.Y, vectorToSweetspot.X));

            targetReached = gc.Kinect.sweetspot.IsUserAnswering(userPosition);
            if (targetReached)
                alpha = Math.Max(alpha - (gameTime.ElapsedGameTime.Milliseconds / FADE_TIME), 0);
            else
                alpha = Math.Min(alpha + (gameTime.ElapsedGameTime.Milliseconds / FADE_TIME), 1);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.Draw(black, imageRect, background * alpha);
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
            drawDebug();
            drawTimer(timer);
            drawAnswerText();
        }
    }
}
