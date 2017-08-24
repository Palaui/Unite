using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBehaviour : MonoBehaviour
{
    private float currentTIme;

    void Awake()
    {
        transform.position = Vector3.right * (float)Test.displacement;
        currentTIme = 0;
    }

	void Update()
    {
        transform.position = Vector3.right * (float)Test.displacement;
        currentTIme += Time.deltaTime;
        if (currentTIme > 8.5f)
            Destroy(gameObject, 0);
    }
}
