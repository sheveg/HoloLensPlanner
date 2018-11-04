using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HoloLensPlanner.Utilities
{
    public static class MathUtility
    {
        /// <summary>
        /// Wraps the given index from 0 to arrayLength.
        /// In Unity "%" is the mathematical rem not mod, they behave differentely for different signs of a,b so we need to transform rem to mod.
        /// Source: http://answers.unity.com/answers/1120641/view.html
        /// </summary>
        /// <param name="index"></param>
        /// <param name="arrayLength"></param>
        /// <returns></returns>
        public static int WrapArrayIndex(int index, int arrayLength)
        {
            return (index % arrayLength + arrayLength) % arrayLength;
        }

        /// <summary>
        /// Returns whether point p is in polygon.
        /// Source: https://stackoverflow.com/a/16391873
        /// </summary>
        /// <param name="p"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsPointInPolygon(Vector2 p, Vector2[] polygon)
        {
            double minX = polygon[0].x;
            double maxX = polygon[0].x;
            double minY = polygon[0].y;
            double maxY = polygon[0].y;
            for (int i = 1; i < polygon.Length; i++)
            {
                Vector2 q = polygon[i];
                minX = Math.Min(q.x, minX);
                maxX = Math.Max(q.x, maxX);
                minY = Math.Min(q.y, minY);
                maxY = Math.Max(q.y, maxY);
            }

            if (p.x < minX || p.x > maxX || p.y < minY || p.y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].y > p.y) != (polygon[j].y > p.y) &&
                     p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        /// <summary>
        /// Returns the area of a flat polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static float GetPolygonArea(List<Vector2> polygon)
        {
            float temp = 0;
            int i = 0;
            for (; i < polygon.Count; i++)
            {
                if (i != polygon.Count - 1)
                {
                    float mulA = polygon[i].x * polygon[i + 1].y;
                    float mulB = polygon[i + 1].x * polygon[i].y;
                    temp = temp + (mulA - mulB);
                }
                else
                {
                    float mulA = polygon[i].x * polygon[0].y;
                    float mulB = polygon[0].x * polygon[i].y;
                    temp = temp + (mulA - mulB);
                }
            }
            temp *= 0.5f;
            return Mathf.Abs(temp);
        }

        /// <summary>
        /// Returns the perimeter of a flat polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static float GetPolygonPerimeter(List<Vector2> polygon)
        {
            float temp = 0f;
            for (int i = 0; i < polygon.Count; i++)
            {
                temp += Vector2.Distance(polygon[i], polygon[WrapArrayIndex(i + 1, polygon.Count)]);
            }
            return temp;
        }

        public static bool ContainsPoint(Vector3[] polyPoints, Vector3 p)
        { 
            var j = polyPoints.Length - 1;
            var inside = false; 

            for (int i = 0; i < polyPoints.Length; j = i++)
            { 
                if (((polyPoints[i].z <= p.z && p.y<polyPoints[j].z) || (polyPoints[j].z <= p.z && p.y<polyPoints[i].z)) && 
                    (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.z - polyPoints[i].z) / (polyPoints[j].z - polyPoints[i].z) + polyPoints[i].x))
                {
                    inside = !inside;

                }
            } 
            return inside;
        }

        /// <summary>
        /// Gets the coordinates of the intersection point of two lines.
        /// </summary>
        /// <param name="A1">A point on the first line.</param>
        /// <param name="A2">Another point on the first line.</param>
        /// <param name="B1">A point on the second line.</param>
        /// <param name="B2">Another point on the second line.</param>
        /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
        /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
        public static Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
        {
            float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

            if (tmp == 0)
            {
                // No solution!
                found = false;
                return Vector2.zero;
            }

            float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

            found = true;

            return new Vector2(
                B1.x + (B2.x - B1.x) * mu,
                B1.y + (B2.y - B1.y) * mu
            );
        }
    }
}
