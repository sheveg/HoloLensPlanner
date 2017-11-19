using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner.Utilities
{
    /// <summary>
    /// Extension class which includes some helper methods for a layermask to make it more user friendly.
    /// </summary>
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Retuns a list of all layers which are included in this layermask.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static List<string> GetLayers(this LayerMask layerMask)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                if ((layerMask & (1 << i)) != 0)
                    result.Add(LayerMask.LayerToName(i));
            }
            return result;
        }

        /// <summary>
        /// Is the given layer included in the layermask?
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool Has(this LayerMask layerMask, string layerName)
        {
            ValidLayerName(layerName);
            return (layerMask.value & (1 << LayerMask.NameToLayer(layerName))) != 0;
        }

        /// <summary>
        /// Returns how many layers the layermask has.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static int Count(this LayerMask layerMask)
        {
            int result = 0;
            int value = layerMask.value;
            while (value != 0)
            {
                if (value % 2 == 1)
                    result++;
                value = value / 2;
            }
            return result;
        }

        /// <summary>
        /// Adds a layer to the layermask.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static LayerMask Add(this LayerMask layerMask, string layerName)
        {
            ValidLayerName(layerName);
            return layerMask | 1 << LayerMask.NameToLayer(layerName);
        }

        /// <summary>
        /// Removes a layer from the layermask.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static LayerMask Remove(this LayerMask layerMask, string layerName)
        {
            ValidLayerName(layerName);
            return layerMask & ~(1 << LayerMask.NameToLayer(layerName));
        }

        /// <summary>
        /// Does the given layer exist in the unity settings?
        /// </summary>
        /// <param name="layerName"></param>
        private static void ValidLayerName(string layerName)
        {
            if (string.IsNullOrEmpty(layerName))
                throw new Exception("Layer name should not be empty");

            if (LayerMask.NameToLayer(layerName) == -1)
                throw new Exception("Invalid layer name: " + layerName);
        }
    }
}