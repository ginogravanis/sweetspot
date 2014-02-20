using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SweetSpot_2._0
{
    class BaselineTextScreen : ImageScreen
    {
        Texture2D black;
        SpriteFont font;
        Rectangle textBox;
        Vector2 textPosition;
        string text = "";
        bool viewerDetected = false;

        public enum Direction { Right, Forward, Left, Back }
        public Dictionary<Direction, string> directionName = new Dictionary<Direction, string>
        {
            { Direction.Right, "rechts" },
            { Direction.Forward, "vorne" },
            { Direction.Left, "links" },
            { Direction.Back, "hinten" }
        };

        public BaselineTextScreen(ScreenManager screenManager, Texture2D image)
            : base(screenManager, image)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();
            black = content.Load<Texture2D>("texture\\black");
            font = content.Load<SpriteFont>("font\\segoe_36");
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            int textBoxHeight = (int)font.MeasureString("W").Y;
            textBox = new Rectangle(0, viewport.Height - textBoxHeight, viewport.Width, textBoxHeight);
            textPosition = new Vector2(textBox.Center.X, textBox.Y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!ScreenManager.Kinect.IsViewerActive())
            {
                viewerDetected = false;
                return;
            }

            viewerDetected = true;
            Vector2 vectorToSweetspot = ScreenManager.Kinect.GetVectorToSweetSpot();
            Direction direction = CalculateDominantDirection(vectorToSweetspot);

            if (ScreenManager.Kinect.GetDistanceFromSweetSpot() < 0.05)
            {
                text = "Stop!";
            }
            else
            {
                text = "Bitte noch etwas nach " + directionName[direction];
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

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(black, textBox, Color.White);
            if (viewerDetected)
                spriteBatch.DrawString(font, text, textPosition, Color.White);
            spriteBatch.End();
        }
    }
}
