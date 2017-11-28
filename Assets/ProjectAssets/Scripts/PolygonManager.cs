using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;

namespace HoloLensPlanner
{
    /// <summary>
    /// Manages all geometries in the scene
    /// </summary>
    public class PolygonManager : Singleton<PolygonManager>, IPolygonClosable
    {
        public PolygonLine LinePrefab;
        public PolygonPoint PointPrefab;

        [HideInInspector]
        public Polygon CurrentPolygon;

        /// <summary>
        ///  Handle new point users place
        /// </summary>
        public void AddPoint()
        {      
            if (CurrentPolygon == null || CurrentPolygon.IsFinished)
            {
                CurrentPolygon = CreateNewPolygon();         
            }
            var hitPoint = GazeManager.Instance.HitPosition;
            var point = Instantiate(PointPrefab, hitPoint, Quaternion.identity);
            point.SetRootPolygon(CurrentPolygon);
            CurrentPolygon.Points.Add(point);
            // create a line when we have more than one point
            if (CurrentPolygon.Points.Count > 1)
            {
                // determine the position and direction of the line
                var index = CurrentPolygon.Points.Count - 1;
                var line = Instantiate(LinePrefab);
                line.SetPoints(CurrentPolygon.Points[index - 1], CurrentPolygon.Points[index]);
                line.SetRootPolygon(CurrentPolygon);
                // connect the line from the previous point to the current point
                CurrentPolygon.Points[index - 1].OutgoingEdge = line;
                point.IngoingEdge = line;
                //disable gazeObject component on last point so it is always only on last point
                GazeObject pointGazeObject = CurrentPolygon.Points[index - 1].GetComponent<GazeObject>();
                if (pointGazeObject)
                {
                    pointGazeObject.enabled = false;
                }
            }  
        }

        /// <summary>
        /// Finish current geometry
        /// </summary>
        public void ClosePolygon()
        {
            if (CurrentPolygon != null && CurrentPolygon.Points.Count > 3)
            {
                CurrentPolygon.IsFinished = true;
                var index = CurrentPolygon.Points.Count - 1;
                var line = Instantiate(LinePrefab);
                line.SetPoints(CurrentPolygon.Points[index - 1], CurrentPolygon.Points[index]);
                line.transform.parent = CurrentPolygon.transform;

                // connect the last point with the first point
                CurrentPolygon.Points[CurrentPolygon.Points.Count - 1].OutgoingEdge = line;
                CurrentPolygon.Points[0].IngoingEdge = line;
            }
        }

        /// <summary>
        /// Calculate an area of triangle
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        private float CalculateTriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var a = Vector3.Distance(p1, p2);
            var b = Vector3.Distance(p1, p3);
            var c = Vector3.Distance(p3, p2);
            var p = (a + b + c) / 2f;
            var s = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));

            return s;
        }
        /// <summary>
        /// Calculate an area of geometry
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private float CalculatePolygonArea(Polygon polygon)
        {
            var s = 0.0f;
            var i = 1;
            var n = polygon.Points.Count;
            for (; i < n - 1; i++)
                s += CalculateTriangleArea(polygon.Points[0].transform.position, polygon.Points[i].transform.position, polygon.Points[i + 1].transform.position);
            return 0.5f * Mathf.Abs(s);
        }

        /// <summary>
        /// reset current unfinished geometry
        /// </summary>
        public void Reset()
        {
            if (CurrentPolygon != null && !CurrentPolygon.IsFinished)
            {
                Destroy(CurrentPolygon);
                CurrentPolygon = CreateNewPolygon();
            }
        }

        private Polygon CreateNewPolygon()
        {
            var newPolygonGO = new GameObject("Polygon");
            Polygon newPolygon = newPolygonGO.AddComponent<Polygon>();
            return newPolygon;
        }
    }
}
