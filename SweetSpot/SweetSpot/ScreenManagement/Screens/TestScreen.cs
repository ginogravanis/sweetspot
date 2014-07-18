﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement.Screens
{
    public class TestScreen : Screen
    {
        const int QUESTION_MARGIN = 20;

        protected int questionId;
        protected string question;
        protected string answerImageFile;
        protected Texture2D image;
        protected SpriteFont questionFont;
        protected Rectangle imageRect;
        protected int questionHeight;
        protected Vector2 questionPosition;

        public TestScreen(ScreenManager screenManager)
            : base(screenManager)
        {
            QuizItem quizItem = screenManager.Database.GetQuestion();
            questionId = quizItem.Id;
            question = quizItem.Question;
            answerImageFile = quizItem.Answer;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            image = Content.Load<Texture2D>(@"texture\answers\" + answerImageFile);
            questionFont = Content.Load<SpriteFont>(@"font\segoe_36");
        }

        public override void Initialize()
        {
            base.Initialize();
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            var questionBounds = questionFont.MeasureString(question);
            questionHeight = (int)questionBounds.Y + 2 * QUESTION_MARGIN;
            float questionX = (viewport.Width - questionBounds.X) / 2;
            float questionY = QUESTION_MARGIN;
            questionPosition = new Vector2(questionX, questionY);

            imageRect = new Rectangle(0, questionHeight, viewport.Width, viewport.Height - questionHeight);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            screenManager.GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(image, imageRect, Color.White);
            spriteBatch.DrawString(questionFont, question, questionPosition, Color.Black);
            spriteBatch.End();
        }
    }
}
