﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement.Screens
{
    public enum TaskState { Active, Completing, GracePeriod, Aborting }

    public class TaskScreen : Screen
    {
        protected static readonly Color TASK_ACTIVE_COLOR = Color.White;
        protected static readonly Color TASK_COMPLETE_COLOR = Color.Chartreuse;
        protected static readonly Color TASK_ABORT_COLOR = Color.MediumVioletRed;
        protected static readonly Color TASK_GRACE_COLOR = Color.Teal;
        protected static readonly double TASK_COMPLETE_TIME = 5f;       // in seconds
        protected static readonly double TASK_ABORT_TIME = 5f;        // in seconds
        protected static readonly double TASK_GRACE_TIME = 2.5f;        // in seconds

        public bool TaskCompleted { get; protected set; }

        protected int questionId;
        protected string questionText;
        protected string answerFilename;
        protected int roundId;
        protected string cue;
        protected Mapping mapping;
        protected TimeSpan elapsedTime;
        protected double timeSinceStateChange = 0f;     // in seconds
        protected double completionTimerSnapshot = 0f;     // in seconds
        protected TaskState currentState;

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
        }

        public override void Initialize()
        {
            base.Initialize();
            roundId = gc.Database.RecordRound(gc.GameId, cue, mapping);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            elapsedTime += gameTime.ElapsedGameTime;
            timeSinceStateChange += gameTime.ElapsedGameTime.TotalSeconds;

            switch (currentState)
            {
                case TaskState.Active:
                    if (isUserAnswering())
                        changeState(TaskState.Completing);
                    else if (!gc.Kinect.IsUserActive())
                        changeState(TaskState.Aborting);
                    break;

                case TaskState.Completing:
                    if (!isUserAnswering())
                    {
                        completionTimerSnapshot = timeSinceStateChange;
                        changeState(TaskState.GracePeriod);
                    }
                    else if (timeSinceStateChange >= TASK_COMPLETE_TIME)
                    {
                        markTaskAsCompleted();
                        NextScreen();
                    }
                    break;

                case TaskState.GracePeriod:
                    if (timeSinceStateChange >= TASK_GRACE_TIME)
                    {
                        completionTimerSnapshot = 0f;
                        changeState(TaskState.Active);
                    }
                    else if (isUserAnswering())
                        changeState(TaskState.Completing);
                    break;
                    
                case TaskState.Aborting:
                    if (gc.Kinect.IsUserActive())
                        changeState(TaskState.Active);
                    else if (timeSinceStateChange >= TASK_ABORT_TIME)
                        NextScreen();
                    break;
            }
        }

        protected void changeState(TaskState newState)
        {
            currentState = newState;
            timeSinceStateChange = 0f;
            switch (newState)
            {
                case TaskState.Active:
                    background = TASK_ACTIVE_COLOR;
                    break;
                case TaskState.Completing:
                    background = TASK_COMPLETE_COLOR;
                    timeSinceStateChange = completionTimerSnapshot;
                    break;
                case TaskState.Aborting:
                    background = TASK_ABORT_COLOR;
                    break;
                case TaskState.GracePeriod:
                    background = TASK_GRACE_COLOR;
                    break;
            }
        }

        public override void NextScreen()
        {
            base.NextScreen();
            if (TaskCompleted)
                gc.NextQuestion();
            else
                gc.EndGame();
        }

        protected bool isTaskCompleted()
        {
            return (gc.Input.IsKeyPressed(Keys.T) || timeSinceStateChange >= TASK_COMPLETE_TIME);
        }

        protected void markTaskAsCompleted()
        {
            gc.Database.RoundCompleted(roundId, (int)elapsedTime.TotalMilliseconds);
            TaskCompleted = true;
        }

        protected bool isUserAnswering()
        {
            var kinect = gc.Kinect;
            return kinect.IsUserActive() && 
                   kinect.sweetspot.IsUserAnswering(kinect.GetUserPosition());
        }
    }
}