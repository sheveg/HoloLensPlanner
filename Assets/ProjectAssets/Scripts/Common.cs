using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collector file for common structs and enums.
/// </summary>
namespace HoloLensPlanner
{
    /// <summary>
    /// Types of room planes.
    /// </summary>
    public enum PlaneType
    {
        Floor,
        Ceiling,
        Wall
    }

    /// <summary>
    /// States of room planes.
    /// </summary>
    public enum PlaneState
    {
        Idle,
        EditMode,
        ObjectMode
    }
}


