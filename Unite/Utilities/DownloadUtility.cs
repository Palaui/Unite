using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Unite.Utilities
{
    public class DownloadUtility: MonoBehaviour
    {

        string platform;
        public string PersistentFileName;
        public string pathURL;
        static string pathLOCAL;
        static WWW objSERVER;
       

        public void StartDownloading()
        {
            if (PersistentFileName!=null && PersistentFileName!="")
            {
                StartCoroutine(Download());
            }else
            {
                Debug.LogError("The file name is empty!!");
            }

        }

        //IEnumerator allows yield so the information is not accessed
        //before it finished downloading
        System.Collections.IEnumerator Download()
        {
            if (Application.isMobilePlatform)
                platform = "Mobile";
            else if (!Application.isMobilePlatform && !Application.isConsolePlatform && !Application.isEditor)
                platform = "StandAlone";
            else if (Application.isEditor)
                platform = "Editor";

            pathURL = "http://3dtechomegazeta.com/apps/tests/" + PersistentFileName; //location of the file on the server
            pathLOCAL = Application.persistentDataPath + "/assetbundles/" + platform + "/" + PersistentFileName; //location of the file on the device

            objSERVER = new WWW(pathURL);

            // Wait for download to finish
            yield return objSERVER;

            // Save it to disk
            SaveDownloadedAsset(objSERVER);
        }

        public void SaveDownloadedAsset(WWW objSERVER)
        {

            // Create the directory if it doesn't already exist
            if (!Directory.Exists(Application.persistentDataPath + "/assetbundles/" + platform))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/assetbundles/" + platform);
            }

            // Initialize the byte string
            byte[] bytes = objSERVER.bytes;

            // Creates a new file, writes the specified byte array to the file, and then closes the file. 
            // If the target file already exists, it is overwritten.
            File.WriteAllBytes(pathLOCAL, bytes);
        }
    }
}
