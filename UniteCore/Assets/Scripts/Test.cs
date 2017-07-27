using System.Collections;
using System.Collections.Generic;
using Unite;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string expression;
    public float value;
    public bool apply = false;
    public bool printValue = false;

    private List<GameObject> gos = new List<GameObject>();
    private List<Vector2> points = new List<Vector2>();
    private Expression ex;

    void Awake()
    {
        ex = new Expression("");
    }
    void Update()
    {
        if (apply)
        {
            ex.AssignExpression(expression);
            points = ex.GetGraphicPoints(new Vector2(-20, 20), 1);

            foreach (GameObject go in gos)
                Destroy(go, 0.1f);
            gos.Clear();

            Ext.ApplyForeach(CreateCube, points);

            apply = false;
        }

        if (printValue)
        {
            ex.AssignExpression(expression);
            ex.AddOrChangeParameter("x", value);
            Debug.Log("Value = " + ex.Solve());
            printValue = false;
        }
    }

    private void CreateCube(Vector2 pos)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(pos.x, pos.y, 0);
        gos.Add(go);
    }
}
