using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using HoloLensPlanner.Utilities;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    public enum PlaneType
    {
        Floor,
        Ceiling,
        Wall
    }

    /// <summary>
    /// RoomManager holds a reference of the current room the user is in. Provides functions to create a <see cref="RoomPlane"/>. 
    /// </summary>
    public class RoomManager : SingleInstance<RoomManager>, IInputClickHandler, IHoldHandler
    {
        [SerializeField]
        private RoomPlane RoomPlanePrefab;

        [SerializeField]
        private float HoldTime = 1f;

        /// <summary>
        /// Timer to show the user how long he has to hold to finish the current roomplane.
        /// </summary>
        [SerializeField]
        private Transform TimerObject;

        /// <summary>
        /// Image component of timer to control the fill amount.
        /// </summary>
        [SerializeField]
        private Image TimerImage;

        public RoomPlane Floor { get; private set; }
        public GameObject Ceiling { get; private set; }
        public List<GameObject> Walls { get; private set; }

        private const string createFloorTutorialText = "Click to place the corners of the floor. Click and hold to create the floor.";

        private IEnumerator m_TimerAnimation;
        private bool m_HoldFinished;

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
            m_HoldFinished = false;
            if (CurrentPlaneType.HasValue && PolygonManager.Instance.CurrentPolygon.Points.Count >= 4)
            {
                startTimerAnimation();
            }
            else
            {
                TextManager.Instance.ShowWarning("You need at least four points to create a floor!");
            }
        }

        public void OnHoldCompleted(HoldEventData eventData)
        {
            stopTimerAnimation();
            if (m_HoldFinished)
            {
                FinishRoomPlane();
            }
        }

        public void OnHoldCanceled(HoldEventData eventData)
        {
            stopTimerAnimation();
            if (m_HoldFinished)
            {
                FinishRoomPlane();
            }
        }

        #endregion // Interface implementations

        public void CreateFloorPlane()
        {
            if (SpatialMappingManager.IsInitialized)
            {
                SpatialMappingManager.Instance.DrawVisualMeshes = true;
            }
            InstructionMenu.Instance.Instruction = createFloorTutorialText;
            InstructionMenu.Instance.ShowFloatingInstruction();
            MenuHub.Instance.ShowMenu(InstructionMenu.Instance.gameObject);
            if (Floor != null)
                Destroy(Floor.gameObject);

            if (TilesGenerator.Instance.FinishedTileFloor != null)
                Destroy(TilesGenerator.Instance.FinishedTileFloor.gameObject);

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
            PolygonManager.Instance.ClosePolygon();
            Polygon polygon = PolygonManager.Instance.CurrentPolygon;
            RoomPlane roomPlane = null;
            switch (CurrentPlaneType)
            {
                // for now we assume that the floor has no stairs or ramps,
                // so it should consist of points approx. on the same plane
                case PlaneType.Floor:
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
            TextManager.Instance.HideTutorial();
            MenuHub.Instance.ShowMenu(MainMenuManager.Instance.gameObject);
            if (SpatialMappingManager.IsInitialized)
            {
                SpatialMappingManager.Instance.DrawVisualMeshes = false;
            }
        }

        private RoomPlane createFloor(Vector3 position)
        {
            RoomPlane floor = Instantiate(RoomPlanePrefab, position, Quaternion.identity);
            floor.gameObject.name = "Floor";
            Floor = floor;
            return floor;

        }

        /// <summary>
        /// Starts the timer animation and kills the last one if needed.
        /// </summary>
        private void startTimerAnimation()
        {
            if (m_TimerAnimation != null)
                StopCoroutine(m_TimerAnimation);

            m_TimerAnimation = timerAnimation();
            StartCoroutine(m_TimerAnimation);
            TimerObject.transform.position = PolygonManager.Instance.CurrentPolygon.Center;
            TimerObject.gameObject.SetActive(true);
        }

        /// <summary>
        /// Stops the current timer animation.
        /// </summary>
        private void stopTimerAnimation()
        {
            if (m_TimerAnimation != null)
                StopCoroutine(m_TimerAnimation);

            TimerObject.gameObject.SetActive(false);
        }

        private IEnumerator timerAnimation()
        {
            float timer = 0f;
            TimerImage.fillAmount = 0f;
            TimerImage.color = Color.grey;
            while (timer < HoldTime)
            {
                timer += Time.deltaTime;
                TimerImage.fillAmount = Mathf.Min(1f, timer / HoldTime);
                yield return null;
            }
            TimerImage.color = Color.green;
            m_HoldFinished = true;
        }
    }
}
