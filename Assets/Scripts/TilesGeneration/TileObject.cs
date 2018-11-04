using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    /// <summary>
    /// TileObject is used to be spawned on a plane according to a <see cref="TileData"/>. 
    /// </summary>
    public class TileObject : MonoBehaviour
    {
        [SerializeField]
        private Material FaceMaterial;

        [SerializeField]
        public Transform TileJoint;

        public bool Visible = false;

        public TileData m_Tile;

        public void LinkTile(TileData tileData)
        {
            m_Tile = tileData;
            // scale tile and joint size
            transform.localScale = new Vector3(tileData.Width, tileData.TileThickness, tileData.Height);
            // we half the joint size because when the tiles are placed next to each other they sum up again to the original size
            var jointSize = tileData.JointSize * 0.5f;
            TileJoint.localScale = new Vector3((tileData.Width + jointSize) / tileData.Width, 0.95f, (tileData.Height + jointSize) / tileData.Height);
            FaceMaterial.mainTexture = GlobalSettings.Instance.TextureLibrary.Textures[tileData.TextureIndex];
            // we need to tile the texture when the tile is not quadratic so the texture does not get stretched
            if (tileData.Width > tileData.Height)
            {
                FaceMaterial.SetTextureScale("_MainTex", new Vector2(1f, tileData.Height / tileData.Width));
            }
            else if (tileData.Width < tileData.Height)
            {
                FaceMaterial.SetTextureScale("_MainTex", new Vector2(tileData.Width / tileData.Height, 1f));
            }
           
        }

        private void OnTriggerEnter(Collider other)
        {
            Visible = true;
        }
    }
}
