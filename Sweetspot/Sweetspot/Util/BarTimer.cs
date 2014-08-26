using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.ScreenManagement;

namespace SweetspotApp.Util
{
    public class BarTimer
    {
        public bool Expired { get { return elapsedTime >= duration; } }
        public bool Running { get; set; }

        protected static readonly float BACKGROUND_OPACITY = 0.4f;

        protected GameController gc;
        protected Rectangle bar;
        protected Texture2D color;
        protected Texture2D backgroundColor;
        protected Texture2D stopColor;
        protected Vector2 captionPosition;
        protected string captionTemplate;
        protected string caption;
        protected float duration;
        protected float elapsedTime;
        protected SpriteFont font;
        protected Rectangle scaledBar;

        public BarTimer(GameController gc, Rectangle bar, Texture2D color, string captionTemplate, float duration, Texture2D stopColor = null)
        {
            this.gc = gc;
            this.bar = bar;
            this.color = color;
            this.captionTemplate = captionTemplate;
            this.duration = duration;
            this.stopColor = stopColor ?? color;
        }

        public void LoadContent()
        {
            var cm = gc.Content;
            font = cm.Load<SpriteFont>(@"font\segoe_24b");
            backgroundColor = cm.Load<Texture2D>(@"texture\black");
        }

        public void Initialize()
        {
            elapsedTime = 0;
            Running = false;
            string caption = String.Format(captionTemplate, 0);
            float questionX = bar.Left + (bar.Width - font.MeasureString(caption).X) / 2;
            float questionY = bar.Top + (bar.Height - font.MeasureString(caption).Y) / 2;
            captionPosition = new Vector2(questionX, questionY);
        }

        public void Update(GameTime gameTime)
        {
            if (Running)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                elapsedTime = Math.Min(elapsedTime, duration);
            }

            float elapsedPercentage = elapsedTime / duration;
            caption = String.Format(captionTemplate, (int)Math.Round(duration - elapsedTime));

            scaledBar = new Rectangle(bar.X, bar.Y, (int)(bar.Width * elapsedPercentage), bar.Height);
        }

        public void Draw(GameTime gameTime)
        {
            var barColor = Running ? color : stopColor;
            drawTimerBackground();
            drawTimerBar(barColor);
            drawTimerCaption();
        }

        public void Start()
        {
            Running = true;
        }

        public void Pause()
        {
            Running = false;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Reset()
        {
            elapsedTime = 0;
        }

        protected void drawTimerBar(Texture2D barColor)
        {
            SpriteBatch sb = gc.SpriteBatch;
            sb.Begin();
            sb.Draw(barColor, scaledBar, Color.White);
            sb.End();
        }

        protected void drawTimerBackground()
        {
            SpriteBatch sb = gc.SpriteBatch;
            sb.Begin();
            sb.Draw(backgroundColor, bar, Color.White * BACKGROUND_OPACITY);
            sb.End();
        }

        protected void drawTimerCaption()
        {
            SpriteBatch sb = gc.SpriteBatch;
            sb.Begin();
            sb.DrawString(font, caption, captionPosition, Color.White);
            sb.End();
        }
    }
}
