using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class TestScreen : TrackingScreen
    {
        protected static readonly int QUESTION_TEXT_MARGIN = 20;

        protected int questionId;
        protected string questionText;
        protected string answerFilename;
        protected Texture2D image;
        protected SpriteFont questionFont;
        protected Rectangle imageRect;
        protected int questionHeight;
        protected Vector2 questionPosition;

        public TestScreen(GameController sm, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(sm, cue, mapping, sweetspot)
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
            sm.GraphicsDevice.Clear(background);

            spriteBatch.Begin();
            spriteBatch.Draw(image, imageRect, Color.White);
            spriteBatch.DrawString(questionFont, questionText, questionPosition, Color.Black);
            spriteBatch.End();
        }
    }
}
