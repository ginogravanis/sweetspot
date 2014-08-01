using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public class BaselineTextScreen : BaselineScreen
    {
        protected SpriteFont font;
        protected Rectangle textBox;
        protected Vector2 textPosition;
        protected string text = "";

        public enum Direction { Right, Forward, Left, Back }
        public Dictionary<Direction, string> directionName = new Dictionary<Direction, string>
        {
            { Direction.Right, "rechts" },
            { Direction.Forward, "vorne" },
            { Direction.Left, "links" },
            { Direction.Back, "hinten" }
        };

        public BaselineTextScreen(GameController gc, string cue, Mapping mapping, Sweetspot sweetspot)
            : base(gc, cue, mapping, sweetspot)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();
            font = Content.Load<SpriteFont>(@"font\segoe_36");
            int textBoxHeight = (int)font.MeasureString("W").Y;
            Viewport viewport = gc.GraphicsDevice.Viewport;
            textBox = new Rectangle(0, viewport.Height - textBoxHeight, viewport.Width, textBoxHeight);
            textPosition = new Vector2(textBox.Center.X, textBox.Y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Vector2 userPosition = gc.Kinect.GetUserPosition();
            Vector2 vectorToSweetspot = gc.Kinect.sweetspot.GetVectorToSweetspot(userPosition);
            Direction direction = CalculateDominantDirection(vectorToSweetspot);

            targetReached = gc.Kinect.sweetspot.GetDistanceFromSweetspot(userPosition) == 0;
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
            spriteBatch.Begin();
            spriteBatch.Draw(black, textBox, Color.White);
            if (userDetected)
                spriteBatch.DrawString(font, text, textPosition, Color.White);

            if (gc.Debug)
                drawDebug();

            spriteBatch.End();
        }
    }
}
