using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class RoomPlane : MonoBehaviour
    {
        [SerializeField]
        private Material MaterialInEditMode;

        [SerializeField]
        private Material PolygonMaterialInEditMode;

        [SerializeField]
        private Material PolygonMaterialInFocusMode;

        public Polygon MeshPolygon { get; private set; }

        public PlaneType Type { get; private set; }

        public PlaneState State { get; private set; }

        private Material m_MaterialInEditModeCopy;

        private Material m_PolygonMaterialInEditModeCopy;

        private Material m_PolygonMaterialInFocusModeCopy;

        private MeshFilter m_MeshFilter;

        /// <summary>
        /// Creates a mesh for this room plane out of the polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="planeType"></param>
        public void Setup(Polygon polygon, PlaneType planeType)
        {
            polygon.MeshPlane = this;
            MeshPolygon = polygon;
            Type = planeType;
            polygon.transform.SetParent(transform, true);
            createMesh();
            projectPointsOnPlane();
            State = PlaneState.Idle;
            gameObject.AddComponent<BoxCollider>();
        }
        
        /// <summary>
        /// Updates the points of the polygon to be projected on the mesh plane.
        /// </summary>
        private void projectPointsOnPlane()
        {
            // The projection of a point q onto a plane given by a point p and a normal n  is : q_proj = q - dot(q - p, n) * n
            foreach (var point in MeshPolygon.Points)
            {
                point.transform.position -= Vector3.Dot(point.transform.position - transform.position, transform.up) * transform.up;
            }
            // correct the lines to the new projected points
            for (int i = 1; i <= MeshPolygon.Points.Count; i++)
            {
                PolygonPoint from = MeshPolygon.Points[i - 1];
                PolygonPoint to = MeshPolygon.Points[i % MeshPolygon.Points.Count];
                MeshPolygon.Points[i - 1].OutgoingEdge.SetPoints(from, to);
            }
        }

        /// <summary>
        /// Creates a mesh for this room plane out of the polygon.
        /// </summary>
        private void createMesh()
        {
            List<Vector2> vertices2D = new List<Vector2>();
            foreach (var point in MeshPolygon.Points)
            {
                vertices2D.Add(new Vector2(point.transform.position.x - MeshPolygon.Center.x, point.transform.position.z - MeshPolygon.Center.z));
            }
            Mesh mesh = MeshUtility.CreatePolygonMesh(vertices2D);
            // create a new gameObject and add mesh components
            m_MeshFilter = GetComponent<MeshFilter>();
            m_MeshFilter.mesh = mesh;
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            m_MaterialInEditModeCopy = new Material(MaterialInEditMode);
            meshRenderer.material = m_MaterialInEditModeCopy;
        }
    }
}


