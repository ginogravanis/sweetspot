using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class BaselineScreen : TaskGUI
    {
        protected static readonly float FADE_TIME = 200;    // in ms

        protected Texture2D black;
        protected bool userDetected = false;
        protected bool targetReached = false;
        protected float alpha = 0;

        public BaselineScreen(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            :base(gc, cue, mapping, sweetspot)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            black = Content.Load<Texture2D>("texture\\black");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!gc.Kinect.IsUserActive())
            {
                userDetected = false;
                return;
            }

            userDetected = true;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), background * alpha);
            spriteBatch.End();
        }
    }
}
