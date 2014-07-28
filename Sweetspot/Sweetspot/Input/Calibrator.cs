using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SweetspotApp.Input
{
    class Calibrator
    {
        protected List<Vector2> referencePoints;

        public static Tuple<Tuple<float, Vector3>, Tuple<float, Vector3>> Calibrate(Calibrator calibrator1, Calibrator calibrator2)
        {
            List<Vector2> points1 = calibrator1.GetReferencePoints();
            List<Vector2> points2 = calibrator2.GetReferencePoints();
            if (points1.Count < 2 || points2.Count < 2)
            {
                throw new InvalidOperationException("Need two reference points for calibration!");
            }

            // Calculate rotation of both coordinate systems
            Vector2 point1A = points1.First<Vector2>();
            Vector2 point1B = points1.Last<Vector2>();
            Vector2 point2A = points2.First<Vector2>();
            Vector2 point2B = points2.Last<Vector2>();
            float axisTilt1 = CalculateAxisTilt(CreateAxis(point1B, point1A));
            float axisTilt2 = CalculateAxisTilt(CreateAxis(point2B, point2A));
            Matrix rotate1 = Matrix.CreateRotationZ(-axisTilt1);
            Matrix rotate2 = Matrix.CreateRotationZ(-axisTilt2);
            
            // Map coordinates to new respective coordinate systems
            point1A = Vector2.Transform(point1A, rotate1);
            point1B = Vector2.Transform(point1B, rotate1);
            point2A = Vector2.Transform(point2A, rotate2);
            point2B = Vector2.Transform(point2B, rotate2);

            // Calculate translation of both coordinate systems
            Vector2 midpoint1 = CalculateMidpoint(point1A, point1B);
            Vector2 midpoint2 = CalculateMidpoint(point2A, point2B);
            Vector2 targetMidpoint = CalculateMidpoint(midpoint1, midpoint2);
            Vector3 offset1 = MakeVector3From(targetMidpoint - midpoint1);
            Vector3 offset2 = MakeVector3From(targetMidpoint - midpoint2);

            calibrator1.Reset();
            calibrator2.Reset();

            Tuple<float, Vector3> calibration1 = new Tuple<float, Vector3>(-axisTilt1, offset1);
            Tuple<float, Vector3> calibration2 = new Tuple<float, Vector3>(-axisTilt2, offset2);
            return new Tuple<Tuple<float, Vector3>, Tuple<float, Vector3>>(calibration1, calibration2);
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

        public static Vector2 CalculateMidpoint(Vector2 a, Vector2 b)
        {
            return new Vector2(
                (a.X + b.X) / 2,
                (a.Y + b.Y) / 2
                );
        }

        public static Vector3 MakeVector3From(Vector2 v)
        {
            return new Vector3(v.X, v.Y, 0);
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
