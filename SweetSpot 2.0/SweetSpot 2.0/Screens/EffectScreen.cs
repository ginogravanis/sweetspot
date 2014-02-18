using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public class EffectScreen : Screen
    {
        Texture2D image;
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
            effect.Parameters["elapsedTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
            effect.Parameters["effectIntensity"].SetValue(currentEffectIntensity);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }
    }
}
