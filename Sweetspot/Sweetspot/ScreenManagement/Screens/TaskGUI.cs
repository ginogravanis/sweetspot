using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class TaskGUI : TrackingScreen
    {
        protected static readonly int QUESTION_TEXT_MARGIN = 20;

        protected Texture2D green;
        protected Texture2D red;
        protected Texture2D image;
        protected SpriteFont questionFont;
        protected Rectangle imageRect;
        protected int questionHeight;
        protected Vector2 questionPosition;

        public TaskGUI(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(gc, cue, mapping, sweetspot)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            green = Content.Load<Texture2D>("texture\\green");
            red = Content.Load<Texture2D>("texture\\red");
            image = Content.Load<Texture2D>(@"answers\" + answerFilename);
            questionFont = Content.Load<SpriteFont>(@"font\segoe_36");
        }

        public override void Initialize()
        {
            base.Initialize();
            Viewport viewport = gc.GraphicsDevice.Viewport;

            var questionBounds = questionFont.MeasureString(questionText);
            questionHeight = (int)questionBounds.Y + 2 * QUESTION_TEXT_MARGIN;
            float questionX = (viewport.Width - questionBounds.X) / 2;
            float questionY = QUESTION_TEXT_MARGIN;
            questionPosition = new Vector2(questionX, questionY);

            imageRect = new Rectangle(
                0,
                questionHeight,
                viewport.Width,
                viewport.Height - questionHeight
                );
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.Draw(image, imageRect, Color.White);
            spriteBatch.DrawString(questionFont, questionText, questionPosition, Color.Black);
            spriteBatch.End();
        }

        protected void drawDebug()
        {
            if (gc.Debug)
            {
                Vector2 sweetspotPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, gc.Kinect.sweetspot.Position);
                Vector2 userPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, gc.Kinect.GetUserPosition());
                Rectangle sweetspot = new Rectangle((int)sweetspotPosition.X - 10, (int)sweetspotPosition.Y - 10, 20, 20);
                Rectangle userRect = new Rectangle((int)userPosition.X - 15, (int)userPosition.Y - 15, 30, 30);
                spriteBatch.Begin();
                spriteBatch.Draw(green, sweetspot, Color.White);
                spriteBatch.Draw(red, userRect, Color.White);
                spriteBatch.End();
            }
        }

        protected void drawTimer(Rectangle bar)
        {
            switch (currentState)
            {
                case TaskState.Active:
                    return;

                case TaskState.Aborting:
                    drawTimerBar(bar, red, TASK_ABORT_TIME);
                    break;

                case TaskState.Completing:
                    drawTimerBar(bar, green, TASK_COMPLETE_TIME);
                    break;

                case TaskState.GracePeriod:
                    drawTimerBar(bar, green, TASK_COMPLETE_TIME);
                    break;
            }
        }

        protected void drawTimerBar(Rectangle bar, Texture2D color, double period)
        {
            double barWidthScale = timeSinceStateChange / period;
            Rectangle scaledBar = new Rectangle(bar.X, bar.Y, (int)(bar.Width * barWidthScale), bar.Height);
            spriteBatch.Begin();
            spriteBatch.Draw(color, scaledBar, Color.White);
            spriteBatch.End();
        }
    }
}
