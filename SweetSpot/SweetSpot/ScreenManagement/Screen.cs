using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Sweetspot.ScreenManagement
{
    public abstract class Screen
    {
        public ContentManager Content { get; internal set; }
        public bool Initialized { get; internal set; }
        public bool Finished { get; internal set; }

        protected ScreenManager sm;
        protected Color background;

        public Screen(ScreenManager sm)
        {
            this.sm = sm;
            background = Color.White;
        }

        ~Screen()
        {
            UnloadContent();
        }

        public virtual void LoadContent()
        {
            Content = new ContentManager(sm.Game.Services, "Content");
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
            if (sm.Input.IsKeyDown(Keys.Escape) || sm.Input.IsGamePadButtonPressed(Buttons.Back))
                sm.Game.Exit();

            if (sm.Input.IsKeyPressed(Keys.Space) || sm.Input.IsGamePadButtonPressed(Buttons.A))
                NextScreen(gameTime);

            if (sm.Input.IsKeyPressed(Keys.F12) || sm.Input.IsGamePadButtonPressed(Buttons.RightTrigger))
                sm.ToggleDebug();
        }

        public virtual void Draw(GameTime gameTime)
        {
            sm.Game.GraphicsDevice.Clear(background);
        }

        public virtual void NextScreen(GameTime gameTime)
        {
            Finished = true;
        }
    }
}
