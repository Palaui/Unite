using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class PlanarCut
    {
        // Subclasses
        #region Subclasses

        private class VertexState
        {
            public Location location;
            public bool hasBeenMoved;

            public VertexState(Location loc)
            {
                location = loc;
                hasBeenMoved = false;
            }
        }

        private class Edge
        {
            public Vector3 a;
            public Vector3 b;

            public Edge()
            {
                a = Vector3.zero;
                b = Vector3.zero;
            }

            public Edge(Vector3 a, Vector3 b)
            {
                this.a = a;
                this.b = b;
            }
        }

        #endregion

        // Enums
        #region Enums

        public enum Location { Above, Cut, Below }

        #endregion

        // Variables
        #region Variables

        private static Dictionary<int, Vector3> perimeterVertices;
        private static List<VertexState> verticesState;
        private static List<Edge> crownEdges;

        private static List<Vector3> vertices;
        private static List<Vector3> normals;
        private static List<Vector2> uvs;
        private static List<int> triangles;

        private static Mesh mesh;
        private static GameObject plane;

        private static Transform initParent;
        private static Vector3 initPosition;
        private static Vector3 initRotation;
        private static Vector3 initScale;

        private static Vector3 selectedCapVertex;
        private static int addedTriangles = 0;

        #endregion

        // Public Static
        #region Public Static

        public static void Cut(GameObject go, GameObject inPlane, bool createCap)
        {
            if (!go.GetComponent<MeshFilter>())
                return;

            InitializeVariables(go, inPlane);
            
            plane.transform.SetParent(go.transform);
            Ext.ResetTransform(go);

            PrepareMesh();
            GetMeshStates();
            List<int> tris = CalculateTriangles();

            if (createCap)
                PrepareCrown(tris);
            BuildMesh(go, tris);
        }

        #endregion

        // Private Static
        #region Private Static

        // Cut
        #region Cut

        private static void InitializeVariables(GameObject go, GameObject inPlane)
        {
            perimeterVertices = new Dictionary<int, Vector3>();
            verticesState = new List<VertexState>();
            crownEdges = new List<Edge>();

            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            uvs = new List<Vector2>();
            triangles = new List<int>();

            mesh = go.GetComponent<MeshFilter>().mesh;
            plane = inPlane;

            initParent = plane.transform.parent;
            initPosition = go.transform.position;
            initRotation = go.transform.eulerAngles;
            initScale = go.transform.localScale;

            addedTriangles = 0;
        }

        private static void PrepareMesh()
        {
            List<int> indexes = new List<int>();

            vertices = Ext.CreateListFromArray(mesh.vertices);
            normals = Ext.CreateListFromArray(mesh.normals);
            uvs = Ext.CreateListFromArray(mesh.uv);
            triangles = Ext.CreateListFromArray(mesh.triangles);

            for (int i = 0; i < triangles.Count; i++)
            {
                if (!indexes.Contains(triangles[i]))
                    indexes.Add(triangles[i]);
                else
                {
                    vertices.Add(vertices[triangles[i]]);
                    normals.Add(normals[triangles[i]]);
                    uvs.Add(uvs[triangles[i]]);
                    triangles[i] = vertices.Count - 1;
                }
            }
        }

        private static void GetMeshStates()
        {
            for (int i = 0; i < vertices.Count; i++)
                verticesState.Add(new VertexState(IsPointBelowCut(vertices[i])));
        }

        private static Location IsPointBelowCut(Vector3 point)
        {
            if (Vector3.Dot(-plane.transform.forward, point - plane.transform.position) < 0)
                return Location.Below;
            else
                return Location.Above;
        }

        private static List<int> CalculateTriangles()
        {
            List<int> tris = new List<int>();
            for (int i = 0; i < triangles.Count; i += 3)
            {
                Location triangleLocation = GetTriangleLocation(i, i + 1, i + 2);
                if (triangleLocation != Location.Above)
                {
                    tris.Add(triangles[i]);
                    tris.Add(triangles[i + 1]);
                    tris.Add(triangles[i + 2]);

                    if (triangleLocation == Location.Cut)
                        AdaptVertices(i);
                }
            }

            for (int i = 0; i < addedTriangles; i++)
            {
                tris.Add((vertices.Count - (3 * i)) - 3);
                tris.Add((vertices.Count - (3 * i)) - 2);
                tris.Add((vertices.Count - (3 * i)) - 1);
            }

            return tris;
        }

        private static Location GetTriangleLocation(int indexA, int indexB, int indexC)
        {
            Location locA = verticesState[triangles[indexA]].location;
            Location locB = verticesState[triangles[indexB]].location;
            Location locC = verticesState[triangles[indexC]].location;

            if (locA != locB || locA != locC)
                return Location.Cut;
            else
                return locA;
        }

        private static void AdaptVertices(int index)
        {
            Vector3 posA = Vector3.zero;
            Vector3 posB = Vector3.zero;
            int outsideVertices = 0;
            int a = 0, b = 0, c = 0;
            bool positionSet = false;
            bool typeA = true;

            for (int i = index; i < index + 3; i++)
            {
                if (verticesState[triangles[i]].location == Location.Above)
                {
                    outsideVertices++;
                    c = i;
                }
                else if (!positionSet)
                {
                    posA = vertices[triangles[i]];
                    positionSet = true;
                    a = i;
                }
                else
                {
                    posB = vertices[triangles[i]];
                    b = i;
                }
            }

            if (outsideVertices == 1)
            {
                for (int i = index; i < index + 3; i++)
                {
                    if (verticesState[triangles[i]].location == Location.Above &&
                        !verticesState[triangles[i]].hasBeenMoved)
                    {
                        Vector3 intersectionA, intersectionB;
                        MathExt.LinePlaneIntersection(
                            posA, (vertices[triangles[i]] - posA).normalized,
                            plane.transform.forward, plane.transform.position, out intersectionA);
                        MathExt.LinePlaneIntersection(
                            posB, (vertices[triangles[i]] - posB).normalized,
                            plane.transform.forward, plane.transform.position, out intersectionB);

                        perimeterVertices.Add(triangles[i], intersectionA);
                        crownEdges.Add(new Edge(intersectionA, intersectionB));
                        verticesState[triangles[i]].hasBeenMoved = true;

                        // Add triangles
                        if ((a > b && b > c) || (b > c && c > a) || (c > a && a > b))
                            typeA = false;

                        vertices.Add(intersectionA);
                        normals.Add(normals[triangles[i]]);
                        uvs.Add(uvs[triangles[i]]);
                        if (typeA)
                        {
                            vertices.Add(posB);
                            normals.Add(normals[triangles[i]]);
                            uvs.Add(uvs[triangles[i]]);
                        }
                        vertices.Add(intersectionB);
                        normals.Add(normals[triangles[i]]);
                        uvs.Add(uvs[triangles[i]]);
                        if (!typeA)
                        {
                            vertices.Add(posB);
                            normals.Add(normals[triangles[i]]);
                            uvs.Add(uvs[triangles[i]]);
                        }
                        addedTriangles++;
                    }
                }
            }
            if (outsideVertices == 2)
            {
                bool isSecondVertex = false;
                crownEdges.Add(new Edge());

                for (int i = index; i < index + 3; i++)
                {
                    if (verticesState[triangles[i]].location == Location.Above &&
                        !verticesState[triangles[i]].hasBeenMoved)
                    {
                        Vector3 intersection;
                        MathExt.LinePlaneIntersection(
                            posA, (vertices[triangles[i]] - posA).normalized,
                            plane.transform.forward, plane.transform.position, out intersection);

                        if (!isSecondVertex)
                        {
                            crownEdges[crownEdges.Count - 1].a = intersection;
                            isSecondVertex = true;
                        }
                        else
                            crownEdges[crownEdges.Count - 1].b = intersection;

                        perimeterVertices.Add(triangles[i], intersection);
                        verticesState[triangles[i]].hasBeenMoved = true;
                    }
                }
            }
        }

        #endregion

        // Cap
        #region Cap

        private static void PrepareCrown(List<int> tris)
        {
            if (crownEdges.Count == 0)
                return;

            Edge edgeA = null;
            Edge edgeB = null;
            Vector3 vertexA = Vector3.zero;
            Vector3 vertexB = Vector3.zero;

            int counter = 0;
            bool foundEdge = false;
            bool foundMatch = false;

            selectedCapVertex = crownEdges[0].a;
            while (crownEdges.Count > 1 && counter < 1000)
            {
                counter++;
                foundEdge = false;

                for (int i = 0; i < crownEdges.Count; i++)
                {
                    if (Vector3.Distance(crownEdges[i].a, selectedCapVertex) < 0.0001f)
                    {
                        if (edgeA == null)
                        {
                            edgeA = crownEdges[i];
                            vertexA = crownEdges[i].b;
                            foundEdge = true;
                        }
                        else
                        {
                            edgeB = crownEdges[i];
                            vertexB = crownEdges[i].b;
                        }
                    }
                    else if (Vector3.Distance(crownEdges[i].b, selectedCapVertex) < 0.0001f)
                    {
                        if (edgeA == null)
                        {
                            edgeA = crownEdges[i];
                            vertexA = crownEdges[i].a;
                            foundEdge = true;
                        }
                        else
                        {
                            edgeB = crownEdges[i];
                            vertexB = crownEdges[i].a;
                        }
                    }

                    if (edgeB != null)
                    {
                        foundMatch = CreateCapVertices(selectedCapVertex, vertexA, vertexB);
                        crownEdges.Remove(edgeA);
                        crownEdges.Remove(edgeB);
                    }

                    if (foundMatch)
                    {
                        tris.Add(vertices.Count - 3);
                        tris.Add(vertices.Count - 2);
                        tris.Add(vertices.Count - 1);
                        normals.Add(-plane.transform.forward);
                        normals.Add(-plane.transform.forward);
                        normals.Add(-plane.transform.forward);
                        uvs.Add(Vector2.zero);
                        uvs.Add(Vector2.zero);
                        uvs.Add(Vector2.one);
                        edgeA = null;
                        edgeB = null;

                        foundMatch = false;
                        break;
                    }
                }

                if (!foundEdge)
                {
                    edgeA = null;
                    edgeB = null;
                    selectedCapVertex = crownEdges[0].a;
                }
            }
        }

        private static bool CreateCapVertices(Vector3 a, Vector3 b, Vector3 c)
        {
            Matrix matrix = new Matrix(a, b, c);
            if (matrix.Determinant() == 0)
                return false;

            Vector3 intersection;
            MathExt.LinePlaneIntersection(
                Vector3.zero, plane.transform.position.normalized,
                plane.transform.forward, plane.transform.position, out intersection);

            if (matrix.Determinant() * Vector3.Dot(plane.transform.forward, intersection) < 0)
            {
                vertices.Add(a);
                vertices.Add(b);
                vertices.Add(c);
            }
            else
            {
                vertices.Add(a);
                vertices.Add(c);
                vertices.Add(b);
            }
            crownEdges.Add(new Edge(b, c));
            selectedCapVertex = c;
            return true;
        }

        #endregion

        private static void BuildMesh(GameObject go, List<int> tris)
        {
            Mesh cutMesh = new Mesh();

            foreach (KeyValuePair<int, Vector3> entry in perimeterVertices)
                vertices[entry.Key] = entry.Value;

            cutMesh.vertices = Ext.CreateArrayFromList(vertices);
            cutMesh.normals = Ext.CreateArrayFromList(normals);
            cutMesh.uv = Ext.CreateArrayFromList(uvs);
            cutMesh.triangles = Ext.CreateArrayFromList(tris);

            go.GetComponent<MeshFilter>().mesh = cutMesh;

            go.transform.position = initPosition;
            go.transform.eulerAngles = initRotation;
            go.transform.localScale = initScale;
            plane.transform.SetParent(initParent);
        }

        #endregion

    }
}
