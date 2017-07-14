using UnityEngine;

public class MainSceneUIController : UIController
{
    // Variables
    #region Variables

    private MainSceneLogicController sceneController;

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

        sceneController = GM.sceneController as MainSceneLogicController;
    }

    #endregion

    // Public
    #region Public

    void OnMainSceneStateChange(MainSceneState currentstate, MainSceneState newState)
    {
        switch (newState)
        {
            case MainSceneState.ChangeScene:

                break;
        }
    }

    #endregion

    // Events
    #region Events

    public void ChangeStateButton(string str)
	{
		MainSceneState state = (MainSceneState)System.Enum.Parse(typeof(MainSceneState), str);
		sceneController.SetSceneState(state);
	}

    #endregion

}
