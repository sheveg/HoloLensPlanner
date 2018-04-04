using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    /// <summary>
    /// Represents a plane in a room e.g. a wall, ceiling or floor.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class RoomPlane : MonoBehaviour
    {
        [SerializeField]
        private Material Material;

        public Polygon MeshPolygon { get; private set; }

        public PlaneType Type { get; private set; }

        /// <summary>
        /// Area of the RoomPlane. (Default value: -1)
        /// </summary>
        public float Area
        {
            get
            {
                if (m_Area < 0f)
                    m_Area = calculateArea();
                return m_Area;
            }
        }

        /// <summary>
        /// Perimeter of the RoomPlane. (Default value: -1)
        /// </summary>
        public float Perimeter
        {
            get
            {
                if (m_Perimeter < 0f)
                    m_Perimeter = calculatePerimeter();
                return m_Perimeter;
            }
        }

        /// <summary>
        /// Cached variable of the area.
        /// </summary>
        private float m_Area = -1f;

        /// <summary>
        /// Cached variable of the perimeter.
        /// </summary>
        private float m_Perimeter = -1f;

        private Material m_MaterialCopy;

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
            m_MaterialCopy = new Material(Material);
            meshRenderer.material = m_MaterialCopy;
        }

        /// <summary>
        /// Returns the polygon points in local space.
        /// </summary>
        /// <returns></returns>
        private List<Vector2> getLocalSpacePolygonPoints()
        {
            List<Vector2> localSpacePoints = new List<Vector2>();
            foreach (var point in MeshPolygon.Points)
            {
                var localPoint3DSpace = transform.InverseTransformPoint(point.transform.position);
                localSpacePoints.Add(new Vector2(localPoint3DSpace.x, localPoint3DSpace.z));
            }
            return localSpacePoints;
        }

        /// <summary>
        /// Returns the area of the polygon.
        /// </summary>
        /// <returns></returns>
        private float calculateArea()
        {
            return MathUtility.GetPolygonArea(getLocalSpacePolygonPoints());
        }

        /// <summary>
        /// Returns the perimeter of the polygon.
        /// </summary>
        /// <returns></returns>
        private float calculatePerimeter()
        {
            return MathUtility.GetPolygonPerimeter(getLocalSpacePolygonPoints());
        }
    }
}


