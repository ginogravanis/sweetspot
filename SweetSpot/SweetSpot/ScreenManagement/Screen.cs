using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot.ScreenManagement
{
    public abstract class Screen
    {
        public ContentManager Content { get; internal set; }
        public bool Initialized { get; internal set; }
        public bool Finished { get; internal set; }

        protected ScreenManager screenManager;
        protected Color background;

        public Screen(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
            background = Color.White;
        }

        public virtual void LoadContent()
        {
            Content = new ContentManager(screenManager.Game.Services, "Content");
        }

        public virtual void UnloadContent() { }

        public virtual void Initialize()
        {
            if (Initialized)
                return;

            Initialized = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (screenManager.Input.IsKeyDown(Keys.Escape) || screenManager.Input.IsGamePadButtonPressed(Buttons.Back))
                screenManager.Game.Exit();

            if (screenManager.Input.IsKeyPressed(Keys.Space) || screenManager.Input.IsGamePadButtonPressed(Buttons.A))
                NextScreen(gameTime);

            if (screenManager.Input.IsKeyPressed(Keys.F12) || screenManager.Input.IsGamePadButtonPressed(Buttons.RightTrigger))
                screenManager.ToggleDebug();
        }

        public virtual void Draw(GameTime gameTime)
        {
            screenManager.Game.GraphicsDevice.Clear(background);
        }

        public virtual void NextScreen(GameTime gameTime)
        {
            Finished = true;
        }
    }
}
