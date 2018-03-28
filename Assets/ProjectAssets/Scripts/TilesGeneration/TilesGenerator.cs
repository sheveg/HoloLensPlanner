using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensPlanner.Utilities;
using HoloToolkit.Unity;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using HoloLensPlanner.GazeResponse;
using System;

namespace HoloLensPlanner
{
    public enum TilesGeneratorState
    {
        Idle,
        ChooseSpawn,
        ChooseDirection
    }

    /// <summary>
    /// TilesGenerator can generate tile floor for a given tile on a given roomplane.
    /// </summary>
    public class TilesGenerator : SingleInstance<TilesGenerator>, IInputClickHandler
    {
        /// <summary>
        /// Default tile prefab with 1x1x1m scale.
        /// </summary>
        [SerializeField]
        private TileObject DefaultTilePrefab;

        /// <summary>
        /// Material for the mask.
        /// </summary>
        [SerializeField]
        private Material DepthMaskMaterial;

        /// <summary>
        /// Material which is not hidden by the mask.
        /// </summary>
        [SerializeField]
        private Material MaskResistentMaterial;

        /// <summary>
        /// UI Instruction for choosing the spawn point.
        /// </summary>
        [SerializeField]
        private Image SpawnPointInstruction;

        /// <summary>
        /// UI instruction for choosing the direction point.
        /// </summary>
        [SerializeField]
        private Image DirectionPointInstruction;

        ///// <summary>
        ///// Button to cancel the floor tile creation from the spawn state.
        ///// </summary>
        //[SerializeField]
        //private Button CancelSpawn;

        ///// <summary>
        ///// Button to cancel the floor tile creation from the direction state.
        ///// </summary>
        //[SerializeField]
        //private Button CancelDirection;

        /// <summary>
        /// Button to switch to spawn state from direction state.
        /// </summary>
        [SerializeField]
        private Button BackFromDirection;

        /// <summary>
        /// Tile floor if there is one created. (Read only)
        /// </summary>
        public GameObject FinishedTileFloor { get; private set; }

        /// <summary>
        /// Current state.
        /// </summary>
        private TilesGeneratorState m_State = TilesGeneratorState.Idle;

        /// <summary>
        /// Current tile data.
        /// </summary>
        private TileData m_CurrentTile = null;

        /// <summary>
        /// Current spawn point.
        /// </summary>
        private Transform m_CurrentSpawnPoint = null;

        /// <summary>
        /// Current direction point.
        /// </summary>
        private Transform m_CurrentDirectionPoint = null;

        /// <summary>
        /// Raycast plane of the floor plane.
        /// </summary>
        private Plane m_RaycastPlane;

        /// <summary>
        /// As the TileGenerator has also UI components we need to check wether the user is focusing the UI to avoid advancing in the state in this case.
        /// </summary>
        private bool m_IsUIFocused;

        private void Start()
        {
            //CancelSpawn.onClick.AddListener(reset);
            //CancelDirection.onClick.AddListener(reset);
            BackFromDirection.onClick.AddListener(goBackToSpawnState);
        }

