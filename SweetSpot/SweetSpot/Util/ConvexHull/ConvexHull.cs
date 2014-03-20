using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SweetSpot.Util.ConvexHull
{
    public class ConvexHull
    {
        protected LinkedList<HullVertex> hull;
        protected Vector2 centroid;
        protected bool validated = false;
        protected Random rng;

        public bool Valid { get { return validated; } }

        public static Vector2 CalculateCentroid(IEnumerable<Vector2> points)
        {
            Vector2 centroid = new Vector2(0, 0);
            int pointCount = points.Count();
            foreach (var point in points)
            {
                centroid += point / pointCount;
            }
            return centroid;
        }

        public static bool TriangleIncludes(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            // compute vectors
            Vector2 v0 = c - a;
            Vector2 v1 = b - a;
            Vector2 v2 = p - a;

            // compute dot products
            float dot00 = Vector2.Dot(v0, v0);
            float dot01 = Vector2.Dot(v0, v1);
            float dot02 = Vector2.Dot(v0, v2);
            float dot11 = Vector2.Dot(v1, v1);
            float dot12 = Vector2.Dot(v1, v2);

            // compute barycentric coordinates
            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // check if point p is in triange abc
            return (u >= 0) && (v >= 0) && (u + v < 1);
        }

        public static Vector2 GeneratePointInBounds(float left, float right, float front, float back, Random rng)
        {
            float width = right - left;
            float depth = back - front;

            float x = (float)rng.NextDouble() * width + left;
            float y = (float)rng.NextDouble() * depth + front;
            Vector2 result = new Vector2((float)x, (float)y);

            return result;
        }

        public ConvexHull()
        {
            hull = new LinkedList<HullVertex>();
            rng = new Random();
        }

        public bool Includes(Vector2 point)
        {
            if (!validated)
                return false;

            HullVertex candidate = constructHullElement(point);
            Tuple<HullVertex, HullVertex> adjacentVertices = findAdjacentVertices(candidate.Angle);

            return TriangleIncludes(centroid, adjacentVertices.Item1.Vector, adjacentVertices.Item2.Vector, point);
        }

        protected Tuple<HullVertex, HullVertex> findAdjacentVertices(double angle)
        {
            HullVertex lower = null;
            HullVertex upper = null;

            if (angle <= hull.First.Value.Angle || angle > hull.Last.Value.Angle)
            {
                lower = hull.Last.Value;
                upper = hull.First.Value;
            }
            else
            {
                for (var vertex = hull.First; vertex != hull.Last; vertex = vertex.Next)
                {
                    if (vertex.Value.Angle > angle)
                    {
                        upper = vertex.Value;
                        lower = vertex.Previous.Value;
                        break;
                    }
                }
            }

            if (lower == null || upper == null)
                throw new ApplicationException("No adjacent hull vertices found.");

            return new Tuple<HullVertex, HullVertex>(lower, upper);
        }

        public void Add(Vector2 point)
        {
            if (!validated)
            {
                hull.AddLast(new HullVertex(point, 0));
                tryValidate();
                return;
            }

            if (Includes(point))
                return;

            HullVertex candidate = constructHullElement(point);
            if (!Includes(point))
            {
                var supportingVertices = findSupportingVertices(candidate);
                addBetween(supportingVertices.Item1, supportingVertices.Item2, candidate);
            }
        }

        protected void tryValidate()
        {
            if (validated || hull.Count < 3)
                return;

            centroid = CalculateCentroid(GetPoints());
            hull = sortPoints(hull.Select(e => e.Vector));
            validated = true;
        }

        protected LinkedList<HullVertex> sortPoints(IEnumerable<Vector2> points)
        {
            var list = points.Select(point => constructHullElement(point));
            return new LinkedList<HullVertex>(list.OrderBy(e => e.Angle));
        }

        protected HullVertex constructHullElement(Vector2 point)
        {
            Vector2 locationVector = point - centroid;
            double r = Math.Atan2(locationVector.Y, locationVector.X);
            if (r < 0)
            {
                r += 2 * Math.PI;
            }
            return new HullVertex(point, r);
        }

        protected Tuple<HullVertex, HullVertex> findSupportingVertices(HullVertex p)
        {
            HullVertex nearest = findNearest(p.Vector);
            LinkedListNode<HullVertex> lower = hull.Find(nearest);
            LinkedListNode<HullVertex> upper = lower;
            double maxAngle = 0;

            while (true)
            {
                var nextNode = lower.PreviousOrLast();
                var nextVector = nextNode.Value.Vector;
                var upperVector = upper.Value.Vector - p.Vector;
                var lowerVector = nextVector - p.Vector;
                double nextAngle = getAngleBetween(lowerVector, upperVector);
                if (nextAngle > maxAngle)
                {
                    lower = nextNode;
                    maxAngle = nextAngle;
                }
                else
                    break;
            }

            while (true)
            {
                var nextNode = upper.NextOrFirst();
                var nextVector = nextNode.Value.Vector;
                var upperVector = nextVector - p.Vector;
                var lowerVector = lower.Value.Vector - p.Vector;
                double nextAngle = getAngleBetween(lowerVector, upperVector);
                if (nextAngle > maxAngle)
                {
                    upper = nextNode;
                    maxAngle = nextAngle;
                }
                else
                    break;
            }

            return new Tuple<HullVertex, HullVertex>(lower.Value, upper.Value);
        }

        protected double getAngleBetween(Vector2 a, Vector2 b)
        {
            double alpha = wrapAngle(Math.Atan2(a.Y, a.X));
            double beta = wrapAngle(Math.Atan2(b.Y, b.X));
            return Math.Abs(Math.Max(alpha, beta) - Math.Min(alpha, beta));
        }

        protected double wrapAngle(double alpha)
        {
            while (alpha < 0)
                alpha += 2 * Math.PI;

            while (alpha >= 2 * Math.PI)
                alpha -= 2 * Math.PI;

            return alpha;
        }

        protected HullVertex findNearest(Vector2 p)
        {
            HullVertex nearest = hull.First.Value;
            float shortestDistance = (p - nearest.Vector).Length();

            foreach (HullVertex v in hull)
            {
                float distance = (v.Vector - p).Length();
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearest = v;
                }
            }

            return nearest;
        }

        protected void addBetween(HullVertex lower, HullVertex upper, HullVertex p)
        {
            var start = hull.Find(lower);
            var end = hull.Find(upper);
            hull.ClearBetween(start, end);
            if (p.Angle > lower.Angle)
                hull.AddAfter(start, p);
            else
                hull.AddBefore(end, p);
        }

        public void AddAll(IEnumerable<Vector2> pointList)
        {
            foreach (var point in pointList)
                Add(point);
        }

        public IEnumerable<Vector2> GetPoints()
        {
            return hull.Select(p => p.Vector);
        }

        public Vector2 GenerateInternalPoint()
        {
            float left = float.MaxValue;
            float right = float.MinValue;
            float front = float.MaxValue;
            float back = float.MinValue;

            foreach (var vertex in hull)
            {
                float x = vertex.Vector.X;
                float y = vertex.Vector.Y;
                left = Math.Min(left, x);
                right = Math.Max(right, x);
                front = Math.Min(front, y);
                back = Math.Max(back, y);
            }

            Vector2 candidate = new Vector2();
            do
            {
                candidate = GeneratePointInBounds(left, right, front, back, rng);
            } while (!Includes(candidate));

            return candidate;
        }
    }
}
