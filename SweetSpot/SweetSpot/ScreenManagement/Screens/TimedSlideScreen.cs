using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot.ScreenManagement.Screens
{
    public class TimedSlideScreen : SlideScreen
    {
        protected float secondsVisible;
        protected float secondsPassed;

        public TimedSlideScreen(ScreenManager screenManager, Texture2D image, float secondsVisible)
            : base(screenManager, image)
        {
            this.secondsVisible = secondsVisible;
        }

        public override void Initialize()
        {
            base.Initialize();
            secondsPassed = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            secondsPassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            if (secondsPassed <= secondsVisible)
                base.Draw(gameTime);
            else
                screenManager.Game.GraphicsDevice.Clear(background);
        }
    }
}
