using System.Drawing;
using UnityEngine;

namespace Unite
{
    public class ImageUtility
    {
        public static Texture2D ConvertBitmap(Bitmap bitMap)
        {
            ImageConverter converter = new ImageConverter();
            byte[] bytes = (byte[])converter.ConvertTo(bitMap, typeof(byte[]));
            Texture2D tex = new Texture2D(bitMap.Width, bitMap.Height);
            tex.LoadImage(bytes);
            return tex;
        }
    }
}
