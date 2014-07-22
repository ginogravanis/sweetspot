using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Database;
using SweetSpot.Input;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement
{
    public enum Scene { Calibration, Game };

    public class ScreenManager : DrawableGameComponent
    {
        public bool Debug { get; internal set; }
        public SensorManager Kinect { get; internal set; }
        public InputManager Input { get; internal set; }
        public IDatabase Database { get; internal set; }
        public SweetspotBounds SweetspotBounds { get; internal set; }
        public SpriteBatch SpriteBatch { get; internal set; }
        public Screen CurrentScreen { get { return screens.First(); } }
        public int GameID { get; internal set; }

        protected Scene scene;
        protected LinkedList<Screen> screens;

        public ScreenManager(Game game, Scene scene)
            : base(game)
        {
            Debug = false;
            var db = new SQLiteAdapter();
            screens = new LinkedList<Screen>();
            Kinect = new SensorManager(db);
            Input = new InputManager();
            Database = db;
            this.scene = scene;
        }

        public override void Initialize()
        {
            base.Initialize();

            switch (scene)
            {
                case Scene.Calibration:
                    Add(ScreenFactory.CreateCalibrationScreen(this));
                    break;
                case Scene.Game:
                    SweetspotBounds = new SweetspotBounds(Database);
                    screens.AddLast(ScreenFactory.CreateTitleScreen(this));
                    break;
            }
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            foreach (Screen screen in screens)
                screen.UnloadContent();
        }

        public void Add(Screen screen)
        {
            screens.AddLast(screen);
        }

        protected void removeFirst()
        {
            screens.RemoveFirst();
        }

        public void ToggleDebug()
        {
            Debug = !Debug;
        }

        public override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            Kinect.Update(gameTime);

            if (CurrentScreen.Finished)
            {
                removeFirst();
            }

            if (!CurrentScreen.Initialized)
            {
                CurrentScreen.LoadContent();
                CurrentScreen.Initialize();
            }

            CurrentScreen.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            CurrentScreen.Draw(gameTime);
        }

        public void NewGame()
        {
            GameID = Database.GetNewGameID();
            NextQuestion();
        }

        public void NextQuestion()
        {
            Add(ScreenFactory.CreateQuestionScreen(
                this,
                Cue.Pixelate,
                Mapping.SCurve,
                SweetspotBounds.GenerateSweetspot()
                ));
        }

        public void EndGame()
        {
            Add(ScreenFactory.CreateTitleScreen(this));
        }
    }
}
