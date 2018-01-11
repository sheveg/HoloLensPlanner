using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    /// <summary>
    /// RoomManager holds a reference of the current room the user is in. Provides functions to create a <see cref="RoomPlane"/>. 
    /// </summary>
    public class RoomManager : Singleton<RoomManager>, IInputClickHandler, IHoldHandler
    {

        public RoomPlane RoomPlanePrefab;

        public GameObject Floor { get; private set; }
        public GameObject Ceiling { get; private set; }
        public List<GameObject> Walls { get; private set; }

        /// <summary>
        /// Planetype of the current roomplane which is in edit/creation mode.
        /// </summary>
        public PlaneType? CurrentPlaneType { get; private set; }

        #region Interface implementations

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (CurrentPlaneType.HasValue)
            {
                eventData.Use();
                switch (CurrentPlaneType)
                {
                    case PlaneType.Floor:
                        // all points of the floor polygon should be under the users head
                        if (GazeManager.Instance.HitObject.layer == 31 && GazeManager.Instance.HitPosition.y < GazeManager.Instance.GazeOrigin.y)
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
            if (CurrentPlaneType.HasValue)
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
            CurrentPlaneType = PlaneType.Floor;
            InputManager.Instance.PushModalInputHandler(gameObject);
        }

        public void CreateWallPlane()
        {
            CurrentPlaneType = PlaneType.Wall;
        }

        public void CreateCeilingPlane()
        {
            CurrentPlaneType = PlaneType.Ceiling;
        }

        public void CreateRoomPlane(PlaneType planeType)
        {
            CurrentPlaneType = planeType;

        }

        /// <summary>
        /// Creates a RoomPlane of the current planetype.
        /// </summary>
        public void FinishRoomPlane()
        {
            // first create the polygon
            IPolygonClosable client = PolygonManager.Instance;
            client.ClosePolygon();
            Polygon polygon = PolygonManager.Instance.CurrentPolygon;
            RoomPlane roomPlane = null;
            switch (CurrentPlaneType)
            {
                // for now we assume that the floor has no stairs or ramps,
                // so it should consist of points approx. on the same plane
                case PlaneType.Floor:
                    if (Floor != null)
                        Destroy(Floor);
                    roomPlane = createFloor(polygon.Center);
                    break;
                case PlaneType.Ceiling:
                case PlaneType.Wall:
                default:
                    break;
            }
            if (roomPlane == null)
                return;

            roomPlane.Setup(polygon, CurrentPlaneType.Value);
            CurrentPlaneType = null;
            InputManager.Instance.PopModalInputHandler();
            MainMenuManager.Instance.Show();
        }

        private RoomPlane createFloor(Vector3 position)
        {
            RoomPlane floor = Instantiate(RoomPlanePrefab, position, Quaternion.identity);
            floor.gameObject.name = "Floor";
            Floor = floor.gameObject;
            return floor;

        }

        private bool checkIfPlacable(PlaneType planeType, Vector3 position)
        {
            return true;
        }
    }
}
