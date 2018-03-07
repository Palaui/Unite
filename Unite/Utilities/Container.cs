using UnityEngine;

namespace Unite
{
    public static class Container
    {
        public static GameObject GetContainer()
        {
            if (Radar.Contains("Unite Container"))
                return Radar.Get("Unite Container");
            else
            {
                GameObject container = new GameObject("Unite Container");
                container.AddComponent<Signal>().UpdateElement("Unite Container");
                Object.DontDestroyOnLoad(container);
                return container;
            }
        }
    }
}
