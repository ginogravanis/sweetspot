using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sweetspot.ScreenManagement;
using Sweetspot.Util;

namespace Sweetspot.Database
{
    public interface IDatabase
    {
        IEnumerable<Vector2> LoadSweetspotBounds();

        void SaveSweetspotBounds(IEnumerable<Vector2> sweetspotBounds);

        int GetNewGameId();

        int RecordRound(int gameId, string cue, Mapping mapping);
        void SetSweetspot(int roundId, Vector2 sweetspot);

        QuizItem GetQuestion();

        void RoundCompleted(int roundId, int timestamp);

        void RecordUserPosition(int roundId, Vector2 position, int timestamp);
    }
}
