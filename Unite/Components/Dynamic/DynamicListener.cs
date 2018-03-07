using System;
using System.Collections;
using UnityEngine;

namespace Unite
{
    public class DynamicListener : MonoBehaviour
    {
        // Public Static
        #region Public Static

        public static void CallDelayed(float time, bool callOnFrameEnd, Action action)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.DelayedCoroutine(time, callOnFrameEnd, action));
        }
        public static void CallDelayed(bool callOnFrameEnd, Action action)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.DelayedCoroutine(0, callOnFrameEnd, action));
        }
        public static void CallDelayed(Action action)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.DelayedCoroutine(0, false, action));
        }

        public static void Timeline(EaseType ease, float time, Action<float> action, Action callback = null)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.TimelineCoroutine(ease, time, action, callback));
        }
        public static void Timeline(float time, Action<float> action, Action callback = null)
        {
            DynamicListener instance = Ext.GetOrAddComponent<DynamicListener>(Container.GetContainer());
            instance.StartCoroutine(instance.TimelineCoroutine(EaseType.Linear, time, action, callback));
        }

        #endregion

        // Coroutines
        #region Coroutines

        private IEnumerator DelayedCoroutine(float time, bool callOnFrameEnd, Action action)
        {
            if (time <= 0)
                yield return null;
            else
                yield return new WaitForSeconds(time);

            if (callOnFrameEnd)
                yield return new WaitForEndOfFrame();

            action.Invoke();
        }

        private IEnumerator TimelineCoroutine(EaseType ease, float time, Action<float> action, Action callback = null)
        {
            float currentTime = 0;

            while (time > currentTime)
            {
                action.Invoke(Ease.Call(ease, (currentTime / time)));
                currentTime += Time.deltaTime;
                yield return null;
            }
            action.Invoke(1);

            if (callback != null)
                callback.Invoke();
        }

        #endregion

    }
}