        private void Update()
        {
            if (m_State != TilesGeneratorState.Idle && GazeManager.Instance.HitObject != null && GazeManager.Instance.HitObject.transform.root == transform)
            {
                m_IsUIFocused = true;
            }
            else
            {
                m_IsUIFocused = false;
            }

            if (m_State == TilesGeneratorState.ChooseSpawn)
            {
                updateSpawnPoint();
            }
            else if (m_State == TilesGeneratorState.ChooseDirection)
            {
                updateDirectionPoint();
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (m_IsUIFocused)
            {
                eventData.Use();
                return;
            }

            switch (m_State)
            {
                case TilesGeneratorState.ChooseSpawn:
                    handleSpawnPointCase();
                    break;
                case TilesGeneratorState.ChooseDirection:
                    handleDirectionPointCase();
                    break;
            }
            eventData.Use();
        }

        public void CreateTileFloor(TileData tileData)
        {
            InputManager.Instance.PushModalInputHandler(gameObject);
            SpawnPointInstruction.gameObject.SetActive(true);
            m_State = TilesGeneratorState.ChooseSpawn;
            m_RaycastPlane = new UnityEngine.Plane();
            m_RaycastPlane.SetNormalAndPosition(Vector3.up, RoomManager.Instance.Floor.transform.position);
            m_CurrentTile = tileData;
        }

        /// <summary>
        /// Spawns an object which has the chosen tile spawned on the roomPlane with a mask around it.
        /// </summary>
        /// <param name="tileData"></param>
        /// <param name="roomPlane"></param>
        /// <param name="spawnPoint"></param>
        /// <param name="directionPoint"></param>
        private bool createTileFloorInternal(TileData tileData, RoomPlane roomPlane, Transform spawnPoint, Transform directionPoint)
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

            var jointSize = tileData.JointSize * 0.5f;
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
            // we need to move the whole tile plane at least a little so it cannot happen that a floor edge lies on the surrounding mask edge
            // because the mesh creation algorithm in MeshUtility cannot create a mesh with overlapping edges => Mathf.Max(...)
            float xOffset = Mathf.Max(tileWidth - Mathf.Repeat(minToSpawn.x, tileWidth), 0.001f);
            float zOffset = Mathf.Max(tileHeight - Mathf.Repeat(minToSpawn.z, tileHeight), 0.001f);
            startPosition -= xOffset * minXminZ_Point.transform.right + zOffset * minXminZ_Point.transform.forward;
            // place the tiles
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    Vector3 offset = i * tileHeight * minXminZ_Point.transform.forward + j * tileWidth * minXminZ_Point.transform.right;
                    var currentTile = Instantiate(DefaultTilePrefab, startPosition + offset, minXminZ_Point.transform.rotation);
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
            // this method can fail for intersecting constraints therefore we need a try/catch block
            Mesh maskPlaneMesh = null;
            try
            {
                maskPlaneMesh = MeshUtility.CreatePolygonMesh(maskPlaneVertices, maskPlaneHoles);
            }
            catch
            {
                // indicate that the tile creation process was not successful
                Destroy(minXminZ_Point);
                Destroy(minXmaxZ_Point);
                Destroy(maxXminZ_Point);
                Destroy(maxXmaxZ_Point);
                Destroy(spawnPointCopy);
                Destroy(tilePlane);
                return false;
            } 
            var maskPlane = new GameObject("MaskPlane");
            var maskPlaneMeshFilter = maskPlane.AddComponent<MeshFilter>();
            maskPlaneMeshFilter.mesh = maskPlaneMesh;
            var maskPlaneMeshRenderer = maskPlane.AddComponent<MeshRenderer>();
            maskPlane.transform.position = (mask_minXminZ + mask_minXmaxZ + mask_maxXminZ + mask_maxXmaxZ) / 4f;
            maskPlane.transform.position += new Vector3(0f, tileData.TileThickness * 0.5f + 0.0001f, 0f);
            maskPlane.transform.localScale = new Vector3(1f, -1f, 1f);
            maskPlane.GetComponent<Renderer>().material = DepthMaskMaterial;

            // create mask planes at the outer boundary of the maskplane so we mask the border tile joints as well
            for (int i = 0; i < maskBoundaries.Count; i++)
            {
                var point1 = maskBoundaries[i];
                var point2 = maskBoundaries[MathUtility.WrapArrayIndex(i + 1, maskBoundaries.Count)];
                var point3 = point1 - new Vector3(0f, tileData.TileThickness, 0f);
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
                var point3 = point1 - new Vector3(0f, tileData.TileThickness, 0f);
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
            if (FinishedTileFloor != null)
                Destroy(FinishedTileFloor);

            FinishedTileFloor = new GameObject("finishedTileFloor");
            FinishedTileFloor.transform.position = tilePlane.transform.position;
            tilePlane.transform.parent = FinishedTileFloor.transform;
            maskPlane.transform.parent = FinishedTileFloor.transform;

            // cleanup
            Destroy(minXminZ_Point);
            Destroy(minXmaxZ_Point);
            Destroy(maxXminZ_Point);
            Destroy(maxXmaxZ_Point);
            Destroy(spawnPointCopy);
            return true;
        }

        private void handleSpawnPointCase()
        {
            if (m_CurrentSpawnPoint != null)
            {
                m_State = TilesGeneratorState.ChooseDirection;
                SpawnPointInstruction.gameObject.SetActive(false);
                DirectionPointInstruction.gameObject.SetActive(true);
            }
            else
                TextManager.Instance.ShowWarning("No spawn point chosen for the tiles!");
        }

        private void handleDirectionPointCase()
        {
            if (m_CurrentDirectionPoint != null)
            {
                if (!createTileFloorInternal(m_CurrentTile, RoomManager.Instance.Floor, m_CurrentSpawnPoint, m_CurrentDirectionPoint))
                {
                    TextManager.Instance.ShowWarning("Oops, something went wrong, try again!", 3f);
                }
                reset();
                MenuHub.Instance.ShowMenu(MainMenuManager.Instance.gameObject);
            }
            else
                TextManager.Instance.ShowWarning("No direction point chosen for the tiles!");
        }

        private void goBackToSpawnState()
        {
            if (m_State != TilesGeneratorState.ChooseDirection)
                return;

            m_CurrentDirectionPoint.GetComponent<GazeResponder>().ForceOnFocusExit();
            m_CurrentDirectionPoint = null;
            m_CurrentSpawnPoint.GetComponent<GazeResponder>().ForceOnFocusExit();
            m_CurrentSpawnPoint = null;
            SpawnPointInstruction.gameObject.SetActive(true);
            DirectionPointInstruction.gameObject.SetActive(false);
            m_State = TilesGeneratorState.ChooseSpawn;
        }

        private void reset()
        {
            if(m_CurrentSpawnPoint)
                m_CurrentSpawnPoint.GetComponent<GazeResponder>().ForceOnFocusExit();
            if(m_CurrentDirectionPoint)
                m_CurrentDirectionPoint.GetComponent<GazeResponder>().ForceOnFocusExit();

            m_CurrentSpawnPoint = null;
            m_CurrentDirectionPoint = null;
            m_CurrentTile = null;

            DirectionPointInstruction.gameObject.SetActive(false);
            SpawnPointInstruction.gameObject.SetActive(false);

            InputManager.Instance.PopModalInputHandler();
            m_State = TilesGeneratorState.Idle;
        }

        /// <summary>
        /// Updates the spawnPoint.
        /// </summary>
        private void updateSpawnPoint()
        {
            Transform newSpawnPoint;
            if ((newSpawnPoint = getNearestPoint()) != null)
            {
                // update the new spawn point and adjust the old spawn point
                if (newSpawnPoint != m_CurrentSpawnPoint)
                {
                    if (m_CurrentSpawnPoint != null)
                    {
                        var oldPolyPointGazeResponder = m_CurrentSpawnPoint.GetComponent<GazeResponder>();
                        if (oldPolyPointGazeResponder)
                        {
                            oldPolyPointGazeResponder.ForceOnFocusExit();
                        }
                    }
                    m_CurrentSpawnPoint = newSpawnPoint;
                    var polyPointGazeResponder = m_CurrentSpawnPoint.GetComponent<GazeResponder>();
                    if (polyPointGazeResponder)
                    {
                        polyPointGazeResponder.ForceOnFocusEnter();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the direction point.
        /// </summary>
        private void updateDirectionPoint()
        {
            Transform newDirectionPoint;
            // exclude the spawn point as the direction point needs to be another point
            if ((newDirectionPoint = getNearestPoint(m_CurrentSpawnPoint)) != null)
            {
                // update the new direction point and adjust the old direction point
                if (newDirectionPoint != m_CurrentDirectionPoint)
                {
                    if (m_CurrentDirectionPoint != null)
                    {
                        var oldPolyPointGazeResponder = m_CurrentDirectionPoint.GetComponent<GazeResponder>();
                        if (oldPolyPointGazeResponder)
                        {
                            oldPolyPointGazeResponder.ForceOnFocusExit();
                        }
                    }
                    m_CurrentDirectionPoint = newDirectionPoint;
                    var polyPointGazeResponder = m_CurrentDirectionPoint.GetComponent<GazeResponder>();
                    if (polyPointGazeResponder)
                    {
                        polyPointGazeResponder.ForceOnFocusEnter();
                    }
                }
            }
        }

        /// <summary>
        /// Returns the neareast point from the intersection point between the gaze forward vector and the floor plane.
        /// </summary>
        /// <returns></returns>
        private Transform getNearestPoint(Transform excludePoint = null)
        {
            // cast a ray from the gaze (camera) to the plane of the floor
            float rayDistance;
            if (m_RaycastPlane.Raycast(GazeManager.Instance.Rays[0], out rayDistance))
            {
                // find the nearest point to the intersection point
                var minDistanceIndex = -1;
                var minDistance = float.MaxValue;
                var gazeOnFloorPlane = GazeManager.Instance.Rays[0].GetPoint(rayDistance);
                for (int i = 0; i < RoomManager.Instance.Floor.MeshPolygon.Points.Count; i++)
                {
                    if (RoomManager.Instance.Floor.MeshPolygon.Points[i].transform == excludePoint)
                        continue;

                    var currentDistance = (RoomManager.Instance.Floor.MeshPolygon.Points[i].transform.position - gazeOnFloorPlane).sqrMagnitude;
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        minDistanceIndex = i;
                    }
                }
                return RoomManager.Instance.Floor.MeshPolygon.Points[minDistanceIndex].transform;
            }
            else return null;
        }
    }
}
