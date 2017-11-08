using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    /// <summary>
    /// Abstract template for any kind of object which responds to the HoloLens gaze.
    /// </summary>
    public abstract class GazeResponder : MonoBehaviour, IFocusable
    {
        /// <summary>
        /// Indicates which event is triggered on focus change.
        /// </summary
        public FocusEvent OnFocus = FocusEvent.None;

        /// <summary>
        /// Scale value for the scale event.
        /// </summary>
        public float EventScale { get { return m_EventScale; } set { m_EventScale = value; } }

        /// <summary>
        /// Color for the color event.
        /// </summary>
        public Color EventColor { get { return m_EventColor; } set { m_EventColor = value; } }

        /// <summary>
        /// Color for the outline event.
        /// </summary>
        public Color EventOutlineColor { get { return m_EventOutlineColor; } set { m_EventOutlineColor = value; } }

        /// <summary>
        /// Helper variable to determine whether the object is focused.
        /// </summary>
        protected bool m_IsFocused = false;

        /// <summary>
        /// Reference of th current coroutine if any. Used to stop a running coroutine when a focus change occurs before a
        /// coroutine is done.
        /// </summary>
        protected IEnumerator m_EventCoroutine;

        /// <summary>
        /// Private variable for scale to avoid property serialization issues of unity.
        /// </summary>
        [SerializeField, HideInInspector]
        private float m_EventScale = 1.1f;

        /// <summary>
        /// Private variable for color to avoid property serialization issues of unity.
        /// </summary>
        [SerializeField, HideInInspector]
        private Color m_EventColor = Color.white;

        /// <summary>
        /// Private variable for outline color to avoid property serialization issues of unity.
        /// </summary>
        [SerializeField, HideInInspector]
        private Color m_EventOutlineColor = Color.white;

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

        /// <summary>
        /// Triggers the respective event function based on the <see cref="OnFocus"/> value.
        /// </summary>
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
                    OnColor();
                    break;
                case FocusEvent.Outline:
                    OnOutline();
                    break;
                case FocusEvent.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Scale event at focus change.
        /// </summary>
        protected virtual void OnScale()
        {
            m_EventCoroutine = scaleCoroutine();
            StartCoroutine(m_EventCoroutine);
        }

        /// <summary>
        /// Color event at focus change.
        /// </summary>
        protected virtual void OnColor() { }

        /// <summary>
        /// Outline event at focus change.
        /// </summary>
        protected virtual void OnOutline() { }

        /// <summary>
        /// Coroutine to change the scale of a gaze responder.
        /// </summary>
        /// <returns></returns>
        private IEnumerator scaleCoroutine()
        {
            float duration = 0.125f;
            float currentDuration = 0f;
            Vector3 initScale = transform.localScale;
            Vector3 targetScale = Vector3.one;
            if (m_IsFocused)
                targetScale *= EventScale;
            while (currentDuration < duration)
            {
                transform.localScale = Vector3.Lerp(initScale, targetScale, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            transform.localScale = targetScale;
        }
    }

    /// <summary>
    /// Possible events for a focus change.
    /// </summary>
    public enum FocusEvent
    {
        None,
        /// <summary>
        /// Scale up/down on focus change.
        /// </summary>
        Scale,
        /// <summary>
        /// Change color on focus change.
        /// </summary>
        Color,
        /// <summary>
        /// Show/hide and outline on focus change.
        /// </summary>
        Outline
    }
}