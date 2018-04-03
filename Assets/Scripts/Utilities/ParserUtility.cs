using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner.Utilities
{
    /// <summary>
    /// Any parser methods which can be used generally should be implemented here.
    /// </summary>
    public static class ParserUtility
    {

        /// <summary>
        /// Converts a string in form of xx.xx cm/mm etc into xx.xx, meaning the unit should be seperated by whitespace from the actual float.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float StringToFloat(string s)
        {
            string[] stringParts = s.Split(' ');
            return float.Parse(stringParts[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}
