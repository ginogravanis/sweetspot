using System;
using Microsoft.Xna.Framework;

namespace Sweetspot.Input
{
    public interface ICalibrationProvider
    {
        bool HasCalibrationDataFor(string deviceID);

        Tuple<float, Vector3> LoadCalibration(string deviceID);

        void SaveCalibration(string deviceID, float axisTilt, Vector3 translate);
    }
}
