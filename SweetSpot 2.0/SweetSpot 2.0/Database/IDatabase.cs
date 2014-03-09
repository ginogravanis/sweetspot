﻿using System;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0.Database
{
    public interface IDatabase
    {
        bool HasCalibrationDataFor(string deviceID);

        Tuple<float, Vector3> GetCalibration(string deviceID);

        void SaveCalibration(string deviceID, float axisTilt, Vector3 translate);

        void SaveSweetSpots();

        void GetSweetSpots();

        void RecordUserPosition();
    }
}
