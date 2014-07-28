using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Input;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
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
        protected bool userDetected = false;
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

        public BaselineTextScreen(ScreenManager sm, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(sm, cue, mapping, sweetspot)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();
            black = Content.Load<Texture2D>(@"texture\black");
            green = Content.Load<Texture2D>("texture\\green");
            red = Content.Load<Texture2D>("texture\\red");
            font = Content.Load<SpriteFont>(@"font\segoe_36");
            Viewport viewport = sm.GraphicsDevice.Viewport;
            int textBoxHeight = (int)font.MeasureString("W").Y;
            textBox = new Rectangle(0, viewport.Height - textBoxHeight, viewport.Width, textBoxHeight);
            textPosition = new Vector2(textBox.Center.X, textBox.Y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!sm.Kinect.IsUserActive())
            {
                userDetected = false;
                return;
            }

            userDetected = true;
            Vector2 userPosition = sm.Kinect.GetUserPosition();
            Vector2 vectorToSweetspot = sm.Kinect.sweetspot.GetVectorToSweetspot(userPosition);
            Direction direction = CalculateDominantDirection(vectorToSweetspot);

            targetReached = sm.Kinect.sweetspot.GetDistanceFromSweetspot(userPosition) == 0;
            if (TaskCompleted || targetReached)
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
            SpriteBatch spriteBatch = sm.SpriteBatch;
            Viewport viewport = sm.GraphicsDevice.Viewport;

            sm.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White * alpha);
            spriteBatch.Draw(black, textBox, Color.White);
            if (userDetected)
                spriteBatch.DrawString(font, text, textPosition, Color.White);
            spriteBatch.End();

            if (sm.Debug)
            {
                // Overlay
                Vector2 sweetspotPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, sm.Kinect.sweetspot.Position);
                Vector2 userPosition = SweetspotBounds.WorldToScreenCoords(viewport.Bounds, sm.Kinect.GetUserPosition());
                Rectangle sweetspot = new Rectangle((int)sweetspotPosition.X - 10, (int)sweetspotPosition.Y - 10, 20, 20);
                Rectangle userRect = new Rectangle((int)userPosition.X - 15, (int)userPosition.Y - 15, 30, 30);
                spriteBatch.Begin();
                spriteBatch.Draw(green, sweetspot, Color.White);
                spriteBatch.Draw(red, userRect, Color.White);
                spriteBatch.End();
            }
        }
    }
}
