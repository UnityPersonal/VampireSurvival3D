using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Unity.Profiling;

public class Graph : MonoBehaviour
{
    [SerializeField] private Sprite pointSprite;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private Gradient gradient;
    [SerializeField] private float fpsGoal = 120;

    private List<float> graphQueue = new List<float>();
    private int maxSize = 38; // modifier et exposer la valeur de mult
    private GameObject[] bars;
    private float xSize = 10;
    private float variableMaxValue;

    private void Start()
    {
        SetGraph();
    }

    private void SetGraph()
    {
        bars = new GameObject[maxSize];

        for (int i = 0; i < maxSize; i++) 
        {
            float xPos = i * xSize + xSize / 2;
            bars[i] = CreateBar(new Vector2(xPos, 0), xSize * 0.85f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void ShowGraph(float value)
    {
        if (graphQueue.Count >= maxSize)
        {
            graphQueue.RemoveAt(0);
        }

        graphQueue.Add(value);

        float yMax = (1 / fpsGoal) * 1000;
        
        for (int i = 0; i < graphQueue.Count; i++)
        {
            SetBar(i, graphQueue[i], yMax, graphContainer.sizeDelta.y);
        }
    }

    private GameObject CreateBar(Vector2 graphPosition, float barWidth)
    {
        GameObject gameObject = new GameObject("bar", typeof(Image));
        gameObject.transform.SetParent(graphContainer);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(graphPosition.x, 0f);
        rect.sizeDelta = new Vector2(barWidth, graphPosition.y);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0f);
        rect.localScale = Vector2.one;
        return gameObject;
    }

    private void SetBar(int i, float value, float max, float maxHeight)
    {
        float yColor = math.remap(3, max * 1.33f, 0, 1, value);

        Image image = bars[i].transform.GetComponent<Image>();
        image.color = gradient.Evaluate(yColor);

        float y = (value / max);

        float yPos = y * maxHeight;
        if (yPos > maxHeight) yPos = maxHeight;

        RectTransform rect = bars[i].GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, yPos);
    }
}

