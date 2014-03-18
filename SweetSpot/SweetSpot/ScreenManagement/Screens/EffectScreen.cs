using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Input;

namespace SweetSpot.ScreenManagement.Screens
{
    public class EffectScreen : ImageScreen
    {
        Effect effect;
        float currentEffectIntensity = 1f;
        float targetEffectIntensity = 1f;
        const float INTENSITY_SMOOTHING_FACTOR = 20f;

        Texture2D green;
        Texture2D red;

        public EffectScreen(ScreenManager screenManager, string cue, Vector2 sweetSpot, Effect effect)
            : base(screenManager, cue, sweetSpot)
        {
            this.effect = effect;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            green = Content.Load<Texture2D>("texture\\green");
            red = Content.Load<Texture2D>("texture\\red");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateEffect(gameTime);
        }

        protected void UpdateEffect(GameTime gameTime)
        {
            targetEffectIntensity = 1.0f;

            if (screenManager.Kinect.IsViewerActive())
            {
                float distanceFromSweetSpot = screenManager.Kinect.GetDistanceFromSweetSpot();
                float margin = screenManager.Kinect.sweetSpotMargin;

                if (distanceFromSweetSpot <= margin)
                {
                    targetEffectIntensity = screenManager.Kinect.GetDistanceFromSweetSpot() / screenManager.Kinect.sweetSpotMargin;
                }
            }

            currentEffectIntensity = WeightedAverage(currentEffectIntensity, targetEffectIntensity, INTENSITY_SMOOTHING_FACTOR);
        }

        public float WeightedAverage(float current, float target, float weight)
        {
            return ((current * (weight - 1)) + target) / weight;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            effect.Parameters["width"].SetValue(image.Width);
            effect.Parameters["height"].SetValue(image.Height);
            effect.Parameters["elapsedTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
            effect.Parameters["effectIntensity"].SetValue(currentEffectIntensity);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();

            if (screenManager.Debug)
            {
                // Overlay
                Vector2 sweetSpotPosition = SensorManager.WorldToScreenCoords(viewport.Bounds, screenManager.Kinect.sweetSpot);
                Vector2 viewerPosition = SensorManager.WorldToScreenCoords(viewport.Bounds, screenManager.Kinect.GetViewerPosition());
                Rectangle sweetSpot = new Rectangle((int)sweetSpotPosition.X - 10, (int)sweetSpotPosition.Y - 10, 20, 20);
                Rectangle viewer = new Rectangle((int)viewerPosition.X - 15, (int)viewerPosition.Y - 15, 30, 30);
                spriteBatch.Begin();
                spriteBatch.Draw(green, sweetSpot, Color.White);
                spriteBatch.Draw(red, viewer, Color.White);
                spriteBatch.End();
            }
        }
    }
}
