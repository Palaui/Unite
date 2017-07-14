using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIController : UIController
{
    MainSceneLogicController sceneController;

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

    void OnMainSceneStateChange(MainSceneState currentstate, MainSceneState newState)
    {
        switch (newState)
        {
            case MainSceneState.ChangeScene:

                break;
        }
    }

	public void ChangeStateButton(string str)
	{
		MainSceneState state = (MainSceneState)System.Enum.Parse(typeof(MainSceneState), str);
		sceneController.SetSceneState(state);
	}
}
