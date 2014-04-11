using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SweetSpot.Database
{
    public interface IDatabase
    {
        bool HasCalibrationDataFor(string deviceID);

        Tuple<float, Vector3> LoadCalibration(string deviceID);

        void SaveCalibration(string deviceID, float axisTilt, Vector3 translate);

        IList<Vector2> LoadSweetSpotBounds();

        void SaveSweetSpotBounds(IEnumerable<Vector2> sweetSpotBounds);

        int GetNewSubjectID();

        int RecordTest(int testSubject, string cue, Vector2 sweetSpot);

        void TestCompleted(int test, int timestamp);

        void RecordUserPosition(int test, Vector2 position, int timestamp);
    }
}
