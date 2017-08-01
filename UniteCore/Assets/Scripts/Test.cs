using System.Collections;
using System.Collections.Generic;
using Unite;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Awake()
    {
        Vector2 a = new Vector2(-4, 2);
        Vector2 b = new Vector2(-1, 1);
        Vector2 c = new Vector2(1, 3);
        Vector2 d = new Vector2(2, 1);

        float[] pointsY = new float[] { a.y, b.y, c.y, d.y };

        Matrix matrix = new Matrix(4, 4);
        matrix.FillRow(new float[] { Mathf.Pow(a.x, 3), Mathf.Pow(a.x, 2), Mathf.Pow(a.x, 1), 1 }, 0);
        matrix.FillRow(new float[] { Mathf.Pow(b.x, 3), Mathf.Pow(b.x, 2), Mathf.Pow(b.x, 1), 1 }, 1);
        matrix.FillRow(new float[] { Mathf.Pow(c.x, 3), Mathf.Pow(c.x, 2), Mathf.Pow(c.x, 1), 1 }, 2);
        matrix.FillRow(new float[] { Mathf.Pow(d.x, 3), Mathf.Pow(d.x, 2), Mathf.Pow(d.x, 1), 1 }, 3);

        float[] sol = matrix.Solve(pointsY);

        foreach (float s in sol)
            Debug.Log(s);
    }

    void Update()
    {

    }

}
