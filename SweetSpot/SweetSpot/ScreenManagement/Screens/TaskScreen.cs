using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot.ScreenManagement.Screens
{
    public class TaskScreen : TestScreen
    {
        protected int test;
        protected int testSubject;
        protected string cue;
        protected Mapping mapping;
        protected bool taskCompleted = false;
        protected TimeSpan elapsedTime;

        public TaskScreen(ScreenManager screenManager, string cue, Mapping mapping, bool shuffleItems=true)
            : base(screenManager)
        {
            elapsedTime = TimeSpan.FromSeconds(0);
            this.cue = cue;
            this.mapping = mapping;
        }

        public override void Initialize()
        {
            base.Initialize();
            testSubject = screenManager.TestSubject;
            test = initializeTest();
        }

        protected virtual int initializeTest()
        {
            return screenManager.Database.RecordTest(testSubject, cue, mapping);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            elapsedTime += gameTime.ElapsedGameTime;

            if (screenManager.Input.IsKeyPressed(Keys.T))
            {
                markTaskAsCompleted();
            }
        }

        public override void NextScreen(GameTime gameTime)
        {
            base.NextScreen(gameTime);
            if (taskCompleted)
                screenManager.NextQuestion();
            else
                screenManager.EndGame();
        }

        protected void markTaskAsCompleted()
        {
            screenManager.Database.TestCompleted(test, (int)elapsedTime.TotalMilliseconds);
            taskCompleted = true;
        }
    }
}
