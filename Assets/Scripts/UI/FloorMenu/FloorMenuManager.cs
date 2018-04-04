using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    public class FloorMenuManager : SingleInstance<FloorMenuManager>
    {
        [SerializeField]
        private Button CreateFloorButton;

        [SerializeField]
        private Button EditFloorButton;

        [SerializeField]
        private Button ScanRoomButton;

        [SerializeField]
        private Text AreaText;

        [SerializeField]
        private Text PerimeterText;

        [SerializeField]
        private RawImage FloorImage;

        [SerializeField]
        private GameObject FloorImagePlaceholder;

        private RenderTexture m_FloorTexture;

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

        private void scanRoom()
        {

        }

        private void createFloor()
        {
            MenuHub.Instance.Pin();
            RoomManager.Instance.CreateFloorPlane();
        }

        private void editFloor()
        {

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
                tempCam.clearFlags = CameraClearFlags.Color;
                tempCam.backgroundColor = GetComponent<Image>().color;
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


