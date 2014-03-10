﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0.Database
{
    public interface IDatabase
    {
        bool HasCalibrationDataFor(string deviceID);

        Tuple<float, Vector3> LoadCalibration(string deviceID);

        void SaveCalibration(string deviceID, float axisTilt, Vector3 translate);

        List<Vector2> LoadSweetSpots();

        void SaveSweetSpot(int id, Vector2 sweetSpot);

        int RecordTest(int testSubject, string screen, int sweetSpot);

        void RecordUserPosition(int test, Vector2 position);
    }
}