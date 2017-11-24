using UnityEngine;

namespace Unite
{
    public class MaterialUtility
    {
        public enum RenderMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        public static void ChangeRenderMode(Material mat, RenderMode blendMode)
        {
            switch (blendMode)
            {
                case RenderMode.Opaque:
                    mat.SetInt("_Mode", 0);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = -1;
                    break;
                case RenderMode.Cutout:
                    mat.SetInt("_Mode", 1);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.EnableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 2450;
                    break;
                case RenderMode.Fade:
                    mat.SetInt("_Mode", 2);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    break;
                case RenderMode.Transparent:
                    mat.SetInt("_Mode", 3);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    break;
            }
        }

        public static void ChangeColorWithoutAlpha(Material mat, Color32 color)
        {
            Color32 alpha = mat.GetColor("_Color");
            mat.SetColor("_Color", new Color32(color.r, color.g, color.b, alpha.a));
        }

        public static void ChangeAlpha(Material mat, byte alpha)
        {
            Color32 color = mat.GetColor("_Color");
            mat.SetColor("_Color", new Color32(color.r, color.g, color.b, alpha));
        }
    }
}
