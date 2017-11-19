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

    /// <summary>
    /// Possible events for a focus change.
    /// </summary>
    public enum FocusEvent
    {
        None,
        /// <summary>
        /// Scale up/down on focus change.
        /// </summary>
        Scale,
        /// <summary>
        /// Change color on focus change.
        /// </summary>
        Color,
        /// <summary>
        /// Show/hide and outline on focus change.
        /// </summary>
        Outline
    }
}


