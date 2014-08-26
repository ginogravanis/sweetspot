using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class BaselineScreen : TaskGUI
    {
        protected static readonly float FADE_TIME = 200;    // in ms
        protected static readonly int ARROW_MARGIN = 400;

        protected bool userDetected = false;
        protected bool targetReached = false;
        protected float alpha = 1;

        protected Texture2D arrow;
        protected Rectangle arrowRect;
        protected float compassOrientation;
        protected Effect perspectiveShader;

        public BaselineScreen(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            :base(gc, cue, mapping, sweetspot)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            arrow = Content.Load<Texture2D>(@"texture\arrow");
            perspectiveShader = Content.Load<Effect>(@"shader\PerspectiveShader");

            Viewport viewport = gc.GraphicsDevice.Viewport;
            arrowRect = new Rectangle(
                (viewport.Width - arrow.Bounds.Width) / 2,
                (viewport.Height - arrow.Bounds.Height) / 2 + ARROW_MARGIN,
                arrow.Bounds.Width,
                arrow.Bounds.Height / 3
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
            if (userDetected && !targetReached)
            {
                    perspectiveShader.Parameters["s"].SetValue((float)Math.Sin(compassOrientation));
                    perspectiveShader.Parameters["c"].SetValue((float)Math.Cos(compassOrientation));
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, perspectiveShader);
                    spriteBatch.Draw(arrow, arrowRect, Color.White);
                    spriteBatch.End();
            }
            drawDebug();
            if (activeBarTimer != null)
                activeBarTimer.Draw(gameTime);
            drawAnswerText();
        }
    }
}
