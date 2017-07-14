using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneUIController : UIController
{
    IntroSceneLogicController sceneController;

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

    void OnIntroSceneStateChange(IntroSceneState state)
    {
        switch (state)
        {
            case IntroSceneState.ChangeScene:

                break;
        }
    }

    public void ChangeStateButton(string str)
    {
        IntroSceneState state = (IntroSceneState)System.Enum.Parse(typeof(IntroSceneState), str);
        sceneController.SetSceneState(state);
    }
}
