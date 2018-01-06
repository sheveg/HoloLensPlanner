using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    public class Tile : MonoBehaviour
    {

        public float Height;

        public float Width;

        public Mesh Mesh;
      
        public const float TileJoint = 1;

        public void Awake()
        {
            Mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
        }
    }
}
