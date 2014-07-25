using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sweetspot.Util;

namespace Sweetspot.ScreenManagement.Screens
{
    public class TestScreen : Screen
    {
        const int QUESTION_TEXT_MARGIN = 20;

        protected int questionId;
        protected string questionText;
        protected string answerFilename;
        protected Texture2D image;
        protected SpriteFont questionFont;
        protected Rectangle imageRect;
        protected int questionHeight;
        protected Vector2 questionPosition;

        public TestScreen(ScreenManager sm)
            : base(sm)
        {
            QuizItem quizItem = sm.Database.GetQuestion();
            questionId = quizItem.Id;
            questionText = quizItem.Question;
            answerFilename = quizItem.AnswerFilename;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            image = Content.Load<Texture2D>(@"answers\" + answerFilename);
            questionFont = Content.Load<SpriteFont>(@"font\segoe_36");
        }

        public override void Initialize()
        {
            base.Initialize();
            Viewport viewport = sm.GraphicsDevice.Viewport;

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
            SpriteBatch spriteBatch = sm.SpriteBatch;
            Viewport viewport = sm.GraphicsDevice.Viewport;
            sm.GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(image, imageRect, Color.White);
            spriteBatch.DrawString(questionFont, questionText, questionPosition, Color.Black);
            spriteBatch.End();
        }
    }
}
