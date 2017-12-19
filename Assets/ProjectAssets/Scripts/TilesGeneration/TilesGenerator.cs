using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    public class TilesGenerator : MonoBehaviour
    {

        public Transform Tile;

        public Transform SpawnPoint;
        public Transform DirectionPoint;

        public float TileOffset = 0.01f;

        public List<Transform> AllPoints = new List<Transform>();

        private IEnumerator Start()
        {

            SpawnPoint.forward = DirectionPoint.position - SpawnPoint.position;

            float maxZ = 0f;
            float maxX = 0f;
            float minX = 0f;
            float minZ = 0f;

            for (int i = 0; i < AllPoints.Count; i++)
            {
                Vector3 localPosition = SpawnPoint.InverseTransformPoint(AllPoints[i].position);

                if (localPosition.x > maxX)
                {
                    maxX = localPosition.x;
                }
                else if (localPosition.x < minX)
                {
                    minX = localPosition.x;
                }


                if (localPosition.z > maxZ)
                {
                    maxZ = localPosition.z;
                }
                else if (localPosition.z < minZ)
                {
                    minZ = localPosition.z;
                }
            }

            var tilePlaneVertex0 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tilePlaneVertex0.transform.localScale = Vector3.one * 0.3f;
            Vector3 vertexLocalPosition = new Vector3(minX, Tile.position.y, minZ);
            tilePlaneVertex0.transform.position = SpawnPoint.TransformPoint(vertexLocalPosition);
            tilePlaneVertex0.name = "Vertex0";

            var tilePlaneVertex1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tilePlaneVertex1.transform.localScale = Vector3.one * 0.3f;
            vertexLocalPosition = new Vector3(maxX, Tile.position.y, maxZ);
            tilePlaneVertex1.transform.position = SpawnPoint.TransformPoint(vertexLocalPosition);
            tilePlaneVertex1.name = "Vertex1";

            var tilePlaneVertex2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tilePlaneVertex2.transform.localScale = Vector3.one * 0.3f;
            vertexLocalPosition = new Vector3(minX, Tile.position.y, maxZ);
            tilePlaneVertex2.transform.position = SpawnPoint.TransformPoint(vertexLocalPosition);
            tilePlaneVertex2.name = "Vertex2";

            var tilePlaneVertex3 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            tilePlaneVertex3.transform.localScale = Vector3.one * 0.3f;
            vertexLocalPosition = new Vector3(maxX, Tile.position.y, minZ);
            tilePlaneVertex3.transform.position = SpawnPoint.TransformPoint(vertexLocalPosition);
            tilePlaneVertex3.name = "Vertex3";

            GameObject tilePlane = new GameObject("TilePlane");
            tilePlane.transform.position = (tilePlaneVertex0.transform.position + tilePlaneVertex1.transform.position + tilePlaneVertex2.transform.position + tilePlaneVertex3.transform.position) / 4f;
            tilePlaneVertex0.transform.parent = tilePlane.transform;
            tilePlaneVertex1.transform.parent = tilePlane.transform;
            tilePlaneVertex2.transform.parent = tilePlane.transform;
            tilePlaneVertex3.transform.parent = tilePlane.transform;


            //Tile.forward = DirectionPoint.position - SpawnPoint.position;
            //Tile.position = SpawnPoint.position + Tile.right * tileMesh.bounds.extents.x * Tile.localScale.x + Tile.forward * tileMesh.bounds.extents.z * Tile.localScale.z;
            Mesh tileMesh = Tile.GetComponent<MeshFilter>().mesh;
            float tileWidth = tileMesh.bounds.extents.x * Tile.localScale.x * 2f;
            float tileHeight = tileMesh.bounds.extents.z * Tile.localScale.z * 2f;
            int columns = Mathf.CeilToInt((tilePlaneVertex0.transform.position - tilePlaneVertex3.transform.position).magnitude / tileWidth);
            int rows = Mathf.CeilToInt((tilePlaneVertex0.transform.position - tilePlaneVertex2.transform.position).magnitude / tileHeight);
            tilePlaneVertex0.transform.forward = (tilePlaneVertex2.transform.position - tilePlaneVertex0.transform.position).normalized;
            tilePlaneVertex0.transform.right = (tilePlaneVertex3.transform.position - tilePlaneVertex0.transform.position).normalized;
            Vector3 startingPosition = tilePlaneVertex0.transform.position + tilePlaneVertex0.transform.right * tileWidth / 2f + tilePlaneVertex0.transform.forward * tileHeight / 2f;

            Vector3 startOffset = tilePlaneVertex0.transform.InverseTransformPoint(SpawnPoint.position) * tilePlaneVertex0.transform.localScale.x;
            float xOffset = tileWidth - Mathf.Repeat(startOffset.x, tileWidth);
            float zOffset = tileHeight - Mathf.Repeat(startOffset.z, tileHeight);
            Debug.Log(xOffset + " : " + zOffset);

            startingPosition -= xOffset * tilePlaneVertex0.transform.right + zOffset * tilePlaneVertex0.transform.forward;


            Debug.Log(rows + " : " + columns);



            for (int i = 0; i < rows + 1; i++)
            {
                for (int j = 0; j < columns + 1; j++)
                {
                    yield return new WaitForSeconds(1f);
                    Vector3 offset = i * tileHeight * tilePlaneVertex0.transform.forward * (1 + TileOffset / tileHeight) + j * tileWidth * tilePlaneVertex0.transform.right * (1 + TileOffset / tileWidth);
                    Transform tileCopy = Instantiate(Tile, startingPosition + offset, tilePlaneVertex0.transform.rotation);
                    tileCopy.transform.parent = tilePlane.transform;
                }
            }
        }

        private void createTiles(RoomPlane plane, Transform spawnPoint, Transform directionPoint)
        {
            // first create the copy so we do not mess with the original spawnpoint, we will destroy this object later on
            GameObject spawnPointCopy = new GameObject("SpawnPointCopy");
            spawnPointCopy.transform.position = spawnPoint.position;
            spawnPointCopy.transform.rotation = spawnPoint.rotation;
            spawnPointCopy.transform.localScale = spawnPoint.localScale;


        }
    }


}
