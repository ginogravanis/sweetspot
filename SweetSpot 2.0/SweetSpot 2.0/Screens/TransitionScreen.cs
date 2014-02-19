using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    public enum TransitionState { PreDelay, FadingIn, Active, FadingOut, PostDelay }

    class TransitionScreen : Screen
    {
        SpriteFont font;
        string caption;
        Vector2 textPosition;
        TransitionState state;
        float timeSinceStateChange = 0f;    // in ms
        float activeTime = 1000f;           // in ms
        float fadeTime = 300;               // in ms
        float paddingTime = 300;            // in ms
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
            font = content.Load<SpriteFont>("font\\segoe_72");
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
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
                    if (timeSinceStateChange >= paddingTime)
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
                    if (timeSinceStateChange >= activeTime)
                    {
                        changeState(TransitionState.FadingOut, gameTime);
                    }                    
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
                    if (timeSinceStateChange >= paddingTime)
                        Finished = true;
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

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, caption, textPosition, Color.White * alpha);
            spriteBatch.End();
        }
    }
}
