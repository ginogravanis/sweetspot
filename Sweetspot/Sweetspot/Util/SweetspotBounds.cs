using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SweetspotApp.Database;
using SweetspotApp.Input;

namespace SweetspotApp.Util
{
    public class SweetspotBounds
    {
        protected readonly int MAX_ROUNDS = 100;
        protected float left;
        protected float right;
        protected float front;
        protected float back;
        protected Random rng;
        protected bool initialized = false;

        public static Vector2 WorldToScreenCoords(Rectangle bounds, Vector2 position)
        {
            float x = bounds.Left + (bounds.Width / 2) + ((bounds.Width / 2f) * position.X / (KinectManager.SENSOR_RANGE / 2f));
            float y = bounds.Top + bounds.Height * (position.Y / KinectManager.SENSOR_RANGE);
            return new Vector2((int)Math.Round(x), (int)Math.Round(y));
        }

        public SweetspotBounds(IDatabase database)
        {
            rng = new Random();
            foreach (var point in database.LoadSweetspotBounds())
                Add(point);
        }

        public Sweetspot GenerateRandomSweetspot()
        {
            float width = right - left;
            float depth = back - front;

            float x = (float)rng.NextDouble() * width + left;
            float y = (float)rng.NextDouble() * depth + front;
            Sweetspot result = new Sweetspot((float)x, (float)y);

            return result;
        }

        public Sweetspot GenerateSweetspot(Vector2 userPosition)
        {
            Sweetspot candidate;
            int roundsRemaining = MAX_ROUNDS;

            do
            {
                candidate = GenerateRandomSweetspot();
                roundsRemaining--;
            }
            while (candidate.IsUserTooCloseToSweetspot(userPosition) && roundsRemaining > 0);

            if (roundsRemaining <= 0)
                Logger.Log(String.Format("Exhausted {0} rounds of sweetspot generation.", MAX_ROUNDS));

            return candidate;
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
