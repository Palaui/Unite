using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Unite
{
    public class DynamicRecorder : MonoBehaviour
    {
        // Structs
        #region Structs

        private struct ActionStruct
        {
            public Action action;
            public bool hasBeenUsed;

            public ActionStruct(Action action, bool hasBeenUsed)
            {
                this.action = action;
                this.hasBeenUsed = hasBeenUsed;
            }
        }

        private class RecordingInfo
        {
            public Dictionary<Transform, List<TransformInfo>> dictionary;
            public IEnumerator coroutine = null;

            public string ID;
            public float length = 0;
            public float currentTime = 0;
            public int frameCount = 0;
            public bool isRecording = false;

            public RecordingInfo(List<Transform> transforms, string key)
            {
                dictionary = new Dictionary<Transform, List<TransformInfo>>();
                foreach (Transform tr in transforms)
                    dictionary.Add(tr, new List<TransformInfo>());
                ID = key;
            }
        }

        private class TransformInfo
        {
            public Vector3 pos;
            public Quaternion rot;
            public Vector3 scale;
            
            public TransformInfo(Vector3 pos, Quaternion rot, Vector3 scale)
            {
                this.pos = pos;
                this.rot = rot;
                this.scale = scale;
            }
        }

        #endregion

        // Variables
        #region Variables

        private static Dictionary<string, ActionStruct> methods = new Dictionary<string, ActionStruct>();
        private static Dictionary<string, RecordingInfo> recordings = new Dictionary<string, RecordingInfo>();

        #endregion

        // Override
        #region Override

        void Update()
        {
            DynamicListener.CallDelayed(true, () => 
            {
                foreach (KeyValuePair<string, ActionStruct> entry in methods)
                    methods[entry.Key] = new ActionStruct(entry.Value.action, false);
            });
        }

        #endregion

        // Public Static
        #region Public Static

        // Recording
        #region Recording

        /// <summary> Starts recording a list of transforms. </summary>
        /// <param name="ts"></param>
        /// <param name="key"></param>
        /// <param name="recordingStep"></param>
        public static void StartRecording(List<Transform> ts, string key, float recordingStep)
        {
            DynamicRecorder instance = Container.GetComponent<DynamicRecorder>();

            RecordingInfo info = new RecordingInfo(ts, key);
            IEnumerator coroutine = instance.RecordCoroutine(info, recordingStep);
            info.coroutine = coroutine;
            recordings.Add(key, info);

            instance.StartCoroutine(recordings[key].coroutine);
        }

        public static void StopRecording(string key)
        {
            if (recordings.ContainsKey(key))
            {
                RecordingInfo info = recordings[key];

                if (info.isRecording)
                {
                    Container.GetComponent<DynamicRecorder>().StopCoroutine(info.coroutine);
                    info.isRecording = false;
                }
            }
            else
                Debug.LogError("TimeController StopRecording: Unable to find the specified key, Aborting");
        }

        #endregion

        // Motion
        #region Motion

        public static void Play(string key, float speed, bool loop)
        {
            if (recordings.ContainsKey(key))
            {
                DynamicRecorder instance = Container.GetComponent<DynamicRecorder>();

                StopRecording(key);
                RecordingInfo info = recordings[key];
                instance.StartCoroutine(instance.PlayCoroutine(info, speed, loop));
            }
            else
                Debug.LogError("TimeController Play: Unable to find the specified key, Aborting");
        }

        public static void Pause(string key)
        {
            if (recordings.ContainsKey(key))
            {
                StopRecording(key);
                RecordingInfo info = recordings[key];
                Container.GetComponent<DynamicRecorder>().StopCoroutine(info.coroutine);
            }
            else
                Debug.LogError("TimeController Pause: Unable to find the specified key, Aborting");
        }

        public static void Stop(string key)
        {
            if (recordings.ContainsKey(key))
            {
                StopRecording(key);
                RecordingInfo info = recordings[key];
                Container.GetComponent<DynamicRecorder>().StopCoroutine(info.coroutine);
                info.currentTime = 0;
            }
            else
                Debug.LogError("TimeController Stop: Unable to find the specified key, Aborting");
        }

        public static void SetTime(string key, float time)
        {
            if (recordings.ContainsKey(key))
            {
                RecordingInfo info = recordings[key];
                time = Mathf.Clamp(time, 0, info.length);
                SetTime01(key, time / info.length);
            }
            else
                Debug.LogError("TimeController SetTime: Unable to find the specified key, Aborting");
        }

        public static void SetTime01(string key, float time)
        {
            if (recordings.ContainsKey(key))
            {
                StopRecording(key);
                RecordingInfo info = recordings[key];
                float frameTime = Mathf.Clamp(time, 0, 1) * (info.frameCount - 1);
                float percent = frameTime % 1;

                foreach (KeyValuePair<Transform, List<TransformInfo>> entry in info.dictionary)
                {
                    entry.Key.localPosition = 
                        entry.Value[Mathf.FloorToInt(frameTime)].pos * (1 - percent) + 
                        entry.Value[Mathf.CeilToInt(frameTime)].pos * percent;
                    entry.Key.localEulerAngles =
                        entry.Value[Mathf.FloorToInt(frameTime)].rot.eulerAngles * (1 - percent) +
                        entry.Value[Mathf.CeilToInt(frameTime)].rot.eulerAngles * percent;
                    entry.Key.localScale = 
                        entry.Value[Mathf.FloorToInt(frameTime)].scale * (1 - percent) +
                        entry.Value[Mathf.CeilToInt(frameTime)].scale * percent;
                }
            }
            else
                Debug.LogError("SetTime01 SetTime: Unable to find the specified key, Aborting");
        }

        #endregion

        #endregion

        // Private
        #region Private

        /// <summary> Method to get info abount a transform. </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        private TransformInfo GetTransformInfo(Transform tr)
        {
            return new TransformInfo(tr.localPosition, tr.localRotation, tr.localScale);
        }

        #endregion

        // Public Static
        #region Public Static

        public static void DeclareMethod(Action action, string key)
        {
            if (!methods.ContainsKey(key))
                methods.Add(key, new ActionStruct(action, false));
        }

        public static void Call(string key)
        {
            if (methods.ContainsKey(key))
            {
                if (!methods["key"].hasBeenUsed)
                {
                    methods["key"].action.Invoke();
                    methods["key"] = new ActionStruct(methods["key"].action, true);
                }
            }
        }

        public static bool HasBeenCalled(string key)
        {
            if (methods.ContainsKey(key) && methods[key].hasBeenUsed)
            {
                return true;
            }
            return false;
        }

        #endregion

        // Corroutines
        #region Corroutines

        /// <summary> Record transform states and stores them. </summary>
        /// <param name="info"> Recording data. </param>
        /// <param name="recordingStep"> Recording frequency. </param>
        /// <returns></returns>
        IEnumerator RecordCoroutine(RecordingInfo info, float recordingStep)
        {
            info.isRecording = true;

            while (true)
            {
                foreach (KeyValuePair<Transform, List<TransformInfo>> entry in info.dictionary)
                    entry.Value.Add(GetTransformInfo(entry.Key));
                info.frameCount++;

                yield return new WaitForSecondsRealtime(recordingStep);

                info.length += recordingStep;
            }
        }

        IEnumerator PlayCoroutine(RecordingInfo info, float speed, bool loop)
        {
            while (true)
            {
                yield return null;

                info.currentTime += Time.deltaTime * speed;
                if (info.currentTime > info.length)
                {
                    if (loop)
                        info.currentTime -= info.length;
                    else
                        break;
                }
                else if (info.currentTime < 0)
                {
                    if (loop)
                        info.currentTime += info.length;
                    else
                        break;
                }

                SetTime(info.ID, info.currentTime);
            }
        }

        #endregion

    }
}
