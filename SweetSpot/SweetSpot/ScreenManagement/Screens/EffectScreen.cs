using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sweetspot.Input;

namespace Sweetspot.ScreenManagement.Screens
{
    public class EffectScreen : TrackingScreen
    {
        Effect effect;
        float currentEffectIntensity = 1f;
        float targetEffectIntensity = 1f;
        const float INTENSITY_SMOOTHING_FACTOR = 20f;

        Texture2D green;
        Texture2D red;

        public EffectScreen(ScreenManager sm, string cue, Mapping mapping, Vector2 sweetspot, Effect effect)
            : base(sm, cue, mapping, sweetspot)
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

            if (sm.Kinect.IsViewerActive())
            {
                float distanceFromSweetspot = sm.Kinect.GetDistanceFromSweetspot();
                float margin = sm.Kinect.sweetspotMargin;

                if (distanceFromSweetspot <= margin)
                {
                    float x = distanceFromSweetspot / margin;
                    System.Diagnostics.Trace.WriteLine(mapping.ToString());
                    switch (mapping)
                    {
                        case Mapping.Linear:
                            targetEffectIntensity = x;
                            break;
                        case Mapping.QuickStart:
                            targetEffectIntensity = (float)-System.Math.Sqrt(1 - System.Math.Pow(x, 2)) + 1;
                            break;
                        case Mapping.SlowStart:
                            targetEffectIntensity = (float)System.Math.Sqrt(1 - System.Math.Pow(x - 1, 2));
                            break;
                        case Mapping.SCurve:
                            targetEffectIntensity = (float)(System.Math.Pow(2 * (x - 0.5), 5) + (2 * (x - 0.5)) + 2) / 4;
                            break;
                        default:
                            throw new System.ArgumentException("Invalid cue type.");
                    }
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
            SpriteBatch spriteBatch = sm.SpriteBatch;
            Viewport viewport = sm.GraphicsDevice.Viewport;

            effect.Parameters["width"].SetValue(image.Width);
            effect.Parameters["height"].SetValue(image.Height);
            effect.Parameters["elapsedTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
            effect.Parameters["effectIntensity"].SetValue(currentEffectIntensity);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(image, imageRect, Color.White);
            spriteBatch.End();

            if (sm.Debug)
            {
                // Overlay
                Vector2 sweetspotPosition = SensorManager.WorldToScreenCoords(viewport.Bounds, sm.Kinect.sweetspot);
                Vector2 viewerPosition = SensorManager.WorldToScreenCoords(viewport.Bounds, sm.Kinect.GetViewerPosition());
                Rectangle sweetspot = new Rectangle((int)sweetspotPosition.X - 10, (int)sweetspotPosition.Y - 10, 20, 20);
                Rectangle viewer = new Rectangle((int)viewerPosition.X - 15, (int)viewerPosition.Y - 15, 30, 30);
                spriteBatch.Begin();
                spriteBatch.Draw(green, sweetspot, Color.White);
                spriteBatch.Draw(red, viewer, Color.White);
                spriteBatch.End();
            }
        }
    }
}
