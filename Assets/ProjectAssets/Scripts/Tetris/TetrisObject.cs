using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class TetrisObject : MonoBehaviour, IInputClickHandler, IFocusable {

    bool m_Rotating;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Vector3 handPos;
        if (eventData.InputSource.TryGetPointerPosition(eventData.SourceId, out handPos))
        {
            if (GazeManager.Instance.GazeTransform.InverseTransformPoint(handPos).x > 0)
            {
                Debug.Log("Right");
                smoothRotation(new Vector3(0f, 0f, 90f));
            }
            else if(GazeManager.Instance.GazeTransform.InverseTransformPoint(handPos).x < 0)
            {
                Debug.Log("Left");
                smoothRotation(new Vector3(0f, 0f, -90f));
            }
        }
    }

    public void OnFocusEnter()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);
        Debug.Log("Enter");
    }

    public void OnFocusExit()
    {
        InputManager.Instance.PopFallbackInputHandler();
    }


    void smoothRotation(Vector3 eulerAngle)
    {
        if (m_Rotating)
            return;
        StartCoroutine(smoothRotationCoroutine(eulerAngle));
    }

    IEnumerator smoothRotationCoroutine(Vector3 eulerAngle)
    {
        m_Rotating = true;
        float currentDuration = 0f;
        float maxDuration = 0.25f;
        Quaternion initRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(transform.eulerAngles.x + eulerAngle.x, transform.eulerAngles.y + eulerAngle.y, transform.eulerAngles.z + eulerAngle.z);
        while (currentDuration < maxDuration)
        {
            transform.rotation = Quaternion.Slerp(initRot, targetRot, currentDuration / maxDuration);
            currentDuration += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRot;
        m_Rotating = false;
    }


}
