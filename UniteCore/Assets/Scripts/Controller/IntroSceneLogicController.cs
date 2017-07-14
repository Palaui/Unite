using UnityEngine;

// Enums
#region Enums

public enum IntroSceneState
{
    None,
    ChangeScene
}

#endregion

public class IntroSceneLogicController : SceneController
{
    // Variables
    #region Variables

    [SerializeField]
    private IntroSceneState sceneState;
    private IntroSceneUIController uiController;

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
        GM.eventManager.introSceneStateChange += OnIntroSceneStateChange;
        uiController = GM.uiController as IntroSceneUIController;
    }

    #endregion

    // Public
    #region Public

    public void SetSceneState(IntroSceneState state)
    {
        // Handles the scene state change from the GM.gameEvent
        GM.eventManager.IntroSceneStateChangeEvent(state);
        sceneState = state;
    }

    #endregion

    // Events
    #region Events

    void OnIntroSceneStateChange(IntroSceneState state)
    {
        switch (state)
        {
            case IntroSceneState.ChangeScene:
                // Remove the scene state callback on exit scene
                GM.eventManager.introSceneStateChange -= OnIntroSceneStateChange;
                // Change to next Scene
                GM.ChangeToScene(GameScene.MainScene);
                break;
        }
    }

    #endregion

}
