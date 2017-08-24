using System.Collections;
using System.Collections.Generic;
using Unite;
using UnityEngine;
using System;

public class Test : MonoBehaviour
{
    private DoubleV3[] pvPoints;
    private double[,] testPoints;


    private double[] R = new double[7] { 0.003, 0.02, 0.02, 0.1, 0.02, 0.03, 1.2 };
    private double[] cR = new double[7] { 0.003, 0.02, 0.02, 0.1, 0.02, 0.03, 1.2 };
    private double[] C = new double[7] { 30, 2.5, 0, 6, 1.25, 0, 1 };

    private double[][] PV = new double[100000][];
    private double[] T = new double[100000];
    private int ix = 0;
    private double[] PV0 = new double[14] { 3, 3, 10, 15, 15, 20, 60, 0, 48, 130, 0, 48, 110, 0 };

    private double[] cQ = new double[7];
    private double[] dP = new double[7];
    private double[] dV = new double[7];

    private double[] el = new double[4];
    private double[] El = { 2, 3, 2, 6 };
    private double[] del = new double[4];

    private double[] atr_poly = { 0, 0, 100, -500, 625 };
    private double[] ven_poly = { 0, 0, 100, -500, 625 };

    private double[] RV_AlV0 = { 0.5, 1.0 / 40, 50 };
    private double[] LV_AlV0 = { 0.6, 1.0 / 35, 50 };

    private double[] Vd = { 0, 35, 60, 0, 35, 30, 0 };

    private DoubleV3[] last = new DoubleV3[15];
    private DoubleV3[] current = new DoubleV3[15];

    private DoubleV3[] lastPV = new DoubleV3[14];
    private DoubleV3[] currentPV = new DoubleV3[14];

    private DoubleV3 la;
    private DoubleV3 cu;

    private int beatN = 0;
    private double beatLen = 0.8;
    public double beatsPerMinute;

    private double t = 0;

    private double debugT = 0;

    private int drawEveryXFrames = 1;
    private int countFrames = 0;


    // Draw
    public static double displacement = 7.5f;
    private List<DoubleV3>[] linesToDraw = new List<DoubleV3>[9];

    void Awake()
    {
        testPoints = new double[3, 3] { { 0, 1, 0 }, { 2, 3, 0 }, { 3, 4, 0 } };
        pvPoints = new DoubleV3[testPoints.GetLength(0)];
        beatsPerMinute = 60 / beatLen;

        for (int i = 0; i < testPoints.GetLength(0); i++)
            pvPoints[i] = new DoubleV3((float)testPoints[i, 0], (float)testPoints[i, 1]);

        for (int i = 0; i < linesToDraw.Length; i++)
            linesToDraw[i] = new List<DoubleV3>();
    }

    private void Start()
    {
        PV[0] = PV0;
        PV[0][1] = (PV[0][1 + 7] - Vd[1])/C[1];
        PV[0][2] = EDP(PV[0][2 + 7], RV_AlV0);
        PV[0][4] = (PV[0][4 + 7] - Vd[4])/C[4];
        PV[0][5] = EDP(PV[0][5 + 7], LV_AlV0);

        for (int i=1; i<PV.Length;i++)
            PV[i] = new double[14];

        for (int i = 0; i < last.Length - 1; i++)
        {
            last[i] = new DoubleV3(-5, (float)PV[ix][i] / 200);
        }
        last[14] = new DoubleV3(-5, (float)Eval_poly(ven_poly, 0, 0, 0.4)-2);

        for (int i = 0; i < 7; i++)
            lastPV[i] = new DoubleV3((float)PV[ix][i+7]/200, (float)PV[ix][i] / 100);
       

        la = new DoubleV3(0, 0);
    }

