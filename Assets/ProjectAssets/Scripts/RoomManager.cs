using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;

public class RoomManager : Singleton<RoomManager>, IInputClickHandler, IHoldHandler {

    public Material FloorMaterial;

    public GameObject Floor { get; private set; }
    public GameObject Ceiling { get; private set; }
    public List<GameObject> Walls { get; private set; }

    private PlaneType? m_CurrentPlaneType;

    private void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);
    }

    #region Interface implementations

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (m_CurrentPlaneType.HasValue)
        {
            switch (m_CurrentPlaneType)
            {
                case PlaneType.Floor:
                    // all points of the floor polygon should be under the users head
                    if (GazeManager.Instance.HitPosition.y < GazeManager.Instance.GazeOrigin.y)
                    {
                        PolygonManager.Instance.AddPoint();
                    }
                    break;
                case PlaneType.Ceiling:
                case PlaneType.Wall:
                    PolygonManager.Instance.AddPoint();
                    break;
                default:
                    break;
            }
        }
    }

    public void OnHoldStarted(HoldEventData eventData)
    {
        if (m_CurrentPlaneType.HasValue)
        {
            FinishRoomPlane();
        }
    }

    public void OnHoldCompleted(HoldEventData eventData)
    {
        
    }

    public void OnHoldCanceled(HoldEventData eventData)
    {
        
    }

    #endregion // Interface implementations

    public void CreateFloorPlane()
    {
        m_CurrentPlaneType = PlaneType.Floor;
    }

    public void CreateWallPlane()
    {
        m_CurrentPlaneType = PlaneType.Wall;
    }

    public void CreateCeilingPlane()
    {
        m_CurrentPlaneType = PlaneType.Ceiling;
    }

    public void CreateRoomPlane(PlaneType planeType)
    {
        m_CurrentPlaneType = planeType;

    }

    public void FinishRoomPlane()
    {
        // first create the polygon
        IPolygonClosable client = PolygonManager.Instance;
        client.ClosePolygon();
        Polygon polygon = PolygonManager.Instance.CurrentPolygon;
        Vector3 polygonCenter = calculatePolygonCenter(polygon);
        switch (m_CurrentPlaneType)
        {
            // for now we assume that the floor has no stairs or ramps,
            // so it should consist of points approx. on the same plane
            case PlaneType.Floor:
                if (Floor != null)
                    Destroy(Floor);
                Floor = createFloor(polygon);
                break;
            case PlaneType.Ceiling:
            case PlaneType.Wall:
            default:
                break;
        }
        m_CurrentPlaneType = null;
    }

    private Vector3 calculatePolygonCenter(Polygon polygon)
    {
        Vector3 center = Vector3.zero ;
        foreach (var point in polygon.Points)
        {
            center += point;
        }
        return center / polygon.Points.Count;
    }

    private GameObject createFloor(Polygon polygon)
    {
        // get the center point of the polygon
        Vector3 center = calculatePolygonCenter(polygon);
        // create a polygon with one point more => the center point
        Vector3[] vertices = new Vector3[polygon.Points.Count + 1];
        // calculate the points in the center space
        for (int i = 0; i < polygon.Points.Count; i++)
        {
            Vector3 pointNotOnPlane = polygon.Points[i] - center;
            vertices[i] = new Vector3(pointNotOnPlane.x, 0, pointNotOnPlane.z);
        }
        vertices[vertices.Length - 1] = Vector3.zero;
        // create triangles from two vertex border points and the center point
        int[] triangles = new int[polygon.Points.Count * 3];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            triangles[i] = i / 3;
            triangles[i + 1] = (i / 3 + 1) % (vertices.Length - 1);
            triangles[i + 2] = vertices.Length - 1;
        }
        // point all normals up
        Vector3[] normals = new Vector3[polygon.Points.Count + 1];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
        }
        // create the actual mesh and assign the calculated mesh arrays
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        // create a new gameObject and add mesh components
        GameObject floor = new GameObject("Floor");
        floor.transform.position = center;
        MeshFilter meshFiler = floor.AddComponent<MeshFilter>();
        meshFiler.mesh = mesh;
        MeshRenderer meshRenderer = floor.AddComponent<MeshRenderer>();
        meshRenderer.material = FloorMaterial;
        return floor;
    }

   
}

public enum PlaneType
{
    Floor,
    Ceiling,
    Wall
}
