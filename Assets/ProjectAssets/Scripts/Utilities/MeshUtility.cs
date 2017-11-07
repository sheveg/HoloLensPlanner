using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner.Utilities
{
    public static class MeshUtility 
    {
        /// <summary>
        /// Creates a mesh out of a polygon.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Mesh CreatePolygonMesh(List<Vector2> vertices)
        {
            // split the polygon into convex parts so we can use fan triangulation https://en.wikipedia.org/wiki/Fan_triangulation
            List<List<Vector2>> convexPolygons = BayazitDecomposer.ConvexPartition(vertices);
            Debug.Log("Polygon is split into " + convexPolygons.Count + " parts");
            // create mesh arrays
            List<List<Vector3>> meshVerticesList = new List<List<Vector3>>();
            List<List<Vector3>> meshNormalsList = new List<List<Vector3>>();
            List<List<int>> meshTrianglesList = new List<List<int>>();
            for (int i = 0; i < convexPolygons.Count; i++)
            {
                // map 2d vertices of the polygon to 3d on the 0-plane
                List<Vector3> polygon3D = new List<Vector3>();
                foreach (var vertex in convexPolygons[i])
                {
                    polygon3D.Add(new Vector3(vertex.x, 0, vertex.y));
                }
                meshVerticesList.Add(polygon3D);
                // fan triangulation
                List<int> triangles = new List<int>();
                for (int k = 1; k < polygon3D.Count - 1; k++)
                {
                    triangles.Add(0);
                    triangles.Add(k);
                    triangles.Add(k + 1);
                }
                meshTrianglesList.Add(triangles);
            }
            // let all normals point up as this is only in 2D
            foreach (var meshVertices in meshVerticesList)
            {
                meshNormalsList.Add(new List<Vector3>(new Vector3[meshVertices.Count]));
            }
            CombineInstance[] combinedMeshes = new CombineInstance[convexPolygons.Count];
            for (int i = 0; i < convexPolygons.Count; i++)
            {
                // create a mesh for each convex polygon
                Mesh meshPart = new Mesh();
                meshPart.vertices = meshVerticesList[i].ToArray();
                meshPart.normals = meshNormalsList[i].ToArray();
                meshPart.triangles = meshTrianglesList[i].ToArray();
                meshPart.RecalculateBounds();
                meshPart.RecalculateNormals();
                combinedMeshes[i].mesh = meshPart;
                combinedMeshes[i].transform = Matrix4x4.identity;
            }
            // combine the meshes to one mesh
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combinedMeshes);
            return mesh;
        }
    }
}

