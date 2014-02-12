using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public abstract class Screen
    {
        protected ContentManager content;
        protected bool initialized = false;

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        public bool Finished { get; internal set; }

        ScreenManager screenManager;

        public Screen(ScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }

        public virtual void LoadContent()
        {
            content = new ContentManager(ScreenManager.Game.Services, "Content");
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

            if (ScreenManager.Input.IsKeyDown(Keys.Escape))
                ScreenManager.Game.Exit();

            if (ScreenManager.Input.IsKeyPressed(Keys.Space))
                Finished = true;
        }

        public virtual void Draw(GameTime gameTime) { }
    }
}
