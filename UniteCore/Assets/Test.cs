using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeSceneLoad()
    {
        foreach (Test test in FindObjectsOfType<Test>())
            if (test)
                Debug.Log(test.name);
    }

    void Awake()
    {
        Debug.Log("Awake");
    }

	void Start ()
    {
        Debug.Log("Start");
	}
}
