using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class EffectScreen : TestScreen
    {
        protected static readonly float INTENSITY_SMOOTHING_FACTOR = 20f;

        protected Effect effect;
        protected float currentEffectIntensity = 1f;
        protected float targetEffectIntensity = 1f;

        protected Texture2D green;
        protected Texture2D red;

        public EffectScreen(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot, Effect effect)
            : base(gc, cue, mapping, sweetspot)
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

            if (gc.Debug)
            {
                // Overlay
                Vector2 sweetspotPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, gc.Kinect.sweetspot.Position);
                Vector2 userPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, gc.Kinect.GetUserPosition());
                Rectangle sweetspot = new Rectangle((int)sweetspotPosition.X - 10, (int)sweetspotPosition.Y - 10, 20, 20);
                Rectangle user = new Rectangle((int)userPosition.X - 15, (int)userPosition.Y - 15, 30, 30);
                spriteBatch.Begin();
                spriteBatch.Draw(green, sweetspot, Color.White);
                spriteBatch.Draw(red, user, Color.White);
                spriteBatch.End();
            }
        }
    }
}
