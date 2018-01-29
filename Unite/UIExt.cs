using UnityEngine;
using UnityEngine.UI;

namespace Unite
{
    public class UIExt
    {
        /// <summary> Gets the screen rectangle occupied by a rectTransform under certain camera. </summary>
        /// <param name="rectTransform"> RectTransform to get the related rect from. </param>
        /// <param name="camera"> Reference camera. </param>
        /// <returns></returns>
        public static Rect GetRectTransformScreenRect(RectTransform rectTransform, Camera camera)
        {
            Rect rect = new Rect();
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            
            int min = Mathf.RoundToInt(camera.WorldToViewportPoint(corners[0]).x * Screen.width);
            int max = Mathf.RoundToInt(camera.WorldToViewportPoint(corners[2]).x * Screen.width);

            rect.x = Mathf.RoundToInt(camera.WorldToViewportPoint(corners[0]).x * Screen.width);
            rect.y = Mathf.RoundToInt(camera.WorldToViewportPoint(corners[0]).y * Screen.height);
            rect.width = Mathf.RoundToInt(camera.WorldToViewportPoint(corners[2]).x * Screen.width) - rect.x;
            rect.height = Mathf.RoundToInt(camera.WorldToViewportPoint(corners[2]).y * Screen.height) - rect.y;

            return rect;
        }

        /// <summary> Get the coeficient between a camera and a canvas viewport resolution. </summary>
        /// <param name="canvasScaler"> Canvas scaler to get the reference from. </param>
        /// <returns></returns>
        public static Vector2 GetUIScreenCoeficient(CanvasScaler canvasScaler)
        {
            return 
                new Vector2(canvasScaler.referenceResolution.x / Screen.width, canvasScaler.referenceResolution.y / Screen.height);
        }
    }
}
