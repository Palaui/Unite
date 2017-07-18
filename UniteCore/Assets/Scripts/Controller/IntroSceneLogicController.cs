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
        GM.eventManager.introSceneStateChange += OnIntroSceneStateChange;
        uiController = GM.uiController as IntroSceneUIController;
    }

    #endregion

    // Public
    #region Public

    public void SetSceneState(IntroSceneState state)
    {
        GM.eventManager.IntroSceneStateChangeEvent(state);
    }

    #endregion

    // Events
    #region Events

    void OnIntroSceneStateChange(IntroSceneState state)
    {
        switch (state)
        {
            case IntroSceneState.ChangeScene:
                GM.eventManager.introSceneStateChange -= OnIntroSceneStateChange;
                GM.ChangeToScene(GameScene.MainScene);
                break;
        }

        sceneState = state;
    }

    #endregion

}
