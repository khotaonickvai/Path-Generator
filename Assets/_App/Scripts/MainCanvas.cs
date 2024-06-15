using System;
using System.Collections.Generic;
using System.Globalization;
using SFB;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    public Connector connector;
    public TextMeshProUGUI
        txtTopRight;
        

    public Image
        imgGrid;

    public Button 
        btnUpdate, 
        btnLoadImage,
        btnCopyData;

    [FormerlySerializedAs("generateData")] public Button 
        btnGenerateData;

    public TMP_InputField inputWidth, inputHeight,inputDataOut;
    public Transform originRef, topRightRef;
    public EventTrigger eventTrigger;

    public int maxWidth;
    public int maxHeight;
    
    public Transform pointHolder;
    [Header("Prefabs")]
    public GameObject pointPrefab;
    public GameObject linePrefab;
    
    private State _currentState;
    private List<Point> _points;
    private Point _selectedPoint;
    private int _gridWidth;
    private int _gridHeight;
    private GuildLine GuildLine => GuildLine.Instance;
    public float DeltaX => _gridWidth / (topRightRef.position.x - originRef.position.x);
    public float DeltaY => _gridHeight / (topRightRef.position.y - originRef.position.y);
    private static MainCanvas _instance;
    private Stack<Command> _commands;
    private Stack<Command> _undoCommands;
    public static MainCanvas Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainCanvas>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _points = new List<Point>();
        _commands = new Stack<Command>();
        _undoCommands = new Stack<Command>();
        SetWidth(10);
        SetHeight(10);
        GuildLine.Hide();
        btnUpdate.onClick.AddListener(OnUpdateButtonClick);
        btnLoadImage.onClick.AddListener(LoadImage);
        btnCopyData.onClick.AddListener(CopyToDataClipBoard);
        btnGenerateData.onClick.AddListener(GenerateData);
        foreach (var trigger in eventTrigger.triggers)
        {
            switch (trigger.eventID)
            {
                case EventTriggerType.PointerDown:
                    break;
                case EventTriggerType.PointerUp:
                    break;
                case EventTriggerType.Drag:
                    break;
            }
        }
    }

    private void OnUpdateButtonClick()
    {
        int.TryParse(inputWidth.text, out _gridWidth);
        int.TryParse(inputHeight.text, out _gridHeight);
        UpdateGridSize();
    }

    private void GenerateData()
    {
        var pathData = new PathData();
        pathData.points = new List<PointData>();
        foreach (var point in _points)
        {
            var pointPosition = point.rectTransform.anchoredPosition;
            var pointData = new PointData()
            {
                x = (int) pointPosition.x,
                y = (int) pointPosition.y,
            };
            pathData.points.Add(pointData);
        }

        inputDataOut.text = JsonUtility.ToJson(pathData);
    }

    private void CopyToDataClipBoard()
    {
        GUIUtility.systemCopyBuffer = inputDataOut.text;
    }

    private void LoadImage()
    {
        var extensions = new ExtensionFilter[]
        {
            new("Image Files", "png", "jpg", "jpeg")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Image", "", extensions, false);
        if (paths != null && paths.Length > 0)
        {
            var texture = Extension.LoadTextureFromFile(paths[0]);
            if (texture == null) return;
            SetHeight(texture.height);
            SetWidth(texture.width);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            if (sprite == null) return;
            imgGrid.sprite = sprite;
            imgGrid.color = Color.white;
        }
    }

    private void UpdateGridSize()
    {
        txtTopRight.text = $"({_gridWidth}<color=red>,</color>{_gridHeight})";
        if ((float)_gridWidth /_gridHeight > (float)maxWidth/maxHeight)
        {
            var curSizeDelta = imgGrid.rectTransform.sizeDelta;
            curSizeDelta.x = maxWidth;
            curSizeDelta.y = (float)maxWidth *  _gridHeight/_gridWidth;
            imgGrid.rectTransform.sizeDelta = curSizeDelta;
        }
        else
        {
            var curSizeDelta = imgGrid.rectTransform.sizeDelta;
            curSizeDelta.y = maxHeight;
            curSizeDelta.x = (float)maxHeight * _gridWidth / _gridHeight;
            imgGrid.rectTransform.sizeDelta = curSizeDelta;
        }

        inputHeight.text = _gridHeight.ToString();
        inputWidth.text = _gridWidth.ToString();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                ChangeState(State.None);
                Command.Undo();
            }else if (Input.GetKeyDown(KeyCode.Y))
            {
                ChangeState(State.None);
                Command.Redo();
            }
        }else if (Input.GetKey(KeyCode.A))
        {
            ChangeState(State.CreatePoint);
        }
        UpdateCurrentState();
        
    }

    private void CreatePoint(Vector2 position)
    {
        var createPointCommand = new CreatePointCommand(position);
        createPointCommand.Execute();
    }
    
    private void SetWidth(int width)
    {
        _gridWidth = width;
        UpdateGridSize();
    }

    private void SetHeight(int height)
    {
        _gridHeight = height;
        UpdateGridSize();
    }
    
    private void UpdateCurrentState()
    {
        switch (_currentState)
        {
            case State.None:
                break;
            case State.CreatePoint:
                GuildLine.UpdateByPosition(Input.mousePosition,DeltaX,DeltaY,originRef.position);
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    CreatePoint(Input.mousePosition);
                    ChangeState(State.None);
                }
                break;
            case State.MovePoint:
                break;
        }
    }
    
    private void ChangeState(State newState)
    {
        if (newState == _currentState) return;
        switch (_currentState)
        {
            case State.None:
                break;
            case State.CreatePoint:
                GuildLine.Hide();
                break;
            case State.MovePoint:
                break;
        }

        _currentState = newState;
        switch (newState)
        {
            case State.None:
                break;
            case State.CreatePoint:
                GuildLine.Show();
                break;
            case State.MovePoint:
                break;
        }
    }
    
    private enum State
    {
        None,
        CreatePoint,
        MovePoint
    }

    public class CreatePointCommand : Command
    {
        public Vector2 Position;
        private Point _point;

        public CreatePointCommand(Vector2 pointPosition)
        {
            Position = pointPosition;
        }
        public override void Execute()
        {
            var newPoint = Instantiate(Instance.pointPrefab, Instance.pointHolder);
            newPoint.transform.position = Position;
            var pointComponent = newPoint.GetComponent<Point>();
            if (Instance._points.Count > 0)
            {
                var lastPoint = Instance._points[^1];
                lastPoint.tail = pointComponent;
                pointComponent.previous = lastPoint;
                Connector.Instance.CreateLine(lastPoint,pointComponent);
            }
            Instance._points.Add(pointComponent);
            _point = pointComponent;
            base.Execute();
        }

        public override void Revert()
        {
            for (int i = _point.connectedLines.Count-1; i >=0; i--)
            {
                if (_point.previous!= null && _point.previous.connectedLines.Contains(_point.connectedLines[i]))
                {
                    _point.previous.connectedLines.Remove(_point.connectedLines[i]);
                }
                Connector.Instance.DestroyLine(_point.connectedLines[i]);
            }
            if(_point.previous!= null)
                _point.previous.tail = null;
            Instance._points.Remove(_point);
            Destroy(_point.gameObject);
            base.Revert();
        }
    }
}
