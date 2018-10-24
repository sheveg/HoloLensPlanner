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

        //public static bool IsInPolygon(this PolygonPoint testPoint, Polygon polygon)
        //{
        //    if (polygon.Points.Count < 3) return false;
        //    bool isInPolygon = false;
        //    var lastVertex = polygon.Points[polygon.Points.Count - 1];
        //    foreach (var vertex in polygon.Points)
        //    {
        //        if (testPoint.transform.position.y.IsBetween(lastVertex.transform.position.y, vertex.transform.position.y))
        //        {
        //            double t = (testPoint.transform.position.y - lastVertex.transform.position.y) / (vertex.transform.position.y - lastVertex.transform.position.y);
        //            double x = t * (vertex.transform.position.x - lastVertex.transform.position.x) + lastVertex.transform.position.x;
        //            if (x >= testPoint.transform.position.x) isInPolygon = !isInPolygon;
        //        }
        //        else
        //        {
        //            if (testPoint.transform.position.y == lastVertex.transform.position.y && testPoint.transform.position.x < lastVertex.transform.position.x && vertex.transform.position.y > testPoint.transform.position.y) isInPolygon = !isInPolygon;
        //            if (testPoint.transform.position.y == vertex.transform.position.y && testPoint.transform.position.x < vertex.transform.position.x && lastVertex.transform.position.y > testPoint.transform.position.y) isInPolygon = !isInPolygon;
        //        }

        //        lastVertex = vertex;
        //    }

        //    return isInPolygon;
        //}

        //public static bool IsBetween(this float x, float a, float b)
        //{
        //    return (x - a) * (x - b) < 0;
        //}
    }
}
