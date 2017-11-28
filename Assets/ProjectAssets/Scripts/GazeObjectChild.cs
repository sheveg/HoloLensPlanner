using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HoloLensPlanner
{
    public class GazeObjectChild : GazeResponder
    {

        /// <summary>
        /// Reference of the current coroutine if any. Used to stop a running coroutine when a focus change occurs before a
        /// coroutine is done.
        /// </summary>
        protected IEnumerator m_EventCoroutine;

        private Vector3 m_defaultScale;

        protected override void OnColor()
        {
            base.OnColor();
        }

        protected override void OnOutline()
        {
            base.OnOutline();
        }

        protected override void OnScale()
        {
            m_EventCoroutine = scaleCoroutine();
            StartCoroutine(m_EventCoroutine);
        }

        public override void Awake()
        {
            m_defaultScale = GetComponentInChildren<Transform>().localScale;
        }

        /// <summary>
        /// Coroutine to change the scale of a gaze responder.
        /// </summary>
        /// <returns></returns>
        private IEnumerator scaleCoroutine()
        {
            Transform childTrans = GetComponentInChildren<Transform>();

            float duration = 0.125f;
            float currentDuration = 0f;
            Vector3 initScale = childTrans.localScale;
            Vector3 targetScale = m_defaultScale;
            if (m_IsFocused)
                targetScale *= EventScale;
            while (currentDuration < duration)
            {
                childTrans.localScale = Vector3.Lerp(initScale, targetScale, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            childTrans.localScale = targetScale;
        }
    }
}
