using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IntroSceneState
{
    None,
    ChangeScene
}

public class IntroSceneLogicController : SceneController
{
    // Only Serialize for Testing
    [SerializeField]
    IntroSceneState sceneState;

    IntroSceneUIController uiController;

    protected override void Awake()
    {
        base.Awake();
    }

    // Use Initialize as a pre-Start method
    protected override void Initialize()
    {
        Debug.Log("Initialize : " + GetType().Name);

        // Add a callback for when the scene state changes
        GM.eventManager.introSceneStateChange += OnIntroSceneStateChange;

        uiController = GM.uiController as IntroSceneUIController;
    }

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

    public void SetSceneState(IntroSceneState state)
    {
        // Handles the scene state change from the GM.gameEvent
        GM.eventManager.IntroSceneStateChangeEvent(state);
        sceneState = state;
    }
}
