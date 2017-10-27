using UnityEngine;
using Unite;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public List<GameObject> gos;
    public GameObject plane;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Chronometer chrono = new Chronometer();
            chrono.Start();
            foreach (GameObject go in gos)
                PlanarCut.Cut(go, plane, true);
            chrono.DebugValue();
            chrono.Pause();
        }
    }
}
