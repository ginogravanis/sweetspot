using System;
using Microsoft.Xna.Framework;

namespace SweetspotApp.Util
{
    public class Sweetspot
    {
        // Following numbers in meters.
        public readonly float MAX_INTERACTION_RADIUS = 2f;
        public readonly float RADIUS = 0.1f;

        // Following numbers in percent of MAX_INTERACTION_RADIUS.
        public readonly float ANSWER_RADIUS = 0.02f;
        protected readonly float SAFTEY_RADIUS = 0.2f;

        public Vector2 Position { get; set; }

        public float X
        {
            get { return Position.X; }
        }

        public float Y
        {
            get { return Position.Y; }
        }
        public Sweetspot()
        {
            new Vector2();
        }

        public Sweetspot(Vector2 sweetspot)
        {
            Position = sweetspot;
        }

        public Sweetspot(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public bool IsUserAnswering(Vector2 userPosition)
        {
            return isUserWithin(userPosition, ANSWER_RADIUS);
        }

        public bool IsUserTooCloseToSweetspot (Vector2 userPosition)
        {
            return isUserWithin(userPosition, SAFTEY_RADIUS);
        }

        protected bool isUserWithin(Vector2 userPosition, float threshold)
        {
            float distanceToSweetspotBorder = GetDistanceFromSweetspot(userPosition);
            float vicinityPercent = distanceToSweetspotBorder / (MAX_INTERACTION_RADIUS - RADIUS);

            return vicinityPercent < threshold;
        }

        public float GetDistanceFromSweetspot(Vector2 userPosition)
        {
            float distanceFromSweetspotCenter = Math.Abs((Position - userPosition).Length());
            return Math.Max(distanceFromSweetspotCenter - RADIUS, 0);
        }

        public Vector2 GetVectorToSweetspot(Vector2 userPosition)
        {
            return Position - userPosition;
        }
    }
}
