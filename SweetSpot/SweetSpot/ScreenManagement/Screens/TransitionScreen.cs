using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot.ScreenManagement.Screens
{
    public enum TransitionState { PreDelay, FadingIn, Active, FadingOut, PostDelay }

    class TransitionScreen : Screen
    {
        SpriteFont font;
        string caption;
        Vector2 textPosition;
        TransitionState state;
        float timeSinceStateChange = 0f;    // in ms
        float fadeTime = 300;               // in ms
        float delay = 300;                  // in ms
        float alpha = 0f;

        public TransitionScreen(ScreenManager screenManager, string caption)
            : base(screenManager)
        {
            this.caption = caption;
            textPosition = new Vector2();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            font = Content.Load<SpriteFont>("font\\segoe_72");
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            Vector2 textSize = font.MeasureString(caption);
            textPosition = new Vector2((viewport.Width - textSize.X) / 2, (viewport.Height - textSize.Y) / 2);
        }

        protected override void initialize(GameTime gameTime)
        {
            base.initialize(gameTime);
            changeState(TransitionState.PreDelay, gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            timeSinceStateChange += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            switch (state)
            {
                case TransitionState.PreDelay:
                    if (timeSinceStateChange >= delay)
                        changeState(TransitionState.FadingIn, gameTime);
                    break;

                case TransitionState.FadingIn:
                    if (timeSinceStateChange >= fadeTime)
                    {
                        alpha = 1f;
                        changeState(TransitionState.Active, gameTime);
                    }
                    else
                    {
                        alpha = timeSinceStateChange / fadeTime;
                    }
                    break;

                case TransitionState.Active:
                    break;

                case TransitionState.FadingOut:
                    if (timeSinceStateChange >= fadeTime)
                    {
                        alpha = 0f;
                        changeState(TransitionState.PostDelay, gameTime);
                    }
                    else
                    {
                        alpha = 1f - (timeSinceStateChange / fadeTime);
                    }
                    break;

                case TransitionState.PostDelay:
                    if (timeSinceStateChange >= delay)
                        ExitScreen(gameTime);
                    break;
            }
        }

        protected void changeState(TransitionState newState, GameTime gameTime)
        {
            state = newState;
            timeSinceStateChange = 0f;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            screenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, caption, textPosition, Color.White * alpha);
            spriteBatch.End();
        }

        public override void SkipAction(GameTime gameTime)
        {
            if (TransitionState.Active == state)
            {
                changeState(TransitionState.FadingOut, gameTime);
            }
        }
    }
}
