using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    class Calibrator
    {
        protected List<Vector2> referencePoints;

        public static Tuple<Matrix, Matrix> Calibrate(Calibrator calibrator1, Calibrator calibrator2)
        {
            List<Vector2> points1 = calibrator1.GetReferencePoints();
            List<Vector2> points2 = calibrator2.GetReferencePoints();
            if (points1.Count < 2 || points2.Count < 2)
            {
                throw new InvalidOperationException("Need two reference points for calibration!");
            }

            float axisTilt1 = CalculateAxisTilt(CreateAxis(points1.Last<Vector2>(), points1.First<Vector2>()));
            float axisTilt2 = CalculateAxisTilt(CreateAxis(points2.Last<Vector2>(), points2.First<Vector2>()));
            Matrix rotation1 = Matrix.CreateRotationZ(-axisTilt1);
            Matrix rotation2 = Matrix.CreateRotationZ(-axisTilt2);

            calibrator1.Reset();
            calibrator2.Reset();

            return new Tuple<Matrix, Matrix>(rotation1, rotation2);
        }

        public static Vector2 CreateAxis(Vector2 v1, Vector2 v2)
        {
            if (v1.X > v2.X)
            {
                return v1 - v2;
            }
            else
            {
                return v2 - v1;
            }
        }

        public static float CalculateAxisTilt(Vector2 axis)
        {
            return ClampAngle((float)Math.Atan2(axis.Y, axis.X));
        }

        public static float ClampAngle(float radians)
        {
            if (radians <= -Math.PI)
            {
                radians += (float)(2 * Math.PI);
            }
            else if (radians > Math.PI)
            {
                radians -= (float)(2 * Math.PI);
            }

            return radians;
        }

        public Calibrator()
        {
            referencePoints = new List<Vector2>();
        }

        public void AddPoint(Vector2 position)
        {
            referencePoints.Add(position);
            if (referencePoints.Count > 2)
                referencePoints.RemoveRange(0, referencePoints.Count - 2);
        }

        public bool HasEnoughReferencePoints()
        {
            return referencePoints.Count >= 2;
        }

        public void Reset()
        {
            referencePoints.Clear();
        }

        public List<Vector2> GetReferencePoints()
        {
            return referencePoints;
        }
    }
}
