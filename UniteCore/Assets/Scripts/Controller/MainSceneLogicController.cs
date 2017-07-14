using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainSceneState
{
    None,
    ChangeScene
}

public class MainSceneLogicController : SceneController
{
    // Only Serialize for Testing
    [SerializeField]
    MainSceneState sceneState;

    MainSceneUIController uiController;

    protected override void Awake()
    {
        base.Awake();
    }

    // Use Initialize as a pre-Start method
    protected override void Initialize()
    {
        Debug.Log("Initialize : " + GetType().Name);

        // Add a callback for when the scene state changes
        GM.eventManager.mainSceneStateChange += OnMainSceneStateChange;

        uiController = GM.uiController as MainSceneUIController;
    }

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

    public void SetSceneState(MainSceneState state)
    {
        // Handles the scene state change from the GM.gameEvent
        GM.eventManager.MainSceneStateChangeEvent(sceneState, state);
        sceneState = state;
    }
}
