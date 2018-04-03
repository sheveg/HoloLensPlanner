using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    [CreateAssetMenu(menuName = "Libraries/PathLibrary")]
    public class PathLibrary : ScriptableObject
    {
        /// <summary>
        /// Directory of saved tiles inside the StreamingAssets folder
        /// </summary>
        public string SavedTilesPath { get { return Application.streamingAssetsPath + "/" + m_SavedTilesPath + "/"; } }

        [Header("Directory of saved tiles inside the StreamingAssets folder")]
        [SerializeField]
        private string m_SavedTilesPath;
        
        /// <summary>
        /// Type for saved tiles like JSON, XML etc.
        /// </summary>
        [Header("Type for saved tiles like JSON, XML etc.")]
        public string SavedTilesType;

        /// <summary>
        /// Path for tile textures inside the StreamingAssets folder
        /// </summary>
        public string TileTexturesPath { get { return Application.streamingAssetsPath + "/" + m_TileTexturesPath + "/"; } }

        [Header("Path for tile textures inside the StreamingAssets folder")]
        [SerializeField]
        private string m_TileTexturesPath;
    }
}


