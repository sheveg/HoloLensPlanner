using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class PlaneManager : Singleton<PlaneManager>, IInputClickHandler, IHoldHandler
{

    // Set up prefabs
    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private GameObject pointPrefab;
    [SerializeField]
    private GameObject textPrefab;


    private bool m_IsCreating = false;

    void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);
    }

    public void CreatePlane(Vector3 position, Material material)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = position;
        plane.GetComponent<Renderer>().material = material;
        m_IsCreating = true;
        plane.layer = LayerMask.NameToLayer("CurrentPlane");
    }

    // if current mode is geometry mode, try to finish geometry
    public void OnPolygonClose()
    {
        IPolygonClosable client = PolygonManager.Instance;
        //client.ClosePolygon(linePrefab, textPrefab);
        Polygon polygon = PolygonManager.Instance.CurrentPolygon;
        if (polygon.IsFinished)
        {

        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (m_IsCreating)
        {
            if (GazeManager.Instance.HitObject != null)
            {
                if (GazeManager.Instance.HitObject.layer == LayerMask.NameToLayer("CurrentPlane"))
                {
                    //PolygonManager.Instance.AddPoint(linePrefab, pointPrefab, textPrefab);
                }
                else Debug.Log("Not right layer!");
            }
            else Debug.Log("Nothing hit!");
            
        }
            
    }

    public void OnHoldStarted(HoldEventData eventData)
    {
        if (m_IsCreating)
        {
            OnPolygonClose();
            m_IsCreating = false;
        }
       
    }

    public void OnHoldCompleted(HoldEventData eventData)
    {
        // Nothing to do
    }

    public void OnHoldCanceled(HoldEventData eventData)
    {
        // Nothing to do
    }
}

