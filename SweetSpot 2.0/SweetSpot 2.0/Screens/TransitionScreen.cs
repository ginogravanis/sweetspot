using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    class TransitionScreen : Screen
    {
        SpriteFont font;
        string caption;
        Vector2 textPosition;

        public TransitionScreen(ScreenManager screenManager, string caption)
            : base(screenManager)
        {
            this.caption = caption;
            textPosition = new Vector2();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            font = content.Load<SpriteFont>("font\\segoe");
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 textSize = font.MeasureString(caption);
            textPosition = new Vector2((viewport.Width - textSize.X) / 2, (viewport.Height - textSize.Y) / 2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, caption, textPosition, Color.White);
            spriteBatch.End();
        }
    }
}
