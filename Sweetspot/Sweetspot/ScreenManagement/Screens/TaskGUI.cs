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

        protected Texture2D green;
        protected Texture2D red;
        protected Texture2D image;
        protected SpriteFont questionFont;
        protected Rectangle imageRect;
        protected int questionBoxHeight;
        protected List<string> lines = new List<string>();
        protected List<Vector2> linePosition = new List<Vector2>();

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

        protected void splitRows(List<string> words, int lineSize)
        {
            while (words.Count > 0)
            {
                var firstLine = getFirstLine(words, lineSize);
                lines.Add(String.Join(" ", firstLine));
                words = words.GetRange(firstLine.Count, words.Count - firstLine.Count);
            }
        }

        protected List<string> getFirstLine(List<string> words, int lineSize)
        {
            int maxLength = 0;
            for (int wordCount = 1; wordCount <= words.Count; wordCount++)
            {
                string line = String.Join(" ", words.GetRange(0, wordCount));
                if (questionFont.MeasureString(line).X <= lineSize)
                    maxLength = wordCount;
                else
                    break;
            }
            return words.GetRange(0, maxLength);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Viewport viewport = gc.GraphicsDevice.Viewport;

            List<string> questionWords = new List<string> (
                questionText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                );

            splitRows(questionWords, viewport.Width - 2 * QUESTION_HORIZONTAL_MARGIN);

            Vector2 questionLineBound;
            for (int i = 0; i < lines.Count ; i++)
            {
                questionLineBound = questionFont.MeasureString(lines[i]);
                float questionX = (viewport.Width - questionLineBound.X) / 2;
                float questionY = QUESTION_VERTICAL_MARGIN + i * (QUESTION_LINE_SPACING + questionFont.MeasureString(lines[i]).Y);
                linePosition.Add(new Vector2(questionX, questionY));
            }

            questionBoxHeight = (int)questionFont.MeasureString(lines[0]).Y * lines.Count + 2 * QUESTION_VERTICAL_MARGIN + QUESTION_LINE_SPACING * (lines.Count - 1);

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
            for (int i = 0; i < lines.Count; i++)
                spriteBatch.DrawString(questionFont, lines[i], linePosition[i], Color.Black);
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
                    break;

                case TaskState.Completing:
                    drawTimerBar(bar, green, timeSinceStateChange, TASK_COMPLETE_TIME);
                    break;

                case TaskState.GracePeriod:
                    drawTimerBar(bar, green, completionTimerSnapshot, TASK_COMPLETE_TIME);
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
    }
}
