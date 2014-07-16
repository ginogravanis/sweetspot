using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SweetSpot.ScreenManagement;
using SweetSpot.Util;

namespace SweetSpot.Database
{
    public interface IDatabase
    {
        IEnumerable<Vector2> LoadSweetSpotBounds();

        void SaveSweetSpotBounds(IEnumerable<Vector2> sweetSpotBounds);

        int GetNewSubjectID();

        int RecordTest(int testSubject, string cue, Mapping mapping);
        int RecordTest(int testSubject, string cue, Mapping mapping, Vector2 sweetSpot);

        QuizItem GetQuestion();

        void TestCompleted(int test, int timestamp);

        void RecordUserPosition(int test, Vector2 position, int timestamp);
    }
}