    void Update()
    {
        displacement = -current[0].x + 2.2f;

        double dt = Time.deltaTime;
        if (dt > 0.016)
            dt = 0.016;
        if (t > beatLen)
        {
            t = 0;
            PV[ix][1] = (PV[ix][1 + 7] - Vd[1]) / C[1];
            PV[ix][2] = EDP(PV[ix][2 + 7], RV_AlV0);
            PV[ix][4] = (PV[ix][4 + 7] - Vd[4]) / C[4];
            PV[ix][5] = EDP(PV[ix][5 + 7], LV_AlV0);
        }
            
        
        double h = 0.001;
        for (double k = 0; k < dt; k = k + h)
        {
            int ix0=ix, ix1=ix+1;
            if (ix == T.Length - 1)
                ix1 = 0;

            PV[ix1] = RK4(t, PV[ix0], h);
            ix=ix1;
            t += h;
            T[ix1] = T[ix0] + h;
        }

        if (countFrames == drawEveryXFrames)
        {
            for (int i = 0; i < current.Length - 1; i++)
                current[i] = new DoubleV3((float)T[ix] - 5, (float)PV[ix][i] / 200);
            current[14] = new DoubleV3((float)T[ix] - 5, (float)Eval_poly(ven_poly, t, 0, 0.4) - 2);

            for (int i = 0; i < 7; i++)
                currentPV[i] = new DoubleV3((float)PV[ix][i + 7] / 200, (float)PV[ix][i] / 200);

            linesToDraw[0].Add(current[0 + 7]);
            linesToDraw[1].Add(current[3 + 7]);
            linesToDraw[2].Add(current[6 + 7]);
            linesToDraw[3].Add(current[5 + 7]);
            linesToDraw[4].Add(current[0]);
            linesToDraw[5].Add(current[2]);
            linesToDraw[6].Add(current[5]);
            linesToDraw[7].Add(current[6]);
            linesToDraw[8].Add(current[14]);

            last = (DoubleV3[])current.Clone();
            lastPV = (DoubleV3[])currentPV.Clone();

            cu = new DoubleV3((float)t, (float)Eval_poly(ven_poly, t, 0, 0.4));
            la = cu;
            countFrames = 0;
        }
        else
            countFrames++;

        if (t > beatLen)
        {
            beatN += 1;

            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[0]), Color.white).AddComponent<LineBehaviour>();
            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[1]), Color.yellow).AddComponent<LineBehaviour>();
            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[2]), Color.gray).AddComponent<LineBehaviour>();
            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[3]), Color.cyan).AddComponent<LineBehaviour>();
            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[4]), Color.blue).AddComponent<LineBehaviour>();
            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[5]), Color.blue).AddComponent<LineBehaviour>();
            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[6]), Color.blue).AddComponent<LineBehaviour>();
            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[7]), Color.green).AddComponent<LineBehaviour>();
            ProcMesh.BuildLine(Ext.CreateArrayFromList(linesToDraw[8]), Color.green).AddComponent<LineBehaviour>();

            for (int i = 0; i < linesToDraw.Length; i++)
                linesToDraw[i] = new List<DoubleV3>();

            linesToDraw[0].Add(current[0 + 7]);
            linesToDraw[1].Add(current[3 + 7]);
            linesToDraw[2].Add(current[6 + 7]);
            linesToDraw[3].Add(current[5 + 7]);
            linesToDraw[4].Add(current[0]);
            linesToDraw[5].Add(current[2]);
            linesToDraw[6].Add(current[5]);
            linesToDraw[7].Add(current[6]);
            linesToDraw[8].Add(current[14]);

            if (beatsPerMinute > 20)
                beatLen = 60 / beatsPerMinute;

            Debug.Log(ix);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 300, 50), PV[ix][0].ToString());
        GUI.Label(new Rect(20, 40, 300, 50), PV[ix][1].ToString());
        GUI.Label(new Rect(20, 60, 300, 50), PV[ix][2].ToString());
        GUI.Label(new Rect(20, 80, 300, 50), PV[ix][3].ToString());
        GUI.Label(new Rect(20, 100, 300, 50), PV[ix][4].ToString());
        GUI.Label(new Rect(20, 120, 300, 50), PV[ix][5].ToString());
        GUI.Label(new Rect(20, 140, 300, 50), PV[ix][6].ToString());

        GUI.Label(new Rect(20, 300, 300, 50), T[ix].ToString());
        GUI.Label(new Rect(20, 320, 300, 50), beatN.ToString());
        GUI.Label(new Rect(20, 400, 300, 50), cR[1].ToString());
        GUI.Label(new Rect(20, 420, 300, 50), cR[2].ToString());
        GUI.Label(new Rect(20, 440, 300, 50), cR[4].ToString());
        GUI.Label(new Rect(20, 460, 300, 50), cR[5].ToString());
        double totalvol = 0;
        for(int i=7; i<14; i++)
        {
            totalvol += PV[ix][i];
        }
        GUI.Label(new Rect(20, 500, 300, 50), totalvol.ToString());

    }

    double[] RK4(double t, double[] pv, double h)
    {
        double[] k1, k2, k3, k4;
        double[] pvk2 = new double[14], pvk3 = new double[14], pvk4 = new double[14];
        double[] pvn1 = new double[14];

        k1 = DP(t, pv);

        for(int i=0; i<pv.Length; i++)
            pvk2[i] = pv[i] + k1[i] * h / 2;
        k2 = DP(t + h / 2, pvk2);

        for (int i = 0; i < pv.Length; i++)
            pvk3[i] = pv[i] + k2[i] * h / 2;
        k3 = DP(t + h / 2, pvk3);

        for (int i = 0; i < pv.Length; i++)
            pvk4[i] = pv[i] + k3[i] * h;
        k4 = DP(t + h, pvk4);

        for (int i = 0; i < pv.Length; i++)
            pvn1[i] = pv[i] + (h / 6) * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);

        return pvn1;

    }

    double[] Euler(double t, double[] pv, double h)
    {
        double[] pvn1 = new double[14];
        double[] k1 =  DP(t, pv);
        for(int i=0;i<k1.Length;i++)
            pvn1[i] = pv[i] + h * k1[i];

        return pvn1;
    }

    double[] DP(double t, double[] pv)
    {
        double[] dPV = new double[14];
        //  0-Ven    1-RA     2-RV     3-Lun     4-LA      5-LV      6-Art
        //      0-V_RA   1-Tri    2-Pul    3-L_LA    4-Mit     5-Aor      6-Sys


        //Valves
        //Tricuspid
        if (pv[2] > pv[1])
            cR[1] = R[1] * 10000;
        else
            cR[1] = R[1];
        //Pulmonary
        if (pv[3] > pv[2])
            cR[2] = R[2] * 10000;
        else
            cR[2] = R[2];
        //Mitral
        if (pv[5] > pv[4])
            cR[4] = R[4] * 10000;
        else
            cR[4] = R[4];
        //Aortic
        if (pv[6] > pv[5])
            cR[5] = R[5] * 10000;
        else
            cR[5] = R[5];

        // Elastances
        // 0: RA ,  1: RV ,  2:LA ,  3: LV
        el[0] = Eval_poly(atr_poly, t, 0, 0.4);
        el[2] = el[0];
        el[1] = Eval_poly(ven_poly, t, 0, 0.4);
        el[3] = el[1];

        //d_Elas
        del[0] = Eval_poly_der(atr_poly, t, 0, 0.4);
        del[2] = del[0];
        del[1] = Eval_poly_der(ven_poly, t, 0, 0.4);
        del[3] = del[1];

        //current Q (Flows)
        cQ[0] = (pv[0] - pv[1]) / cR[0];
        cQ[1] = (pv[1] - pv[2]) / cR[1];
        cQ[2] = (pv[2] - pv[3]) / cR[2];
        cQ[3] = (pv[3] - pv[4]) / cR[3];
        cQ[4] = (pv[4] - pv[5]) / cR[4];
        cQ[5] = (pv[5] - pv[6]) / cR[5];
        cQ[6] = (pv[6] - pv[0]) / cR[6];

        //Volume variation
        dPV[7+0] = cQ[6] - cQ[0];
        dPV[7+1] = cQ[0] - cQ[1];
        dPV[7+2] = cQ[1] - cQ[2];
        dPV[7+3] = cQ[2] - cQ[3];
        dPV[7+4] = cQ[3] - cQ[4];
        dPV[7+5] = cQ[4] - cQ[5];
        dPV[7+6] = cQ[5] - cQ[6];

        double Pdrv = EDP(pv[7+2], RV_AlV0);
        double Pdlv = EDP(pv[7+5], LV_AlV0);

        dPV[0] = dPV[7+0] / C[0];
        dPV[1] = el[0] * El[0] * dPV[7+1] + del[0] * El[0] * (pv[7+1] - Vd[1]) + dPV[7+1] / C[1];
        dPV[2] = del[1] * El[1] * (pv[7+2] - Vd[2]) + el[1] * El[1] * dPV[7+2] - (del[1] * Pdrv) + (1 - el[1]) * RV_AlV0[1] * dPV[7+2] * (Pdrv + RV_AlV0[0]);
        dPV[3] = dPV[7+3] / C[3];
        dPV[4] = el[2] * El[2] * dPV[7+4] + del[2] * El[2] * (pv[7+4] - Vd[4]) + dPV[7+4] / C[4];
        dPV[5] = del[3] * El[3] * (pv[7+5] - Vd[5]) + el[3] * El[3] * dPV[7+5] - (del[3] * Pdlv) + (1 - el[3]) * LV_AlV0[1] * dPV[7+5] * (Pdlv + LV_AlV0[0]);
        dPV[6] = dPV[7+6] / C[6];

        return dPV;

    }

    double EDP(double V, double[] AlV0)
    {
        return AlV0[0] * (Math.Exp(AlV0[1] * (V - AlV0[2])) - 1);
    }

    double Eval_poly(double[] coeffs, double x, double xlow, double xhigh)
    {
        double y = 0;

        if (x > xhigh)
        {
            return 0;
        }
        if (x < xlow)
        {
            return 0;
        }
        for (int i = 0; i < coeffs.Length; i++)
        {
            y += coeffs[i] * Math.Pow(x, i);
        }
        return y;
    }

    double Eval_poly_der(double[] coeffs, double x, double xlow, double xhigh)
    {
        double y = 0;

        if (x > xhigh)
        {
            return 0;
        }
        if (x < xlow)
        {
            return 0;
        }
        for (int i = 1; i < coeffs.Length; i++)
        {
            y += i * coeffs[i] * Math.Pow(x, i - 1);
        }
        return y;
    }
}