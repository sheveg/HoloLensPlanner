using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    public class PolygonLine : PolygonObject
    {
        public PolygonPoint From { get; private set; }
        public PolygonPoint To { get; private set; }

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


