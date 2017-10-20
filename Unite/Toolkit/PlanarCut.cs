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

        private static Dictionary<int, Vector3> verticesToMove = new Dictionary<int, Vector3>();
        private static List<VertexState> verticesState = new List<VertexState>();
        private static List<Location> triangleStates = new List<Location>();

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

            GetMeshStates(go.transform.localToWorldMatrix);
            BuildMesh(go, CalculateTriangles(go.transform.localToWorldMatrix, go.transform.worldToLocalMatrix));

            chrono.DebugValue();
            chrono.Pause();
        }

        #endregion

        // Private Static
        #region Private Static

        private static void GetMeshStates(Matrix4x4 localToWorld)
        {
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                verticesState.Add(
                    new VertexState(IsPointBelowLine(localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[i]]))));
                verticesState.Add(
                    new VertexState(IsPointBelowLine(localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[i + 1]]))));
                verticesState.Add(
                    new VertexState(IsPointBelowLine(localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[i + 2]]))));
                triangleStates.Add(GetTriangleState(verticesState[i], verticesState[i + 1], verticesState[i + 2]));
            }
        }

        private static Location GetTriangleState(VertexState vertexA, VertexState vertexB, VertexState vertexC)
        {
            if (vertexA.location != vertexB.location || vertexA.location != vertexC.location)
                return Location.Cut;
            else
                return vertexA.location;
        }

        private static Location IsPointBelowLine(Vector3 point)
        {
            if (Vector3.Dot(-plane.transform.forward, point - plane.transform.position) <= 0)
                return Location.Below;
            else
                return Location.Above;
        }

        private static List<int> CalculateTriangles(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
        {
            List<int> triangles = new List<int>();
            for (int i = 0; i < triangleStates.Count; i++)
            {
                if (triangleStates[i] != Location.Above)
                {
                    triangles.Add(mesh.triangles[i * 3]);
                    triangles.Add(mesh.triangles[i * 3 + 1]);
                    triangles.Add(mesh.triangles[i * 3 + 2]);

                    if (triangleStates[i] == Location.Cut)
                        AdaptVertices(localToWorld, worldToLocal, i);
                }
                    
            }
            return triangles;
        }

        private static void AdaptVertices(Matrix4x4 localToWorld, Matrix4x4 worldToLocal, int index)
        {
            Vector3 pos = Vector3.zero;
            int outsideVertices = 0;
            for (int i = index * 3; i < index * 3 + 3; i++)
            {
                if (verticesState[i].location == Location.Above)
                    outsideVertices++;
                else
                    pos += localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[i]]);
            }

            if (outsideVertices == 1)
            {
                pos /= 2;
                for (int i = index * 3; i < index * 3 + 3; i++)
                {
                    if (verticesState[i].location == Location.Above && !verticesState[i].hasBeenMoved)
                    {
                        Vector3 intesection;
                        MathExt.LinePlaneIntersection(
                            pos, (localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[i]]) - pos).normalized, 
                            plane.transform.forward,  plane.transform.position, out intesection);

                        if (verticesToMove.ContainsKey(mesh.triangles[i]))
                            verticesToMove[mesh.triangles[i]] = intesection;
                        else
                            verticesToMove.Add(mesh.triangles[i], intesection);
                        verticesState[i].hasBeenMoved = true;
                    }
                }
            }
            else if (outsideVertices == 2)
            {
                for (int i = index * 3; i < index * 3 + 3; i++)
                {
                    if (verticesState[i].location == Location.Above && !verticesState[i].hasBeenMoved)
                    {
                        Vector3 intesection;
                        MathExt.LinePlaneIntersection(
                            pos, (localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[i]]) - pos).normalized,
                            plane.transform.forward, plane.transform.position, out intesection);

                        if (verticesToMove.ContainsKey(mesh.triangles[i]))
                            verticesToMove[mesh.triangles[i]] = intesection;
                        else
                            verticesToMove.Add(mesh.triangles[i], intesection);
                        verticesState[i].hasBeenMoved = true;
                    }
                }
            }
        }

        private static void BuildMesh(GameObject go, List<int> triangles)
        {
            Mesh newMesh = new Mesh();

            List<Vector3> test = Ext.CreateListFromArray(mesh.vertices);
            //newMesh.vertices = mesh.vertices;
            foreach (KeyValuePair<int, Vector3> entry in verticesToMove)
            {
                //Debug.Log(newMesh.vertices[entry.Key]);
                //Debug.Log(entry.Value);
                //newMesh.vertices[entry.Key] = Vector3.zero;
                test[entry.Key] = entry.Value;
            }

            //for (int i = 0; i < newMesh.vertices.Length; i++)
            //    newMesh.vertices[i] = Vector3.zero;

            newMesh.vertices = Ext.CreateArrayFromList(test);
            newMesh.uv = mesh.uv;
            newMesh.triangles = Ext.CreateArrayFromList(triangles);

            go.GetComponent<MeshFilter>().mesh = newMesh;
        }

        #endregion
    }
}
