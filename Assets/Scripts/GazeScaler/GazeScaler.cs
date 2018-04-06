using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloLensPlanner
{
    /// <summary>
    /// Responds to the HoloLens Gaze in scale change.
    /// </summary>
    public class GazeScaler : MonoBehaviour, IFocusable
    {
        /// <summary>
        /// Scale value for the scale event.
        /// </summary>
        public float FocusScale = 1.2f;

        /// <summary>
        /// Helper variable to determine whether the object is focused.
        /// </summary>
        private bool m_IsFocused = false;

        /// <summary>
        /// If this is true then the focus can be forced to enter/exit independent of the gaze.
        /// </summary>
        private bool m_FocusLocked = false;

        /// <summary>
        /// Default scale of this object at the start of the scene.
        /// </summary>
        private Vector3 m_DefaultScale;

        /// <summary>
        /// Reference of th current coroutine if any. Used to stop a running coroutine when a focus change occurs before a
        /// coroutine is done.
        /// </summary>
        private IEnumerator m_ScaleCoroutine;

        private void Awake()
        {
            m_DefaultScale = transform.localScale;
        }

        public void OnFocusEnter()
        {
            if (m_FocusLocked || m_IsFocused)
                return;
            m_IsFocused = true;
            OnScale();
        }

        public void OnFocusExit()
        {
            if (m_FocusLocked || !m_IsFocused)
                return;
            m_IsFocused = false;
            OnScale();
        }

        /// <summary>
        /// Forces the object to enter the OnFocus method even if it is not focused by the gaze. Call <see cref="ForceOnFocusExit"/> to disable this behaviour. 
        /// </summary>
        public void ForceOnFocusEnter()
        {
            if (m_FocusLocked)
                return;
            OnFocusEnter();
            m_FocusLocked = true;
        }

        /// <summary>
        /// Forces the object to exit the OnFocus method even if it is focused by the gaze. Call <see cref="ForceOnFocusEnter"/> to enable this behaviour again. 
        /// </summary>
        public void ForceOnFocusExit()
        {
            if (!m_FocusLocked)
                return;
            m_FocusLocked = false;
            OnFocusExit();
        }

        /// <summary>
        /// Scale event at focus change.
        /// </summary>
        private void OnScale()
        {
            if (m_ScaleCoroutine != null)
                StopCoroutine(m_ScaleCoroutine);

            m_ScaleCoroutine = scaleCoroutine();
            StartCoroutine(m_ScaleCoroutine);
        }

        /// <summary>
        /// Coroutine to change the scale smoothly over time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator scaleCoroutine()
        {
            float duration = 0.125f;
            float currentDuration = 0f;
            Vector3 initScale = transform.localScale;
            Vector3 targetScale = m_DefaultScale;
            if (m_IsFocused)
                targetScale *= FocusScale;
            while (currentDuration < duration)
            {
                transform.localScale = Vector3.Lerp(initScale, targetScale, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            transform.localScale = targetScale;
        }
    }
}


