using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    public class RoomManager : Singleton<RoomManager>, IInputClickHandler, IHoldHandler
    {

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
            GameObject planeGO = null;
            switch (m_CurrentPlaneType)
            {
                // for now we assume that the floor has no stairs or ramps,
                // so it should consist of points approx. on the same plane
                case PlaneType.Floor:
                    if (Floor != null)
                        Destroy(Floor);
                    Floor = createFloor(polygon);
                    planeGO = Floor;
                    break;
                case PlaneType.Ceiling:
                case PlaneType.Wall:
                default:
                    break;
            }
            var roomPlane = planeGO.AddComponent<RoomPlane>();
            roomPlane.Setup(polygon, m_CurrentPlaneType.Value);
            m_CurrentPlaneType = null;
        }

        private Vector3 calculatePolygonCenter(Polygon polygon)
        {
            Vector3 center = Vector3.zero;
            foreach (var point in polygon.Points)
            {
                center += point;
            }
            return center / polygon.Points.Count;
        }

        private GameObject createFloor(Polygon polygon)
        {
            // get the center point of the polygon
            Vector3 polygonCenter = calculatePolygonCenter(polygon);
            List<Vector2> vertices2D = new List<Vector2>();
            foreach (var point in polygon.Points)
            {
                vertices2D.Add(new Vector2(point.x - polygonCenter.x, point.z - polygonCenter.z));
            }
            Mesh realMesh = MeshUtility.CreatePolygonMesh(vertices2D);
            // create a new gameObject and add mesh components
            GameObject floor = new GameObject("Floor");
            floor.transform.position = polygonCenter;
            MeshFilter meshFiler = floor.AddComponent<MeshFilter>();
            meshFiler.mesh = realMesh;
            MeshRenderer meshRenderer = floor.AddComponent<MeshRenderer>();
            meshRenderer.material = FloorMaterial;
            return floor;
        }

        private bool checkIfPlacable(PlaneType planeType, Vector3 position)
        {
            return true;
        }
    }

    public enum PlaneType
    {
        Floor,
        Ceiling,
        Wall
    }
}
