using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGenerator : MonoBehaviour {

    public Transform Tile;

    public Transform SpawnPoint;
    public Transform DirectionPoint;

    public List<Transform> AllPoints = new List<Transform>();

    private void Start()
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
        Vector3 startingPosition = tilePlaneVertex0.transform.position + tilePlaneVertex0.transform.right * tileWidth / 2f +  tilePlaneVertex0.transform.forward * tileHeight / 2f;

        Debug.Log(rows + " : " + columns);


        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 offset = i * tileHeight * tilePlaneVertex0.transform.forward + j * tileWidth * tilePlaneVertex0.transform.right;
                Transform tileCopy = Instantiate(Tile, startingPosition + offset, tilePlaneVertex0.transform.rotation);
                tileCopy.transform.parent = tilePlane.transform;
            }
        }



        //Vector3 topLeftTilePosition = new Vector3(Tile.position.x - tileMesh.bounds.extents.x, Tile.position.y, Tile.position.z - tileMesh.bounds.extents.z);
        //var topLeftTile = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        //topLeftTile.transform.position = topLeftTilePosition;
        //topLeftTile.transform.localScale = Vector3.one * 0.1f;
        //topLeftTile.name = "topLeftTile";

        //Vector3 topRightTilePosition = new Vector3(Tile.position.x + tileMesh.bounds.extents.x, Tile.position.y, Tile.position.z - tileMesh.bounds.extents.z);
        //var topRightTile = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        //topRightTile.transform.position = topRightTilePosition;
        //topRightTile.transform.localScale = Vector3.one * 0.1f;
        //topRightTile.name = "topRightTile";

        //Vector3 bottomLeftTilePosition = new Vector3(Tile.position.x - tileMesh.bounds.extents.x, Tile.position.y, Tile.position.z + tileMesh.bounds.extents.z);
        //var bottomLeftTile = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        //bottomLeftTile.transform.position = bottomLeftTilePosition;
        //bottomLeftTile.transform.localScale = Vector3.one * 0.1f;
        //bottomLeftTile.name = "bottomLeftTile";

        //Vector3 bottomRightTilePosition = new Vector3(Tile.position.x + tileMesh.bounds.extents.x, Tile.position.y, Tile.position.z + tileMesh.bounds.extents.z);
        //var bottomRightTile = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        //bottomRightTile.transform.position = bottomRightTilePosition;
        //bottomRightTile.transform.localScale = Vector3.one * 0.1f;
        //bottomRightTile.name = "bottomRightTile";
    }
}
