using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class PlanarCut
    {
        // Subclasses
        #region Subclasses

        internal class VertexState
        {
            public Location location;
            public bool hasBeenMoved;

            public VertexState(Location loc)
            {
                location = loc;
                hasBeenMoved = false;
            }
        }

        #endregion

        // Enums
        #region Enums

        public enum Location { Above, Cut, Below }

        #endregion

        // Variables
        #region Variables

        private static Dictionary<int, Vector3> perimeterVertices = new Dictionary<int, Vector3>();
        private static List<VertexState> verticesState = new List<VertexState>();

        private static List<Vector3> vertices = new List<Vector3>();
        private static List<Vector3> normals = new List<Vector3>();
        private static List<Vector2> uvs = new List<Vector2>();
        private static List<int> triangles = new List<int>();

        private static Mesh mesh;
        private static GameObject plane;

        #endregion

        // Public Static
        #region Public Static

        public static void Cut(GameObject go, GameObject inPlane)
        {
            Chronometer chrono = new Chronometer();
            chrono.Start();
            if (!go.GetComponent<MeshFilter>())
                return;

            mesh = go.GetComponent<MeshFilter>().mesh;
            plane = inPlane;

            PrepareMesh();
            GetMeshStates(go.transform.localToWorldMatrix);
            BuildMesh(go, CalculateTriangles(go.transform.localToWorldMatrix, go.transform.worldToLocalMatrix));

            chrono.DebugValue();
            chrono.Pause();
        }

        #endregion

        // Private Static
        #region Private Static

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

        private static void GetMeshStates(Matrix4x4 localToWorld)
        {
            for (int i = 0; i < vertices.Count; i++)
                verticesState.Add(new VertexState(IsPointBelowCut(localToWorld.MultiplyPoint3x4(vertices[i]))));
        }

        private static Location IsPointBelowCut(Vector3 point)
        {
            if (Vector3.Dot(-plane.transform.forward, point - plane.transform.position) < 0)
                return Location.Below;
            else
                return Location.Above;
        }

        private static List<int> CalculateTriangles(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
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
                        AdaptVertices(localToWorld, worldToLocal, i);
                }
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

        private static void AdaptVertices(Matrix4x4 localToWorld, Matrix4x4 worldToLocal, int index)
        {
            Vector3 pos = Vector3.zero;
            int outsideVertices = 0;

            for (int i = index; i < index + 3; i++)
            {
                if (verticesState[triangles[i]].location == Location.Above)
                    outsideVertices++;
                else
                    pos += localToWorld.MultiplyPoint3x4(vertices[triangles[i]]);
            }

            if (outsideVertices == 1)
                PlaceOnPlaneIntersection(localToWorld, pos / 2, index);
            if (outsideVertices == 2)
                PlaceOnPlaneIntersection(localToWorld, pos, index);
        }

        private static void PlaceOnPlaneIntersection(Matrix4x4 localToWorld, Vector3 pos, int index)
        {
            for (int i = index; i < index + 3; i++)
            {
                if (verticesState[triangles[i]].location == Location.Above &&
                    !verticesState[triangles[i]].hasBeenMoved)
                {
                    Vector3 intesection;
                    MathExt.LinePlaneIntersection(
                        pos, (localToWorld.MultiplyPoint3x4(vertices[triangles[i]]) - pos).normalized,
                        plane.transform.forward, plane.transform.position, out intesection);

                    perimeterVertices.Add(triangles[i], intesection);
                    verticesState[triangles[i]].hasBeenMoved = true;
                }
            }
        }

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
        }

        #endregion
    }
}
