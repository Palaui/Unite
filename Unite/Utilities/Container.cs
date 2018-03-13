using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unite
{
    public static class Container
    {
        // Variables
        #region Variables

        public static GameObject container;

        #endregion

        // Public Static
        #region Public Static

        public static GameObject GetContainer()
        {
            if (container)
                return container;
            else
            {
                Scene activeScene = SceneManager.GetActiveScene();
                SceneManager.SetActiveScene(SceneManager.CreateScene("Unite"));
                container = new GameObject("Unite Container");
                DynamicListener.CallDelayed(false, () => SceneManager.SetActiveScene(activeScene));

                return container;
            }
        }

        public static T GetComponent<T>() where T : Component
        {
            container = GetContainer();
            return Ext.GetOrAddComponent<T>(container);
        }

        public static void RemoveComponent<T>() where T : Component
        {
            container = GetContainer();
            if (container.GetComponent<T>())
                Object.DestroyImmediate(container.GetComponent<T>());
        }

        #endregion

    }
}
