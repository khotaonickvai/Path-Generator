using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connector : MonoBehaviour
{
    private static Connector _instance;

    public static Connector Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Connector>();
            }
            return _instance;
        }
    }

    public static Queue<Line> Lines = new Queue<Line>();

    private void Awake()
    {
    }
    
    public void CreateLine(Point startPoint, Point endPoint)
    {
        var newLine = SpawnLine();
        var newLineComponent = newLine.GetComponent<Line>();
        newLineComponent.SetEnds(startPoint,endPoint);
        newLineComponent.Show();
        
        startPoint.connectedLines.Add(newLineComponent);
        endPoint.connectedLines.Add(newLineComponent);
    }

    public Line SpawnLine()
    {
        if (Lines.Count > 0)
        {
            return Lines.Dequeue();
        }
        var newLine = Instantiate(MainCanvas.Instance.linePrefab, transform);
        var newLineComponent = newLine.GetComponent<Line>();
        return newLineComponent;
    }

    public void DestroyLine(Line line)
    {
        line.gameObject.SetActive(false);
        Lines.Enqueue(line);
    }
}
