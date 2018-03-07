using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEditorInternal;


namespace Unite
{
    public class TimeController : MonoBehaviour
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
            public IEnumerator coroutine;
            public string ID;
            public int frameCount;

            public RecordingInfo(List<Transform> transforms, string key)
            {
                dictionary = new Dictionary<Transform, List<TransformInfo>>();
                foreach (Transform tr in transforms)
                    dictionary.Add(tr, new List<TransformInfo>());

                coroutine = null;
                ID = key;
                frameCount = 0;
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
        public float MaxFrameRecording = 100;




        [Range(0f, 100f)]
        public float recordInterval = 1f;
        [Range(0f, 100f)]
        public float frameSelected = 1f;


        string oneKey;
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


        // Public
        #region Public

        /// <summary> Start Recording the transform values given in arg1 </summary>
        /// <param name="ts"></param>
        /// <param name="key"></param>
        /// <param name="recordingStep"></param>
        public void StartRecording(List<Transform> ts, string key, float recordingStep)
        {
            oneKey = key;
            RecordingInfo info = new RecordingInfo(ts, key);
            IEnumerator coroutine = RecordCoroutine(info, recordingStep);
            info.coroutine = coroutine;
            recordings.Add(key, info);

            StartCoroutine(recordings[key].coroutine);
        }

        /// <summary> Set the recorded transforms to variables of the frame given by arg2</summary>
        /// <param name="key"></param>
        /// <param name="frameSample"></param>
        public void SetTransformFrameValues(string key, float frameTarget)
        {
            int frame;
            RecordingInfo info = recordings[key];
            // frameSelected is a slider to set values easely
            if (frameTarget == 0)
                frame = Mathf.Clamp((int)frameSelected, 0, info.frameCount - 1);
            else
                frame =  Mathf.Clamp((int)frameTarget, 0, info.frameCount - 1);

            foreach (KeyValuePair<Transform, List<TransformInfo>> entry in info.dictionary)
            {
                entry.Key.localPosition = entry.Value[frame].pos;
                entry.Key.localRotation = entry.Value[frame].rot;
                entry.Key.localScale = entry.Value[frame].scale;
            }

        }

        /// <summary> Apply values between 0 & 1 to the amount of frames recorded </summary>
        /// <param name="key"></param>
        /// <param name="f"></param>
        public void SetRecordedTransformsToFrameAmount(string key, float f)
        {
             oneKey = key;
            int f1 = recordings[key].frameCount;
                f1 = Mathf.RoundToInt(f1 * f);
            SetTransformFrameValues(key, f1);

        }

        bool DrawFramesRuning = false;
        public void DrawFramesRecorded()
        {
            if (!DrawFramesRuning)
                StartCoroutine(DrawFrames());
            else
                StopCoroutine(DrawFrames());

            DrawFramesRuning = !DrawFramesRuning;
        }

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

        //IEnumerator RecordTransform(Transform t)
        //{
        //    while (t)
        //    {

        //        Vector3 pos = t.localPosition;
        //        Vector3 rot = t.localRotation.eulerAngles;
        //        Vector3 scale = t.localScale;

        //        TransformInfo ti = new TransformInfo(t.GetInstanceID(),pos,rot,scale);
        //        frame_List_singleObject.Add(ti);
        //    yield return new WaitForSecondsRealtime(RecordInterval);
        //    }
        //    yield return new WaitForEndOfFrame();
        //}

        //IEnumerator RecordMultiTransform(List<Transform> ts)
        //{
        //    yield return new WaitForEndOfFrame();

        //    while (true)
        //    {
        //    List<TransformInfo> frameRecord = new List<TransformInfo>();
        //        for (int i=0;i<ts.Count;i++) {
        //            Vector3 pos = ts[i].localPosition;
        //            Vector3 rot = ts[i].localRotation.eulerAngles;
        //            Vector3 scale = ts[i].localScale;
        //            TransformInfo ti = new TransformInfo(ts[i].GetInstanceID(), pos, rot, scale);
        //            frameRecord.Add(ti);
        //        }

        //        frame_List_multiObject.Add(frameRecord);
        //        yield return new WaitForSecondsRealtime(RecordInterval);
        //    }
        //}
        /// <summary> Coroutine to record info about certain transforms included in RecordingInfo of arg1.
        /// The frequency of recording can be changed using the RecordInterval variable.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="recordingStep"></param>
        /// <returns></returns>
        IEnumerator RecordCoroutine(RecordingInfo info, float recordingStep)
        {
            while (true)
            {
                foreach (KeyValuePair<Transform, List<TransformInfo>> entry in info.dictionary)
                    entry.Value.Add(GetTransformInfo(entry.Key));

                info.frameCount++;
                yield return new WaitForSecondsRealtime(recordInterval);
            }
        }
        /// <summary>Coroutine to show the amount of frames sampled by the controller</summary>
        /// <returns></returns>
        IEnumerator DrawFrames()
        {
            while (true) { 
                yield return new WaitForEndOfFrame();
                GUI.Label(new Rect(10, 10, 100, 20), recordings[oneKey].frameCount.ToString());
            }
        }
            #endregion
        }
}
