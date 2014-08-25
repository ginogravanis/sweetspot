using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SweetspotApp.ScreenManagement;
using SweetspotApp.Util;

namespace SweetspotApp.Database
{
    public interface IDatabase
    {
        IEnumerable<Vector2> LoadSweetspotBounds();
        void SaveSweetspotBounds(IEnumerable<Vector2> sweetspotBounds);

        int GetNewGameId();
        QuizItem GetQuestion();

        int RecordRound(int gameId, DateTime startTime, string cue, Mapping mapping);
        void SetSweetspot(int roundId, Sweetspot sweetspot);
        void RoundCompleted(int roundId, int timestamp, float accuracy, bool taskCompleted);
        void RecordUserPosition(int roundId, Vector2 position, int timestamp);
    }
}
