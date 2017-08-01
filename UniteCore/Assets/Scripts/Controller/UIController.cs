using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class UIController : MonoBehaviour
{
    // Variables
    #region Variables

    protected GameManager GM;

    #endregion

    // Override
    #region Override

    protected virtual void Awake()
	{
		GM = GameManager.Instance;
		GM.eventManager.controllerStateChange += OnControllerStateChange;
		transform.SetParent(GM.transform);

		GM.SetControllerToInitializeScene (this);
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

    #endregion

    // Protected
    #region Protected

    protected abstract void Initialize();

    #endregion

    // Events
    #region Events

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void OnControllerStateChange(ControllerState state)
    {
        switch (state)
        {
            case ControllerState.Initialize:
                Initialize();
                break;
            case ControllerState.Exit:
                Destroy(gameObject);
                GM.uiController = null;
                GM.eventManager.controllerStateChange -= OnControllerStateChange;
                break;
        }
    }

    #endregion

}