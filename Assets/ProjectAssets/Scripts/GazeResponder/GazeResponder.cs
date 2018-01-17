using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner.GazeResponse
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
        private float m_EventScale = 1.2f;

        private Vector3 m_DefaultScale;

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

        protected virtual void Awake()
        {
            m_DefaultScale = transform.localScale;
        }

        public void OnFocusEnter()
        {
            if (!InputManager.Instance.IsInputEnabled)
                return;

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
            m_EventCoroutine = scaleCoroutine(transform, m_DefaultScale);
            StartCoroutine(m_EventCoroutine);
        }

        /// <summary>
        /// Scale event at focus change with target parameter, e.g. to handle scale events of children.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="defaultScale"></param>
        protected void OnScale(Transform target, Vector3 defaultScale)
        {
            m_EventCoroutine = scaleCoroutine(target, defaultScale);
            StartCoroutine(m_EventCoroutine);
        }

        /// <summary>
        /// Color event at focus change.
        /// </summary>
        protected abstract void OnColor();

        /// <summary>
        /// Outline event at focus change.
        /// </summary>
        protected abstract void OnOutline();

        /// <summary>
        /// Coroutine to change the scale of a gaze responder.
        /// </summary>
        /// <returns></returns>
        private IEnumerator scaleCoroutine(Transform target, Vector3 defaultScale)
        {
            float duration = 0.125f;
            float currentDuration = 0f;
            Vector3 initScale = target.localScale;
            Vector3 targetScale = defaultScale;
            if (m_IsFocused)
                targetScale *= EventScale;
            while (currentDuration < duration)
            {
                target.localScale = Vector3.Lerp(initScale, targetScale, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            target.localScale = targetScale;
        }
    }
}