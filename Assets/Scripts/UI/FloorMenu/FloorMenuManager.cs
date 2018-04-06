using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

namespace HoloLensPlanner
{
    /// <summary>
    /// Floor menu shows details of the current floor and offers functionality to create/edit a floor via buttons.
    /// </summary>
    public class FloorMenuManager : SingleInstance<FloorMenuManager>, IInputClickHandler
    {
        #region Editor variables

        /// <summary>
        /// Button to create a new floor.
        /// </summary>
        [SerializeField]
        private Button CreateFloorButton;

        /// <summary>
        /// Button to edit the current floor.
        /// </summary>
        [SerializeField]
        private Button EditFloorButton;

        /// <summary>
        /// Text to display the area of the current floor.
        /// </summary>
        [SerializeField]
        private Text AreaText;

        /// <summary>
        /// Text to display the perimeter of the current floor.
        /// </summary>
        [SerializeField]
        private Text PerimeterText;

        /// <summary>
        /// Top view image of the current floor.
        /// </summary>
        [SerializeField]
        private RawImage FloorImage;

        /// <summary>
        /// Placeholder image when no floor is created yet.
        /// </summary>
        [SerializeField]
        private GameObject FloorImagePlaceholder;

        /// <summary>
        /// Background to indicate that we are in edit mode.
        /// </summary>
        [SerializeField]
        private GameObject EditBackground;

        #endregion // Editor variables

        /// <summary>
        /// RenderTexture to create the top view image of the current floor.
        /// </summary>
        private RenderTexture m_FloorTexture;

        /// <summary>
        /// Background color for the floor image.
        /// </summary>
        private Color m_FloorImageBackgroundColor = new Color(40/255f, 45/255f, 48/255f, 1.0f);

        /// <summary>
        /// Small percentage offset for the floor image so it has a small border between the floor image and the background image.
        /// </summary>
        private float m_FloorImageOffset = 0.05f;

        /// <summary>
        /// In Edit mode we can move the floor or each point in 3D.
        /// </summary>
        private bool m_EditMode;

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (!m_EditMode)
                return;

            eventData.Use();
            FloorTransformOperator.Instance.ChangeState(eventData.selectedObject);
        }

        private void Start()
        {
            CreateFloorButton.onClick.AddListener(createFloor);
            EditFloorButton.onClick.AddListener(editFloor);
        }

        private void OnEnable()
        {
            createFloorImage();
            updateAreaText();
            updatePerimeterText();
        }

        private void createFloor()
        {
            MenuHub.Instance.Pin();
            RoomManager.Instance.CreateFloorPlane();
        }

        private void editFloor()
        {
            if (RoomManager.Instance.Floor == null)
                return;

            if (m_EditMode)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
                if (SpatialMappingManager.IsInitialized)
                    SpatialMappingManager.Instance.gameObject.SetActive(true);
                EditBackground.gameObject.SetActive(false);
                FloorTransformOperator.Instance.HideAll();
                foreach (var point in RoomManager.Instance.Floor.MeshPolygon.Points)
                {
                    GazeScaler gazeScaler;
                    if ((gazeScaler = point.GetComponent<GazeScaler>()) != null)
                    {
                        gazeScaler.enabled = false;
                        gazeScaler.FocusScale = 5f;
                    }
                }
                m_EditMode = false;
            }
            else
            {
                if (TilesGenerator.Instance.FinishedTileFloor != null)
                {
                    Destroy(TilesGenerator.Instance.FinishedTileFloor);
                }
                InputManager.Instance.AddGlobalListener(gameObject);
                if (SpatialMappingManager.IsInitialized)
                    SpatialMappingManager.Instance.gameObject.SetActive(false);
                EditBackground.gameObject.SetActive(true);
                foreach (var point in RoomManager.Instance.Floor.MeshPolygon.Points)
                {
                    GazeScaler gazeScaler;
                    if ((gazeScaler = point.GetComponent<GazeScaler>()) != null)
                    {
                        gazeScaler.enabled = true;
                        gazeScaler.FocusScale = 2f;
                    }
                }
                m_EditMode = true;
            }
        }

        private void createFloorImage()
        {
            if (RoomManager.Instance.Floor != null)
            {
                FloorImage.gameObject.SetActive(true);
                FloorImagePlaceholder.SetActive(false);
                // find out the size of the bounding box of the floor
                float xScale = RoomManager.Instance.Floor.GetComponent<BoxCollider>().size.x;
                float zScale = RoomManager.Instance.Floor.GetComponent<BoxCollider>().size.z;
                // create an orthographic camera with has the floor exactly in its frustum
                GameObject tempCamGO = new GameObject("tempCam");
                Camera tempCam = tempCamGO.AddComponent<Camera>();
                tempCam.orthographic = true;
                tempCam.orthographicSize = Mathf.Max(xScale, zScale) * 0.5f;
                tempCam.orthographicSize += tempCam.orthographicSize * m_FloorImageOffset;
                tempCam.clearFlags = CameraClearFlags.Color;
                tempCam.backgroundColor = m_FloorImageBackgroundColor;
                // place the camera above the floor
                tempCam.transform.position = RoomManager.Instance.Floor.transform.TransformPoint(RoomManager.Instance.Floor.GetComponent<BoxCollider>().center) + Vector3.up;
                tempCam.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                int resolution = 512;
                // create a rendertexture to render the view of the camera on it
                m_FloorTexture = new RenderTexture(resolution, resolution, 24);
                RenderTexture.active = m_FloorTexture;
                tempCam.targetTexture = m_FloorTexture;
                tempCam.Render();
                FloorImage.texture = m_FloorTexture;

                // cleanup
                RenderTexture.active = null;
                tempCam.targetTexture = null;
                Destroy(tempCamGO);
            }
            else
            {
                FloorImage.gameObject.SetActive(false);
                FloorImagePlaceholder.SetActive(true);
            }
        }

        private void updateAreaText()
        {
            if (RoomManager.Instance.Floor != null)
                AreaText.text = RoomManager.Instance.Floor.Area.ToString("n2") + " m²";
        }

        private void updatePerimeterText()
        {
            if (RoomManager.Instance.Floor != null)
                PerimeterText.text = RoomManager.Instance.Floor.Perimeter.ToString("n2") + " m";
        }
    }
}


