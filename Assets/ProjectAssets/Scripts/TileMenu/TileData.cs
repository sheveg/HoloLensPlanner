using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Globalization;

namespace HoloLensPlanner.TEST
{
    /// <summary>
    /// TileData is only used to save/load a tile and store data. It cannot be instantiated as a component to a gameobject as it does not derive from MonoBehaviour. Use <see cref="TileObject"/> for that. 
    /// </summary>
    [Serializable]
    public class TileData
    {
        /// <summary>
        /// Height in m.
        /// </summary>
        public float Height;

        /// <summary>
        /// Width in m.
        /// </summary>
        public float Width;

        /// <summary>
        /// Area in m²
        /// </summary>
        public float Area { get { return Height * Width; } }

        /// <summary>
        /// Tile thickness in specific format.
        /// </summary>
        public TileThickness TileThickness;


        /// <summary>
        /// Joint thickness in specific format.
        /// </summary>
        public JointThickness JointThickness;

        /// <summary>
        /// TextureIndex in the TextureLibrary.
        /// </summary>
        public int TextureIndex;

        /// <summary>
        /// Respective texture when loaded.
        /// </summary>
        private Texture2D m_Texture;

        /// <summary>
        /// Name of the tile. Change to unique identifier?? or make unique identifier responsible for loading purposes.
        /// </summary>
        public string Name;

        /// <summary>
        /// CreationDate in standard Date.Now format as string.
        /// </summary>
        public string CreationDate;

        /// <summary>
        /// Saves the tile data in JSON format.
        /// </summary>
        public void SaveToJson()
        {
            CreationDate = DateTime.Now.ToString();
            string json = JsonUtility.ToJson(this, true);
            string filePath = Path.Combine(GlobalSettings.Instance.PathLibrary.SavedTilesPath, Name + ".json");
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads the tile data from JSON format.
        /// </summary>
        /// <param name="name"></param>
        public void LoadFromJson(string name)
        {
            string filePath = Path.Combine(GlobalSettings.Instance.PathLibrary.SavedTilesPath, name + ".json");
            if (File.Exists(filePath))
            {
                string jsonTile = File.ReadAllText(filePath);
                JsonUtility.FromJsonOverwrite(jsonTile, this);
                m_Texture = GlobalSettings.Instance.TextureLibrary.Textures[TextureIndex];
            }
        }
    }
}
