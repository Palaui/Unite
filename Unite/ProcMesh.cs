using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public static class ProcMesh
    {
        // Public Static
        #region Public Static

        public static  GameObject BuildPlane(Vector3[,] points)
        {
            int rows = points.GetLength(0);
            if (rows > 0)
            {
                int columns = points.GetLength(1);

                GameObject go = new GameObject("Graphic3D");
                Ext.ResetTransform(go);
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

                MeshFilter filter = go.AddComponent<MeshFilter>();
                MeshRenderer rend = go.AddComponent<MeshRenderer>();
                filter.mesh.vertices = Ext.CreateArrayFromList(vertices);
                filter.mesh.uv = Ext.CreateArrayFromList(uvs);
                filter.mesh.triangles = Ext.CreateArrayFromList(triangles);
                filter.mesh.RecalculateNormals();
                rend.material = new Material(Shader.Find("Standard"));
                rend.material.SetColor("_Color", Color.white);
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                return go;
            }

            Debug.Log("ProcMesh BuildPlane: Unable to build plane");
            return null;
        }

        #endregion

    }
}
