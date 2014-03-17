using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement.Screens
{
    public class ImageScreen : Screen
    {
        const int bgImageWidth = 1920;
        const int bgImageHeight = 1080;
        const int itemsPerRow = 5;
        const int itemRows = 5;
        const int imageDifferences = 3;

        protected Texture2D image;
        protected string cue;
        protected Vector2 sweetSpot;
        protected int test;
        protected int testSubject;
        protected TimeSpan lastPositionCaptured;
        protected TimeSpan recordingIntervall = TimeSpan.FromMilliseconds(100);

        protected Texture2D shelfTexture;
        protected Texture2D separatorTexture;
        protected List<Texture2D> items;

        protected Rectangle separatorRect;

        public ImageScreen(ScreenManager screenManager, string cue, Vector2 sweetSpot)
            : base(screenManager)
        {
            this.cue = cue;
            this.sweetSpot = sweetSpot;
            lastPositionCaptured = TimeSpan.FromSeconds(-1);
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
            testSubject = screenManager.TestSubject;
            test = screenManager.Database.RecordTest(testSubject, cue, sweetSpot);
            screenManager.Kinect.sweetSpot = sweetSpot;

            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            separatorRect = new Rectangle(viewport.Width / 2, 0, 1, viewport.Height);
            image = createBackgroundImage();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (screenManager.Kinect.IsViewerActive() && recordingIntervalElapsed(gameTime))
                recordPosition(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }

        protected bool recordingIntervalElapsed(GameTime gameTime)
        {
            return lastPositionCaptured + recordingIntervall <= gameTime.TotalGameTime;
        }

        protected void recordPosition(GameTime gameTime)
        {
            lastPositionCaptured = gameTime.TotalGameTime;
            screenManager.Database.RecordUserPosition(test, screenManager.Kinect.GetViewerPosition(), (int)gameTime.TotalGameTime.TotalMilliseconds);
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

            items.Shuffle();
            for (int i = 0; i < items.Count; i++)
            {
                int x = 60 + (i % itemsPerRow) * (60 + 120);
                int y = 135 + (i / itemRows) * (56 + 120) - 120;
                spriteBatch.Draw(items[i], new Rectangle(x, y, 120, 120), Color.White);
            }

            items.SwapPairs(imageDifferences);
            for (int i = 0; i < items.Count; i++)
            {
                int x = 60 + (i % itemsPerRow) * (60 + 120);
                int y = 135 + (i / itemRows) * (56 + 120) - 120;
                spriteBatch.Draw(items[i], new Rectangle(x + viewport.Width / 2, y, 120, 120), Color.White);
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
