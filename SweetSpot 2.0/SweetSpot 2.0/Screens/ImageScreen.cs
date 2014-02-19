using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    public class ImageScreen : Screen
    {
        protected Texture2D image;

        public ImageScreen(ScreenManager screenManager, Texture2D image)
            : base(screenManager)
        {
            this.image = image;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }
    }
}
