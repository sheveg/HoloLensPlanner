using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner.TEST
{
    /// <summary>
    /// Tile Thickness in mm
    /// </summary>
    public enum TileThickness
    {
        Five,
        Eight,
        Twelve,
        Twenty,
        Thirty
    }

    /// <summary>
    /// Joint Thickness in mm
    /// </summary>
    public enum JointThickness
    {
        One,
        Two,
        Four,
        Ten
    }

    /// <summary>
    /// Library class for standard tile dimensions.
    /// </summary>
    public static class TileDimensionsLibrary
    {
        /// <summary>
        /// Returns the tile thickness in m.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetTileThickness(TileThickness t)
        {
            switch (t)
            {
                case TileThickness.Five:
                    return 0.005f;
                case TileThickness.Twelve:
                    return 0.012f;
                case TileThickness.Twenty:
                    return 0.020f;
                case TileThickness.Thirty:
                    return 0.030f;
                case TileThickness.Eight:
                default:
                    return 0.008f;
            }
        }

        /// <summary>
        /// Returns the joint thickness in m.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public static float GetJointThickness(JointThickness j)
        {
            switch (j)
            {
                case JointThickness.One:
                    return 0.001f;
                case JointThickness.Four:
                    return 0.004f;
                case JointThickness.Ten:
                    return 0.010f;
                case JointThickness.Two:
                default:
                    return 0.002f;
            }
        }
    }
}