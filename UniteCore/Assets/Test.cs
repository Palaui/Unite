using UnityEngine;
using Unite;

public class Test : MonoBehaviour
{
    public GameObject go;
    public GameObject plane;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PlanarCut.Cut(go, plane);
    }
}
