using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public class EffectScreen : Screen
    {
        RenderTarget2D screen;
        Texture2D image;
        Texture2D green;
        Texture2D red;
        Effect effect;
        float currentEffectIntensity = 1f;
        float targetEffectIntensity = 1f;
        float intensitySmoothingFactor = 20f;

        public EffectScreen(ScreenManager screenManager, Texture2D image, Effect effect)
            : base(screenManager)
        {
            this.image = image;
            this.effect = effect;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            screen = new RenderTarget2D(ScreenManager.GraphicsDevice,
                ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth,
                ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight);
            green = content.Load<Texture2D>("texture\\green");
            red = content.Load<Texture2D>("texture\\red");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateEffect(gameTime);
        }

        private void UpdateEffect(GameTime gameTime)
        {
            if (ScreenManager.Kinect.IsViewerActive())
            {
                targetEffectIntensity = ScreenManager.Kinect.GetDistanceFromSweetSpot() / ScreenManager.Kinect.sweetSpotMargin;
            }
            else
            {
                targetEffectIntensity = 1.0f;
            }
            currentEffectIntensity = WeightedAverage(currentEffectIntensity, targetEffectIntensity, intensitySmoothingFactor);
            effect.Parameters["effectIntensity"].SetValue(currentEffectIntensity);
        }

        public float WeightedAverage(float current, float target, float weight)
        {
            return ((current * (weight - 1)) + target) / weight;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            effect.Parameters["width"].SetValue(image.Width);
            effect.Parameters["height"].SetValue(image.Height);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();

            // Overlay
            Vector2 sweetSpotPosition = WorldToScreenCoords(viewport.Bounds, ScreenManager.Kinect.sweetSpot);
            Vector2 viewerPosition = WorldToScreenCoords(viewport.Bounds, ScreenManager.Kinect.GetViewerPosition());
            Rectangle sweetSpot = new Rectangle((int)sweetSpotPosition.X - 10, (int)sweetSpotPosition.Y - 10, 20, 20);
            Rectangle viewer = new Rectangle((int)viewerPosition.X - 15, (int)viewerPosition.Y - 15, 30, 30);
            spriteBatch.Begin();
            spriteBatch.Draw(green, sweetSpot, Color.White);
            spriteBatch.Draw(red, viewer, Color.White);
            spriteBatch.End();
        }

        private Vector2 WorldToScreenCoords(Rectangle viewport, Vector2 position)
        {
            float x = (viewport.Width / 2) + (int)(position.X * 250);
            float y = (int)(position.Y * 250);
            return new Vector2(x, y);
        }
    }
}
