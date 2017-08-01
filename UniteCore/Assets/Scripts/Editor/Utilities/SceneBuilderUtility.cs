using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unite;

public class SceneBuilderUtility
{
    [MenuItem("Custom/Scene/AssureBasiBuilding")]
    public static void AssureBasicBuilding()
    {
        GameObject sceneController = null;
        GameObject uiController = null;
		GameObject canvas = null;
		GameObject eventSystem = null;

        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            if (go)
            {
                if (go.name == "SceneController")
                    sceneController = go;
                if (go.name == "UIController")
                    uiController = go;
				if (go.name == "Canvas")
					canvas = go;
				if (go.name == "EventSystem")
					eventSystem = go;
            }
        }

        if (!sceneController)
        {
            sceneController = new GameObject();
            sceneController.name = "SceneController";
            Debug.Log("Remember to add a script to the SceneController");
        }

        if (!uiController)
        {
            uiController = new GameObject();
            uiController.name = "UIController";
            Debug.Log("Remember to add a script to the UIController");
        }

		if (!canvas)
		{
			canvas = new GameObject();
			canvas.name = "Canvas";
		}

		Ext.GetOrAddComponent(canvas, new System.Type[]{ typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster) });
		canvas.transform.SetParent(uiController.transform);
		canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

		if (!eventSystem)
		{
			eventSystem = new GameObject();
			eventSystem.name = "EventSystem";
		}

		Ext.GetOrAddComponent(eventSystem, new System.Type[]{ typeof(EventSystem), typeof(StandaloneInputModule)});
		eventSystem.transform.SetParent(uiController.transform);
		Ext.ResetTransform(eventSystem, true);

	}
}
