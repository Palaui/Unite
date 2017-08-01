
internal class EventManager
{
    // Event Declaration
    #region Event Delcaration

    public delegate void OnControllerStateChange(ControllerState state);
    public event OnControllerStateChange controllerStateChange;

    public delegate void OnIntroSceneState(IntroSceneState state);
    public event OnIntroSceneState introSceneStateChange;

    public delegate void OnMainSceneState(MainSceneState currentState, MainSceneState newState);
    public event OnMainSceneState mainSceneStateChange;

    public delegate void OnChangeLanguage(Language language);
    public event OnChangeLanguage changeLanguage;

    #endregion

    // Public
    #region Public

    public void ControllerStateChangeEvent(ControllerState state)
    {
        if (controllerStateChange != null)
            controllerStateChange(state);
    }

    public void IntroSceneStateChangeEvent(IntroSceneState state)
    {
        if (introSceneStateChange != null)
            introSceneStateChange(state);
    }

    public void MainSceneStateChangeEvent(MainSceneState currentState, MainSceneState newState)
    {
        if (mainSceneStateChange != null)
            mainSceneStateChange(currentState, newState);
    }

    public void ChangeLanguage(Language language)
    {
        if (changeLanguage != null)
            changeLanguage(language);
    }

    #endregion
}
