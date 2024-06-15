using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    private Image _image;

    public Point startPoint;
    public Point endPoint;
    public void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Show()
    {
        UpdateByPoint();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetEnds(Point newStartPoint, Point newEndPoint)
    {
        startPoint = newStartPoint;
        endPoint = newEndPoint;
        UpdateByPoint();
    }

    public void UpdateByPoint()
    {
        if (endPoint == null) return;
        if (startPoint == null) return;
        transform.position = startPoint.transform.position;
        _image.rectTransform.SetSizeDeltaX(Point.GetDistanceTwoPoint(startPoint,endPoint));
        var direction = Point.GetDirectionFromTwoPoint(startPoint, endPoint);
        
        _image.rectTransform.eulerAngles = new Vector3(0, 0, direction.ConvertDirectionToZAngle());
    }

  
   
}
