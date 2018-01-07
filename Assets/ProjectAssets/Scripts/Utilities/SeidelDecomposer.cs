﻿using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace HoloLensPlanner.Utilities.Decomposition
{
    /// <summary>
    /// Convex decomposition algorithm created by Raimund Seidel
    /// 
    /// Properties:
    /// - Decompose the polygon into trapezoids, then triangulate.
    /// - To use the trapezoid data, use ConvexPartitionTrapezoid()
    /// - Generate a lot of garbage due to incapsulation of the Poly2Tri library.
    /// - Running time is O(n log n), n = number of vertices.
    /// - Running time is almost linear for most simple polygons.
    /// - Does not care about winding order. 
    /// 
    /// For more information, see Raimund Seidel's paper "A simple and fast incremental randomized
    /// algorithm for computing trapezoidal decompositions and for triangulating polygons"
    /// 
    /// See also: "Computational Geometry", 3rd edition, by Mark de Berg et al, Chapter 6.2
    ///           "Computational Geometry in C", 2nd edition, by Joseph O'Rourke
    /// 
    /// Original code from the Poly2Tri project by Mason Green.
    /// http://code.google.com/p/poly2tri/source/browse?repo=archive#hg/scala/src/org/poly2tri/seidel
    /// 
    /// This implementation is from Dec 14, 2010
    /// </summary>
    internal static class SeidelDecomposer
    {
        /// <summary>
        /// Decompose the polygon into several smaller non-concave polygons.
        /// </summary>
        /// <param name="vertices">The polygon to decompose.</param>
        /// <param name="sheer">The sheer to use if you get bad results, try using a higher value.</param>
        /// <returns>A list of triangles</returns>
        public static List<List<Vector2>> ConvexPartition(List<Vector2> vertices, float sheer = 0.001f)
        {
            List<Point> compatList = new List<Point>(vertices.Count);

            foreach (Vector2 vertex in vertices)
            {
                compatList.Add(new Point(vertex.x, vertex.y));
            }

            Triangulator t = new Triangulator(compatList, sheer);

            List<List<Vector2>> list = new List<List<Vector2>>();

            foreach (List<Point> triangle in t.Triangles)
            {
                List<Vector2> outTriangles = new List<Vector2>(triangle.Count);

                foreach (Point outTriangle in triangle)
                {
                    outTriangles.Add(new Vector2(outTriangle.X, outTriangle.Y));
                }

                list.Add(outTriangles);
            }

            return list;
        }

        /// <summary>
        /// Decompose the polygon into several smaller non-concave polygons.
        /// </summary>
        /// <param name="vertices">The polygon to decompose.</param>
        /// <param name="sheer">The sheer to use if you get bad results, try using a higher value.</param>
        /// <returns>A list of trapezoids</returns>
        public static List<List<Vector2>> ConvexPartitionTrapezoid(List<Vector2> vertices, float sheer = 0.001f)
        {
            List<Point> compatList = new List<Point>(vertices.Count);

            foreach (Vector2 vertex in vertices)
            {
                compatList.Add(new Point(vertex.x, vertex.y));
            }

            Triangulator t = new Triangulator(compatList, sheer);

            List<List<Vector2>> list = new List<List<Vector2>>();

            foreach (Trapezoid trapezoid in t.Trapezoids)
            {
                List<Vector2> verts = new List<Vector2>();

                List<Point> points = trapezoid.GetVertices();
                foreach (Point point in points)
                {
                    verts.Add(new Vector2(point.X, point.Y));
                }

                list.Add(verts);
            }

            return list;
        }
    }
}