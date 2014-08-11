using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public enum GameState { Active, Completing, GracePeriod, Aborting, Timeout }
    public enum TaskState { Complete, Fail, Skip }

    public class TaskScreen : Screen
    {
        protected static readonly double TASK_COMPLETE_TIME = 7f;       // in seconds
        protected static readonly double TASK_ABORT_TIME = 5f;        // in seconds
        protected static readonly double TASK_GRACE_TIME = 1.5f;        // in seconds
        protected static readonly double TASK_TIMEOUT_TIME = 3f;        // in seconds

        public bool TaskCompleted { get; protected set; }

        protected int questionId;
        protected string questionText;
        protected string answerFilename;
        protected string answerText;
        protected int roundId;
        protected string cue;
        protected Mapping mapping;
        protected TimeSpan elapsedTime;
        protected double timeSinceStateChange = 0f;     // in seconds
        protected double completionTimerSnapshot = 0f;     // in seconds
        protected double timeoutTimer = 40f;     //in seconds
        protected GameState currentGameState;
        protected TaskState currentTaskState;

        public TaskScreen(GameController gc, string cue, Mapping mapping)
            : base(gc)
        {
            elapsedTime = TimeSpan.FromSeconds(0);
            this.cue = cue;
            this.mapping = mapping;
            TaskCompleted = false;
            QuizItem quizItem = gc.Database.GetQuestion();
            questionId = quizItem.Id;
            questionText = quizItem.Question;
            answerFilename = quizItem.AnswerFilename;
            answerText = quizItem.AnswerText;
        }

        public override void Initialize()
        {
            base.Initialize();
            DateTime now = DateTime.Now;
            roundId = gc.Database.RecordRound(gc.GameId, now, cue, mapping);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            elapsedTime += gameTime.ElapsedGameTime;
            timeSinceStateChange += gameTime.ElapsedGameTime.TotalSeconds;
            timeoutTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            switch (currentGameState)
            {
                case GameState.Active:
                    if (isUserAnswering())
                        changeGameState(GameState.Completing);
                    else if (!gc.Kinect.IsUserActive())
                        changeGameState(GameState.Aborting);
                    else if (timeoutTimer <= TASK_TIMEOUT_TIME)
                        changeGameState(GameState.Timeout);
                    break;

                case GameState.Completing:
                    if (!isUserAnswering())
                    {
                        completionTimerSnapshot = timeSinceStateChange;
                        changeGameState(GameState.GracePeriod);
                    }
                    else if (timeSinceStateChange >= TASK_COMPLETE_TIME)
                    {
                        markTaskAsCompleted();
                        NextScreen();
                    }
                    break;

                case GameState.GracePeriod:
                    if (timeSinceStateChange >= TASK_GRACE_TIME)
                    {
                        completionTimerSnapshot = 0f;
                        changeGameState(GameState.Active);
                    }
                    else if (isUserAnswering())
                        changeGameState(GameState.Completing);
                    break;
                    
                case GameState.Aborting:
                    if (isUserAnswering())
                        changeGameState(GameState.Active);
                    else if (timeSinceStateChange >= TASK_ABORT_TIME)
                    {
                        markGamekAsFail();
                        NextScreen();
                    }
                    break;

                case GameState.Timeout:
                    Console.WriteLine(timeoutTimer);
                    if (isUserAnswering())
                        changeGameState(GameState.Active);
                    else if (timeoutTimer <= 0)
                    {
                        markTaskAsIncomplete();
                        NextScreen();
                    }
                    break;
            }
        }

        protected void changeGameState(GameState newState)
        {
            currentGameState = newState;
            timeSinceStateChange = 0f;

            if (currentGameState == GameState.Completing)
                timeSinceStateChange = completionTimerSnapshot;

            if (currentGameState != GameState.Timeout)
                timeoutTimer = 5;
        }

        public override void NextScreen()
        {
            base.NextScreen();

            switch (currentTaskState)
            {
                case TaskState.Complete:
                    gc.NextQuestion();
                    break;

                case TaskState.Fail:
                    gc.EndGame();
                    break;

                case TaskState.Skip:
                    gc.NextQuestion();
                    break;
            }
        }

        protected bool isTaskCompleted()
        {
            return (gc.Input.IsKeyPressed(Keys.T) || timeSinceStateChange >= TASK_COMPLETE_TIME);
        }

        protected void markTaskAsCompleted()
        {
            gc.Database.RoundCompleted(roundId, (int)elapsedTime.TotalMilliseconds);
            currentTaskState = TaskState.Complete;
        }

        protected void markTaskAsIncomplete()
        {
            currentTaskState = TaskState.Skip;
        }

        protected void markGamekAsFail()
        {
            currentTaskState = TaskState.Fail;
        }

        protected bool isUserAnswering()
        {
            var kinect = gc.Kinect;
            return kinect.IsUserActive() && 
                   kinect.sweetspot.IsUserAnswering(kinect.GetUserPosition());
        }
    }
}
