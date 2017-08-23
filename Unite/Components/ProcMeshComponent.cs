using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    class ProcMeshComponent : MonoBehaviour
    {
        // Variables
        #region Variables

        private IEnumerator AnimateCoroutine;

        private Mesh currentMesh;
        private Mesh objMesh;

        #endregion

        // Internal
        #region Internal

        internal void UpdateMesh(Mesh mesh, float time)
        {
            if (!mesh)
                return;
            if (GetComponent<MeshFilter>().mesh.vertexCount != mesh.vertexCount)
                return;

            if (AnimateCoroutine != null)
                StopCoroutine(AnimateCoroutine);

            currentMesh = GetComponent<MeshFilter>().mesh;
            objMesh = mesh;

            AnimateCoroutine = Animate(time);
            StartCoroutine(AnimateCoroutine);
        }

        #endregion

        // Coroutines
        #region Coroutines

        private IEnumerator Animate(float time)
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            float currentTime = 0;

            while (currentTime < time)
            {
                Mesh interpMesh = new Mesh();
                List<Vector3> vertices = new List<Vector3>();
                for (int i = 0; i < currentMesh.vertexCount; i++)
                    vertices.Add(Vector3.Lerp(currentMesh.vertices[i], objMesh.vertices[i], currentTime / time));

                interpMesh.vertices = Ext.CreateArrayFromList(vertices);
                interpMesh.uv = currentMesh.uv;
                interpMesh.triangles = currentMesh.triangles;
                interpMesh.RecalculateNormals();
                filter.mesh = interpMesh;

                yield return null;
                currentTime += Time.deltaTime;
            }

            filter.mesh = objMesh;
            filter.mesh.RecalculateNormals();
        }

        #endregion

    }
}
