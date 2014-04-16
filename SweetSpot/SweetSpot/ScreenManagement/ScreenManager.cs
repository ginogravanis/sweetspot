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
        public Screen CurrentScreen { get { return screens[currentScreen]; } }
        public int TestSubject { get; internal set; }

        protected Scene scene;
        protected IList<Screen> screens;
        protected int currentScreen;

        public ScreenManager(Game game, Scene scene)
            : base(game)
        {
            Debug = false;
            var db = new SQLiteAdapter();
            screens = new List<Screen>();
            Kinect = new SensorManager(db);
            Input = new InputManager();
            Database = db;
            this.scene = scene;
        }

        public override void Initialize()
        {
            base.Initialize();
            TestSubject = Database.GetNewSubjectID();

            switch (scene)
            {
                case Scene.Calibration:
                    Add(ScreenFactory.CreateCalibration(this));
                    break;
                case Scene.Tests:
                    ConvexHull sweetSpotBounds = new ConvexHull();
                    foreach (var point in Database.LoadSweetSpotBounds())
                        sweetSpotBounds.Add(point);
                    Add(GenerateTestSession(sweetSpotBounds));
                    break;
            }

            currentScreen = 0;
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

        public void Add(IEnumerable<Screen> screens)
        {
            foreach (var screen in screens)
                Add(screen);
        }

        public void Add(Screen screen)
        {
            screens.Add(screen);
        }

        public IEnumerable<Screen> GenerateTestSession(ConvexHull sweetSpotBounds)
        {
            List<Screen> screens = new List<Screen>();
            List<Cue> cues = EnumUtil.GetValues<Cue>().Skip(1).ToList();
            cues.Shuffle();
            int cueIndex = 0;

            string[] startingPositions = { "rechts", "links" };
            int startingPosition = 0;

            screens.Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Test subject {0}", TestSubject)));
            screens.AddRange(ScreenFactory.CreateSnellenTest(this));
            screens.AddRange(ScreenFactory.CreateIshiharaTest(this));
            screens.AddRange(ScreenFactory.CreatePelliRobsonTest(this));
            screens.AddRange(ScreenFactory.CreateDemo(this));
            for (int i = 1; i <= TESTS_PER_CUE; i++)
            {
                screens.Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Dry run {0}", i, TESTS_PER_CUE)));
                screens.Add(ScreenFactory.CreateBaselineScreen(this));
            }
            foreach (Cue cue in cues)
            {
                cueIndex++;
                for (int test = 1; test <= TESTS_PER_CUE; test++)
                {
                    startingPosition = (TestSubject + cueIndex + test) % startingPositions.Length;
                    screens.Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Cue {0}\nTest {2}\nStart von {4}", cueIndex, cues.Count, test, TESTS_PER_CUE, startingPositions[startingPosition])));
                    screens.Add(ScreenFactory.CreateTestScreen(this, cue, sweetSpotBounds.GenerateInternalPoint()));
                }
                screens.Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Questionnaire {0}", cueIndex)));
            }
            screens.Add(ScreenFactory.CreateTransitionScreen(this, "Thank you for participating!"));

            return screens;
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
                currentScreen += 1;
                if (currentScreen >= screens.Count)
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

        public override void Draw(GameTime gameTime)
        {
            CurrentScreen.Draw(gameTime);
        }
    }
}
