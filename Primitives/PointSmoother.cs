using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Primitives
{  
    public class PointSmoother
    {
        public Vector2[] InitialPoints;

        /// <summary>
        /// This uses De Casteljau's algorithm to smooth a list of Vector2s. Be aware that it can be quite laggy if used with a lot of points.<\br>
        /// Create a new instance, and set <see cref=" InitialPoints"/> to the list, then call <see cref="GetSmoothedPoints(int)"/>.
        /// </summary>
        public PointSmoother(params Vector2[] initialPoints) => InitialPoints = initialPoints;

        /// <summary>
        /// Gets a list of smoothed points from the provided list.
        /// </summary>
        /// <param name="totalPoints">Do NOT put too many of these, anything above ~70-100 can cause lag</param>
        /// <returns></returns>
        public List<Vector2> GetSmoothedPoints(int totalPoints)
        {
            // Get the amount per step to increment based on the total number of points.
            float perStep = 1f / totalPoints;

            // Create a new list of points.
            List<Vector2> points = new();

            // Go through the amount of steps, adding a new point based on the current position through the point amount and the relevant position in the
            // ControlPoints array.
            for (float step = 0; step < 1f; step += perStep)
                points.Add(Evaluate(MathHelper.Clamp(step, 0f, 1f)));

            // Return the smoothed list.
            return points;
        }

        /// <summary>
        /// Use this to get a Vector2 at the current position on a bezier curve.
        /// </summary>
        /// <param name="interpolant">The current progress through the curve</param>
        /// <returns></returns>
        private Vector2 Evaluate(float interpolant)
        {
            // Create a local version of the main points to iterate over.
            Vector2[] points = InitialPoints;

            // While there is more than 2 points left in the list.
            while (points.Length > 2)
            {
                // Create a new array of points, one size less than the main.
                Vector2[] nextPoints = new Vector2[points.Length - 1];

                // For each new point,
                for (int i = 0; i < points.Length - 1; i++)
                    // Set the position in the next point array to between it and the next one, based on the provided interpolant.
                    nextPoints[i] = Vector2.Lerp(points[i], points[i + 1], interpolant);

                // Set points to the new next points array, effectively reducing its size by one and making its contents smoother
                points = nextPoints;
            }

            // An array of points of 1 or less has not been smoothened and will not draw.
            if (points.Length <= 1)
                return Vector2.Zero;

            // Return the amount final, smoothed points based on the same provided interpolant.
            return Vector2.Lerp(points[0], points[1], interpolant);
        }
    }
}