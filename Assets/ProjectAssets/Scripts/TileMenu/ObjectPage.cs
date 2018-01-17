using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    public class ObjectPage : MonoBehaviour
    {
        public const int MaxObjectsCount = 6;

        public List<MenuObject> Objects = new List<MenuObject>();

        private void Awake()
        {
            // trim the list so we do not have more objects than possible
            if (Objects.Count > MaxObjectsCount)
            {
                Objects.RemoveRange(MaxObjectsCount, Objects.Count);
            }
        }

    }
}


