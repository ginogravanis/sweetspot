using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public class EffectDebugScreen : EffectScreen
    {
        Texture2D green;
        Texture2D red;

        public EffectDebugScreen(ScreenManager screenManager, Texture2D image, Effect effect)
            : base(screenManager, image, effect)
        { }

        public override void LoadContent()
        {
            base.LoadContent();
            green = content.Load<Texture2D>("texture\\green");
            red = content.Load<Texture2D>("texture\\red");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            // Overlay
            Vector2 sweetSpotPosition = WorldToScreenCoords(viewport.Bounds, ScreenManager.Kinect.sweetSpot);
            Vector2 viewerPosition = WorldToScreenCoords(viewport.Bounds, ScreenManager.Kinect.GetViewerPosition());
            Rectangle sweetSpot = new Rectangle((int)sweetSpotPosition.X - 10, (int)sweetSpotPosition.Y - 10, 20, 20);
            Rectangle viewer = new Rectangle((int)viewerPosition.X - 15, (int)viewerPosition.Y - 15, 30, 30);
            spriteBatch.Begin();
            spriteBatch.Draw(green, sweetSpot, Color.White);
            spriteBatch.Draw(red, viewer, Color.White);
            spriteBatch.End();
        }

        protected Vector2 WorldToScreenCoords(Rectangle viewport, Vector2 position)
        {
            float x = (viewport.Width / 2) + (int)(position.X * 250);
            float y = (int)(position.Y * 250);
            return new Vector2(x, y);
        }
    }
}
