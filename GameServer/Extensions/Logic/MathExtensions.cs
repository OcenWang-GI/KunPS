using Protocol;

namespace GameServer.Extensions.Logic
{
    internal static class MathExtensions
    {
        /// <summary>
        /// Calculates the Euclidean distance between two vectors.
        /// </summary>
        /// <param name="self">The first vector.</param>
        /// <param name="other">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        public static float GetDistance(this Vector self, Vector other)
        {
            float x = self.X - other.X;
            float y = self.Y - other.Y;

            return (float)Math.Sqrt(x * x + y * y);
        }
    }
}