using UnityEngine;

// Enums
#region Enums

public enum MainSceneState
{
    None,
    ChangeScene
}

#endregion

public class MainSceneLogicController : SceneController
{
    // Variables
    #region variables

    [SerializeField]
    private MainSceneState sceneState;
    private MainSceneUIController uiController;

    #endregion

    // Override
    #region Override

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Initialize()
    {
        Debug.Log("Initialize : " + GetType().Name);
        GM.eventManager.mainSceneStateChange += OnMainSceneStateChange;
        uiController = GM.uiController as MainSceneUIController;
    }

    #endregion

    // Public
    #region Public

    public void SetSceneState(MainSceneState state)
    {
        // Handles the scene state change from the GM.gameEvent
        GM.eventManager.MainSceneStateChangeEvent(sceneState, state);
        sceneState = state;
    }

    #endregion

    // Events
    #region Events

    void OnMainSceneStateChange(MainSceneState currentState, MainSceneState newState)
    {
        switch (newState)
        {
            case MainSceneState.ChangeScene:
                // Remove the scene state callback on exit scene
                GM.eventManager.mainSceneStateChange -= OnMainSceneStateChange;
                // Change to next Scene
                GM.ChangeToScene(GameScene.IntroScene);
                break;
        }
    }

    #endregion

}
