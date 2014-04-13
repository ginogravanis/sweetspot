using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement.Screens
{
    public class TestScreen : Screen
    {
        const int ITEMS_PER_ROW = 5;
        const int ITEM_ROWS = 5;
        const int IMAGE_DIFFERENCES = 3;
        const float RELATIVE_ITEM_SIZE = 0.111111f;               // fraction from viewport height
        const float RELATIVE_VERTICAL_ITEM_SPACING = 0.051852f;    // fraction from viewport height
        const float RELATIVE_VERTICAL_ITEM_OFFSET = 0.13426f;      // fraction from viewport height
        const float RELATIVE_SEPARATOR_WIDTH = 0.00521f;          // fraction from viewport width

        protected Texture2D image;
        protected bool shuffleItems;

        protected List<Texture2D> items;
        protected Texture2D shelfTexture;
        protected Texture2D separatorTexture;
        protected Rectangle separatorRect;

        public TestScreen(ScreenManager screenManager, bool shuffleItems = true)
            : base(screenManager)
        {
            this.shuffleItems = shuffleItems;
            items = new List<Texture2D>();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            shelfTexture = Content.Load<Texture2D>(@"texture\shelf");
            separatorTexture = Content.Load<Texture2D>(@"texture\black");

            items.Add(Content.Load<Texture2D>(@"texture\items\item_01"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_02"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_03"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_04"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_05"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_06"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_07"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_08"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_09"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_10"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_11"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_12"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_13"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_14"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_15"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_16"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_17"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_18"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_19"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_20"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_21"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_22"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_23"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_24"));
            items.Add(Content.Load<Texture2D>(@"texture\items\item_25"));
        }

        public override void Initialize()
        {
            base.Initialize();
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            separatorRect = new Rectangle(
                (int)((1 - RELATIVE_SEPARATOR_WIDTH) * viewport.Width / 2),
                0,
                (int)(RELATIVE_SEPARATOR_WIDTH * viewport.Width),
                viewport.Height
                );
            image = createBackgroundImage();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }

        protected Texture2D createBackgroundImage()
        {
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            RenderTarget2D renderTarget = new RenderTarget2D(graphics, viewport.Width, viewport.Height);

            graphics.SetRenderTarget(renderTarget);
            spriteBatch.Begin();
            spriteBatch.Draw(shelfTexture, new Rectangle(0, 0, viewport.Width/2, viewport.Height), Color.White);
            spriteBatch.Draw(shelfTexture, new Rectangle(viewport.Width/2, 0, viewport.Width / 2, viewport.Height), Color.White);
            
            int itemSize = (int)Math.Round(viewport.Height * RELATIVE_ITEM_SIZE);
            int horizontalSpacing = (int)Math.Round(((viewport.Width / 2f) - (ITEMS_PER_ROW * itemSize)) / (ITEMS_PER_ROW + 1));
            int verticalOffset = (int)Math.Round(viewport.Height * RELATIVE_VERTICAL_ITEM_OFFSET);
            int verticalSpacing = (int)Math.Round(viewport.Height * RELATIVE_VERTICAL_ITEM_SPACING);
            
            if (shuffleItems)
                items.Shuffle();
            for (int i = 0; i < items.Count; i++)
            {
                int x = ((i % ITEMS_PER_ROW) + 1) * horizontalSpacing + (i % ITEMS_PER_ROW) * itemSize;
                int y = verticalOffset + (i / ITEM_ROWS) * (verticalSpacing + itemSize) - itemSize;
                spriteBatch.Draw(items[i], new Rectangle(x, y, itemSize, itemSize), Color.White);
            }

            if (shuffleItems)
                items.ShuffleSubset(IMAGE_DIFFERENCES);
            for (int i = 0; i < items.Count; i++)
            {
                int x = ((i % ITEMS_PER_ROW) + 1) * horizontalSpacing + (i % ITEMS_PER_ROW) * itemSize;
                int y = verticalOffset + (i / ITEM_ROWS) * (verticalSpacing + itemSize) - itemSize;
                spriteBatch.Draw(items[i], new Rectangle(x + viewport.Width / 2, y, itemSize, itemSize), Color.White);
            }

            spriteBatch.Draw(separatorTexture, separatorRect, Color.White);
            spriteBatch.End();
            graphics.SetRenderTarget(null);

            Color[] colors = new Color[renderTarget.Width * renderTarget.Height];
            renderTarget.GetData(colors);
            Texture2D image = new Texture2D(graphics, renderTarget.Width, renderTarget.Height);
            image.SetData(colors);

            return image;
        }
    }
}
