using UnityEngine;
using UnityEngine.Events;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;


public class GazeButton : MonoBehaviour, IFocusable {

    public enum FocusEvent
    {
        None,
        Scale,
        Color
    }

    public FocusEvent OnFocus = FocusEvent.None;

    private float m_ScaleValue = 1.1f;

    private bool m_IsFocused = false;

    private IEnumerator m_EventCoroutine;

    public void OnFocusEnter()
    {
        m_IsFocused = true;
        handleFocusChange();
    }

    public void OnFocusExit()
    {
        m_IsFocused = false;
        handleFocusChange();
    }

    private void handleFocusChange()
    {
        if (m_EventCoroutine != null)
            StopCoroutine(m_EventCoroutine);
        switch (OnFocus)
        {
            case FocusEvent.Scale:
                OnScale();
                break;
            case FocusEvent.Color:
                break;
            case FocusEvent.None:
            default:
                break;
        }
    }

    private void OnScale()
    {
        m_EventCoroutine = scaleCoroutine();
        StartCoroutine(m_EventCoroutine);
    }

    private void OnColor()
    {

    }

    private IEnumerator scaleCoroutine()
    {
        float duration = 0.125f;
        float currentDuration = 0f;
        Vector3 initScale = transform.localScale;
        Vector3 targetScale = Vector3.one;
        if (m_IsFocused)
            targetScale *= m_ScaleValue;
        while (currentDuration < duration)
        {
            transform.localScale = Vector3.Lerp(initScale, targetScale, currentDuration / duration);
            currentDuration += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
