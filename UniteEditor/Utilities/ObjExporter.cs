using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace UniteEditor
{
    public class ObjExporter
    {
        private static int startIndex = 0;

        public static void Start()
        {
            startIndex = 0;
        }

        public static void End()
        {
            startIndex = 0;
        }

        public static string MeshToString(Mesh mesh, Transform tr)
        {
            Vector3 s = tr.localScale;
            Vector3 p = tr.localPosition;
            Quaternion r = tr.localRotation;

            Material[] mats = tr.GetComponent<Renderer>().materials;
            int numVertices = 0;
            if (!mesh)
                return "Mesh not found";

            StringBuilder sb = new StringBuilder();

            foreach (Vector3 vv in mesh.vertices)
            {
                Vector3 v = tr.TransformPoint(vv);
                numVertices++;
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
            }
            sb.Append("\n");
            foreach (Vector3 nn in mesh.normals)
            {
                Vector3 v = r * nn;
                sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in mesh.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (int material = 0; material < mesh.subMeshCount; material++)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                sb.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = mesh.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1 + startIndex, triangles[i + 1] + 1 + startIndex, triangles[i + 2] + 1 + startIndex));
                }
            }

            startIndex += numVertices;
            return sb.ToString();
        }
    }

    public class ObjExporterScriptable : ScriptableObject
    {
        // Private Static
        #region Private Static

        [MenuItem("Unite/Export/Basic Obj")]
        private static void BasicExport()
        {
            BasicExport(false);
        }

        private static void BasicExport(bool makeSubmeshes)
        {
            if (Selection.gameObjects.Length == 0)
            {
                Debug.Log("ObjExporter BasicExport: No mesh selected, Aborting.");
                return;
            }

            string meshName = Selection.gameObjects[0].name;
            string fileName = EditorUtility.SaveFilePanel("Export .obj file", "", meshName, "obj");

            ObjExporter.Start();

            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + meshName + ".obj"
                                + "\n#" + System.DateTime.Now.ToLongDateString()
                                + "\n#" + System.DateTime.Now.ToLongTimeString()
                                + "\n#-------"
                                + "\n\n");

            Transform tr = Selection.gameObjects[0].transform;

            Vector3 originalPosition = tr.position;
            tr.position = Vector3.zero;

            if (!makeSubmeshes)
                meshString.Append("g ").Append(tr.name).Append("\n");

            meshString.Append(ProcessTransform(tr, makeSubmeshes));

            WriteToFile(meshString.ToString(), fileName);

            tr.position = originalPosition;

            ObjExporter.End();
            Debug.Log("Exported Mesh: " + fileName);
        }

        private static string ProcessTransform(Transform tr, bool makeSubmeshes)
        {
            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + tr.name + "\n#-------" + "\n");

            //if (makeSubmeshes)
            //    meshString.Append("g ").Append(tr.name).Append("\n");

            Mesh mesh = new Mesh();
            if (tr.GetComponent<SkinnedMeshRenderer>())
                tr.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
            else
                mesh = tr.GetComponent<MeshFilter>().mesh;

            if (mesh)
                meshString.Append(ObjExporter.MeshToString(mesh, tr));

            //for (int i = 0; i < tr.childCount; i++)
            //    meshString.Append(ProcessTransform(tr.GetChild(i), makeSubmeshes));

            return meshString.ToString();
        }

        private static void WriteToFile(string str, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
                sw.Write(str);
        }

        #endregion

    }
}