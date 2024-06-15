using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildLine : MonoBehaviour
{
    public TextMeshProUGUI 
        txtHorizontal,
        txtVertical;

    public Image
        imgHorizontal,
        imgVertical,
        imgSelectPoint;

    private float _offsetTxtVertical;
    private float _offsetTxtHorizontal;

    private static GuildLine _instance;

    public static GuildLine Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GuildLine>(true);
            }

            return _instance;
        }
    }
   

    private void Awake()
    {
        _offsetTxtVertical = txtVertical.transform.position.y - imgVertical.transform.position.y;
        _offsetTxtHorizontal = txtHorizontal.transform.position.x - imgHorizontal.transform.position.x;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateByPosition(Vector2 position,float deltaX,float deltaY,Vector2 refPosition)
    {
        imgSelectPoint.transform.position = position;
        imgVertical.transform.SetYPosition(position.y);
        imgHorizontal.transform.SetXPosition(position.x);
        //var refPosition = originRef.position;
        txtHorizontal.text = ((position.x - refPosition.x) * deltaX).ToString(CultureInfo.InvariantCulture);
        txtVertical.text = ((position.y - refPosition.y) * deltaY).ToString(CultureInfo.InvariantCulture);
        txtVertical.transform.SetYPosition(_offsetTxtVertical + imgVertical.transform.position.y);
        txtHorizontal.transform.SetXPosition(_offsetTxtHorizontal + imgHorizontal.transform.position.x);
    }
}
