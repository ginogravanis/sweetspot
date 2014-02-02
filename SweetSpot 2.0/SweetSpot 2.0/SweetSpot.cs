using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    public class SweetSpot : Microsoft.Xna.Framework.Game
    {
        const int ScreenWidth = 1920;
        const int ScreenHeight = 1080;
        GraphicsDeviceManager graphics;

        public SweetSpot()
        {
            Content.RootDirectory = "Content";
            this.Window.Title = "SweetSpot 2.0";
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.IsFullScreen = true;
            this.graphics.PreferredBackBufferWidth = ScreenWidth;
            this.graphics.PreferredBackBufferHeight = ScreenHeight;
            this.graphics.SynchronizeWithVerticalRetrace = true;

            Components.Add(new ScreenManager(this));
        }

        /// <summary>
        /// Ermöglicht dem Spiel, alle Initialisierungen durchzuführen, die es benötigt, bevor die Ausführung gestartet wird.
        /// Hier können erforderliche Dienste abgefragt und alle nicht mit Grafiken
        /// verbundenen Inhalte geladen werden.  Bei Aufruf von base.Initialize werden alle Komponenten aufgezählt
        /// sowie initialisiert.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
