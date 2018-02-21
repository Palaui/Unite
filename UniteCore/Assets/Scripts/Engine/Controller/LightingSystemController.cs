using System.Collections.Generic;
using Unite;
using UnityEngine;

namespace UniteCore
{
    // Enums
    #region Enums

    public enum LightBlendType { OutIn, Simultaneous }
    public enum LightChangeType { Immediate, Fade, Driven, DrivenWeighted }

    #endregion

    public class LightingSystemController : MonoBehaviour
    {
        // Variables
        #region Variables

        private Color darkness;
        private JSon currentJSon;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            RemoveCurrentSystem();
            darkness = Color.black;
            RenderSettings.ambientLight = darkness;
        }

        #endregion

        // Public
        #region Public

        public void RemoveCurrentSystem()
        {
            Ext.DestroyChildren(gameObject);

            Light[] lights = FindObjectsOfType<Light>();
            for (int i = lights.Length - 1; i >= 0; i--)
            {
                if (lights[i])
                    DestroyImmediate(lights[i]);
            }
        }

        public void ChangeLightScheme(LightBlendType blendType, LightChangeType changeType, JSon schemeJSon)
        {
            switch (changeType)
            {
                case LightChangeType.Immediate:
                    ImmediateChange(schemeJSon);
                    break;
                case LightChangeType.Fade:
                    FadeOutIn(currentJSon, schemeJSon);
                    break;
                case LightChangeType.Driven:
                    FadeOutIn(currentJSon, schemeJSon);
                    break;
                case LightChangeType.DrivenWeighted:
                    break;
                default:
                    break;
            }

            currentJSon = schemeJSon;
        }

        public List<Light> CreateSystem(JSon schemeJSon)
        {
            List<Light> lights = new List<Light>();

            foreach (JSon json in schemeJSon["Lights"].GetNodeValues())
            {
                GameObject go = new GameObject();
                Light li = go.AddComponent<Light>();

                go.name = json.ID;
                go.transform.SetParent(transform);
                li.type = (LightType)System.Enum.Parse(typeof(LightType), json.GetValue("type"));
                li.color = json.GetColorValue("color", false);
                li.intensity = json.GetFloatValue("intensity");
                go.transform.position = new Vector3(json.GetFloatValue("positionX"),
                    json.GetFloatValue("positionY"), json.GetFloatValue("positionZ"));
                go.transform.eulerAngles = new Vector3(json.GetFloatValue("rotationX"),
                    json.GetFloatValue("rotationY"), json.GetFloatValue("rotationZ"));

                lights.Add(li);
            }

            return lights;
        }

        #endregion

        // Private
        #region Private

        private List<Light> GetCurrentLights()
        {
            List<Light> lights = new List<Light>();
            foreach (Light li in GetComponentsInChildren<Light>())
            {
                if (li)
                    lights.Add(li);
            }

            return lights;
        }

        private void FadeLightsIn(JSon schemeJSon, bool fadeAmbient)
        {
            List<Light> lights = CreateSystem(schemeJSon);
            List<float> intensities = new List<float>();
            foreach (Light li in lights)
                intensities.Add(li.intensity);

            DynamicListener.Timeline(EaseType.QuarticEaseInOut, schemeJSon.GetFloatValue("AnimTime"), (float alpha) =>
            {
                for (int i = 0; i < lights.Count; i++)
                    lights[i].intensity = alpha * intensities[i];
                if (fadeAmbient)
                    RenderSettings.ambientLight = Color.Lerp(darkness, schemeJSon.GetColorValue("AmbientColor", false), alpha);
            });
        }

        // System Changers
        #region System Changers

        private void ImmediateChange(JSon schemeJSon)
        {
            if (transform.childCount > 0)
                RemoveCurrentSystem();
            CreateSystem(schemeJSon);
        }

        private void FadeOutIn(JSon lastJSon, JSon schemeJSon)
        {
            List<Light> lights;
            List<float> intensities = new List<float>();

            if (lastJSon)
            {
                lights = GetCurrentLights();
                foreach (Light li in lights)
                    intensities.Add(li.intensity);

                DynamicListener.Timeline(EaseType.QuarticEaseInOut, lastJSon.GetFloatValue("AnimTime"), (float alpha) =>
                {
                    for (int i = 0; i < lights.Count; i++)
                        lights[i].intensity = (1 - alpha) * intensities[i];
                    RenderSettings.ambientLight = Color.Lerp(lastJSon.GetColorValue("AmbientColor", false), darkness, alpha);
                }, 
                () => FadeLightsIn(schemeJSon, true));

                return;
            }

            FadeLightsIn(schemeJSon, true);
        }

        private void DrivenOutIn(JSon lastJSon, JSon schemeJSon)
        {
            List<Light> lights;
            List<float> intensities = new List<float>();

            if (lastJSon)
            {
                lights = GetCurrentLights();
                foreach (Light li in lights)
                    intensities.Add(li.intensity);

                DynamicListener.Timeline(EaseType.QuarticEaseInOut, lastJSon.GetFloatValue("AnimTime"), (float alpha) =>
                {
                    for (int i = 0; i < lights.Count; i++)
                        lights[i].intensity = (1 - alpha) * intensities[i];
                    RenderSettings.ambientLight = Color.Lerp(lastJSon.GetColorValue("AmbientColor", false), darkness, alpha);
                },
                () => FadeLightsIn(schemeJSon, true));

                return;
            }

            FadeLightsIn(schemeJSon, true);
        }

        #endregion

        #endregion

    }
}