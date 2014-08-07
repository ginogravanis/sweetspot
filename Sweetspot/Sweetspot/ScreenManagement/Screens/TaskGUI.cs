using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;
using System.Collections.Generic;
using System;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class TaskGUI : TrackingScreen
    {
        protected static readonly int QUESTION_VERTICAL_MARGIN = 5;
        protected static readonly int QUESTION_LINE_SPACING = -10;
        protected static readonly int QUESTION_HORIZONTAL_MARGIN = 20;
        protected static readonly string COMPLETE_TIMER_CAPTION = "Next question in {0}...";
        protected static readonly string ABORT_TIMER_CAPTION = "Game ends in {0}...";
        protected static int ANSWER_HEIGHT;
        protected static readonly float ANSWER_PERCENTAGE = 0.5f;

        protected Texture2D green;
        protected Texture2D yellow;
        protected Texture2D red;
        protected Texture2D image;
        protected SpriteFont quizFont;
        protected SpriteFont timerFont;
        protected Rectangle imageRect;
        protected Rectangle answerRect;
        protected int questionBoxHeight;
        protected List<string> questionLines = new List<string>();
        protected List<string> answerLines = new List<string>();
        protected List<Vector2> questionLinePositions = new List<Vector2>();
        protected List<Vector2> answerLinePositions = new List<Vector2>();

        public TaskGUI(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(gc, cue, mapping, sweetspot)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            green = Content.Load<Texture2D>("texture\\green");
            yellow = Content.Load<Texture2D>("texture\\yellow");
            red = Content.Load<Texture2D>("texture\\red");
            image = Content.Load<Texture2D>(@"answers\" + answerFilename);
            quizFont = Content.Load<SpriteFont>(@"font\segoe_36b");
            timerFont = Content.Load<SpriteFont>(@"font\segoe_24b");
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Viewport viewport = gc.GraphicsDevice.Viewport;
            ANSWER_HEIGHT = (int)(viewport.Height * .63f);

            questionLines = SplitString.SplitRows(questionText, viewport.Width - 2 * QUESTION_HORIZONTAL_MARGIN, quizFont);
            answerLines = SplitString.SplitRows(answerText, viewport.Width/2, quizFont);

            Vector2 questionLineBound;
            for (int i = 0; i < questionLines.Count ; i++)
            {
                questionLineBound = quizFont.MeasureString(questionLines[i]);
                float questionX = (viewport.Width - questionLineBound.X) / 2;
                float questionY = QUESTION_VERTICAL_MARGIN + i * (QUESTION_LINE_SPACING + quizFont.MeasureString(questionLines[i]).Y);
                questionLinePositions.Add(new Vector2(questionX, questionY));
            }

            questionBoxHeight = (int)quizFont.MeasureString(questionLines[0]).Y * questionLines.Count +
                2 * QUESTION_VERTICAL_MARGIN +
                QUESTION_LINE_SPACING * (questionLines.Count - 1);

            Vector2 answerLineDimensions;
            for (int i = 0; i < answerLines.Count; i++)
            {
                answerLineDimensions = quizFont.MeasureString(answerLines[i]);
                float answerX = (viewport.Width - answerLineDimensions.X) / 2;
                float answerY = ANSWER_PERCENTAGE * viewport.Height + i * (QUESTION_LINE_SPACING + quizFont.MeasureString(answerLines[i]).Y);
                answerLinePositions.Add(new Vector2(answerX, answerY));
            }

            imageRect = new Rectangle(
                0,
                questionBoxHeight,
                viewport.Width,
                viewport.Height - questionBoxHeight
                );
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.Draw(image, imageRect, Color.White);
            for (int i = 0; i < questionLines.Count; i++)
                spriteBatch.DrawString(quizFont, questionLines[i], questionLinePositions[i], Color.Black);
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
                    drawTimerBar(bar, red, timeSinceStateChange, TASK_ABORT_TIME);
                    drawTimerCaption(ABORT_TIMER_CAPTION, bar, timeSinceStateChange, TASK_ABORT_TIME);
                    break;

                case TaskState.Completing:
                    drawTimerBar(bar, green, timeSinceStateChange, TASK_ABORT_TIME);
                    drawTimerCaption(COMPLETE_TIMER_CAPTION, bar, timeSinceStateChange, TASK_COMPLETE_TIME);
                    break;

                case TaskState.GracePeriod:
                    drawTimerBar(bar, yellow, completionTimerSnapshot, TASK_ABORT_TIME);
                    drawTimerCaption(COMPLETE_TIMER_CAPTION, bar, completionTimerSnapshot, TASK_COMPLETE_TIME);
                    break;
            }
        }

        protected void drawTimerBar(Rectangle bar, Texture2D color, double currentTime, double period)
        {
            double barWidthScale = currentTime / period;
            Rectangle scaledBar = new Rectangle(bar.X, bar.Y, (int)(bar.Width * barWidthScale), bar.Height);
            spriteBatch.Begin();
            spriteBatch.Draw(color, scaledBar, Color.White);
            spriteBatch.End();
        }

        protected void drawTimerCaption(string caption, Rectangle captionBounds, double currentTime, double period)
        {
            int lineWidth = captionBounds.Width;
            int timer = (int)Math.Round(period - currentTime);
            string timerText = String.Format(caption, timer);
            float questionX = (captionBounds.Width -  timerFont.MeasureString(timerText).X) / 2;
            Vector2 captionPosition = new Vector2(questionX, captionBounds.Y);

            spriteBatch.Begin();
            spriteBatch.DrawString(timerFont, timerText, captionPosition + new Vector2(1, 1), Color.Black);
            spriteBatch.DrawString(timerFont, timerText, captionPosition + new Vector2(-1, 1), Color.Black);
            spriteBatch.DrawString(timerFont, timerText, captionPosition + new Vector2(1, -1), Color.Black);
            spriteBatch.DrawString(timerFont, timerText, captionPosition + new Vector2(-1, -1), Color.Black);
            spriteBatch.DrawString(timerFont, timerText, captionPosition, Color.White);
            spriteBatch.End();
        }

        protected void drawAnswerText()
        {
            if (TaskState.Completing != currentState)
                return;
            
            bool showAnswer = timeSinceStateChange / TASK_COMPLETE_TIME >= ANSWER_PERCENTAGE;
            if (!showAnswer)
                return;

            spriteBatch.Begin();
            for (int i = 0; i < answerLines.Count; i++)
            {
                spriteBatch.DrawString(quizFont, answerLines[i], answerLinePositions[i] + new Vector2(1, 1), Color.Black);
                spriteBatch.DrawString(quizFont, answerLines[i], answerLinePositions[i] + new Vector2(-1, 1), Color.Black);
                spriteBatch.DrawString(quizFont, answerLines[i], answerLinePositions[i] + new Vector2(1, -1), Color.Black);
                spriteBatch.DrawString(quizFont, answerLines[i], answerLinePositions[i] + new Vector2(-1, -1), Color.Black);
                spriteBatch.DrawString(quizFont, answerLines[i], answerLinePositions[i], Color.White);
            }
            spriteBatch.End();
        }
    }
}
