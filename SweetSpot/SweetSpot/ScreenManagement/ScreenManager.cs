using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Database;
using SweetSpot.Input;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement
{
    public enum Scene { Calibration, Tests };

    public class ScreenManager : DrawableGameComponent
    {
        public readonly int TESTS_PER_CUE = 5;

        public bool Debug { get; internal set; }
        public SensorManager Kinect { get; internal set; }
        public InputManager Input { get; internal set; }
        public IDatabase Database { get; internal set; }
        public SpriteBatch SpriteBatch { get; internal set; }
        public Screen CurrentScreen { get { return screens[0]; } }
        public int TestSubject { get; internal set; }

        protected IList<Screen> screens;

        public ScreenManager(Game game)
            : base(game)
        {
            Debug = false;
            var db = new SQLiteAdapter();
            screens = new List<Screen>();
            Kinect = new SensorManager(db);
            Input = new InputManager();
            Database = db;
        }

        public override void Initialize()
        {
            base.Initialize();
            Kinect.sweetSpot = new Vector2(0f, 2f);
            TestSubject = Database.GetNewSubjectID();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            //Add(ScreenFactory.CreateCalibration(this));
            GenerateTestSession(new ConvexHull());
        }

        protected override void UnloadContent()
        {
            foreach (Screen screen in screens)
                screen.UnloadContent();
        }

        public void Add(IEnumerable<Screen> screens)
        {
            foreach (var screen in screens)
                Add(screen);
        }

        public void Add(Screen screen)
        {
            screens.Add(screen);
        }

        public void GenerateTestSession(ConvexHull sweetSpotBounds)
        {
            List<Cue> cues = EnumUtil.GetValues<Cue>().Skip(1).ToList();
            cues.Shuffle();
            int cueIndex = 0;

            string[] startingPositions = { "rechts", "links" };
            int startingPosition = 0;

            Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Test subject {0}", TestSubject)));
            Add(ScreenFactory.CreateSnellenTest(this));
            Add(ScreenFactory.CreateIshiharaTest(this));
            Add(ScreenFactory.CreatePelliRobsonTest(this));
            Add(ScreenFactory.CreateDemo(this));
            for (int i = 1; i <= TESTS_PER_CUE; i++)
            {
                Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Dry run {0}", i, TESTS_PER_CUE)));
                Add(ScreenFactory.CreateBaselineScreen(this));
            }
            foreach (Cue cue in cues)
            {
                cueIndex++;
                for (int test = 1; test <= TESTS_PER_CUE; test++)
                {
                    startingPosition = (TestSubject + cueIndex + test) % startingPositions.Length;
                    Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Cue {0}\nTest {2}\nStart von {4}", cueIndex, cues.Count, test, TESTS_PER_CUE, startingPositions[startingPosition])));
                    Add(ScreenFactory.CreateTestScreen(this, cue, sweetSpotBounds.GenerateInternalPoint()));
                }
                Add(ScreenFactory.CreateTransitionScreen(this, "Questionnaire"));
            }
            Add(ScreenFactory.CreateTransitionScreen(this, "Thank you for participating!"));
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
                RemoveScreen();
                if (screens.Count == 0)
                {
                    Game.Exit();
                    base.Update(gameTime);
                    return;
                }
            }

            if (!CurrentScreen.Initialized)
            {
                CurrentScreen.LoadContent();
                CurrentScreen.Initialize();
            }

            CurrentScreen.Update(gameTime);
        }

        protected void RemoveScreen()
        {
            CurrentScreen.UnloadContent();
            screens.RemoveAt(0);
        }

        public override void Draw(GameTime gameTime)
        {
            CurrentScreen.Draw(gameTime);
        }
    }
}
