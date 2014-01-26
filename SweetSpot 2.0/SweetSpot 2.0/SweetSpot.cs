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

        KinectSensor sensor;
        Vector2 viewerPosition;
        GameTime viewerLastDetected;

        /// <summary>
        /// The position the user should optimally be standing relative to the sensor.
        /// The x-coordinate corresponds to the user's position parallel to the sensor,
        /// 0 being the center of the sensor and positive coordinates going to the right of the sensor.
        /// The y-coordinate corresponds to the user's position away from the sensor,
        /// 0 being the surface of the sensor.
        /// </summary>
        Vector2 sweetSpot;

        /// <summary>
        /// The minumum interaction distance from the sweetspot measured in meters.
        /// </summary>
        float sweetSpotPerimeter = 1f;

        public SweetSpot()
        {
            this.Window.Title = "SweetSpot 2.0";

            this.inputManager = new InputManager();

            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.IsFullScreen = true;
            this.graphics.PreferredBackBufferWidth = ScreenWidth;
            this.graphics.PreferredBackBufferHeight = ScreenHeight;
            this.graphics.SynchronizeWithVerticalRetrace = true;

            this.sweetSpot = new Vector2(0.0f, 2.0f);
            this.viewerLastDetected = new GameTime(new TimeSpan(-10), new TimeSpan(-10));

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
            this.sensor = FindSensor();
            this.sensor.SkeletonStream.Enable(this.GetSmoothingParameters());
            this.sensor.Start();
        }

        private TransformSmoothParameters GetSmoothingParameters()
        {
            TransformSmoothParameters smoothingParameters = new TransformSmoothParameters();
            {
                smoothingParameters.Smoothing = 0.5f;
                smoothingParameters.Correction = 0.5f;
                smoothingParameters.Prediction = 0.5f;
                smoothingParameters.JitterRadius = 0.05f;
                smoothingParameters.MaxDeviationRadius = 0.04f;
            };

            return smoothingParameters;
        }

        private KinectSensor FindSensor()
        {
            return KinectSensor.KinectSensors.FirstOrDefault(sensor => sensor.Status == KinectStatus.Connected);
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

        protected override void UnloadContent()
        {
            this.sensor.Stop();
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
            this.UpdateUserPosition(gameTime);
            this.UpdateEffect(gameTime);

            if (this.inputManager.IsKeyPressed(Keys.Escape))
            {
                Exit();
            }
        }

        private void UpdateUserPosition(GameTime gameTime)
        {
            try
            {
                this.viewerPosition = this.GetNearestViewerPosition();
                this.viewerLastDetected = gameTime;
            }
            catch (ApplicationException)
            {
            }
        }

        private void UpdateEffect(GameTime gameTime)
        {
            if (this.ViewerRecentlyDetected(gameTime))
            {
                float distanceFromSweetSpot = Math.Abs((this.sweetSpot - this.viewerPosition).Length());
                distanceFromSweetSpot = Math.Min(distanceFromSweetSpot, this.sweetSpotPerimeter);
                this.effectAmount = distanceFromSweetSpot / this.sweetSpotPerimeter;
            }
            else
            {
                this.effectAmount = 1.0f;
            }
        }

        private bool ViewerRecentlyDetected(GameTime gameTime)
        {
            bool viewerRecentlyDetected = false;
            if (this.viewerLastDetected.TotalGameTime > gameTime.TotalGameTime - new TimeSpan(0, 0, 0, 0, 50))
            {
                viewerRecentlyDetected = true;
            }

            return viewerRecentlyDetected;
        }

        private Vector2 GetNearestViewerPosition()
        {
            List<Vector2> viewerPositions = GetSkeletonPositions();
            if (viewerPositions.Count == 0)
                throw new ApplicationException("No skeletons detected.");

            float shortestDistanceToSweetSpot = this.DistanceToSweetSpot(viewerPositions[0]);
            Vector2 nearestUserPosition = viewerPositions[0];
            foreach (Vector2 position in viewerPositions.Skip(1))
            {
                float newDistance = this.DistanceToSweetSpot(position);
                if (newDistance < shortestDistanceToSweetSpot)
                {
                    shortestDistanceToSweetSpot = newDistance;
                    nearestUserPosition = position;
                }
            }

            return nearestUserPosition;
        }

        private float DistanceToSweetSpot(Vector2 position)
        {
            return Math.Abs((this.sweetSpot - position).Length());
        }

        private List<Vector2> GetSkeletonPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (Skeleton skeleton in this.GetSkeletonData())
            {
                if (skeleton.TrackingState != SkeletonTrackingState.NotTracked)
                {
                    positions.Add(new Vector2(skeleton.Position.X, skeleton.Position.Z));
                }
            }
            return positions;
        }

        private Skeleton[] GetSkeletonData()
        {
            using (SkeletonFrame frame = this.sensor.SkeletonStream.OpenNextFrame(0))
            {
                Skeleton[] skeletons;
                if (null != frame)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
                else
                {
                    skeletons = new Skeleton[0];
                }
                return skeletons;
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

            this.spriteBatch.Begin();
            this.spriteBatch.Draw(this.greenPixel, new Rectangle((int)Coords(sweetSpot).X - 10, (int)Coords(sweetSpot).Y - 10, 20, 20), Color.White);
            if (this.ViewerRecentlyDetected(gameTime))
            {
                this.spriteBatch.Draw(this.redPixel, new Rectangle((int)Coords(viewerPosition).X - 15, (int)Coords(viewerPosition).Y - 15, 30, 30), Color.White);
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
