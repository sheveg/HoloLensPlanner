using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

public class DrawRoom : MonoBehaviour {

    [SerializeField]
    private float scanTime = 20f;

    [SerializeField]
    private Material defaultMaterial;

    [SerializeField]
    private Material floorPlaneMaterial;

    [SerializeField]
    private Text instructionText;

    [SerializeField]
    private LayerMask currentPlaneLayer;

    private float? m_FloorHeight;

    private GameObject m_FloorPlane;

    private LayerMask[] m_GazeManagerLayerMask;

    // Use this for initialization
    void Start () {
        SpatialMappingManager.Instance.SetSurfaceMaterial(defaultMaterial);
        m_GazeManagerLayerMask = GazeManager.Instance.PrioritizedLayerMasksOverride;

        
    }
	
	// Update is called once per frame
	void Update () {
        if (!m_FloorHeight.HasValue)
        {
            if ((Time.unscaledTime - SpatialMappingManager.Instance.StartTime) < scanTime)
            {
                Debug.Log("Scanning!");
            }
            else
            {
                if (SpatialMappingManager.Instance.IsObserverRunning())
                {
                    // Stop the observer.
                    SpatialMappingManager.Instance.StopObserver();
                }

                m_FloorHeight = GazeManager.Instance.HitPosition.y;
                PlaneManager.Instance.CreatePlane(new Vector3(0, m_FloorHeight.Value, 0), floorPlaneMaterial);
            }
        }
        else
        {
            instructionText.text = "Create points by air tapping to create your floor! When you are done do a holding gesture!";
        }
	}
}
