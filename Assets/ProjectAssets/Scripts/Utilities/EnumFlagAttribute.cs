using UnityEngine;

/// <summary>
/// Source : http://wiki.unity3d.com/index.php/EnumFlagPropertyDrawer
/// </summary>
namespace HoloLensPlanner.Utilities
{
    public class EnumFlagAttribute : PropertyAttribute
    {
        public string enumName;

        public EnumFlagAttribute() { }

        public EnumFlagAttribute(string name)
        {
            enumName = name;
        }
    }
}