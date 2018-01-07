/*
* Farseer Physics Engine:
* Copyright (c) 2012 Ian Qvist
*/

using UnityEngine;
using System.Collections.Generic;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.Decomposition.CDT;
using FarseerPhysics.Common.Decomposition.CDT.Delaunay;
using FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;
using FarseerPhysics.Common.Decomposition.CDT.Polygon;

namespace HoloLensPlanner.Utilities.Decomposition
{
    /// <summary>
    /// 2D constrained Delaunay triangulation algorithm.
    /// Based on the paper "Sweep-line algorithm for constrained Delaunay triangulation" by V. Domiter and and B. Zalik
    /// 
    /// Properties:
    /// - Creates triangles with a large interior angle.
    /// - Supports holes
    /// - Generate a lot of garbage due to incapsulation of the Poly2Tri library.
    /// - Running time is O(n^2), n = number of vertices.
    /// - Does not care about winding order.
    /// 
    /// Source: http://code.google.com/p/poly2tri/
    /// </summary>
    internal static class CDTDecomposer
    {
        /// <summary>
        /// Decompose the polygon into several smaller non-concave polygon.
        /// </summary>
        public static List<List<Vector2>> ConvexPartition(List<Vector2> vertices, List<List<Vector2>> holes = null)
        {

            FarseerPhysics.Common.Decomposition.CDT.Polygon.Polygon poly = new FarseerPhysics.Common.Decomposition.CDT.Polygon.Polygon();

            foreach (Vector2 vertex in vertices)
                poly.Points.Add(new TriangulationPoint(vertex.x, vertex.y));

            if (holes != null)
            {
                foreach (List<Vector2> holeVertices in holes)
                {
                    FarseerPhysics.Common.Decomposition.CDT.Polygon.Polygon hole = new FarseerPhysics.Common.Decomposition.CDT.Polygon.Polygon();

                    foreach (Vector2 vertex in holeVertices)
                        hole.Points.Add(new TriangulationPoint(vertex.x, vertex.y));

                    poly.AddHole(hole);
                }
            }

            DTSweepContext tcx = new DTSweepContext();
            tcx.PrepareTriangulation(poly);
            DTSweep.Triangulate(tcx);

            List<List<Vector2>> results = new List<List<Vector2>>();

            foreach (DelaunayTriangle triangle in poly.Triangles)
            {
                List<Vector2> v = new List<Vector2>();
                foreach (TriangulationPoint p in triangle.Points)
                {
                    v.Add(new Vector2((float)p.X, (float)p.Y));
                }
                results.Add(v);
            }

            return results;
        }
    }
}