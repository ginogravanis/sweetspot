﻿using Microsoft.Xna.Framework;
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
        float alpha = 0f;
        float timeSinceStateChange = 0f;    // in ms
        const float FADE_TIME = 300;         // in ms
        const float DELAY = 300;            // in ms

        public TransitionScreen(ScreenManager screenManager, string caption)
            : base(screenManager)
        {
            this.caption = caption;
            textPosition = new Vector2();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            font = Content.Load<SpriteFont>(@"font\segoe_72");
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            Vector2 textSize = font.MeasureString(caption);
            textPosition = new Vector2((viewport.Width - textSize.X) / 2, (viewport.Height - textSize.Y) / 2);
        }

        public override void Initialize()
        {
            base.Initialize();
            changeState(TransitionState.PreDelay);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            timeSinceStateChange += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            switch (state)
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
                        ExitScreen(gameTime);
                    break;
            }
        }

        protected void changeState(TransitionState newState)
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
                changeState(TransitionState.FadingOut);
            }
        }
    }
}
