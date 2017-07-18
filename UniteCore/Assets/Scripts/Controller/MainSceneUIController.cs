
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
        GM.eventManager.mainSceneStateChange += OnMainSceneStateChange;
        sceneController = GM.sceneController as MainSceneLogicController;
    }

    #endregion

    // Public
    #region Public

    public void ChangeStateButton(string str)
	{
		MainSceneState state = (MainSceneState)System.Enum.Parse(typeof(MainSceneState), str);
		sceneController.SetSceneState(state);
	}

    #endregion

    // Events
    #region Events

    void OnMainSceneStateChange(MainSceneState currentstate, MainSceneState newState)
    {
        
    }

    #endregion

}
