using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Globalization;

namespace HoloLensPlanner
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
        /// Tile thickness in m.
        /// </summary>
        public float TileThickness;
        
        /// <summary>
        /// Joint thickness in m.
        /// </summary>
        public float JointSize;

        /// <summary>
        /// Area in m²
        /// </summary>
        public float Area { get { return Height * Width; } }

        /// <summary>
        /// Unique global ID.
        /// </summary>
        public Guid Guid
        {
            get
            {
                if (string.IsNullOrEmpty(m_GuidString))
                    return Guid.Empty;
                else
                    return new Guid(m_GuidString);
            }
            set
            {
                m_GuidString = value.ToString("N");
            }
        }
        
        /// <summary>
        /// TextureIndex in the TextureLibrary.
        /// </summary>
        public int TextureIndex;
        
        /// <summary>
        /// Name of the tile. Change to unique identifier?? or make unique identifier responsible for loading purposes.
        /// </summary>
        public string Name;

        /// <summary>
        /// CreationDate in standard Date.Now format as string.
        /// </summary>
        public string CreationDate;

        /// <summary>
        /// Respective texture when loaded.
        /// </summary>
        private Texture2D m_Texture;

        /// <summary>
        /// Identifier as Guid class.
        /// </summary>
        private string m_GuidString;

        /// <summary>
        /// Constants for new tile creation.
        /// </summary>
        public const float DefaultHeightInCM = 10;
        public const float DefaultWidthInCM = 10;
        public const float DefaultTileThicknessInMM = 8f;
        public const float DefaultJointThicknessInMM = 2f;
        public const int DefaultTextureIndex = 0;
        public const string DefaultName = "New Tile";

        public TileData()
        {

        }

        /// <summary>
        /// Creates new tile data object
        /// </summary>
        /// <param name="Height"></param>
        /// <param name="Width"></param>
        /// <param name="TileThickness"></param>
        /// <param name="JointThickness"></param>
        /// <param name="TextureIndex"></param>
        /// <param name="Name"></param>
        public TileData(float Height, float Width, float TileThickness, float JointThickness, int TextureIndex, string Name, Guid guid)
        {
            this.Height = Height;
            this.Width = Width;
            this.TileThickness = TileThickness;
            this.JointSize = JointThickness;
            this.TextureIndex = TextureIndex;
            if (Name == null)
            {
                this.Name = "";
            }
            else
            {
                this.Name = Name;
            }
            this.m_GuidString = guid.ToString("N");
        }

        /// <summary>
        /// Saves the tile data in JSON format.
        /// </summary>
        public void SaveToJson()
        {
            CreationDate = DateTime.Now.ToString();
            string json = JsonUtility.ToJson(this, true);
            string filePath = Path.Combine(GlobalSettings.Instance.PathLibrary.SavedTilesPath, m_GuidString + ".json");
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads the tile data from JSON format.
        /// </summary>
        /// <param name="guid"></param>
        public void LoadFromJson(string guid)
        {
            string filePath = Path.Combine(GlobalSettings.Instance.PathLibrary.SavedTilesPath, guid + ".json");
            if (File.Exists(filePath))
            {
                string jsonTile = File.ReadAllText(filePath);
                JsonUtility.FromJsonOverwrite(jsonTile, this);
                m_Texture = GlobalSettings.Instance.TextureLibrary.Textures[TextureIndex];
                m_GuidString = guid;
            }
        }
    }
}
