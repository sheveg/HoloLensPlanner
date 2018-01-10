using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensPlanner.Utilities;
using HoloLensPlanner.TEST;

namespace HoloLensPlanner
{
    /// <summary>
    /// TilesGenerator can generate tile floor for a given tile on a given roomplane.
    /// </summary>
    public class TilesGenerator : MonoBehaviour
    {

        public TileObject DefaultTile;
        public Material DepthMaskMaterial;
        public Material MaskResistentMaterial;

        public RoomPlane Plane;
        public Transform Spawn;
        public Transform Direction;

        /// <summary>
        /// Spawns an object which has the chosen tile spawned on the roomPlane with a mask around it.
        /// </summary>
        /// <param name="tileData"></param>
        /// <param name="roomPlane"></param>
        /// <param name="spawnPoint"></param>
        /// <param name="directionPoint"></param>
        public void SpawnTilesOnFloor(TileData tileData, RoomPlane roomPlane, Transform spawnPoint, Transform directionPoint)
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

            var jointSize = TileDimensionsLibrary.GetJointThickness(tileData.JointThickness) * 0.5f;
            var tileWidth = (tileData.Width + jointSize);
            var tileHeight = (tileData.Height + jointSize);

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
                    var currentTile = Instantiate(DefaultTile, startPosition + offset, minXminZ_Point.transform.rotation);
                    currentTile.transform.parent = tilePlane.transform;
                    currentTile.LinkTile(tileData);
                }

            // create the mask plane with has the room plane polygon as a hole
            var maskPlaneVertices = new List<Vector2>();
            // max and min points as boundary points
            Vector3 mask_minXminZ = startPosition - (minXminZ_Point.transform.right * tileWidth / 2f + minXminZ_Point.transform.forward * tileHeight / 2f);
            Vector3 mask_minXmaxZ = mask_minXminZ + minXminZ_Point.transform.forward * tileHeight * rows;
            Vector3 mask_maxXminZ = mask_minXminZ + minXminZ_Point.transform.right * tileWidth * columns;
            Vector3 mask_maxXmaxZ = mask_minXminZ + minXminZ_Point.transform.forward * tileHeight * rows + minXminZ_Point.transform.right * tileWidth * columns;
            Vector3 mask_center = (mask_minXminZ + mask_minXmaxZ + mask_maxXminZ + mask_maxXmaxZ) / 4f;
            // add the outer mask boundary as list for later uses
            List<Vector3> maskBoundaries = new List<Vector3> { mask_minXminZ, mask_minXmaxZ, mask_maxXmaxZ, mask_maxXminZ};
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
            maskPlane.transform.position += new Vector3(0f, TileDimensionsLibrary.GetTileThickness(tileData.TileThickness) * 0.5f + 0.0001f, 0f);
            maskPlane.transform.localScale = new Vector3(1f, -1f, 1f);
            maskPlane.GetComponent<Renderer>().material = DepthMaskMaterial;

            // create mask planes at the outer boundary of the maskplane so we mask the border tile joints as well
            for (int i = 0; i < maskBoundaries.Count; i++)
            {
                var point1 = maskBoundaries[i];
                var point2 = maskBoundaries[MathUtility.WrapArrayIndex(i + 1, maskBoundaries.Count)];
                var point3 = point1 - new Vector3(0f, TileDimensionsLibrary.GetTileThickness(tileData.TileThickness), 0f);
                // create the joint plane in the center of two points
                var jointMaskPlanePosition = (point1 + point2) * 0.5f;
                // lower it depending on the tile thickness
                //jointPlanePosition -= new Vector3(0f, TileDimensionsLibrary.GetTileThickness(tileData.TileThickness) * 0.5f, 0f);
                var jointMaskPlaneScale = new Vector3((point2 - point1).magnitude, 0.001f, (point3 - point1).magnitude);
                // create joint plane
                var jointMaskPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // adjust position and forward vector
                jointMaskPlane.transform.position = jointMaskPlanePosition;
                jointMaskPlane.transform.right = (point2 - point1).normalized;
                jointMaskPlane.transform.localEulerAngles = new Vector3(90f, jointMaskPlane.transform.localEulerAngles.y, jointMaskPlane.transform.localEulerAngles.z);
                jointMaskPlane.transform.localScale = jointMaskPlaneScale;
                jointMaskPlane.GetComponent<Renderer>().material = DepthMaskMaterial;
                jointMaskPlane.name = "jointMaskPlane";
                jointMaskPlane.transform.parent = maskPlane.transform;
            }

            // create planes at the corner of the roomplane so it looks like these are the tiles ends so we can see the joints from the side
            for (int i = 0; i < roomPlane.MeshPolygon.Points.Count; i++)
            {
                var point1 = roomPlane.MeshPolygon.Points[i].transform.position;
                var point2 = roomPlane.MeshPolygon.Points[MathUtility.WrapArrayIndex(i + 1, roomPlane.MeshPolygon.Points.Count)].transform.position;
                var point3 = point1 - new Vector3(0f, TileDimensionsLibrary.GetTileThickness(tileData.TileThickness), 0f);
                // create the joint plane in the center of two points
                var jointPlanePosition = (point1 + point2) * 0.5f;
                // lower it depending on the tile thickness
                //jointPlanePosition -= new Vector3(0f, TileDimensionsLibrary.GetTileThickness(tileData.TileThickness) * 0.5f, 0f);
                var jointPlaneScale = new Vector3((point2 - point1).magnitude, 0.001f, (point3 - point1).magnitude);
                // create joint plane
                var jointPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // adjust position and forward vector
                jointPlane.transform.position = jointPlanePosition;
                jointPlane.transform.right = (point2 - point1).normalized;
                jointPlane.transform.localEulerAngles = new Vector3(90f, jointPlane.transform.localEulerAngles.y, jointPlane.transform.localEulerAngles.z);
                jointPlane.transform.localScale = jointPlaneScale;
                jointPlane.GetComponent<Renderer>().material = MaskResistentMaterial;
                jointPlane.name = "jointPlane";
                jointPlane.transform.parent = maskPlane.transform;

            }
            // create a parent for overview purposes
            var maskedTilePlane = new GameObject("maskedTilePlane");
            maskedTilePlane.transform.position = tilePlane.transform.position;
            tilePlane.transform.parent = maskedTilePlane.transform;
            maskPlane.transform.parent = maskedTilePlane.transform;

            // cleanup
            Destroy(minXminZ_Point);
            Destroy(minXmaxZ_Point);
            Destroy(maxXminZ_Point);
            Destroy(maxXmaxZ_Point);
            Destroy(spawnPointCopy);
        }
    }
}
