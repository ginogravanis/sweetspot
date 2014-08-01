using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class BaselineScreen : TestScreen
    {
        protected static readonly float FADE_TIME = 200;    // in ms

        protected Texture2D black;
        protected Texture2D green;
        protected Texture2D red;
        protected bool userDetected = false;
        protected bool targetReached = false;
        protected float alpha = 0;

        public BaselineScreen(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            :base(gc, cue, mapping, sweetspot)
        { 
        }

        public override void LoadContent()
        {
            base.LoadContent();
            black = Content.Load<Texture2D>("texture\\black");
            green = Content.Load<Texture2D>("texture\\green");
            red = Content.Load<Texture2D>("texture\\red");

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

        protected void drawDebug()
        {
            Vector2 sweetspotPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, gc.Kinect.sweetspot.Position);
            Vector2 userPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, gc.Kinect.GetUserPosition());
            Rectangle sweetspot = new Rectangle((int)sweetspotPosition.X - 10, (int)sweetspotPosition.Y - 10, 20, 20);
            Rectangle userRect = new Rectangle((int)userPosition.X - 15, (int)userPosition.Y - 15, 30, 30);
            spriteBatch.Draw(green, sweetspot, Color.White);
            spriteBatch.Draw(red, userRect, Color.White);
        }
    }
}
