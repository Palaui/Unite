using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unite
{
    public class JSon
    {
        // Variables
        #region Variables

        private Dictionary<string, JSon> nodes = new Dictionary<string, JSon>();
        private Dictionary<string, string> values = new Dictionary<string, string>();
        List<string> lines = new List<string>();
        private string dataPath;

        #endregion

        // Structs
        #region Structs

        public struct JSonStruct
        {
            public string key;
            public string value;
            public string node;
            public int level;

            public JSonStruct(string inKey, string inValue, string inNode, int inLevel)
            {
                key = inKey;
                value = inValue;
                node = inNode;
                level = inLevel;
            }
        }

        #endregion

        // Properties
        #region Properties

        public JSon this[string key]
        {
            get
            {
                if (GetAllNodes().ContainsKey(key))
                    return GetAllNodes()[key];
                else
                {
                    Debug.LogError(key + " Does not exist in the nodes dictionary");
                    return null;
                }
            }
        }

        #endregion

        // Public
        #region Public

        // Constructors, Loaders & Writers
        #region Constructors, Loaders & Writers

        public JSon() { }
        public JSon(TextAsset asset)
        {
            Load(asset);
            dataPath = "";
        }
        public JSon(string path)
        {
            Load(path);
        }

        public void Load(TextAsset asset)
        {
            if (asset)
            {
                Parse(asset.text);
                return;
            }
            Debug.LogError("No asset assigned");
        }
        public void Load(string path)
        {
            if (dataPath == "")
                Debug.LogError("This JSon was not loaded with path, use TextAsset instead");

            string str;
            try { str = File.ReadAllText(path + ".json"); }
            catch
            {
                Debug.LogError("Unable to read JSon at path " + path);
                return;
            }
            Parse(str);
            dataPath = path;
        }

        public void Rewrite()
        {
            if (dataPath == "")
                Debug.LogError("This JSon was not loaded with path, only JSon with path are allowed to be modified");

            if (dataPath.Contains(" "))
            {
                Debug.Log("Trying to rewrite a subnode, use Rewrite(bool writeNodeOnly) instead");
                return;
            }

            if (File.Exists(dataPath + ".json"))
                WriteJSon(dataPath + ".json");
            else
                Debug.LogError("Unable to find File at path: " + dataPath);
        }
        public void Rewrite(bool writeNodeOnly)
        {
            if (dataPath == "")
                Debug.LogError("This JSon was not loaded with path, only JSon with path are allowed to be modified");

            string path = dataPath.Split(' ')[0];
            if (writeNodeOnly)
            {
                if (File.Exists(path + ".json"))
                    WriteJSon(path + ".json");
                else
                    Debug.LogError("Unable to find File at path: " + path);
            }
            else
            {

            }
        }

        #endregion

        // Read Dictionary Values
        #region Read Dictionary Values

        public Dictionary<string, string> GetNodeKeyValues(string nodeName)
        {
            return GetNode(nodeName).values;
        }

        public Dictionary<string, float> GetNodeKeyValuesAsFloat(string nodeName)
        {
            Dictionary<string, float> keyValues = new Dictionary<string, float>();
            foreach (KeyValuePair<string, string> entry in GetNodeKeyValues(nodeName))
                keyValues.Add(entry.Key, GetFloatValue(entry.Value));
            return keyValues;
        }

        public Dictionary<string, int> GetNodeKeyValuesAsInt(string nodeName)
        {
            Dictionary<string, int> keyValues = new Dictionary<string, int>();
            foreach (KeyValuePair<string, string> entry in GetNodeKeyValues(nodeName))
                keyValues.Add(entry.Key, GetIntValue(entry.Value));
            return keyValues;
        }

        public Dictionary<string, bool> GetNodeKeyValuesAsBool(string nodeName)
        {
            Dictionary<string, bool> keyValues = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, string> entry in GetNodeKeyValues(nodeName))
                keyValues.Add(entry.Key, GetBoolValue(entry.Value));
            return keyValues;
        }

        #endregion

        // Read List Values
        #region Read List Values

        public List<string> GetNodeKeys(string nodeName)
        {
            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, string> entry in GetNodeKeyValues(nodeName))
                keys.Add(entry.Key);
            return keys;
        }

        public List<string> GetNodeValues(string nodeName)
        {
            List<string> values = new List<string>();
            foreach (KeyValuePair<string, string> entry in GetNodeKeyValues(nodeName))
                values.Add(entry.Value);
            return values;
        }

        public List<float> GetNodeValuesAsFloat(string nodeName)
        {
            List<float> values = new List<float>();
            foreach (KeyValuePair<string, string> entry in GetNodeKeyValues(nodeName))
                values.Add(GetFloatValue(entry.Value));
            return values;
        }

        public List<int> GetNodeValuesAsInt(string nodeName)
        {
            List<int> values = new List<int>();
            foreach (KeyValuePair<string, string> entry in GetNodeKeyValues(nodeName))
                values.Add(GetIntValue(entry.Value));
            return values;
        }

        public List<bool> GetNodeValuesAsBool(string nodeName)
        {
            List<bool> values = new List<bool>();
            foreach (KeyValuePair<string, string> entry in GetNodeKeyValues(nodeName))
                values.Add(GetBoolValue(entry.Value));
            return values;
        }

        #endregion

        // Read Single Values
        #region Read Single Values

        public JSon GetNode(string key)
        {
            if (!GetAllNodes().ContainsKey(key))
            {
                Debug.LogError("This node does not contain the node " + key);
                return null;
            }
            return GetAllNodes()[key];
        }

        public string GetValue(string key)
        {
            if (!values.ContainsKey(key))
            {
                Debug.LogError("This node does not contain the value " + key);
                return null;
            }
            return values[key];
        }

        public float GetFloatValue(string key)
        {
            if (!values.ContainsKey(key))
                Debug.LogError("This node does not contain the value " + key);
            string value = values[key];
            float f;
            if (float.TryParse(value, out f))
                return f;
            Debug.LogError("Unable to parse " + key + " to float");
            return 0;
        }

        public int GetIntValue(string key)
        {
            if (!values.ContainsKey(key))
                Debug.LogError("This node does not contain the value " + key);
            string value = values[key];
            int i;
            if (int.TryParse(value, out i))
                return i;
            Debug.LogError("Unable to parse " + key + " to int");
            return 0;
        }

        public bool GetBoolValue(string key)
        {
            if (!values.ContainsKey(key))
                Debug.LogError("This node does not contain the value " + key);
            string value = values[key];
            bool b;
            if (bool.TryParse(value, out b))
                return b;
            Debug.LogError("Unable to parse " + key + " to bool");
            return false;
        }

        #endregion

        // Add Values
        #region Add Values

        public void AddNode(string key)
        {
            nodes.Add(key, new JSon());
        }
        public void AddNode(string parentName, string key)
        {
            var dictionary = GetAllNodes();
            if (!dictionary.ContainsKey(parentName))
            {
                Debug.LogError("Node " + parentName + " was not found, unable to add " + key);
                return;
            }
            dictionary[parentName].nodes.Add(key, new JSon());
        }

        public void AddNodeEntry(string key, object value)
        {
            values.Add(key, value.ToString());
        }
        public void AddNodeEntry(string nodeName, string key, object value)
        {
            var dictionary = GetAllNodes();
            if (!dictionary.ContainsKey(nodeName))
            {
                Debug.LogError("Node " + nodeName + " was not found, unable to add " + key);
                return;
            }
            dictionary[nodeName].values.Add(key, value.ToString());
        }

        public void AddNodeEntries(Dictionary<string, object> entries)
        {
            foreach (KeyValuePair<string, object> entry in entries)
                values.Add(entry.Key, entry.Value.ToString());
        }
        public void AddNodeEntries(string nodeName, Dictionary<string, object> entries)
        {
            var dictionary = GetAllNodes();
            if (!dictionary.ContainsKey(nodeName))
            {
                Debug.LogError("Node " + nodeName + " was not found, unable to add entries");
                return;
            }
            foreach (KeyValuePair<string, object> entry in entries)
                dictionary[nodeName].values.Add(entry.Key, entry.Value.ToString());
        }

        #endregion

        // Remove Values
        #region Remove Values

        public void RemoveNode(string nodeName)
        {
            if (!nodes.ContainsKey(nodeName))
            {
                Debug.LogError(nodeName + " was not found, unable to remove it");
                return;
            }
            nodes.Remove(nodeName);
        }

        public void RemoveValue(string key)
        {
            if (!values.ContainsKey(key))
            {
                Debug.LogError(key + " was not found, unable to remove it");
                return;
            }
            values.Remove(key);
        }
        public void RemoveValue(string nodeName, string key)
        {
            var dictionary = GetAllNodes();
            if (!dictionary.ContainsKey(nodeName))
            {
                Debug.LogError("Node " + nodeName + " was not found, unable to remove " + key);
                return;
            }
            if (!dictionary[nodeName].values.ContainsKey(key))
            {
                Debug.LogError("Node " + nodeName + " does not contain the key " + key);
                return;
            }
            dictionary[nodeName].values.Remove(key);
        }

        public void RemoveValues(List<string> keys)
        {
            foreach (string str in keys)
                RemoveValue(str);
        }
        public void RemoveNodeEntries(string nodeName, List<string> keys)
        {
            foreach (string str in keys)
                RemoveValue(nodeName, str);
        }

        #endregion

        // Text Management
        #region Text Management

        public string GetAsString()
        {
            return ToString();
        }

        public string GetNodeAsString(string key)
        {
            if (!GetAllNodes().ContainsKey(key))
            {
                Debug.LogError("Node " + key + " was not found");
                return null;
            }
            return GetAllNodes()[key].ToString();
        }

        public override string ToString()
        {
            CreateLines();
            string str = "";
            foreach (string line in lines)
                str += line;
            return str;
        }

        #endregion

        #endregion

        // Public Static
        #region Public Static

        public static bool isValid(string path)
        {
            return File.Exists(path + ".json");
        }

        #endregion

        // Private
        #region Private

        private void Parse(string str)
        {
            List<JSonStruct> list = new List<JSonStruct>();
            List<string> words = new List<string>();
            string currentKey = "";
            string currentValue = "";
            int currentLevel = 0;
            bool keySet = false;
            bool inString = false;

            foreach (char ch in str)
            {
                if (inString)
                {
                    if (ch == '"')
                    {
                        inString = false;
                        continue;
                    }
                    if (!keySet)
                        currentKey += ch.ToString();
                    else
                        currentValue += ch.ToString();
                    continue;
                }

                if (ch == '{' || ch == '[')
                {
                    words.Add(currentKey.Trim());
                    if (currentLevel != 0)
                        list.Add(new JSonStruct(currentKey.Trim(), "", words[currentLevel - 1], currentLevel));
                    currentLevel++;
                    keySet = false;
                }
                else if (ch == '}' || ch == ']')
                {
                    if (currentKey != "")
                        list.Add(new JSonStruct(currentKey.Trim(), currentValue.Trim(), words[currentLevel - 1], currentLevel));
                    words.RemoveAt(words.Count - 1);
                    currentKey = "";
                    currentLevel--;
                }
                else if (ch == '"')
                {
                    if (!keySet)
                        currentKey = "";
                    else
                        currentValue = "";
                    inString = true;
                }
                else if (ch == ':')
                    keySet = true;
                else if (ch == ',')
                {
                    list.Add(new JSonStruct(currentKey.Trim(), currentValue.Trim(), words[currentLevel - 1], currentLevel));
                    currentKey = "";
                    currentValue = "";
                    keySet = false;
                }
                else if (keySet)
                    currentValue += ch.ToString();
            }

            // Crate dictionaries
            words.Clear();
            nodes.Clear();
            values.Clear();
            List<JSon> jsons = new List<JSon>();
            currentLevel = 1;
            jsons.Add(this);
            foreach (JSonStruct json in list)
            {
                while (json.level < currentLevel)
                {
                    currentLevel--;
                    jsons.RemoveAt(jsons.Count - 1);
                }

                if (json.value == "")
                {
                    JSon subJSon = new JSon();
                    jsons[jsons.Count - 1].nodes.Add(json.key, subJSon);
                    jsons.Add(subJSon);
                    currentLevel++;
                }
                else
                    jsons[jsons.Count - 1].values.Add(json.key, json.value);
            }
        }

        private Dictionary<string, JSon> GetAllNodes()
        {
            Dictionary<string, JSon> dictionary = new Dictionary<string, JSon>();
            GetAllNodesRecursively(this, dictionary);
            return dictionary;
        }

        private void GetAllNodesRecursively(JSon json, Dictionary<string, JSon> dictionary)
        {
            foreach (KeyValuePair<string, JSon> node in json.nodes)
            {
                dictionary.Add(node.Key, node.Value);
                GetAllNodesRecursively(node.Value, dictionary);
            }
        }

        private void WriteJSon(string path)
        {
            CreateLines();
            StreamWriter writer = new StreamWriter(path);
            foreach (string line in lines)
                writer.WriteLine(line);
            writer.Flush();
            writer.Close();
        }

        private void CreateLines()
        {
            lines.Clear();
            lines.Add("{");
            CreateLinesRecursively(this, 0);
            lines.Add("}");
        }

        private void CreateLinesRecursively(JSon json, int level)
        {
            string str = "";
            int foreachCounter = 0;

            for (int i = 0; i <= level; i++)
                str += "\t";

            foreach (KeyValuePair<string, string> entry in json.values)
            {
                foreachCounter++;
                if (json.values.Count > foreachCounter || json.nodes.Count > 0)
                    lines.Add(str + '"' + entry.Key + '"' + ": " + '"' + entry.Value + '"' + ',');
                else
                    lines.Add(str + '"' + entry.Key + '"' + ": " + '"' + entry.Value + '"');
            }

            foreachCounter = 0;
            foreach (KeyValuePair<string, JSon> entry in json.nodes)
            {
                foreachCounter++;
                lines.Add(str + '"' + entry.Key + '"' + ": " + '{');
                CreateLinesRecursively(entry.Value, level + 1);
                if (json.nodes.Count > foreachCounter)
                    lines.Add(str + "},");
                else
                    lines.Add(str + "}");
            }
        }

        #endregion

    }
}
