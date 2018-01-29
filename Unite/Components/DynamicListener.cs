using System;
using System.Collections;
using UnityEngine;

namespace Unite
{
    class DynamicListener : MonoBehaviour
    {
        // Public Static
        #region Public Static

        public static void CallDelayed(float time, Action action)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.DelayedCoroutine(time, action));
        }
        public static void CallDelayed(Action action)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.DelayedCoroutine(0, action));
        }

        public static void Timeline(EaseType ease, float time, Action<float> action)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.TimelineCoroutine(ease, time, action));
        }
        public static void Timeline(float time, Action<float> action)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.TimelineCoroutine(time, action));
        }

        #endregion

        // Coroutines
        #region Coroutines

        private IEnumerator DelayedCoroutine(float time, Action action)
        {
            if (time <= 0)
                yield return null;
            else
                yield return new WaitForSeconds(time);

            action.Invoke();
        }

        private IEnumerator TimelineCoroutine(EaseType ease, float time, Action<float> action)
        {
            float currentTime = 0;

            while (time > currentTime)
            {
                action.Invoke(Ease.Call(ease, (currentTime / time)));
                currentTime += Time.deltaTime;
                yield return null;
            }
            action.Invoke(1);
        }
        private IEnumerator TimelineCoroutine(float time, Action<float> action)
        {
            float currentTime = 0;

            while (time > currentTime)
            {
                action.Invoke(currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
            action.Invoke(1);
        }

        #endregion

    }
}
