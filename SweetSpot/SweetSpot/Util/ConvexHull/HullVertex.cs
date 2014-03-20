using Microsoft.Xna.Framework;

namespace SweetSpot.Util.ConvexHull
{
    public class HullVertex
    {
        public Vector2 Vector { get; set; }
        public double Angle { get; set; }

        public HullVertex(Vector2 v, double r)
        {
            Vector = v;
            Angle = r;
        }
    }
}
