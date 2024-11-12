using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphSetup : MonoBehaviour
{
    [SerializeField] private GraphHandler graphHandlerPrefab;
    
    [SerializeField] private int valuesAmount = 1000;

    [SerializeField] private Canvas graphCanvas;

    [SerializeField] private Color[] colorPool;
    [SerializeField] private SO_GrowthCurve[] growthCurvePool;
    [SerializeField] private Transform legendParent;
    [SerializeField] private LegendItem legendItemPrefab;
    
    private void Start()
    {
        for (int i = 0; i < growthCurvePool.Length; i++)
        {
            SetupNewGraph(growthCurvePool[i], colorPool[i]);
        }
    }

    private void SetupNewGraph(SO_GrowthCurve growthCurve, Color graphColor)
    {
        string title = growthCurve.previewUpgrade.upgradeType + " " + growthCurve.previewUpgrade.title;
        LegendItem legendItem = Instantiate(legendItemPrefab, legendParent);
        legendItem.Setup(graphColor, title);
        legendItem.gameObject.name = title + " legend";
        GraphSettings newGraph = Instantiate(graphHandlerPrefab, transform).GetComponent<GraphSettings>();
        newGraph.gameObject.name = title + " Graph";
        GraphHandler newGraphHandler = newGraph.GetComponent<GraphHandler>();
        newGraphHandler.Initialize(graphCanvas);
        newGraphHandler.DrawGraph(growthCurve, valuesAmount);
        newGraph.LineColor = graphColor;
    }
}
