using System;
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

        public bool systemEnabled = true;

        private Color darkness;
        private JSon currentJSon;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            RemoveAllSystems();
            darkness = Color.black;
            RenderSettings.ambientLight = darkness;
        }

        #endregion

        // Internal
        #region Internal

        internal void RemoveAllSystems()
        {
            if (!systemEnabled)
                return;

            Ext.DestroyChildren(gameObject);
        }


        internal void RemoveSystem(List<Light> lights)
        {
            for (int i = lights.Count - 1; i >= 0; i--)
            {
                if (lights[i])
                    DestroyImmediate(lights[i].gameObject);
            }
        }

        internal void ChangeLightScheme(LightBlendType blendType, LightChangeType changeType, JSon schemeJSon)
        {
            if (!systemEnabled)
                return;

            switch (changeType)
            {
                case LightChangeType.Immediate:
                    ImmediateChange(schemeJSon);
                    break;
                case LightChangeType.Fade:
                    switch (blendType)
                    {
                        case LightBlendType.Simultaneous:
                            FadeSimultaneous(currentJSon, schemeJSon);
                            break;
                        case LightBlendType.OutIn:
                            FadeOutIn(currentJSon, schemeJSon);
                            break;
                    }
                    break;
                case LightChangeType.Driven:
                    switch (blendType)
                    {
                        case LightBlendType.OutIn:
                            DrivenOutIn(currentJSon, schemeJSon);
                            break;
                        case LightBlendType.Simultaneous:
                            DrivenSimultaneous(currentJSon, schemeJSon);
                            break;
                    }
                    break;
                case LightChangeType.DrivenWeighted:
                    switch (blendType)
                    {
                        case LightBlendType.OutIn:
                            DrivenWeightedOutIn(currentJSon, schemeJSon);
                            break;
                        case LightBlendType.Simultaneous:
                            FadeSimultaneous(currentJSon, schemeJSon);
                            break;
                    }
                    break;
            }

            currentJSon = schemeJSon;
        }

        internal List<Light> CreateSystem(JSon schemeJSon)
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

        // System Changers
        #region System Changers

        private void ImmediateChange(JSon schemeJSon)
        {
            if (transform.childCount > 0)
                RemoveAllSystems();

            CreateSystem(schemeJSon);
            RenderSettings.ambientLight = schemeJSon.GetColorValue("AmbientColor", false);
        }

        private void FadeOutIn(JSon lastJSon, JSon schemeJSon)
        {
            if (lastJSon)
            {
                Fade(lastJSon, false, true, () => Fade(schemeJSon, true, true));
                return;
            }

            Fade(schemeJSon, true, true);
        }

        private void FadeSimultaneous(JSon lastJSon, JSon schemeJSon)
        {
            if (lastJSon)
            {
                Color initialColor = lastJSon.GetColorValue("AmbientColor", false);
                Color finalColor = schemeJSon.GetColorValue("AmbientColor", false);
                float averageTime = (schemeJSon.GetFloatValue("AnimTime") + schemeJSon.GetFloatValue("AnimTime")) / 2;

                DynamicListener.Timeline(EaseType.QuadraticEaseInOut, averageTime, (float alpha) =>
                {
                    RenderSettings.ambientLight = Color.Lerp(initialColor, finalColor, alpha);
                });
                Fade(lastJSon, false, false);
                Fade(schemeJSon, true, false);

                return;
            }

            Fade(schemeJSon, true, true);
        }

        private void DrivenOutIn(JSon lastJSon, JSon schemeJSon)
        {
            if (lastJSon)
            {
                DrivenFade(lastJSon, false, true, () => DrivenFade(schemeJSon, true, true));
                return;
            }

            DrivenFade(schemeJSon, true, true);
        }

        private void DrivenSimultaneous(JSon lastJSon, JSon schemeJSon)
        {
            if (lastJSon)
            {
                Color initialColor = lastJSon.GetColorValue("AmbientColor", false);
                Color finalColor = schemeJSon.GetColorValue("AmbientColor", false);
                float averageTime = (schemeJSon.GetFloatValue("AnimTime") + schemeJSon.GetFloatValue("AnimTime")) / 2;

                DynamicListener.Timeline(EaseType.QuadraticEaseInOut, averageTime, (float alpha) =>
                {
                    RenderSettings.ambientLight = Color.Lerp(initialColor, finalColor, alpha);
                });
                DrivenFade(lastJSon, false, false);
                DrivenFade(schemeJSon, true, false);
                return;
            }

            DrivenFade(schemeJSon, true, true);
        }

        private void DrivenWeightedOutIn(JSon lastJSon, JSon schemeJSon)
        {
            if (lastJSon)
            {
                DrivenWeighted(lastJSon, false, true, () => DrivenWeighted(schemeJSon, true, true));
                return;
            }

            DrivenFade(schemeJSon, true, true);
        }

        #endregion

        // Calculus
        #region Calculus

        private void Fade(JSon schemeJSon, bool isFadeIn, bool fadeAmbient, Action callback = null)
        {
            List<Light> lights;
            if (isFadeIn)
                lights = CreateSystem(schemeJSon);
            else
                lights = GetCurrentLights();

            List<float> intensities = new List<float>();
            foreach (Light li in lights)
                intensities.Add(li.intensity);

            DynamicListener.Timeline(EaseType.QuadraticEaseInOut, schemeJSon.GetFloatValue("AnimTime"), (float alpha) =>
            {
                float mappedAlpha = alpha;

                if (!isFadeIn)
                    mappedAlpha = 1 - alpha;

                // Adapt lights intensity
                for (int i = 0; i < lights.Count; i++)
                    if (lights[i] != null)
                        lights[i].intensity = mappedAlpha * intensities[i];

                // Adapt ambient light
                if (fadeAmbient)
                {
                    RenderSettings.ambientLight =
                        Color.Lerp(darkness, schemeJSon.GetColorValue("AmbientColor", false), mappedAlpha);
                }
            }, () =>
            {
                if (!isFadeIn)
                    RemoveSystem(lights);

                if (callback != null)
                    callback.Invoke();
            });
        }

        private void DrivenFade(JSon schemeJSon, bool isFadeIn, bool fadeAmbient, Action callback = null)
        {
            List<Light> lights;
            if (isFadeIn)
                lights = CreateSystem(schemeJSon);
            else
                lights = GetCurrentLights();

            List<float> intensities = new List<float>();
            foreach (Light li in lights)
                intensities.Add(li.intensity);

            // Driven fade may not update intensity every frame, so when fading in all lights are set to 0 intensity
            if (isFadeIn)
            {
                for (int i = 0; i < lights.Count; i++)
                    lights[i].intensity = 0;
            }

            DynamicListener.Timeline(EaseType.QuadraticEaseInOut, schemeJSon.GetFloatValue("AnimTime"), (float alpha) =>
            {
                float mappedAlpha = alpha;

                if (!isFadeIn)
                    mappedAlpha = 1 - alpha;

                // Adapt lights intensity
                for (int i = 0; i < lights.Count; i++)
                {
                    JSon lightJSon = schemeJSon["Lights"][lights[i].name];
                    float initFade = lightJSon.GetFloatValue("initFade");
                    float endFade = lightJSon.GetFloatValue("endFade");

                    if (mappedAlpha > initFade && mappedAlpha < endFade)
                    {
                        float remappedAlpha = MathExt.Remap(mappedAlpha, initFade, endFade, 0, 1);
                        lights[i].intensity = remappedAlpha * intensities[i];
                    }
                }

                // Adapt ambient light
                if (fadeAmbient)
                {
                    RenderSettings.ambientLight = 
                        Color.Lerp(darkness, schemeJSon.GetColorValue("AmbientColor", false), mappedAlpha);
                }
            }, () =>
            {
                if (!isFadeIn)
                    RemoveSystem(lights);

                if (callback != null)
                    callback.Invoke();
            });
        }


        private void DrivenWeighted(JSon schemeJSon, bool isFadeIn, bool fadeAmbient, Action callback = null)
        {
            List<Light> lights;
            if (isFadeIn)
                lights = CreateSystem(schemeJSon);
            else
                lights = GetCurrentLights();

            List<float> intensities = new List<float>();
            float totalIntensity = 0;
            foreach (Light li in lights)
            {
                intensities.Add(li.intensity);
                totalIntensity += li.intensity;
            }

            // Driven fade may not update intensity every frame, so when fading in all lights are set to 0 intensity
            if (isFadeIn)
            {
                for (int i = 0; i < lights.Count; i++)
                    lights[i].intensity = 0;
            }

            DynamicListener.Timeline(EaseType.QuadraticEaseInOut, schemeJSon.GetFloatValue("AnimTime"), (float alpha) =>
            {
                float mappedAlpha = alpha;

                Color initialColor = RenderSettings.ambientLight;

                if (!isFadeIn)
                    mappedAlpha = 1 - alpha;

                // Adapt lights intensity
                for (int i = 0; i < lights.Count; i++)
                {
                    JSon lightJSon = schemeJSon["Lights"][lights[i].name];
                    float initFade = lightJSon.GetFloatValue("initFade");
                    float endFade = lightJSon.GetFloatValue("endFade");

                    if (mappedAlpha > initFade && mappedAlpha < endFade)
                    {
                        float remappedAlpha = MathExt.Remap(mappedAlpha, initFade, endFade, 0, 1);
                        lights[i].intensity = remappedAlpha * intensities[i];
                    }
                }

                // Adapt ambient light
                if (fadeAmbient)
                {
                    float averageLight = 0;
                    foreach (Light li in lights)
                        averageLight += li.intensity;

                    RenderSettings.ambientLight = 
                        Color.Lerp(schemeJSon.GetColorValue("AmbientColor", false), darkness, 1 - averageLight/totalIntensity);
                }

            }, () =>
            {
                if (!isFadeIn)
                    RemoveSystem(lights);

                if (callback != null)
                    callback.Invoke();
            });
        }
        #endregion

        #endregion

    }
}