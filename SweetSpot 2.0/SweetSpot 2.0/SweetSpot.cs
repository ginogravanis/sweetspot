using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public class SweetSpot : Microsoft.Xna.Framework.Game
    {
        const int ScreenWidth = 1920;
        const int ScreenHeight = 1080;
        InputManager inputManager;
        GraphicsDeviceManager graphics;
        RenderTarget2D screen;
        SpriteBatch spriteBatch;
        Texture2D image;
        Texture2D redPixel;
        Texture2D greenPixel;
        Effect effect;
        float effectAmount = 1f;

        Kinect kinect;

        public SweetSpot()
        {
            this.Window.Title = "SweetSpot 2.0";

            this.inputManager = new InputManager();

            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.IsFullScreen = true;
            this.graphics.PreferredBackBufferWidth = ScreenWidth;
            this.graphics.PreferredBackBufferHeight = ScreenHeight;
            this.graphics.SynchronizeWithVerticalRetrace = true;

            Vector2 sweetspot = new Vector2(0.0f, 2.0f);
            this.kinect = new Kinect(sweetspot);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Ermöglicht dem Spiel, alle Initialisierungen durchzuführen, die es benötigt, bevor die Ausführung gestartet wird.
        /// Hier können erforderliche Dienste abgefragt und alle nicht mit Grafiken
        /// verbundenen Inhalte geladen werden.  Bei Aufruf von base.Initialize werden alle Komponenten aufgezählt
        /// sowie initialisiert.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.kinect.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.screen = new RenderTarget2D(graphics.GraphicsDevice,
                graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
            this.image = Content.Load<Texture2D>("testimage");
            this.redPixel = Content.Load<Texture2D>("redpixel");
            this.greenPixel = Content.Load<Texture2D>("greenpixel");
            this.effect = Content.Load<Effect>("SaturationShader");
            base.LoadContent();
        }

        /// <summary>
        /// Ermöglicht dem Spiel die Ausführung der Logik, wie zum Beispiel Aktualisierung der Welt,
        /// Überprüfung auf Kollisionen, Erfassung von Eingaben und Abspielen von Ton.
        /// </summary>
        /// <param name="gameTime">Bietet einen Schnappschuss der Timing-Werte.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.inputManager.Update(gameTime);
            this.kinect.Update(gameTime);
            this.UpdateEffect(gameTime);

            if (this.inputManager.IsKeyPressed(Keys.Escape))
            {
                Exit();
            }
        }

        private void UpdateEffect(GameTime gameTime)
        {
            if (this.kinect.IsViewerActive())
            {
                this.effectAmount = this.kinect.GetDistanceFromSweetSpot() / this.kinect.sweetSpotMargin;
            }
            else
            {
                this.effectAmount = 1.0f;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            this.graphics.GraphicsDevice.Clear(Color.Black);
            this.graphics.GraphicsDevice.SetRenderTarget(this.screen);
            this.spriteBatch.Begin();
            this.spriteBatch.Draw(this.image, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            this.spriteBatch.End();
            this.graphics.GraphicsDevice.SetRenderTarget(null);
            
            this.effect.Parameters["effectAmount"].SetValue(this.effectAmount);
            this.graphics.GraphicsDevice.Clear(Color.Black);
            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, this.effect);
            this.spriteBatch.Draw((Texture2D)screen, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            this.spriteBatch.End();

            Vector2 sweetSpotPosition = Coords(this.kinect.sweetSpot);
            Rectangle sweetSpot = new Rectangle((int)sweetSpotPosition.X - 10, (int)sweetSpotPosition.Y - 10, 20, 20);
            this.spriteBatch.Begin();
            this.spriteBatch.Draw(this.greenPixel, sweetSpot, Color.White);
            if (this.kinect.IsViewerActive())
            {
                Vector2 viewerPosition = this.kinect.GetViewerPosition();
                Rectangle viewer = new Rectangle((int)Coords(viewerPosition).X - 15, (int)Coords(viewerPosition).Y - 15, 30, 30);
                this.spriteBatch.Draw(this.redPixel, viewer, Color.White);
            }
            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        // Transform world coordinates to screen coordinates.
        private Vector2 Coords(Vector2 position)
        {
            float x = (ScreenWidth / 2) + (int)(position.X * 300);
            float y = (int)(position.Y * 200);
            return new Vector2(x, y);
        }
    }
}
