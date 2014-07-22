using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot.ScreenManagement.Screens
{
    public class TaskScreen : TestScreen
    {
        protected int roundID;
        protected string cue;
        protected Mapping mapping;
        protected bool taskCompleted = false;
        protected TimeSpan elapsedTime;

        public TaskScreen(ScreenManager sm, string cue, Mapping mapping)
            : base(sm)
        {
            elapsedTime = TimeSpan.FromSeconds(0);
            this.cue = cue;
            this.mapping = mapping;
        }

        public override void Initialize()
        {
            base.Initialize();
            roundID = sm.Database.RecordRound(sm.GameID, cue, mapping);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            elapsedTime += gameTime.ElapsedGameTime;

            if (sm.Input.IsKeyPressed(Keys.T))
            {
                markTaskAsCompleted();
            }
        }

        public override void NextScreen(GameTime gameTime)
        {
            base.NextScreen(gameTime);
            if (taskCompleted)
                sm.NextQuestion();
            else
                sm.EndGame();
        }

        protected void markTaskAsCompleted()
        {
            sm.Database.RoundCompleted(roundID, (int)elapsedTime.TotalMilliseconds);
            taskCompleted = true;
        }
    }
}
