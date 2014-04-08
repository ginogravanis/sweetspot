using Microsoft.Xna.Framework;

namespace SweetSpot.ScreenManagement.Screens
{
    public class AutoTransitionScreen : TransitionScreen
    {
        const float ACTIVE_TIME = 300;      // in ms

        public AutoTransitionScreen(ScreenManager screenManager, string caption)
            : base(screenManager, caption) { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (currentState == TransitionState.Active)
            {
                if (timeSinceStateChange >= ACTIVE_TIME)
                    changeState(TransitionState.FadingOut);
            }
        }
    }
}
