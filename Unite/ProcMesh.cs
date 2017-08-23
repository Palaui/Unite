using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public static class ProcMesh
    {
        // Public Static
        #region Public Static

        // Plane
        #region Plane

        public static GameObject BuildPlane(Vector3[,] points)
        {
            Mesh mesh = CreatePlane(points);
            if (mesh)
            {
                GameObject go = new GameObject("Graphic3D");
                Ext.ResetTransform(go);

                go.AddComponent<MeshFilter>().mesh = mesh;
                MeshRenderer rend = go.AddComponent<MeshRenderer>();
                rend.material = new Material(Shader.Find("Standard"));
                rend.material.SetColor("_Color", Color.white);
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.receiveShadows = false;

                go.AddComponent<ProcMeshComponent>();
                return go;
            }

            Debug.LogError("ProcMesh BuildPlane: Unable to build plane");
            return null;
        }

        public static Mesh UpdatePlane(GameObject go, Vector3[,] points, float time)
        {
            Mesh mesh = CreatePlane(points);
            if (mesh)
            {
                ProcMeshComponent proc = go.GetComponent<ProcMeshComponent>();
                if (proc)
                    proc.UpdateMesh(mesh, time);
                else
                {
                    go.AddComponent<ProcMeshComponent>();
                    Ext.GetOrAddComponent<MeshFilter>(go).mesh = mesh;
                }
                return mesh;
            }

            Debug.LogError("ProcMesh UpdatePlane: Unable to update plane");
            return null;
        }

        public static Mesh CreatePlane(Vector3[,] points)
        {
            int rows = points.GetLength(0);
            if (rows > 0)
            {
                int columns = points.GetLength(1);

                List<Vector3> vertices = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();
                List<int> triangles = new List<int>();

                for (int i = 0; i < columns; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        vertices.Add(points[i, j]);
                        uvs.Add(new Vector2(i / (columns - 1.0f), (float)(j) / (rows - 1.0f)));
                    }
                }

                for (int i = 0; i < columns - 1; i++)
                {
                    for (int j = 0; j < rows - 1; j++)
                    {
                        triangles.Add(i + j * rows);
                        triangles.Add((i + 1) + j * rows);
                        triangles.Add(i + (j + 1) * rows);
                        triangles.Add(i + (j + 1) * rows);
                        triangles.Add((i + 1) + j * rows);
                        triangles.Add((i + 1) + (j + 1) * rows);
                    }
                }

                Mesh mesh = new Mesh();
                mesh.vertices = Ext.CreateArrayFromList(vertices);
                mesh.uv = Ext.CreateArrayFromList(uvs);
                mesh.triangles = Ext.CreateArrayFromList(triangles);
                mesh.RecalculateNormals();

                return mesh;
            }

            Debug.LogError("ProcMesh CreatePlane: Unable to create plane");
            return null;
        }

        #endregion

        // Line
        #region Line

        public static GameObject BuildLine(Vector3[] points) { return BuildLine(points, Color.white, 0.005f ); }
        public static GameObject BuildLine(Vector3[] points, Color color) { return BuildLine(points, color, 0.005f ); }
        public static GameObject BuildLine(Vector3[] points, Color color, float width)
        {
            Mesh mesh = CreateLine(points, width);
            if (mesh)
            {
                GameObject go = new GameObject("Graphic2D");
                Ext.ResetTransform(go);

                go.AddComponent<MeshFilter>().mesh = mesh;
                MeshRenderer rend = go.AddComponent<MeshRenderer>();
                rend.material = new Material(Shader.Find("Standard"));
                rend.material.SetColor("_Color", color);
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.receiveShadows = false;

                go.AddComponent<ProcMeshComponent>();
                return go;
            }

            Debug.LogError("ProcMesh BuildLine: Unable to build line");
            return null;
        }

        public static Mesh UpdateLine(GameObject go, Vector3[] points, float time) { return UpdateLine(go, points, time, 0.005f); }
        public static Mesh UpdateLine(GameObject go, Vector3[] points, float time, float width)
        {
            Mesh mesh = CreateLine(points, width);
            if (mesh)
            {
                ProcMeshComponent proc = go.GetComponent<ProcMeshComponent>();
                if (proc)
                    proc.UpdateMesh(mesh, time);
                else
                {
                    go.AddComponent<ProcMeshComponent>();
                    Ext.GetOrAddComponent<MeshFilter>(go).mesh = mesh;
                }
                return mesh;
            }

            Debug.LogError("ProcMesh UpdateLine: Unable to update line");
            return null;
        }

        public static Mesh CreateLine(Vector3[] points, float width)
        {
            if (points.Length > 2)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();
                List<int> triangles = new List<int>();
                int vertexCount = 0;

                Vector3 slope = (points[1] - points[0]).normalized;
                Vector3 perpendicular = new Vector3(slope.y, -slope.x, slope.z) * width;
                vertices.Add(points[0] - perpendicular);
                vertices.Add(points[0] + perpendicular);
                for (int i = 1; i < points.Length - 1; i++)
                {
                    slope = (points[i + 1] - points[i - 1]).normalized;
                    perpendicular = new Vector3(slope.y, -slope.x, slope.z) * width;
                    vertices.Add(points[i] - perpendicular);
                    vertices.Add(points[i] + perpendicular);
                }
                slope = (points[points.Length - 1] - points[points.Length - 2]).normalized;
                perpendicular = new Vector3(slope.y, -slope.x, slope.z) * width;
                vertices.Add(points[points.Length - 1] - perpendicular);
                vertices.Add(points[points.Length - 1] + perpendicular);

                for (int i = 0; i < points.Length; i++)
                {
                    uvs.Add(new Vector2(1 - i / points.Length, 1));
                    uvs.Add(new Vector2(1 - i / points.Length, 0));
                }

                for (int i = 0; i < points.Length - 1; i++)
                {
                    triangles.Add(vertexCount);
                    triangles.Add(vertexCount + 2);
                    triangles.Add(vertexCount + 1);
                    triangles.Add(vertexCount + 1);
                    triangles.Add(vertexCount + 2);
                    triangles.Add(vertexCount + 3);

                    vertexCount += 2;
                }

                Mesh mesh = new Mesh();
                mesh.vertices = Ext.CreateArrayFromList(vertices);
                mesh.uv = Ext.CreateArrayFromList(uvs);
                mesh.triangles = Ext.CreateArrayFromList(triangles);
                mesh.RecalculateNormals();

                return mesh;
            }

            Debug.LogError("ProcMesh CreateLine: Unable to create line");
            return null;
        }

        #endregion

        #endregion

    }
}
