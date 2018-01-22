using UnityEngine;
using UnityEngine.UI;

namespace Unite
{
    public class UIExt
    {
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

        public static float GetUIScreenCoeficient(CanvasScaler canvasScaler)
        {
            return canvasScaler.referenceResolution.x / Screen.width;
        }
    }
}
