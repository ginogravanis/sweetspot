using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0.ScreenManagement.Screens
{
    public class EffectScreen : ImageScreen
    {
        Effect effect;
        float currentEffectIntensity = 1f;
        float targetEffectIntensity = 1f;
        float intensitySmoothingFactor = 20f;

        public EffectScreen(ScreenManager screenManager, Texture2D image, Effect effect)
            : base(screenManager, image)
        {
            this.effect = effect;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateEffect(gameTime);
        }

        protected void UpdateEffect(GameTime gameTime)
        {
            if (screenManager.Kinect.IsViewerActive())
            {
                targetEffectIntensity = screenManager.Kinect.GetDistanceFromSweetSpot() / screenManager.Kinect.sweetSpotMargin;
            }
            else
            {
                targetEffectIntensity = 1.0f;
            }
            currentEffectIntensity = WeightedAverage(currentEffectIntensity, targetEffectIntensity, intensitySmoothingFactor);
        }

        public float WeightedAverage(float current, float target, float weight)
        {
            return ((current * (weight - 1)) + target) / weight;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            effect.Parameters["width"].SetValue(image.Width);
            effect.Parameters["height"].SetValue(image.Height);
            effect.Parameters["elapsedTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
            effect.Parameters["effectIntensity"].SetValue(currentEffectIntensity);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }
    }
}
