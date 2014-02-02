using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public class EffectScreen : Screen
    {
        ContentManager content;
        RenderTarget2D screen;
        Texture2D image;
        Effect effect;
        float effectAmount = 1f;

        public EffectScreen(Texture2D image, Effect effect)
        {
            this.image = image;
            this.effect = effect;
        }

        public override void LoadContent()
        {
            content = new ContentManager(ScreenManager.Game.Services, "Content");
            screen = new RenderTarget2D(ScreenManager.GraphicsDevice,
                ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth,
                ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateEffect(gameTime);

            if (ScreenManager.Input.IsKeyDown(Keys.Escape))
            {
                ScreenManager.Game.Exit();
            }
        }

        private void UpdateEffect(GameTime gameTime)
        {
            if (ScreenManager.Kinect.IsViewerActive())
            {
                effectAmount = ScreenManager.Kinect.GetDistanceFromSweetSpot() / ScreenManager.Kinect.sweetSpotMargin;
            }
            else
            {
                effectAmount = 1.0f;
            }
            effect.Parameters["effectAmount"].SetValue(effectAmount);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }
    }
}
