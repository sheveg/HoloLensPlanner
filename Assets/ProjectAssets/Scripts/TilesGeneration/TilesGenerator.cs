using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    public class TilesGenerator : MonoBehaviour
    {

        public Tile Tile;
        public RoomPlane Plane;
        public Transform SpawnPoint;
        public Transform DirectionPoint;

        private void Start()
        {
        }

        public void createTiles(Tile tile, RoomPlane roomPlane, Transform spawnPoint, Transform directionPoint)
        {
            // first create the copy so we do not mess with the original spawnpoint, we will destroy this object later on
            var spawnPointCopy = new GameObject("SpawnPointCopy");
            spawnPointCopy.transform.position = spawnPoint.position;
            spawnPointCopy.transform.rotation = spawnPoint.rotation;
            spawnPointCopy.transform.forward = directionPoint.position - spawnPoint.position;
            // now find the min and max values of the plane
            float maxZ, maxX, minZ, minX;
            maxZ = maxX = minZ = minX = 0f;
            for (int i = 0; i < roomPlane.MeshPolygon.Points.Count; i++)
            {
                // transform the point into the local space of the plane
                Vector3 localPosition = spawnPointCopy.transform.InverseTransformPoint(roomPlane.MeshPolygon.Points[i].transform.position);
                // adjust min and max values
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

            // create the border points of the plane given by the min and max values
            var minXminZ_Point = new GameObject();
            minXminZ_Point.transform.position = spawnPointCopy.transform.TransformPoint(new Vector3(minX, 0f, minZ));


            var minXmaxZ_Point = new GameObject();
            minXmaxZ_Point.transform.position = spawnPointCopy.transform.TransformPoint(new Vector3(minX, 0f, maxZ));

            var maxXminZ_Point = new GameObject();
            maxXminZ_Point.transform.position = spawnPointCopy.transform.TransformPoint(new Vector3(maxX, 0f, minZ));

            var maxXmaxZ_Point = new GameObject();
            maxXmaxZ_Point.transform.position = spawnPointCopy.transform.TransformPoint(new Vector3(maxX, 0f, maxZ));

            // create a parent object for all the tiles
            var tilePlane = new GameObject("TilePlane");
            tilePlane.transform.position = roomPlane.MeshPolygon.Center;

            var tileWidth = tile.Width;
            var tileHeight = tile.Height;

            // calculate how many rows and columns are needed for the tile creation, we go one row and column further because of the offset
            int columns = Mathf.CeilToInt((minXminZ_Point.transform.position - maxXminZ_Point.transform.position).magnitude / tileWidth) + 1;
            int rows = Mathf.CeilToInt((minXminZ_Point.transform.position - minXmaxZ_Point.transform.position).magnitude / tileHeight) + 1;

            // adjust forward and right vector of the starting vertex of the plane
            minXminZ_Point.transform.forward = (minXmaxZ_Point.transform.position - minXminZ_Point.transform.position).normalized;
            minXminZ_Point.transform.right = (maxXminZ_Point.transform.position - minXminZ_Point.transform.position).normalized;
            Vector3 startPosition = minXminZ_Point.transform.position + minXminZ_Point.transform.right * tileWidth / 2f + minXminZ_Point.transform.forward * tileHeight / 2f;
            // calculate the offset needed so that the tiles align perfectely at the spawn point
            Vector3 minToSpawn = minXminZ_Point.transform.InverseTransformPoint(spawnPoint.position);
            float xOffset = tileWidth - Mathf.Repeat(minToSpawn.x, tileWidth);
            float zOffset = tileHeight - Mathf.Repeat(minToSpawn.z, tileHeight);
            startPosition -= xOffset * minXminZ_Point.transform.right + zOffset * minXminZ_Point.transform.forward;
            // place the tiles
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    Vector3 offset = i * tileHeight * minXminZ_Point.transform.forward + j * tileWidth * minXminZ_Point.transform.right;
                    var currentTile = Instantiate(tile, startPosition + offset, minXminZ_Point.transform.rotation);
                    currentTile.transform.parent = tilePlane.transform;
                }

            // create the mask plane with has the room plane polygon as a hole
            var maskPlaneVertices = new List<Vector2>();
            // max and min points as boundary points
            Vector3 mask_minXminZ = startPosition - (minXminZ_Point.transform.right * tileWidth / 2f + minXminZ_Point.transform.forward * tileHeight / 2f);
            Vector3 mask_minXmaxZ = mask_minXminZ + minXminZ_Point.transform.forward * tileHeight * rows;
            Vector3 mask_maxXminZ = mask_minXminZ + minXminZ_Point.transform.right * tileWidth * columns;
            Vector3 mask_maxXmaxZ = mask_minXminZ + minXminZ_Point.transform.forward * tileHeight * rows + minXminZ_Point.transform.right * tileWidth * columns;
            Vector3 mask_center = (mask_minXminZ + mask_minXmaxZ + mask_maxXminZ + mask_maxXmaxZ) / 4f;
            //--------------------------
            var v1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            v1.transform.localScale = Vector3.one * 0.1f;
            v1.transform.position = mask_minXminZ;
            v1.name = "v1";

            var v2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            v2.transform.localScale = Vector3.one * 0.1f;
            v2.transform.position = mask_minXmaxZ;
            v2.name = "v2";

            var v3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            v3.transform.localScale = Vector3.one * 0.1f;
            v3.transform.position = mask_maxXmaxZ;
            v3.name = "v3";

            var v4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            v4.transform.localScale = Vector3.one * 0.1f;
            v4.transform.position = mask_maxXminZ;
            v4.name = "v4";
            //--------------------------


            maskPlaneVertices.Add(new Vector2(mask_minXminZ.x - mask_center.x, mask_minXminZ.z - mask_center.z));
            maskPlaneVertices.Add(new Vector2(mask_minXmaxZ.x - mask_center.x, mask_minXmaxZ.z - mask_center.z));
            maskPlaneVertices.Add(new Vector2(mask_maxXmaxZ.x - mask_center.x, mask_maxXmaxZ.z - mask_center.z));
            maskPlaneVertices.Add(new Vector2(mask_maxXminZ.x - mask_center.x, mask_maxXminZ.z - mask_center.z));
            var maskPlaneHoles = new List<List<Vector2>>();
            var maskPlaneHole = new List<Vector2>();
            // room plane as hole
            foreach (var point in roomPlane.MeshPolygon.Points)
            {
                maskPlaneHole.Add(new Vector2(point.transform.position.x - mask_center.x, point.transform.position.z - mask_center.z));
            }
            maskPlaneHoles.Add(maskPlaneHole);
            Mesh maskPlaneMesh = MeshUtility.CreatePolygonMesh(maskPlaneVertices, maskPlaneHoles);
            var maskPlane = new GameObject("MaskPlane");
            var maskPlaneMeshFilter = maskPlane.AddComponent<MeshFilter>();
            maskPlaneMeshFilter.mesh = maskPlaneMesh;
            var maskPlaneMeshRenderer = maskPlane.AddComponent<MeshRenderer>();
            maskPlane.transform.position = (mask_minXminZ + mask_minXmaxZ + mask_maxXminZ + mask_maxXmaxZ) / 4f;
        }

        public bool InsidePolygon(Polygon polygon, Vector3 point)
        {
            int j = polygon.Points.Count - 1;
            bool inside = false;
            for (int i = 0; i < polygon.Points.Count; j = i++)
            {
                if (((polygon.Points[i].transform.position.z <= point.z && point.z < polygon.Points[j].transform.position.z) || (polygon.Points[j].transform.position.z <= point.z && point.z < polygon.Points[i].transform.position.z))
                    && (point.x < (polygon.Points[j].transform.position.x - polygon.Points[i].transform.position.x) * (point.z - polygon.Points[i].transform.position.z) / (polygon.Points[j].transform.position.z - polygon.Points[i].transform.position.z) + polygon.Points[i].transform.position.x))
                    inside = !inside;
            }
            return inside;
        }
    }
}
