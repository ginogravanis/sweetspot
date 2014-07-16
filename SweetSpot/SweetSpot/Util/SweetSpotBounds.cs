using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SweetSpot.Util
{
    public class SweetSpotBounds
    {
        protected float left;
        protected float right;
        protected float front;
        protected float back;
        protected Random rng;
        protected bool initialized = false;

        public Vector2 GenerateInternalPoint()
        {
            float width = right - left;
            float depth = back - front;

            float x = (float)rng.NextDouble() * width + left;
            float y = (float)rng.NextDouble() * depth + front;
            Vector2 result = new Vector2((float)x, (float)y);

            return result;
        }

        public SweetSpotBounds()
        {
            rng = new Random();
        }

        public bool Includes(Vector2 point)
        {
            if (!initialized)
                return false;

            return point.X >= left
                && point.X <= right
                && point.Y >= front
                && point.Y <= back;
        }

        public void Add(Vector2 point)
        {
            if (!initialized)
            {
                left = right = point.X;
                back = front = point.Y;
                initialized = true;
                return;
            }

            if (point.X < left)
                left = point.X;
            else if (point.X > right)
                right = point.X;

            if (point.Y < front)
                front = point.Y;
            else if (point.Y > back)
                back = point.Y;
        }

        public void AddAll(IEnumerable<Vector2> points)
        {
            foreach (var point in points)
                Add(point);
        }

        public IEnumerable<Vector2> GetPoints()
        {
            List<Vector2> points = new List<Vector2>();

            if (!initialized)
                return points;

            points.Add(new Vector2(left, back));
            points.Add(new Vector2(left, front));
            points.Add(new Vector2(right, back));
            points.Add(new Vector2(right, front));
            return points;
        }

        public void Clear()
        {
            initialized = false;
        }
    }
}
