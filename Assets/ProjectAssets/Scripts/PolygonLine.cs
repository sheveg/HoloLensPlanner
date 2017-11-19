using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    /// <summary>
    /// A PolygonLine is defined by two <see cref="PolygonPoint"/>s. It is used so we can reference each object in the polygon.
    /// </summary>
    public class PolygonLine : PolygonObject
    {
        public PolygonPoint From { get; private set; }
        public PolygonPoint To { get; private set; }

        /// <summary>
        /// Calculates the transform variables for the line depending on the given points.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void SetPoints(PolygonPoint from, PolygonPoint to)
        {
            var centerPos = (from.transform.position + to.transform.position) * 0.5f;
            var direction = to.transform.position - from.transform.position;
            var distance = Vector3.Distance(to.transform.position, from.transform.position);
            transform.position = centerPos;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.localScale = new Vector3(distance, 0.005f, 0.005f);
            transform.Rotate(Vector3.down, 90f);
            From = from;
            To = to;
        }
    }
}


