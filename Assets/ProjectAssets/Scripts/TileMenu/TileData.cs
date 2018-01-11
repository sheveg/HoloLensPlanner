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
        /// Unique global identifier
        /// </summary>
        public string ID;

        /// <summary>
        /// Identifier as Guid class.
        /// </summary>
        private Guid m_ID;

        /// <summary>
        /// Constants for new tile creation.
        /// </summary>
        public const float DefaultHeightInCM = 10;
        public const float DefaultWidthInCM = 10;
        public const TileThickness DefaultTileThickness = TileThickness.Five;
        public const JointThickness DefaultJointThickness = JointThickness.Four;
        public const int DefaultTextureIndex = 0;

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
        public TileData(float Height, float Width, float TileThickness, float JointThickness, int TextureIndex, string Name)
        {
            this.Height = Height;
            this.Width = Width;
            this.TileThickness = TileDimensionsLibrary.SetTileThickness(TileThickness);
            this.JointThickness = TileDimensionsLibrary.SetJointThickness(JointThickness);
            this.TextureIndex = TextureIndex;
            if (Name == null)
            {
                this.Name = "";
            }
            else
            {
                this.Name = Name;
            }
        }

        /// <summary>
        /// Saves the tile data in JSON format.
        /// </summary>
        public void SaveToJson()
        {
            CreationDate = DateTime.Now.ToString();
            if (m_ID == null)
                m_ID = Guid.NewGuid();
            ID = m_ID.ToString("N");
            string json = JsonUtility.ToJson(this, true);
            string filePath = Path.Combine(GlobalSettings.Instance.PathLibrary.SavedTilesPath, m_ID + ".json");
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
                if (string.IsNullOrEmpty(ID))
                    m_ID = Guid.NewGuid();
                else
                    m_ID = new Guid(ID);
            }
        }
    }
}
