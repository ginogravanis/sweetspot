using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SweetSpot_2._0
{
    class Calibrator
    {
        protected List<Vector2> referencePoints;

        public static Tuple<Matrix, Matrix> Calibrate(Calibrator calibrator1, Calibrator calibrator2)
        {
            Matrix matrix1 = Matrix.Identity;
            Matrix matrix2 = Matrix.Identity;

            calibrator1.Reset();
            calibrator2.Reset();

            return new Tuple<Matrix, Matrix>(matrix1, matrix2);
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
