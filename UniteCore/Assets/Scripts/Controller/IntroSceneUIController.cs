using UnityEngine;

public class IntroSceneUIController : UIController
{
    // Variables
    #region Variables

    private IntroSceneLogicController sceneController;

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
        sceneController = GM.sceneController as IntroSceneLogicController;
    }

    #endregion

    // Public
    #region Public

    public void ChangeStateButton(string str)
    {
        IntroSceneState state = (IntroSceneState)System.Enum.Parse(typeof(IntroSceneState), str);
        sceneController.SetSceneState(state);
    }

    #endregion

    // Events
    #region Events

    void OnIntroSceneStateChange(IntroSceneState state)
    {
        switch (state)
        {
            case IntroSceneState.ChangeScene:

                break;
        }
    }

    #endregion

}
