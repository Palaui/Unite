﻿using System.Collections.Generic;
using System.IO;
using Unite;
using UnityEngine;

namespace UniteCore
{
    [ExecuteInEditMode]
    public class LightingSchemeManager : MonoBehaviour
    {
        // Structs
        #region Structs

        [System.Serializable]
        public class LightSystem
        {
            public LightSystem(string name) { this.name = name; }

            public string name;
            public List<LightStats> lightStats = new List<LightStats>();

            public static implicit operator bool(LightSystem instance) { return instance != null; }
        }

        [System.Serializable]
        public class LightStats
        {
            public string name;
            public Light light;
            public LightType type;
            public Color color;
            public float intensity;
            public Vector3 position;
            public Vector3 rotation;

            public static implicit operator bool(LightStats instance) { return instance != null; }
        }

        #endregion

        // Variables
        #region Variables

        public List<LightSystem> lightSystems = new List<LightSystem>();
        public string systemName;
        public Light addLight;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            addLight = null;
        }

        void Update()
        {
            if (addLight)
            {
                if (systemName != "")
                {
                    LightSystem system = GetLightSystemByName(systemName);
                    if (system)
                        AddLightToSystem(system, addLight);
                    else
                    {
                        system = new LightSystem(systemName);
                        lightSystems.Add(system);
                        AddLightToSystem(system, addLight);
                    }
                }

                addLight = null;
            }
        }

        #endregion

        // Public
        #region Public

        /// <summary> Generate a new palette, should be called using the editor tool at (Core -> ColorScheme) </summary>
        public void Generate()
        {
            if (systemName == "")
            {
                Debug.LogError("LightingManager Generate: SystemName can not be empty, Aborting");
                return;
            }

            LightSystem system = GetLightSystemByName(systemName);
            if (system)
            {

                TextAsset textAsset = Resources.Load("Engine/Data/LightingSchemes/" + systemName + ".json") as TextAsset;

#if UNITY_EDITOR

                if (!textAsset)
                {
                    File.WriteAllText(Application.dataPath + "/Resources/Engine/Data/LightingSchemes/" + systemName + ".json", " ");
                    UnityEditor.AssetDatabase.Refresh();
                }

#endif

                JSon schemeJson = new JSon("Assets/Resources/Engine/Data/LightingSchemes/" + system.name);
                schemeJson.RemoveAll();

                schemeJson.AddValue("ID", system.name);
                schemeJson.AddNode("Systems");
                foreach (LightStats stats in system.lightStats)
                {
                    schemeJson.AddNode("Systems", stats.name);
                    schemeJson["Systems"][stats.name].AddValue("type", stats.type.ToString());
                    schemeJson["Systems"][stats.name].AddValue("color", stats.color.r.ToString("F3") + ", " +
                        stats.color.g.ToString("F3") + ", " + stats.color.b.ToString("F3") + ", 1.000");
                    schemeJson["Systems"][stats.name].AddValue("intensity", stats.intensity.ToString("F3"));
                    schemeJson["Systems"][stats.name].AddValue("positionX", stats.position.x.ToString("F3"));
                    schemeJson["Systems"][stats.name].AddValue("positionY", stats.position.y.ToString("F3"));
                    schemeJson["Systems"][stats.name].AddValue("positionZ", stats.position.z.ToString("F3"));
                    schemeJson["Systems"][stats.name].AddValue("rotationX", stats.rotation.x.ToString("F3"));
                    schemeJson["Systems"][stats.name].AddValue("rotationY", stats.rotation.y.ToString("F3"));
                    schemeJson["Systems"][stats.name].AddValue("rotationZ", stats.rotation.z.ToString("F3"));
                }

                schemeJson.Rewrite();
            }
        }

        public void Load()
        {
            JSon schemeJSon = new JSon(Resources.Load("Engine/Data/LightingSchemes/" + systemName) as TextAsset);
            if (schemeJSon == null)
            {
                Debug.LogError("LightingManager Load: Unable to find " + systemName + " JSon, Aborting");
                return;
            }

            LightSystem system = GetLightSystemByName(systemName);
            if (system)
            {

            }
            else
            {
                system = new LightSystem(systemName);
                lightSystems.Add(system);
                system.lightStats = GetLightStatsFromJSon(schemeJSon);
            }
        }

        #endregion

        // Private
        #region Private

        private void AddLightToSystem(LightSystem system, Light addLight)
        {
            LightStats stats = GetLightRecordedInSystem(system, addLight);
            if (stats)
                stats = GenerateLightStats(stats, addLight);
            else
                system.lightStats.Add(GenerateLightStats(addLight));
        }

        private LightSystem GetLightSystemByName(string inSystemName)
        {
            foreach (LightSystem system in lightSystems)
            {
                if (system.name == inSystemName)
                    return system;
            }

            return null;
        }

        private LightStats GetLightRecordedInSystem(LightSystem system, Light light)
        {
            foreach (LightStats stats in system.lightStats)
            {
                if (stats.light == light)
                    return stats;
            }

            return null;
        }

        private LightStats GenerateLightStats(Light light)
        {
            LightStats stats = new LightStats();
            return GenerateLightStats(stats, light);
        }
        private LightStats GenerateLightStats(LightStats stats, Light light)
        {
            stats.name = light.name;
            stats.light = light;
            stats.type = light.type;
            stats.color = light.color;
            stats.intensity = light.intensity;
            stats.position = light.transform.position;
            stats.rotation = light.transform.eulerAngles;

            return stats;
        }

        private List<LightStats> GetLightStatsFromJSon(JSon schemeJSon)
        {
            List<LightStats> statsList = new List<LightStats>();

            foreach (JSon json in schemeJSon["Systems"].GetNodeValues())
            {
                LightStats stats = new LightStats();
                stats.name = json.ID;
                stats.type = (LightType)System.Enum.Parse(typeof(LightType), json.GetValue("type"));
                stats.color = json.GetColorValue("type");
                stats.intensity = json.GetFloatValue("intensity");
                stats.position = new Vector3(json.GetFloatValue("positionX"),
                    json.GetFloatValue("positionY"), json.GetFloatValue("positionZ"));
                stats.rotation = new Vector3(json.GetFloatValue("rotationX"),
                    json.GetFloatValue("rotationY"), json.GetFloatValue("rotationZ"));

                statsList.Add(stats);
            }

            return statsList;
        }

        #endregion

    }
}
