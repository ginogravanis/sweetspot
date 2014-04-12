using System;
using Microsoft.Xna.Framework;

namespace SweetSpot.ScreenManagement.Screens
{
    public class TaskScreen : TestScreen
    {
        protected int test;
        protected int testSubject;
        protected string cue;
        protected bool taskCompleted = false;
        protected TimeSpan elapsedTime;

        public TaskScreen(ScreenManager screenManager, string cue, bool shuffleItems=true)
            : base(screenManager)
        {
            elapsedTime = TimeSpan.FromSeconds(0);
            this.cue = cue;
        }

        public override void Initialize()
        {
            base.Initialize();
            testSubject = screenManager.TestSubject;
            test = initializeTest();
        }

        protected virtual int initializeTest()
        {
            return screenManager.Database.RecordTest(testSubject, cue);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            elapsedTime += gameTime.ElapsedGameTime;
        }

        public override void SkipAction(GameTime gameTime)
        {
            if (taskCompleted)
                base.SkipAction(gameTime);
            else
                markTaskAsCompleted();
        }

        protected void markTaskAsCompleted()
        {
            screenManager.Database.TestCompleted(test, (int)elapsedTime.TotalMilliseconds);
            taskCompleted = true;
        }
    }
}
