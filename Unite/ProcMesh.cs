﻿using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public static class ProcMesh
    {
        // Public Static
        #region Public Static

        // Plane
        #region Plane

        public static GameObject BuildPlane(DoubleV3[,] points)
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

        public static Mesh UpdatePlane(GameObject go, DoubleV3[,] points, double time)
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

        public static Mesh CreatePlane(DoubleV3[,] points)
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
                        uvs.Add(new Vector2(i / (columns - 1.0f), j / (float)(rows - 1)));
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

        public static GameObject BuildLine(DoubleV3[] points) { return BuildLine(points, Color.white); }
        public static GameObject BuildLine(DoubleV3[] points, Color color, double width = 0.005f, double z = 0)
        {
            Mesh mesh = CreateLine(points, width, z);
            if (mesh)
            {
                GameObject go = new GameObject("Graphic2D");
                Ext.ResetTransform(go);

                go.AddComponent<MeshFilter>().mesh = mesh;
                MeshRenderer rend = go.AddComponent<MeshRenderer>();
                rend.sharedMaterial = new Material(Shader.Find("Standard"));
                rend.sharedMaterial.SetColor("_Color", color);
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.receiveShadows = false;

                go.AddComponent<ProcMeshComponent>();
                return go;
            }

            Debug.LogError("ProcMesh BuildLine: Unable to build line");
            return null;
        }

        public static Mesh UpdateLine(GameObject go, DoubleV3[] points, double time, double z = 0) { return UpdateLine(go, points, time, 0.005f, z); }
        public static Mesh UpdateLine(GameObject go, DoubleV3[] points, double time, double width, double z = 0)
        {
            Mesh mesh = CreateLine(points, width, z);
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

        public static Mesh CreateLine(DoubleV3[] points, double width, double z)
        {
            if (points.Length > 2)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();
                List<int> triangles = new List<int>();
                int vertexCount = 0;

                // Z adapt
                for (int i = 0; i < points.Length; i++)
                    points[i].z += z;

                // Vertices
                DoubleV3 slope = (points[1] - points[0]).Normalize();
                DoubleV3 perpendicular = new DoubleV3(slope.y, -slope.x, slope.z) * width;
                vertices.Add(points[0] - perpendicular);
                vertices.Add(points[0] + perpendicular);
                for (int i = 1; i < points.Length - 1; i++)
                {
                    slope = (points[i + 1] - points[i - 1]).Normalize();
                    perpendicular = new DoubleV3(slope.y, -slope.x, slope.z) * width;
                    vertices.Add(points[i] - perpendicular);
                    vertices.Add(points[i] + perpendicular);
                }
                slope = (points[points.Length - 1] - points[points.Length - 2]).Normalize();
                perpendicular = new DoubleV3(slope.y, -slope.x, slope.z) * width;
                vertices.Add(points[points.Length - 1] - perpendicular);
                vertices.Add(points[points.Length - 1] + perpendicular);

                // UV
                for (int i = 0; i < points.Length; i++)
                {
                    uvs.Add(new DoubleV2(1 - i / points.Length, 1));
                    uvs.Add(new DoubleV2(1 - i / points.Length, 0));
                }

                // Triangles
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

                // Mesh cretion
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
