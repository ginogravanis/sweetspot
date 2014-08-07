using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetspotApp.ScreenManagement.Screens
{
    public enum TransitionState { PreDelay, FadingIn, Active, FadingOut, PostDelay }

    public class TitleScreen : Screen
    {
        static readonly float FADE_TIME = 200;  // in ms
        static readonly float DELAY = 300;  // in ms

        protected SpriteFont titleFont;
        protected SpriteFont instructionFont;
        protected string titleText;
        protected string instructionText;
        protected Vector2 titlePosition;
        protected Vector2 instructionPosition;
        protected TransitionState currentState;
        protected float alpha = 0f;
        protected float timeSinceStateChange = 0f;  // in ms
        protected bool userActive = false;

        public TitleScreen(GameController gc)
            : base(gc)
        {
            titleText = "Quiz";
            instructionText = "Step closer to play";
            titlePosition = new Vector2();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            titleFont = Content.Load<SpriteFont>(@"font\segoe_72b");
            instructionFont = Content.Load<SpriteFont>(@"font\segoe_36b");

            Viewport viewport = gc.GraphicsDevice.Viewport;
            Vector2 titleTextSize = titleFont.MeasureString(titleText);
            titlePosition = new Vector2(
                (viewport.Width - titleTextSize.X) / 2,
                (viewport.Height - titleTextSize.Y) / 2
                );
            Vector2 instructionTextSize = instructionFont.MeasureString(instructionText);
            instructionPosition = new Vector2(
                (viewport.Width - instructionTextSize.X) / 2,
                (viewport.Height - instructionTextSize.Y + titleTextSize.Y) / 2 + 25
                );
        }

        public override void Initialize()
        {
            base.Initialize();
            changeState(TransitionState.PreDelay);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            userActive = gc.Kinect.IsUserActive();
            updateTransitionState(gameTime);
        }

        protected void updateTransitionState(GameTime gameTime)
        {
            timeSinceStateChange += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            switch (currentState)
            {
                case TransitionState.PreDelay:
                    if (timeSinceStateChange >= DELAY)
                        changeState(TransitionState.FadingIn);
                    break;

                case TransitionState.FadingIn:
                    if (timeSinceStateChange >= FADE_TIME)
                    {
                        alpha = 1f;
                        changeState(TransitionState.Active);
                    }
                    else
                    {
                        alpha = timeSinceStateChange / FADE_TIME;
                    }
                    break;

                case TransitionState.Active:
                    if (userActive)
                    {
                        changeState(TransitionState.FadingOut);
                    }
                    break;

                case TransitionState.FadingOut:
                    if (timeSinceStateChange >= FADE_TIME)
                    {
                        alpha = 0f;
                        changeState(TransitionState.PostDelay);
                    }
                    else
                    {
                        alpha = 1f - (timeSinceStateChange / FADE_TIME);
                    }
                    break;

                case TransitionState.PostDelay:
                    if (timeSinceStateChange >= DELAY)
                        NextScreen();
                    break;

            }
        }

        protected void changeState(TransitionState newState)
        {
            currentState = newState;
            timeSinceStateChange = 0f;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.DrawString(titleFont, titleText, titlePosition, Color.Black * alpha);
            spriteBatch.DrawString(instructionFont, instructionText, instructionPosition, Color.Black * alpha);
            spriteBatch.End();
        }

        public override void NextScreen()
        {
            switch (currentState)
            {
                case TransitionState.Active:
                    changeState(TransitionState.FadingOut);
                    break;
                case TransitionState.PostDelay:
                    base.NextScreen();
                    gc.NewGame();
                    break;
                default:
                    break;
            }
        }
    }
}
