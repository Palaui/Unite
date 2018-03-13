using System.Collections;
using UnityEngine;

namespace Unite
{
    public class DynamicDrive : MonoBehaviour
    {
        // Event Declaration
        #region Event Declaration

        public delegate void OnRecieveSpreadSheet(JSon json);
        public static event OnRecieveSpreadSheet recieveSpreadSheet;

        #endregion

        // Public
        #region Public

        public void RecieveSpreadSheet(JSon json)
        {
            if (recieveSpreadSheet != null)
                recieveSpreadSheet(json);
        }

        #endregion

        // Public Static
        #region Public Static

        public static void GetSpreadSheet(string url, string tabName, string init, string end)
        {
            DynamicDrive instance = Container.GetComponent<DynamicDrive>();
            instance.StartCoroutine(instance.ReadSpreadSheet(url, tabName, init, end));
        }

        #endregion

        // Coroutines
        #region Coroutines

        IEnumerator ReadSpreadSheet(string url, string tabName, string init, string end)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                Debug.Log(www.text);
                string content = www.text.Split(new string[] { init, end },
                    System.StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                JSon json = new JSon();
                string currentNode = "";
                foreach (string entry in content.Split('\n'))
                {
                    string[] strs = entry.Split(',');
                    if (strs.Length >= 2)
                    {
                        if (strs[0].Trim() == tabName)
                        {
                            currentNode = strs[1].Trim();
                            json.AddNode(currentNode);
                        }
                        else if (currentNode != "")
                            json.AddValue(currentNode, strs[0].Trim(), entry.Replace(strs[0] + ",", "").Trim());
                    }
                }

                RecieveSpreadSheet(json);
            }
        }

        #endregion

    }
}
