using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class EventManager
{
    // SceneController State Change Event
    public delegate void OnControllerStateChange(ControllerState state);
    public event OnControllerStateChange controllerStateChange;

    public void ControllerStateChangeEvent(ControllerState state)
    {
        if (controllerStateChange != null)
        {

            controllerStateChange(state);
        }
    }

    // Intro Scene State Change Event
    public delegate void OnIntroSceneState(IntroSceneState state);
    public event OnIntroSceneState introSceneStateChange;

    public void IntroSceneStateChangeEvent(IntroSceneState state)
    {
        if (introSceneStateChange != null)
        {

            introSceneStateChange(state);
        }
    }

    // Main Scene State Change Event
    public delegate void OnMainSceneState(MainSceneState currentState, MainSceneState newState);
    public event OnMainSceneState mainSceneStateChange;

    public void MainSceneStateChangeEvent(MainSceneState currentState, MainSceneState newState)
    {
        if (mainSceneStateChange != null)
        {
            mainSceneStateChange(currentState, newState);
        }
    }

    // Language State
    public delegate void OnChangeLanguage(Language language);
    public event OnChangeLanguage changeLanguage;

    public void ChangeLanguage(Language language)
    {
        if (changeLanguage != null)
        {
            changeLanguage(language);
        }
    }
}
