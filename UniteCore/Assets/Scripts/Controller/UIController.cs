using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class UIController : MonoBehaviour {

	protected GameManager GM;

	protected virtual void Awake ()
	{
		Debug.Log ("Awake : " + GetType().Name);

		GM = GameManager.Instance;

		//Add a callback for when the controller state changes
		GM.eventManager.controllerStateChange += OnControllerStateChange;

		// Add to GM as a child & pass the controller reference
		transform.SetParent(GM.transform);
		GM.SetControllerToInitializeScene (this);
	}

	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
	}

	void OnEnable()
	{
		//Tell our ‘OnSceneFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	void OnControllerStateChange (ControllerState state)
	{
		switch (state)
		{
		case ControllerState.Initialize:

			Initialize();

			break;

		case ControllerState.Exit:

			// Remove child from GM to erase when scene changes it dosent work anymore with DontDestroyOnLoad
//			transform.SetParent(null);

//			transform.SetParent(Camera.main.transform);
			Destroy (gameObject);

			GM.uiController = null;

			//Remove the controller state callback on exit scene
			GM.eventManager.controllerStateChange -= OnControllerStateChange;

			break;
		}
	}

	protected abstract void Initialize();
}
