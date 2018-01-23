using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Unite
{
    public class JSon
    {
        // Variables
        #region Variables

        private static List<JSon> auxiliarJSons = new List<JSon>();
        private static List<string> auxiliarNames = new List<string>();
        private static int auxiliarCount;

        private Dictionary<string, JSon> nodes = new Dictionary<string, JSon>();
        private Dictionary<string, string> values = new Dictionary<string, string>();
        private Dictionary<string, JSon> leaves = new Dictionary<string, JSon>();
        private List<string> lines = new List<string>();

        private JSon parent;
        private string dataPath;
        private string id;

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
                if (nodes.ContainsKey(key))
                {
                    nodes[key].parent = this;
                    return nodes[key];
                }
                else
                {
                    Debug.LogError(key + " Does not exist in the nodes dictionary");
                    return null;
                }
            }
        }

        public string ID
        {
            get { return id; }
        }

        #endregion

        // Override
        #region Override

        public JSon(TextAsset asset, string id = "")
        {
            this.id = id;
            Load(asset);
            dataPath = "";
        }
        public JSon(string path, string id = "")
        {
            this.id = id;
            Load(path);
        }
        public JSon() { }

        private JSon(JSon json, string id)
        {
            this.id = id;
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

        // Public
        #region Public

        // Loaders & Writers
        #region Loaders & Writers

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
            {
                Debug.LogError("This JSon was not loaded with path, only JSon with path are allowed to be modified");
                return;
            }

            if (dataPath.Contains(" "))
            {
                Debug.Log("Unable to rewrite a subnode");
                return;
            }

            if (File.Exists(dataPath + ".json"))
                GetBaseParent().WriteJSon(dataPath + ".json");
            else
                Debug.LogError("Unable to find File at path: " + dataPath);
        }

        #endregion

        // Read Dictionary Values
        #region Read Dictionary Values

        public Dictionary<string, JSon> GetKeyValueNodes()
        {
            Dictionary<string, JSon> keyValues = new Dictionary<string, JSon>();
            foreach (KeyValuePair<string, JSon> entry in nodes)
                keyValues.Add(entry.Key, entry.Value);
            return keyValues;
        }
        public Dictionary<string, JSon> GetKeyValueNodes(string nodeName)
        {
            return GetNode(nodeName).GetKeyValueNodes();
        }

        public Dictionary<string, string> GetKeyValueValues()
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in values)
                keyValues.Add(entry.Key, entry.Value);
            return keyValues;
        }
        public Dictionary<string, string> GetKeyValueValues(string nodeName)
        {
            return GetNode(nodeName).GetKeyValueValues();
        }


        public Dictionary<string, float> GetKeyNodeValuesAsFloat(string nodeName)
        {
            Dictionary<string, float> keyValues = new Dictionary<string, float>();
            foreach (KeyValuePair<string, string> entry in GetKeyValueValues(nodeName))
                keyValues.Add(entry.Key, GetFloatValue(entry.Value));
            return keyValues;
        }

        public Dictionary<string, int> GetKeyNodeValuesAsInt(string nodeName)
        {
            Dictionary<string, int> keyValues = new Dictionary<string, int>();
            foreach (KeyValuePair<string, string> entry in GetKeyValueValues(nodeName))
                keyValues.Add(entry.Key, GetIntValue(entry.Value));
            return keyValues;
        }

        public Dictionary<string, bool> GetKeyNodeValuesAsBool(string nodeName)
        {
            Dictionary<string, bool> keyValues = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, string> entry in GetKeyValueValues(nodeName))
                keyValues.Add(entry.Key, GetBoolValue(entry.Value));
            return keyValues;
        }

        #endregion

        // Read List Values
        #region Read List Values

        public List<string> GetNodeKeys()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, JSon> entry in nodes)
                list.Add(entry.Key);
            return list;
        }

        public List<JSon> GetNodeValues()
        {
            List<JSon> list = new List<JSon>();
            foreach (KeyValuePair<string, JSon> entry in nodes)
                list.Add(entry.Value);
            return list;
        }

        public List<string> GetValueKeys()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> entry in values)
                list.Add(entry.Key);
            return list;
        }

        public List<string> GetValuesValues()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> entry in values)
                list.Add(entry.Value);
            return list;
        }

        public List<string> GetValues(string nodeName)
        {
            List<string> values = new List<string>();
            foreach (KeyValuePair<string, string> entry in GetKeyValueValues(nodeName))
                values.Add(entry.Value);
            return values;
        }

        public List<float> GetValuesAsFloat(string nodeName)
        {
            List<float> values = new List<float>();
            foreach (KeyValuePair<string, string> entry in GetKeyValueValues(nodeName))
                values.Add(GetFloatValue(entry.Value));
            return values;
        }

        public List<int> GetValuesAsInt(string nodeName)
        {
            List<int> values = new List<int>();
            foreach (KeyValuePair<string, string> entry in GetKeyValueValues(nodeName))
                values.Add(GetIntValue(entry.Value));
            return values;
        }

        public List<bool> GetValuesAsBool(string nodeName)
        {
            List<bool> values = new List<bool>();
            foreach (KeyValuePair<string, string> entry in GetKeyValueValues(nodeName))
                values.Add(GetBoolValue(entry.Value));
            return values;
        }

        public List<JSon> GetLeaveNodes()
        {
            if (leaves.Count == 0)
                GetLeavesRecursively(this);

            List<JSon> list = new List<JSon>();
            foreach (KeyValuePair<string, JSon> entry in leaves)
                list.Add(entry.Value);
            return list;
        }

        public List<string> GetLeaveValues()
        {
            if (leaves.Count == 0)
                GetLeavesRecursively(this);

            List<string> list = new List<string>();
            foreach (KeyValuePair<string, JSon> entry in leaves)
            {
                foreach (string value in entry.Value.GetValuesValues())
                    list.Add(value);
            }
            return list;
        }

        #endregion

        // Read Single Values
        #region Read Single Values

        public JSon GetNode(string key)
        {
            if (!nodes.ContainsKey(key))
            {
                Debug.LogError("This node does not contain the node " + key);
                return null;
            }
            return nodes[key];
        }

        public bool ContainsNode(string key)
        {
            return nodes.ContainsKey(key);
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

        public bool ContainsValue(string key)
        {
            return values.ContainsKey(key);
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

        public Color GetColorValue(string key, bool useColor32 = true)
        {
            if (!values.ContainsKey(key))
                Debug.LogError("This node does not contain the value " + key);

            return Ext.GetColorFromString(values[key], useColor32);
        }

        public ColorBlock GetColorBlockValue(string key, bool useColor32 = true)
        {
            if (!values.ContainsKey(key))
                Debug.LogError("This node does not contain the value " + key);

            return Ext.GetColorBlockFromString(values[key], useColor32);
        }

        public Sprite GetSprite(string key)
        {
            if (!values.ContainsKey(key))
                Debug.LogError("This node does not contain the value " + key);

            return Resources.Load<Sprite>(values[key]) as Sprite;
        }

        #endregion

        // Add Values
        #region Add Values

        public void AddNode(string key)
        {
            nodes.Add(key, new JSon(this, key));
        }
        public void AddNode(string parentName, string key)
        {
            if (!nodes.ContainsKey(parentName))
            {
                Debug.LogError("Node " + parentName + " was not found, unable to add " + key);
                return;
            }
            nodes[parentName].nodes.Add(key, new JSon(this, key));
        }

        public void AddNodes(Dictionary<string, JSon> entries)
        {
            foreach (KeyValuePair<string, JSon> entry in entries)
                nodes.Add(entry.Key, entry.Value);
        }
        public void AddNodes(string nodeName, Dictionary<string, JSon> entries)
        {
            if (!nodes.ContainsKey(nodeName))
            {
                Debug.LogError("Node " + nodeName + " was not found, unable to add nodes");
                return;
            }
            foreach (KeyValuePair<string, JSon> entry in entries)
                nodes[nodeName].nodes.Add(entry.Key, entry.Value);
        }

        public void AddValue(string key, string value)
        {
            if (values.ContainsKey(key))
                values[key] = value;
            else
                values.Add(key, value.ToString());
        }
        public void AddValue(string nodeName, string key, string value)
        {
            if (!nodes.ContainsKey(nodeName))
            {
                Debug.LogError("Node " + nodeName + " was not found, unable to add " + key);
                return;
            }
            nodes[nodeName].values.Add(key, value.ToString());
        }

        public void AddValues(Dictionary<string, string> entries)
        {
            foreach (KeyValuePair<string, string> entry in entries)
                values.Add(entry.Key, entry.Value.ToString());
        }
        public void AddValues(string nodeName, Dictionary<string, string> entries)
        {
            if (!nodes.ContainsKey(nodeName))
            {
                Debug.LogError("Node " + nodeName + " was not found, unable to add values");
                return;
            }
            foreach (KeyValuePair<string, string> entry in entries)
                nodes[nodeName].values.Add(entry.Key, entry.Value.ToString());
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
            if (!nodes.ContainsKey(nodeName))
            {
                Debug.LogError("Node " + nodeName + " was not found, unable to remove " + key);
                return;
            }
            if (!nodes[nodeName].values.ContainsKey(key))
            {
                Debug.LogError("Node " + nodeName + " does not contain the key " + key);
                return;
            }
            nodes[nodeName].values.Remove(key);
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

        public void RemoveAll()
        {
            nodes.Clear();
            values.Clear();
        }

        public void RemoveAllNodes()
        {
            nodes.Clear();
        }

        public void RemoveAllValues()
        {
            values.Clear();
        }

        #endregion

        // Text Management
        #region Text Management

        public string GetNodeAsString(string key)
        {
            if (!nodes.ContainsKey(key))
            {
                Debug.LogError("Node " + key + " was not found");
                return null;
            }
            return nodes[key].ToString();
        }

        public void SortAlphabetically()
        {
            SortAlphabetically(this);
        }

        #endregion

        // Navigation
        #region Navigation

        public JSon GetParent()
        {
            return parent;
        }

        public JSon GetBaseParent()
        {
            JSon json = this;
            while (json.parent != null)
                json = json.parent;
            return json;
        }

        #endregion

        // Recursive
        #region Recursive

        public List<JSon> GetAllNodesAndSubNodes()
        {
            auxiliarJSons.Clear();
            GetNodeNamesRecursively(this);
            return auxiliarJSons;
        }

        public int GetTotalNumberOfNodes()
        {
            auxiliarCount = 0;
            GetNodesCountRecursively(this);
            return auxiliarCount;
        }

        public List<string> GetAllValuesAndSubValues()
        {
            auxiliarNames.Clear();
            GetValueNamesRecursively(this);
            return auxiliarNames;
        }

        public int GetTotalNumberOfValues()
        {
            auxiliarCount = 0;
            GetValuesCountRecursively(this);
            return auxiliarCount;
        }

        #endregion

        #endregion

        // Public Static
        #region Public Static

        public static bool isValid(string path)
        {
            return File.Exists(path + ".json");
        }

        public static JSon GetFromString(string str)
        {
            JSon json = new JSon();
            json.Parse(str);
            json.id = "";
            json.dataPath = "";
            return json;
        }

        public static void SortAlphabetically(JSon json)
        {
            Dictionary<string, string> valsDictionary = new Dictionary<string, string>();
            List<string> vals = json.GetValueKeys();
            vals.Sort();
            for (int i = 0; i < vals.Count; i++)
                valsDictionary.Add(vals[i], json.values[vals[i]]);
            json.values = valsDictionary;

            Dictionary<string, JSon> nodsDictionary = new Dictionary<string, JSon>();
            List<string> nods = json.GetNodeKeys();
            nods.Sort();
            for (int i = 0; i < nods.Count; i++)
            {
                nodsDictionary.Add(nods[i], json.nodes[nods[i]]);
                SortAlphabetically(json.nodes[nods[i]]);
            }
            json.nodes = nodsDictionary;
        }

        public static void WriteJSonAtPath(JSon json, string path)
        {
            if (File.Exists(path + ".json"))
            {
                json.CreateLines();
                StreamWriter writer = new StreamWriter(path + ".json");
                foreach (string line in json.lines)
                    writer.WriteLine(line);
                writer.Flush();
                writer.Close();
            }
        }

        #endregion

        // Private
        #region Private

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

        // Calculus
        #region Calculus

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
                    if (currentLevel != 0 && currentKey != "")
                        list.Add(new JSonStruct(currentKey.Trim(), "", words[currentLevel - 1], currentLevel));
                    currentLevel++;
                    keySet = false;
                }
                else if (ch == '}' || ch == ']')
                {
                    if (currentValue.Trim() == "")
                        currentValue = "Empty";
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
                    if (currentValue.Trim() == "")
                        currentValue = "Empty";
                    if (currentKey != "")
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
                    JSon subJSon = new JSon(this, json.key);
                    jsons[jsons.Count - 1].nodes.Add(json.key, subJSon);
                    jsons.Add(subJSon);
                    currentLevel++;
                }
                else
                    jsons[jsons.Count - 1].values.Add(json.key, json.value);
            }
        }

        #endregion

        // Recursivity
        #region Recursivity

        private void GetLeavesRecursively(JSon inJSon)
        {
            foreach (JSon json in inJSon.GetNodeValues())
            {
                if (json.nodes.Count == 0)
                    leaves.Add(json.ID, json);
                else
                    GetLeavesRecursively(json);
            }
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

        private void GetNodeNamesRecursively(JSon inJson)
        {
            foreach (JSon subJSon in inJson.GetNodeValues())
            {
                GetNodeNamesRecursively(subJSon);
                auxiliarJSons.Add(subJSon);
            }
        }

        private void GetNodesCountRecursively(JSon inJson)
        {
            foreach (JSon subJSon in inJson.GetNodeValues())
                GetNodesCountRecursively(subJSon);
            auxiliarCount += inJson.GetNodeValues().Count;
        }

        private void GetValueNamesRecursively(JSon inJson)
        {
            foreach (JSon subJSon in inJson.GetNodeValues())
                GetValueNamesRecursively(subJSon);

            foreach (string str in inJson.GetValueKeys())
                auxiliarNames.Add(str);
        }

        private void GetValuesCountRecursively(JSon inJson)
        {
            foreach (JSon subJSon in inJson.GetNodeValues())
                GetValuesCountRecursively(subJSon);
            auxiliarCount += inJson.GetValuesValues().Count;
        }

        #endregion

        #endregion

    }
}