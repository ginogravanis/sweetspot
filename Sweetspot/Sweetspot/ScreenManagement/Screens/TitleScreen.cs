using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;
using System;

namespace SweetspotApp.ScreenManagement.Screens
{
    public enum TransitionState { PreDelay, FadingIn, Active, FadingOut, PostDelay }

    public class TitleScreen : Screen
    {
        protected static readonly float FADE_TIME = 200;  // in ms
        protected static readonly float DELAY = 300;  // in ms
        protected static readonly int TIMER_HEIGHT = 50;
        protected static readonly string START_GAME_TIMER_CAPTION = "Game starts in {0}...";
        protected static readonly float START_GAME_TIMER_DURATION = 7f; // in sec

        protected SpriteFont titleFont;
        protected Texture2D titleImage;
        protected SpriteFont instructionFont;
        protected string titleText;
        protected string instructionText;
        protected Vector2 titlePosition;
        protected Vector2 instructionPosition;
        protected TransitionState currentState;
        protected Rectangle imageRect;
        protected float alpha = 0f;
        protected float timeSinceStateChange = 0f;  // in ms
        protected bool userActive = false;
        protected BarTimer startGameTimer;
        protected Rectangle startGameTimerBar;
        protected Texture2D startGameTimerColor;

        public TitleScreen(GameController gc)
            : base(gc)
        {
            titleText = "Quiz";
            instructionText = "Walk around to play";
            titlePosition = new Vector2();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            titleFont = Content.Load<SpriteFont>(@"font\segoe_72b");
            instructionFont = Content.Load<SpriteFont>(@"font\segoe_36b");
            startGameTimerColor = Content.Load<Texture2D>(@"texture\green");

            Viewport viewport = gc.GraphicsDevice.Viewport;
            Vector2 titleTextSize = titleFont.MeasureString(titleText);
            titlePosition = new Vector2(
                (viewport.Width - titleTextSize.X) / 2,
                (viewport.Height - titleTextSize.Y) / 2 - 100
                );
            Vector2 instructionTextSize = instructionFont.MeasureString(instructionText);
            instructionPosition = new Vector2(
                (viewport.Width - instructionTextSize.X) / 2,
                (viewport.Height - instructionTextSize.Y + titleTextSize.Y) / 2 - 75
                );

            titleImage = Content.Load<Texture2D>(@"texture\pawn");
            imageRect = new Rectangle(
                (int)(viewport.Width - titleImage.Width) / 2,
                (int)(viewport.Height - instructionTextSize.Y + titleTextSize.Y) / 2 + 25,
                titleImage.Width,
                titleImage.Height
                );
        }

        public override void Initialize()
        {
            base.Initialize();
            changeState(TransitionState.PreDelay);

            int screenHeight = gc.GraphicsDevice.Viewport.Height;
            int screenWidth = gc.GraphicsDevice.Viewport.Width;
            startGameTimerBar = new Rectangle(0, screenHeight - TIMER_HEIGHT, screenWidth, TIMER_HEIGHT);
            startGameTimer = new BarTimer(gc, startGameTimerBar, startGameTimerColor,
                START_GAME_TIMER_CAPTION, START_GAME_TIMER_DURATION);
            startGameTimer.LoadContent();
            startGameTimer.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            userActive = gc.Kinect.IsUserActive();
            startGameTimer.Update(gameTime);
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
                        startGameTimer.Start();
                    else
                        startGameTimer.Stop();

                    if (startGameTimer.Expired)
                        changeState(TransitionState.FadingOut);

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
            spriteBatch.Draw(titleImage, imageRect, Color.White * alpha);
            spriteBatch.End();
            if (startGameTimer.Running)
                startGameTimer.Draw(gameTime);
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
