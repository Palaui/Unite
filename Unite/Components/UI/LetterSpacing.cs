using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace Unite
{
    [AddComponentMenu("UI/Effects/Letter Spacing", 15)]
    [RequireComponent(typeof(Text))]
    public class LetterSpacing : BaseMeshEffect
    {
        // Variables
        #region Variables

        private const string SupportedTagRegexPattersn = @"<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>";

        [SerializeField]
        private float spacing = 0f;
        [SerializeField]
        private bool useRichText;

        #endregion

        // Properties
        #region Properties

        public float Spacing
        {
            get { return spacing; }
            set
            {
                if (spacing == value)
                    return;
                spacing = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        #endregion

        // Override
        #region Override

        protected LetterSpacing() { }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            Spacing = spacing;
            base.OnValidate();
        }

#endif

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            List<UIVertex> list = new List<UIVertex>();
            vh.GetUIVertexStream(list);

            ModifyVertices(list);

            vh.Clear();
            vh.AddUIVertexTriangleStream(list);
        }

        #endregion

        // Public
        #region Public

        public void ModifyVertices(List<UIVertex> verts)
        {
            if (!IsActive())
                return;

            Text text = GetComponent<Text>();
            if (text == null)
            {
                Debug.LogWarning("LetterSpacing: Missing Text component");
                return;
            }

            IEnumerator matchedTagCollection = null;
            IList<UILineInfo> lineInfos = text.cachedTextGenerator.lines;

            Match currentMatchedTag = null;
            Vector3 pos;

            string str = text.text;
            float letterOffset = Spacing * text.fontSize / 100f;
            float alignmentFactor = 0;
            int glyphIdx = 0;
            bool isRichText = useRichText && text.supportRichText;

            for (int i = lineInfos.Count - 1; i > 0; i--)
            {
                str = str.Insert(lineInfos[i].startCharIdx, "\n");
                str = str.Remove(lineInfos[i].startCharIdx - 1, 1);
            }

            switch (text.alignment)
            {
                case TextAnchor.LowerLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.UpperLeft:
                    alignmentFactor = 0f;
                    break;

                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                    alignmentFactor = 0.5f;
                    break;

                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                    alignmentFactor = 1f;
                    break;
            }

            string[] lines = str.Split('\n');
            for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
            {
                string line = lines[lineIdx];
                float lineOffset = (line.Length - 1) * letterOffset * alignmentFactor;
                int lineLength = line.Length;

                if (isRichText)
                {
                    matchedTagCollection = GetRegexMatchedTagCollection(line, out lineLength);
                    currentMatchedTag = null;
                    if (matchedTagCollection.MoveNext())
                        currentMatchedTag = (Match)matchedTagCollection.Current;
                }

                for (int charIdx = 0, actualCharIndex = 0; charIdx < line.Length; charIdx++, actualCharIndex++)
                {
                    if (isRichText)
                    {
                        if (currentMatchedTag != null && currentMatchedTag.Index == charIdx)
                        {
                            // Skip matched RichText tag
                            charIdx += currentMatchedTag.Length - 1;
                            actualCharIndex--;
                            glyphIdx += currentMatchedTag.Length;

                            // Prepare next tag to detect
                            currentMatchedTag = null;
                            if (matchedTagCollection.MoveNext())
                                currentMatchedTag = (Match)matchedTagCollection.Current;

                            continue;
                        }
                    }

                    int idx1 = glyphIdx * 6 + 0;
                    int idx2 = glyphIdx * 6 + 1;
                    int idx3 = glyphIdx * 6 + 2;
                    int idx4 = glyphIdx * 6 + 3;
                    int idx5 = glyphIdx * 6 + 4;
                    int idx6 = glyphIdx * 6 + 5;

                    if (idx6 > verts.Count - 1)
                        return;

                    UIVertex vert1 = verts[idx1];
                    UIVertex vert2 = verts[idx2];
                    UIVertex vert3 = verts[idx3];
                    UIVertex vert4 = verts[idx4];
                    UIVertex vert5 = verts[idx5];
                    UIVertex vert6 = verts[idx6];

                    pos = Vector3.right * (letterOffset * actualCharIndex - lineOffset);

                    vert1.position += pos;
                    vert2.position += pos;
                    vert3.position += pos;
                    vert4.position += pos;
                    vert5.position += pos;
                    vert6.position += pos;

                    verts[idx1] = vert1;
                    verts[idx2] = vert2;
                    verts[idx3] = vert3;
                    verts[idx4] = vert4;
                    verts[idx5] = vert5;
                    verts[idx6] = vert6;

                    glyphIdx++;
                }
                glyphIdx++;
            }
        }

        #endregion

        // Coroutines
        #region Coroutines

        private IEnumerator GetRegexMatchedTagCollection(string line, out int lineLengthWithoutTags)
        {
            MatchCollection matchedTagCollection = Regex.Matches(line, SupportedTagRegexPattersn);
            lineLengthWithoutTags = 0;
            int tagsLength = 0;

            if (matchedTagCollection.Count > 0)
            {
                foreach (Match matchedTag in matchedTagCollection)
                    tagsLength += matchedTag.Length;
            }
            lineLengthWithoutTags = line.Length - tagsLength;
            return matchedTagCollection.GetEnumerator();
        }

        #endregion

    }
}
