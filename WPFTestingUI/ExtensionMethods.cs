using OpenTK;
using System.Drawing;

namespace WPFTestingUI
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds the given <paramref name="point"/>'s X and Y components to this point and returns the result.
        /// </summary>
        /// <param name="otherPoint">The current point to add the given point to.</param>
        /// <param name="point">The point to add to this point.</param>
        /// <returns></returns>
        public static PointF Add(this PointF otherPoint, PointF point) => new PointF(otherPoint.X + point.X, otherPoint.Y + point.Y);


        public static float MapValue(this byte value, float from0, float from1, float to0, float to1)
        {
            return ((float)value).MapValue(from0, from1, to0, to1);
        }


        public static float MapValue(this float value, float fromStart, float fromStop, float toStart, float toStop)
        {
            //http://james-ramsden.com/map-a-value-from-one-number-scale-to-another-formula-and-c-code/
            return toStart + (toStop - toStart) * ((value - fromStart) / (fromStop - fromStart));
        }


        public static Vector4 MapValues(this Vector4 value, float fromStart, float fromStop, float toStart, float toStop)
        {
            return new Vector4
            {
                X = value.X.MapValue(fromStart, fromStop, toStart, toStop),
                Y = value.Y.MapValue(fromStart, fromStop, toStart, toStop),
                Z = value.Z.MapValue(fromStart, fromStop, toStart, toStop),
                W = value.W.MapValue(fromStart, fromStop, toStart, toStop)
            };
        }


        public static Vector4 ToVector4(this Color clr)
        {
            return new Vector4(clr.R, clr.G, clr.B, clr.A);
        }
    }
}
