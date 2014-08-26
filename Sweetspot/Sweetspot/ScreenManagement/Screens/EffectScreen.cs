using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class EffectScreen : TaskGUI
    {
        protected static readonly float INTENSITY_SMOOTHING_FACTOR = 20f;

        protected Effect effect;
        protected float currentEffectIntensity = 1f;
        protected float targetEffectIntensity = 1f;

        public EffectScreen(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot, Effect effect)
            : base(gc, cue, mapping, sweetspot)
        {
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

        protected void UpdateEffect(GameTime gameTime)
        {
            targetEffectIntensity = 1.0f;

            if (gc.Kinect.IsUserActive())
            {
                float distanceFromSweetspot = gc.Kinect.sweetspot.GetDistanceFromSweetspot(gc.Kinect.GetUserPosition());
                float margin = gc.Kinect.sweetspot.MAX_INTERACTION_RADIUS;

                if (distanceFromSweetspot <= margin)
                {
                    float x = distanceFromSweetspot / margin;
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
            effect.Parameters["width"].SetValue(image.Width);
            effect.Parameters["height"].SetValue(image.Height);
            effect.Parameters["elapsedTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
            effect.Parameters["effectIntensity"].SetValue(currentEffectIntensity);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(image, imageRect, Color.White);
            spriteBatch.End();
            drawDebug();
            if (activeBarTimer != null)
                activeBarTimer.Draw(gameTime);
            drawAnswerText();
        }
    }
}
