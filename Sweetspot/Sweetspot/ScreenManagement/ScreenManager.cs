using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.Database;
using SweetspotApp.Input;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement
{
    public enum Scene { Calibration, Game };

    public class ScreenManager : DrawableGameComponent
    {
        public bool Debug { get; protected set; }
        public SensorManager Kinect { get; protected set; }
        public InputManager Input { get; protected set; }
        public IDatabase Database { get; protected set; }
        public SweetspotBounds SweetspotBounds { get; protected set; }
        public SpriteBatch SpriteBatch { get; protected set; }
        public Screen CurrentScreen { get { return screens.First(); } }
        public int GameId { get; internal set; }

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
            GameId = Database.GetNewGameId();
            NextQuestion();
            Kinect.StartRecording(GameId);
        }

        public void NextQuestion()
        {
            Add(ScreenFactory.CreateQuestionScreen(
                this,
                Cue.Pixelate,
                Mapping.SCurve,
                SweetspotBounds.GenerateSweetspot(Kinect.GetViewerPosition())
                ));
        }

        public void EndGame()
        {
            Add(ScreenFactory.CreateTitleScreen(this));
        }
    }
}
