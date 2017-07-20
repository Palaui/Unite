using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Pool
    {
        // Variables
        #region Variables

        private List<GameObject> list = new List<GameObject>();
        private GameObject[] references;
        private int capacity;

        #endregion

        // Override
        #region Override

        public Pool(GameObject go, int capacity = 10)
        {
            references = new GameObject[] { go };
            this.capacity = capacity;
            AdaptPool();
        }

        public Pool(GameObject[] gos, int capacity = 10)
        {
            references = gos;
            this.capacity = capacity;
            AdaptPool();
        }

        ~Pool()
        {
            foreach (GameObject go in list)
                Object.Destroy(go, 0.035f);
        }

        #endregion

        // Public
        #region Public

        public GameObject Next()
        {
            GameObject go = GetRandomItem();
            list.Add(go);
            if (list.Count > capacity)
            {
                GameObject goToDelete = list[0];
                list.RemoveAt(0);
                Object.Destroy(goToDelete);
            }

            return go;
        }

        public void Resize(int newCapacity, bool mantainIndex)
        {
            capacity = newCapacity;
            AdaptPool();
        }

        #endregion

        // Private
        #region Private

        private GameObject GetRandomItem()
        {
            if (references.Length == 1)
                return references[0];

            int rand = Random.Range(0, references.Length);
            return references[rand];
        }

        private void AdaptPool()
        {
            while (list.Count > capacity)
                list.RemoveAt(0);

            if (capacity < 1)
                capacity = 1;
        }

        #endregion

    }
}
