using System;
using Microsoft.Xna.Framework;

namespace Sweetspot.Input
{
    public interface ICalibrationProvider
    {
        bool HasCalibrationDataFor(string deviceId);

        Tuple<float, Vector3> LoadCalibration(string deviceId);

        void SaveCalibration(string deviceId, float axisTilt, Vector3 translate);
    }
}
