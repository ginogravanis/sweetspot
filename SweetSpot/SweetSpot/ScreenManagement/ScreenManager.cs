﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.Database;
using SweetSpot.Input;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement
{
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
            screens = new List<Screen>();
            Kinect = new SensorManager();
            Input = new InputManager();
            Database = new SQLiteAdapter();
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

            AddScreen(ScreenFactory.CreateTransitionScreen(this, "Calibration"));
            AddScreen(ScreenFactory.CreateCalibrationScreen(this));
        }

        protected override void UnloadContent()
        {
            foreach (Screen screen in screens)
                screen.UnloadContent();
        }

        public void AddScreen(Screen screen)
        {
            screen.LoadContent();
            screens.Add(screen);
        }

        public void GenerateTestSession(ConvexHull sweetSpotBounds)
        {
            List<Screen> testSession = new List<Screen>();
            List<Cue> cues = EnumUtil.GetValues<Cue>().Skip(1).ToList();
            cues.Shuffle();
            int cueIndex = 0;

            string[] startingPositions = { "rechts", "links" };
            int startingPosition = 0;

            testSession.Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Test subject {0}", TestSubject)));
            testSession.AddRange(ScreenFactory.CreateSnellenTest(this));
            testSession.AddRange(ScreenFactory.CreateIshiharaTest(this));
            testSession.AddRange(ScreenFactory.CreatePelliRobsonTest(this));
            testSession.Add(ScreenFactory.CreateTransitionScreen(this, "Demo"));
            testSession.Add(ScreenFactory.CreateDemoScreen(this));
            for (int i = 1; i <= TESTS_PER_CUE; i++)
            {
                testSession.Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Dry run {0}/{1}", i, TESTS_PER_CUE)));
                testSession.Add(ScreenFactory.CreateTestScreen(this, Cue.Baseline, new Vector2()));
            }
            foreach (Cue cue in cues)
            {
                cueIndex++;
                for (int test = 1; test <= TESTS_PER_CUE; test++)
                {
                    startingPosition = (TestSubject + cueIndex + test) % startingPositions.Length;
                    testSession.Add(ScreenFactory.CreateTransitionScreen(this, String.Format("Cue {0}/{1}\nTest {2}/{3}\nStart von {4}", cueIndex, cues.Count, test, TESTS_PER_CUE, startingPositions[startingPosition])));
                    testSession.Add(ScreenFactory.CreateTestScreen(this, cue, sweetSpotBounds.GenerateInternalPoint()));
                }
                testSession.Add(ScreenFactory.CreateTransitionScreen(this, "Questionnaire"));
            }
            testSession.Add(ScreenFactory.CreateTransitionScreen(this, "Thank you for participating!"));

            foreach (Screen screen in testSession)
            {
                AddScreen(screen);
            }
        }

        public void ToggleDebug()
        {
            Debug = !Debug;
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentScreen.Finished)
                RemoveScreen();

            if (!CurrentScreen.Initialized)
                CurrentScreen.Initialize();

            Input.Update(gameTime);
            Kinect.Update(gameTime);
            CurrentScreen.Update(gameTime);
        }

        protected void RemoveScreen()
        {
            CurrentScreen.UnloadContent();
            screens.RemoveAt(0);
            if (screens.Count == 0)
                Game.Exit();
        }

        public override void Draw(GameTime gameTime)
        {
            CurrentScreen.Draw(gameTime);
        }
    }
}
