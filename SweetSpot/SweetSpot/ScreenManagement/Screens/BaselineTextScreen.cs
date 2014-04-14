using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Input;

namespace SweetSpot.ScreenManagement.Screens
{
    public class BaselineTextScreen : TrackingScreen
    {
        const float FADE_TIME = 200;    // in ms

        protected Texture2D black;
        protected Texture2D green;
        protected Texture2D red;
        protected SpriteFont font;
        protected Rectangle textBox;
        protected Vector2 textPosition;
        protected string text = "";
        protected bool viewerDetected = false;
        protected bool targetReached = false;
        protected float alpha = 0;

        public enum Direction { Right, Forward, Left, Back }
        public Dictionary<Direction, string> directionName = new Dictionary<Direction, string>
        {
            { Direction.Right, "rechts" },
            { Direction.Forward, "vorne" },
            { Direction.Left, "links" },
            { Direction.Back, "hinten" }
        };

        public BaselineTextScreen(ScreenManager screenManager, string cue, Vector2 sweetSpot)
            : base(screenManager, cue, sweetSpot)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();
            black = Content.Load<Texture2D>(@"texture\black");
            green = Content.Load<Texture2D>("texture\\green");
            red = Content.Load<Texture2D>("texture\\red");
            font = Content.Load<SpriteFont>(@"font\segoe_36");
            Viewport viewport = screenManager.GraphicsDevice.Viewport;
            int textBoxHeight = (int)font.MeasureString("W").Y;
            textBox = new Rectangle(0, viewport.Height - textBoxHeight, viewport.Width, textBoxHeight);
            textPosition = new Vector2(textBox.Center.X, textBox.Y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!screenManager.Kinect.IsViewerActive())
            {
                viewerDetected = false;
                return;
            }

            viewerDetected = true;
            Vector2 vectorToSweetspot = screenManager.Kinect.GetVectorToSweetSpot();
            Direction direction = CalculateDominantDirection(vectorToSweetspot);

            targetReached = screenManager.Kinect.GetDistanceFromSweetSpot() == 0;
            if (taskCompleted || targetReached)
            {
                text = "Stop!";
                alpha = Math.Min(alpha + (gameTime.ElapsedGameTime.Milliseconds / FADE_TIME), 1);
            }
            else
            {
                text = "Bitte noch etwas nach " + directionName[direction];
                alpha = Math.Max(alpha - (gameTime.ElapsedGameTime.Milliseconds / FADE_TIME), 0);
            }

            float textWidth = font.MeasureString(text).X;
            textPosition = new Vector2(textBox.Center.X - (int)textWidth/2, textBox.Y);
        }

        protected Direction CalculateDominantDirection(Vector2 vector)
        {
            if (Math.Abs(vector.X) > Math.Abs(vector.Y))
            {
                if (vector.X < 0)
                {
                    return Direction.Left;
                }
                else
                {
                    return Direction.Right;
                }
            }
            else
            {
                if (vector.Y < 0)
                {
                    return Direction.Forward;
                }
                else
                {
                    return Direction.Back;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            screenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White * alpha);
            spriteBatch.Draw(black, textBox, Color.White);
            if (viewerDetected)
                spriteBatch.DrawString(font, text, textPosition, Color.White);
            spriteBatch.End();

            if (screenManager.Debug)
            {
                // Overlay
                Vector2 sweetSpotPosition = SensorManager.WorldToScreenCoords(viewport.Bounds, screenManager.Kinect.sweetSpot);
                Vector2 viewerPosition = SensorManager.WorldToScreenCoords(viewport.Bounds, screenManager.Kinect.GetViewerPosition());
                Rectangle sweetSpot = new Rectangle((int)sweetSpotPosition.X - 10, (int)sweetSpotPosition.Y - 10, 20, 20);
                Rectangle viewer = new Rectangle((int)viewerPosition.X - 15, (int)viewerPosition.Y - 15, 30, 30);
                spriteBatch.Begin();
                spriteBatch.Draw(green, sweetSpot, Color.White);
                spriteBatch.Draw(red, viewer, Color.White);
                spriteBatch.End();
            }
        }
    }
}
