using UnityEngine;
using UnityEngine.SceneManagement;

// Enums
#region Enums

internal enum ControllerState
{
	None,
	Initialize,
	InScene,
	Exit
}

#endregion

public abstract class SceneController : MonoBehaviour
{
    // Variables
    #region Variables

    protected GameManager GM;
	protected Scene currentScene;

	[SerializeField]
	private ControllerState controllerState;

    #endregion

    // Override
    #region Override

    protected virtual void Awake()
	{
		Debug.Log ("Awake : " + GetType().Name);
		GM = GameManager.Instance;
		GM.eventManager.controllerStateChange += OnControllerStateChange;
		transform.SetParent(GM.transform);

		// Set the controller reference to Initialize scene from GM
		GM.SetControllerToInitializeScene(this);
	}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    #endregion

    // protected
    #region Protected

    protected abstract void Initialize();

    #endregion

    // Private
    #region Private

    internal void SetControllerState (ControllerState state)
	{		
		GM.eventManager.ControllerStateChangeEvent(state);
		controllerState = state;
	}

    #endregion

    // Events
    #region Events

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		currentScene = scene;
		gameObject.name = currentScene.name;
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
	}

	void OnControllerStateChange(ControllerState state)
	{
		switch (state)
        {
		case ControllerState.Initialize:
			Initialize();
			break;
		case ControllerState.InScene:
			break;
		case ControllerState.Exit:
			Destroy (gameObject);
			GM.sceneController = null;
			GM.eventManager.controllerStateChange -= OnControllerStateChange;
			break;
		}
	}

    #endregion

}
