using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot.ScreenManagement.Screens
{
    public class SlideScreen : Screen
    {
        protected Texture2D image;
        protected Rectangle imageRect;

        public SlideScreen(ScreenManager screenManager, Texture2D image)
            : base(screenManager)
        {
            this.image = image;
        }

        public override void Initialize()
        {
            base.Initialize();
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            imageRect = createImageRect(viewport);
        }

        protected Rectangle createImageRect(Viewport viewport)
        {
            int imageWidth = image.Bounds.Width;
            int imageHeight = image.Bounds.Height;
            float scaleX = (float)imageWidth / (float)viewport.Width;
            float scaleY = (float)imageHeight / (float)viewport.Height;
            float imageScale = Math.Max(scaleX, scaleY);
            imageScale = Math.Max(imageScale, 1);

            imageWidth = (int)Math.Round(imageWidth / imageScale);
            imageHeight = (int)Math.Round(imageHeight / imageScale);
            int x = (viewport.Width - imageWidth) / 2;
            int y = (viewport.Height - imageHeight) / 2;
            return new Rectangle(x, y, imageWidth, imageHeight);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(image, imageRect, Color.White);
            spriteBatch.End();
        }
    }
}
