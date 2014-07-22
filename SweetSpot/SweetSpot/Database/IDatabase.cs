using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SweetSpot.ScreenManagement;
using SweetSpot.Util;

namespace SweetSpot.Database
{
    public interface IDatabase
    {
        IEnumerable<Vector2> LoadSweetspotBounds();

        void SaveSweetspotBounds(IEnumerable<Vector2> sweetspotBounds);

        int GetNewGameID();

        int RecordRound(int gameID, string cue, Mapping mapping);
        void SetSweetspot(int roundID, Vector2 sweetspot);

        QuizItem GetQuestion();

        void RoundCompleted(int roundID, int timestamp);

        void RecordUserPosition(int roundID, Vector2 position, int timestamp);
    }
}
