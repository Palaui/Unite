using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Add all necessary states for ScenesController
internal enum ControllerState
{
	None,
	Initialize,
	InScene,
	Exit
}

public abstract class SceneController : MonoBehaviour {

	protected GameManager GM;

	protected Scene currentScene;

	//Only Serialize for Testing
	[SerializeField]
	ControllerState controllerState;

	protected virtual void Awake ()
	{
		Debug.Log ("Awake : " + GetType().Name);

		GM = GameManager.Instance;

		//Add a callback for when the controller state changes
		GM.eventManager.controllerStateChange += OnControllerStateChange;

		// Add to GM as a child
		transform.SetParent(GM.transform);

		// SET the controller reference to Initialize scene from GM
		GM.SetControllerToInitializeScene (this);
	}

	//This is called each time a scene is loaded.
	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		currentScene = scene;

		gameObject.name = currentScene.name;

		//Tell our ‘OnSceneFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
	}

	void OnEnable()
	{
		//Tell our ‘OnSceneFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	void OnControllerStateChange (ControllerState state)
	{
		switch (state) {

		case ControllerState.Initialize:

			Initialize();

			break;

		case ControllerState.InScene:


			break;

		case ControllerState.Exit:

			// Remove child from GM to erase when scene changes it dosent work anymore with DontDestroyOnLoad
//			transform.SetParent(null);

//			transform.SetParent(Camera.main.transform);
			Destroy (gameObject);

			GM.sceneController = null;

			//Remove the controller state callback on exit scene
			GM.eventManager.controllerStateChange -= OnControllerStateChange;

			break;
		}
	}


	internal void SetControllerState (ControllerState state)
	{		
		//Handles the controller state change from the GM.gameEvent
		GM.eventManager.ControllerStateChangeEvent(state);
		controllerState = state;
	}


	//Use Initialize as a pre-start method
	protected abstract void Initialize ();
}
