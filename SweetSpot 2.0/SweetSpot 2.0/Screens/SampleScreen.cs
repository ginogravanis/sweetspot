using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public class SampleScreen : Screen
    {
        ContentManager content;
        RenderTarget2D screen;
        Texture2D image;
        Texture2D redPixel;
        Texture2D greenPixel;
        Effect effect;
        float effectAmount = 1f;

        public override void LoadContent()
        {
            content = new ContentManager(ScreenManager.Game.Services, "Content");
            screen = new RenderTarget2D(ScreenManager.GraphicsDevice,
                ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth,
                ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight);
            image = content.Load<Texture2D>("testimage");
            redPixel = content.Load<Texture2D>("redpixel");
            greenPixel = content.Load<Texture2D>("greenpixel");
            effect = content.Load<Effect>("SaturationShader");
        }

        public override void Update(GameTime gameTime)
        {
            UpdateEffect(gameTime);

            if (ScreenManager.Input.IsKeyDown(Keys.Escape))
            {
                ScreenManager.Game.Exit();
            }
        }

        private void UpdateEffect(GameTime gameTime)
        {
            if (ScreenManager.Kinect.IsViewerActive())
            {
                effectAmount = ScreenManager.Kinect.GetDistanceFromSweetSpot() / ScreenManager.Kinect.sweetSpotMargin;
            }
            else
            {
                effectAmount = 1.0f;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = graphics.Viewport;

            graphics.SetRenderTarget(screen);
            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
            graphics.SetRenderTarget(null);

            effect.Parameters["effectAmount"].SetValue(effectAmount);
            graphics.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw((Texture2D)screen, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();

            Vector2 sweetSpotPosition = WorldToScreenCoords(viewport.Bounds, ScreenManager.Kinect.sweetSpot);
            Rectangle sweetSpot = new Rectangle((int)sweetSpotPosition.X - 10, (int)sweetSpotPosition.Y - 10, 20, 20);
            spriteBatch.Begin();
            spriteBatch.Draw(greenPixel, sweetSpot, Color.White);
            if (ScreenManager.Kinect.IsViewerActive())
            {
                Vector2 viewerPosition = WorldToScreenCoords(viewport.Bounds, ScreenManager.Kinect.GetViewerPosition());
                Rectangle viewer = new Rectangle((int)viewerPosition.X - 15, (int)viewerPosition.Y - 15, 30, 30);
                spriteBatch.Draw(redPixel, viewer, Color.White);
            }
            spriteBatch.End();
        }

        private Vector2 WorldToScreenCoords(Rectangle viewport, Vector2 position)
        {
            float x = (viewport.Width / 2) + (int)(position.X * 300);
            float y = (int)(position.Y * 200);
            return new Vector2(x, y);
        }
    }
}
