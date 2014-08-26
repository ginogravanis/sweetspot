using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SweetspotApp.ScreenManagement
{
    public abstract class Screen
    {
        public ContentManager Content { get { return gc.Content; } }
        public bool Initialized { get; protected set; }
        public bool Finished { get; protected set; }

        protected GameController gc;
        protected SpriteBatch spriteBatch;
        protected Viewport viewport;
        protected Color background;

        public Screen(GameController gc)
        {
            this.gc = gc;
            background = Color.White;
        }

        ~Screen()
        {
            UnloadContent();
        }

        public virtual void LoadContent()
        { }

        public virtual void UnloadContent() { }

        public virtual void Initialize()
        {
            if (Initialized)
                return;

            Initialized = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (gc.Input.IsKeyDown(Keys.Escape) || gc.Input.IsGamePadButtonPressed(Buttons.Back))
                gc.Game.Exit();

            if (gc.Input.IsKeyPressed(Keys.Space) || gc.Input.IsGamePadButtonPressed(Buttons.A))
            {
                NextScreen();
            }

            if (gc.Input.IsKeyPressed(Keys.F12) || gc.Input.IsGamePadButtonPressed(Buttons.RightTrigger))
                gc.ToggleDebug();
        }

        public virtual void Draw(GameTime gameTime)
        {
            spriteBatch = gc.SpriteBatch;
            viewport = gc.GraphicsDevice.Viewport;
            gc.Game.GraphicsDevice.Clear(background);
        }

        public virtual void NextScreen()
        {
            Finished = true;
        }
    }
}
