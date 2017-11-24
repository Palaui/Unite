using System.Drawing;
using UnityEngine;

namespace Unite
{
    public class ImageUtility
    {
        public static Texture2D BitmapToTexture2D(Bitmap bitMap)
        {
            Texture2D tex = new Texture2D(bitMap.Width, bitMap.Height);
            System.Drawing.Color col;
            for (int i = 0; i < bitMap.Width; i++)
            {
                for (int j = 0; j < bitMap.Width; j++)
                {
                    col = bitMap.GetPixel(i, j);
                    tex.SetPixel(i, j, new Color32(col.R, col.G, col.B, col.A));
                }
            }
            return tex;
        }
    }
}
