using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;


namespace HoloLensPlanner
{
    public class FloorTransformOperator : SingleInstance<FloorTransformOperator>
    {
        [SerializeField]
        private TransformOperator TranformOperatorPrefab;

        private bool m_Initialized;

        private Dictionary<GameObject, TransformOperator> m_TransformOperators = new Dictionary<GameObject, TransformOperator>();

        private TransformOperator m_LastTransformOperator;

        public void Create()
        {
            if (RoomManager.Instance.Floor == null)
                return;

            m_TransformOperators.Clear();
            m_LastTransformOperator = null;
            var transformOperator = Instantiate(TranformOperatorPrefab);
            transformOperator.transform.localScale *= 0.5f;
            transformOperator.AttachToTarget(RoomManager.Instance.Floor.transform, false, true, false, TransformPosition.Above);
            transformOperator.gameObject.SetActive(false);
            m_TransformOperators.Add(RoomManager.Instance.Floor.gameObject, transformOperator);

            foreach (var point in RoomManager.Instance.Floor.MeshPolygon.Points)
            {
                var transformOperatorPoint = Instantiate(TranformOperatorPrefab);
                transformOperatorPoint.AttachToTarget(point.transform, true, false, true, TransformPosition.Centered);
                transformOperatorPoint.gameObject.SetActive(false);
                m_TransformOperators.Add(point.gameObject, transformOperatorPoint);
            }
            m_Initialized = true;
        }

        public void ChangeState(GameObject g)
        {
            if (!m_Initialized || g == null || !m_TransformOperators.ContainsKey(g))
                return;

            if (m_LastTransformOperator)
                m_LastTransformOperator.gameObject.SetActive(false);

            if (m_LastTransformOperator == m_TransformOperators[g])
            {
                m_LastTransformOperator = null;
                return;
            }

            m_TransformOperators[g].gameObject.SetActive(!m_TransformOperators[g].gameObject.activeSelf);
            m_LastTransformOperator = m_TransformOperators[g];
        }

        public void HideAll()
        {
            if (!m_Initialized)
                return;

            foreach (var transformOP in m_TransformOperators)
            {
                transformOP.Value.gameObject.SetActive(false);
                m_LastTransformOperator = null;
            }
        }
    }
}
