using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public abstract class Screen
    {
        public ContentManager Content { get; internal set; }
        protected bool initialized = false;

        protected ScreenManager screenManager;

        public bool Finished { get; internal set; }

        public Screen(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
        }

        public virtual void LoadContent()
        {
            Content = new ContentManager(screenManager.Game.Services, "Content");
        }

        public virtual void UnloadContent() { }

        protected virtual void initialize(GameTime gameTime) { }

        public virtual void Update(GameTime gameTime)
        {
            if (!initialized)
            {
                initialize(gameTime);
                initialized = true;
            }

            if (screenManager.Input.IsKeyDown(Keys.Escape))
                screenManager.Game.Exit();

            if (screenManager.Input.IsKeyPressed(Keys.Space) || screenManager.Input.IsGamePadButtonPressed(Buttons.A))
                SkipAction(gameTime);
        }

        public virtual void Draw(GameTime gameTime) { }

        public virtual void SkipAction(GameTime gameTime)
        {
            ExitScreen(gameTime);
        }

        public virtual void ExitScreen(GameTime gameTime)
        {
            Finished = true;
        }
    }
}
