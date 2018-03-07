using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace Unite.Utilities
{

    public class PersistanceUtility : MonoBehaviour
    {

// Variables
#region Variables
        public string std;

        private static string StrmAssetPath;
        private static string PrstDataPath;
#endregion



        void Start()
        {
            StrmAssetPath = UnityEngine.Application.streamingAssetsPath;
            PrstDataPath = UnityEngine.Application.persistentDataPath;
        }


// Public
#region Public

        public string GetStreamingAssetsPath()
        {
            return StrmAssetPath;
        }

        public string GetPrstDataPath()
        {
            return PrstDataPath;
        }




        /// <summary> Saves data into the given file of persistantdatapath </summary>
        /// <param name="filename"></param>
        /// <param name=""></param>
        public void SaveFile(string filename, UnityEngine.Object data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(PrstDataPath + "/" + filename, FileMode.OpenOrCreate);

            bf.Serialize(file, data);
            file.Close();
        }


        /// <summary>Saves string data into the given file of persistantdatapath</summary>
        /// <param name="filename"></param>
        /// <param name=""></param>
        public void SaveStringFile(string filename, string data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(PrstDataPath + "/" + filename, FileMode.OpenOrCreate);

            std = data;

            bf.Serialize(file, data);
            file.Close();
        }

        /// <summary> Load data from the given file of persistantdatapath</summary>
        /// <param name="filename"></param>
        /// <param name=""></param>
        public object LoadFile(string filename, UnityEngine.Object data)
        {
            if (File.Exists(PrstDataPath + "/" + filename))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(PrstDataPath + "/" + filename, FileMode.Open);

                data =  (UnityEngine.Object)bf.Deserialize(file);
                file.Close();
                return data;
            }else
            {
                Debug.LogError("The selected File does not exist!!");
                return null;
            }
        }

        /// <summary> Load string from the given file of persistantdatapath </summary>
        /// <param name="filename"></param>
        /// <param name=""></param>
        public string LoadStringFile(string filename)
        {
            if (File.Exists(PrstDataPath + "/" + filename))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(PrstDataPath + "/" + filename, FileMode.Open);
                 std = (string)bf.Deserialize(file);
                file.Close();

                return std;             
            }
           Debug.LogError("The selected File does not exist!!");
            return null;
        }

#endregion
    }

}