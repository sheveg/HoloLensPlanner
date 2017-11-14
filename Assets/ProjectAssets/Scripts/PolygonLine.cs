using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonLine : MonoBehaviour {

    public void SetPosition(Vector3 from, Vector3 to)
    {
        var centerPos = (from + to) * 0.5f;
        var direction = to - from;
        var distance = Vector3.Distance(to, from);
        transform.position = centerPos;
        transform.rotation = Quaternion.LookRotation(direction);
        transform.localScale = new Vector3(distance, 0.005f, 0.005f);
        transform.Rotate(Vector3.down, 90f);
    }
}
