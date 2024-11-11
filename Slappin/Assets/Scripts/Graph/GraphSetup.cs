using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphSetup : MonoBehaviour
{
    [SerializeField] private GraphHandler graphHandlerPrefab;
    
    [FormerlySerializedAs("slapDamageGrwoth")] [FormerlySerializedAs("growthCurve")] [SerializeField] private SO_GrowthCurve slapDamageGrowth;
    [FormerlySerializedAs("growthCurve2")] [SerializeField] private SO_GrowthCurve enemyMaxHPGrowth;

    [SerializeField] private int valuesAmount = 1000;

    [SerializeField] private Canvas graphCanvas;
    
    [SerializeField] private Color slapDamageColor = Color.white;
    [SerializeField] private Color enemyMaxHpColor = Color.white;
    
    private void Start()
    {
        // StartCoroutine(DelayStart());
        SetupNewGraph(slapDamageGrowth, "Slap Damage", slapDamageColor);
        SetupNewGraph(enemyMaxHPGrowth, "Enemy Max Hp", enemyMaxHpColor);
    }

    private void SetupNewGraph(SO_GrowthCurve growthCurve, string title, Color graphColor)
    {
        GraphSettings newGraph = Instantiate(graphHandlerPrefab, transform).GetComponent<GraphSettings>();
        newGraph.gameObject.name = title + " Graph";
        GraphHandler newGraphHandler = newGraph.GetComponent<GraphHandler>();
        newGraphHandler.Initialize(graphCanvas);
        newGraphHandler.DrawGraph(growthCurve, valuesAmount);
        newGraph.LineColor = graphColor;
    }
}
