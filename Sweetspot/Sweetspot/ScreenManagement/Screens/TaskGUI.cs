using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class TaskGUI : TrackingScreen
    {
        protected static readonly int QUESTION_VERTICAL_MARGIN = 5;
        protected static readonly int QUESTION_LINE_SPACING = -10;
        protected static readonly int QUESTION_HORIZONTAL_MARGIN = 20;
        protected static readonly float ANSWER_HEIGHT = 0.5f;
        protected static readonly float ANSWER_PERCENTAGE = 0.25f;
        protected static readonly float ANSWER_BOX_OPACITY = 0.75f;
        protected static readonly int ANSWER_BOX_MARGIN = 15;
        protected static readonly int TIMER_HEIGHT = 50;
        protected static readonly string COMPLETE_TIMER_CAPTION = "Next question in {0}...";
        protected static readonly string ABORT_TIMER_CAPTION = "Game ends in {0}...";
        protected static readonly string TIMEOUT_CAPTION = "Skipping question in {0}...";

        protected Texture2D black;
        protected Texture2D green;
        protected Texture2D yellow;
        protected Texture2D red;
        protected Texture2D image;
        protected SpriteFont questionFont;
        protected SpriteFont answerFont;
        protected Rectangle imageRect;
        protected Rectangle answerRect;
        protected int questionBoxHeight;
        protected List<string> questionLines = new List<string>();
        protected List<string> answerLines = new List<string>();
        protected List<Vector2> questionLinePositions = new List<Vector2>();
        protected List<Vector2> answerLinePositions = new List<Vector2>();
        protected Rectangle answerBox = new Rectangle();
        protected Rectangle timerBar;
        protected BarTimer completeBarTimer;
        protected BarTimer abortBarTimer;
        protected BarTimer timeoutBarTimer;
        protected BarTimer activeBarTimer;

        public TaskGUI(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(gc, cue, mapping, sweetspot)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            black = Content.Load<Texture2D>(@"texture\black");
            green = Content.Load<Texture2D>(@"texture\green");
            yellow = Content.Load<Texture2D>(@"texture\yellow");
            red = Content.Load<Texture2D>(@"texture\red");
            image = Content.Load<Texture2D>(@"answers\" + answerFilename);
            questionFont = Content.Load<SpriteFont>(@"font\segoe_36b");
            answerFont = Content.Load<SpriteFont>(@"font\segoe_44b");
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Viewport viewport = gc.GraphicsDevice.Viewport;

            questionLines = SplitString.SplitRows(questionText, viewport.Width - 2 * QUESTION_HORIZONTAL_MARGIN, questionFont);
            answerLines = SplitString.SplitRows(answerText, viewport.Width/2, questionFont);

            Vector2 questionLineBound;
            for (int i = 0; i < questionLines.Count ; i++)
            {
                questionLineBound = questionFont.MeasureString(questionLines[i]);
                float questionX = (viewport.Width - questionLineBound.X) / 2;
                float questionY = QUESTION_VERTICAL_MARGIN + i * (QUESTION_LINE_SPACING + questionFont.MeasureString(questionLines[i]).Y);
                questionLinePositions.Add(new Vector2(questionX, questionY));
            }

            questionBoxHeight = (int)questionFont.MeasureString(questionLines[0]).Y * questionLines.Count +
                2 * QUESTION_VERTICAL_MARGIN +
                QUESTION_LINE_SPACING * (questionLines.Count - 1);

            int minAnswerLineX = viewport.Width;
            int maxAnswerLineX = 0;
            int minAnswerLineY = viewport.Height;
            int maxAnswerLineY = 0;
            Vector2 answerLineDimensions;
            for (int i = 0; i < answerLines.Count; i++)
            {
                answerLineDimensions = answerFont.MeasureString(answerLines[i]);
                float answerX = (viewport.Width - answerLineDimensions.X) / 2;
                float answerY = ANSWER_HEIGHT * viewport.Height + i * (QUESTION_LINE_SPACING + questionFont.MeasureString(answerLines[i]).Y);
                answerLinePositions.Add(new Vector2(answerX, answerY));

                minAnswerLineX = (int)Math.Min(minAnswerLineX, answerX - ANSWER_BOX_MARGIN);
                maxAnswerLineX = (int)Math.Max(maxAnswerLineX, answerX + answerLineDimensions.X + ANSWER_BOX_MARGIN);
                minAnswerLineY = (int)Math.Min(minAnswerLineY, answerY);
                maxAnswerLineY = (int)Math.Max(maxAnswerLineY, answerY + answerLineDimensions.Y);
            }
            int answerBoxWidth = maxAnswerLineX - minAnswerLineX;
            int answerBoxHeight = maxAnswerLineY - minAnswerLineY;
            answerBox = new Rectangle(minAnswerLineX, minAnswerLineY, answerBoxWidth, answerBoxHeight);

            imageRect = new Rectangle(
                0,
                questionBoxHeight,
                viewport.Width,
                viewport.Height - questionBoxHeight
                );

            int screenHeight = gc.GraphicsDevice.Viewport.Height;
            int screenWidth = gc.GraphicsDevice.Viewport.Width;
            timerBar = new Rectangle(0, viewport.Height - TIMER_HEIGHT, viewport.Width, TIMER_HEIGHT);
            completeBarTimer = new BarTimer(gc, timerBar, green, COMPLETE_TIMER_CAPTION, TASK_COMPLETE_TIME, yellow);
            abortBarTimer = new BarTimer(gc, timerBar, red, ABORT_TIMER_CAPTION, TASK_ABORT_TIME);
            timeoutBarTimer = new BarTimer(gc, timerBar, red, TIMEOUT_CAPTION, TASK_TIMEOUT_TIME);
            activeBarTimer = null;
            completeBarTimer.LoadContent();
            abortBarTimer.LoadContent();
            timeoutBarTimer.LoadContent();
            completeBarTimer.Initialize();
            abortBarTimer.Initialize();
            timeoutBarTimer.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (activeBarTimer != null)
                activeBarTimer.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.Draw(image, imageRect, Color.White);
            for (int i = 0; i < questionLines.Count; i++)
                spriteBatch.DrawString(questionFont, questionLines[i], questionLinePositions[i], Color.Black);
            spriteBatch.End();
        }

        protected void drawDebug()
        {
            Viewport viewport = gc.GraphicsDevice.Viewport;
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

        protected override void changeGameState(GameState gameState)
        {
            base.changeGameState(gameState);
            switch (currentGameState)
            {
                case GameState.Active:
                    completeBarTimer.Stop();
                    timeoutBarTimer.Stop();
                    abortBarTimer.Stop();
                    activeBarTimer = null;
                    break;
                case GameState.Aborting:
                    abortBarTimer.Start();
                    activeBarTimer = abortBarTimer;
                    break;
                case GameState.Completing:
                    completeBarTimer.Start();
                    activeBarTimer = completeBarTimer;
                    break;
                case GameState.GracePeriod:
                    completeBarTimer.Pause();
                    break;
                case GameState.Timeout:
                    timeoutBarTimer.Start();
                    activeBarTimer = timeoutBarTimer;
                    break;
            }
        }

        protected void drawAnswerText()
        {
            if (GameState.Completing != currentGameState)
                return;
            
            bool showAnswer = timeSinceStateChange / TASK_COMPLETE_TIME >= ANSWER_PERCENTAGE;
            if (!showAnswer)
                return;

            spriteBatch.Begin();
            spriteBatch.Draw(black, answerBox, Color.White * ANSWER_BOX_OPACITY);
            for (int i = 0; i < answerLines.Count; i++)
                spriteBatch.DrawString(answerFont, answerLines[i], answerLinePositions[i], Color.White);
            spriteBatch.End();
        }
    }
}
