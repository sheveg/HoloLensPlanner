using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using System;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    public class AssistantManager : SingleInstance<AssistantManager>
    {
        [SerializeField]
        private GameObject Joint;

        public List<GameObject> verticalJoints = new List<GameObject>();
        public List<GameObject> horizontalJoints = new List<GameObject>();

        public GameObject TilePlane;
        public GameObject JointPlane;
        public RoomPlane Floor;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void activate()
        {
            if (TiledPlane.Instance != null)
            {
                // deactivate not needed edges
                verticalJoints[verticalJoints.Count - 1].gameObject.SetActive(false);
                verticalJoints[0].gameObject.SetActive(false);
                horizontalJoints[0].gameObject.SetActive(false);

                // deactivate other planes
                TilePlane.SetActive(false);
                Floor.gameObject.SetActive(false);
                JointPlane.SetActive(true);
            }
        }

        public void deactivate()
        {
            if (TiledPlane.Instance != null)
            {
                // deactivate not needed edges
                verticalJoints[verticalJoints.Count - 1].gameObject.SetActive(true);
                verticalJoints[0].gameObject.SetActive(true);
                horizontalJoints[0].gameObject.SetActive(true);

                // deactivate other planes
                TilePlane.SetActive(true);
                Floor.gameObject.SetActive(true);
                JointPlane.SetActive(false);
            }
        }

        public void addVerticalJoint(GameObject joint)
        {
            verticalJoints.Add(joint);
        }

        public void addHorizontalJoint(GameObject joint)
        {
            verticalJoints.Add(joint);
        }
    }
}
