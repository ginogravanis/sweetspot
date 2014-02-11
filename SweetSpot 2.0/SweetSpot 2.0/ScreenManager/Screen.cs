using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    public abstract class Screen
    {
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        public bool Finished { get; internal set; }

        ScreenManager screenManager;

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }
    }
}
