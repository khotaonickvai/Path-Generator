using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
   private EventTrigger _eventTrigger;
   private Image _image;
   private bool _isMouseHover;
   private State _currentState;
   private MainCanvas MainCanvas=> MainCanvas.Instance;

   public RectTransform rectTransform;
   public Point previous;
   public Point tail;
   public List<Line> connectedLines;
   private void Awake()
   {
      _eventTrigger = GetComponent<EventTrigger>();
      _image = GetComponent<Image>();
      rectTransform = GetComponent<RectTransform>();
      foreach (var trigger in _eventTrigger.triggers)
      {
         switch (trigger.eventID)
         {
            case EventTriggerType.PointerEnter:
               trigger.callback.AddListener(pointData =>
               {
                  _isMouseHover = true;
                  ChangeState(State.MouseHover);
               });
               break;
            case EventTriggerType.PointerExit:
               trigger.callback.AddListener(pointData =>
               {
                  _isMouseHover = false;
                  ChangeState(State.None);
               });
               break;
            case EventTriggerType.Drag:
               trigger.callback.AddListener(pointData =>
               {
                  ChangeState(State.Move);
               });
               break;
            case EventTriggerType.Drop:
               trigger.callback.AddListener(pointData =>
               {
                  if (_isMouseHover)
                  {
                     ChangeState(State.MouseHover);
                  }
                  else
                  {
                     ChangeState(State.None);
                  }
               });
               break;
         }
      }
   }

   private void Update()
   {
      UpdateCurrentState();
   }

   private void UpdateCurrentState()
   {
      switch (_currentState)
      {
         case State.None:
            break;
         case State.MouseHover:
            break;
         case State.Move:
            UpdatePositionByMouse();
            break;
      }
   }

   private void SetAlphaColorImage(float newAlpha)
   {
      var color = Color.green;
      color.a = newAlpha;
      _image.color = color;
   }

   private void UpdatePositionByMouse()
   {
      transform.position = Input.mousePosition;
      GuildLine.Instance.UpdateByPosition(Input.mousePosition,MainCanvas.DeltaX,MainCanvas.DeltaY,MainCanvas.originRef.position);
      UpdateConnectedLines();
   }

   private void UpdateConnectedLines()
   {
      foreach (var line in connectedLines)
      {
         line.UpdateByPoint();
      }
   }
   private void ChangeState(State newState)
   {
      if (newState == _currentState) return;
      switch (_currentState)
      {
         case State.None:
            break;
         case State.MouseHover:
            break;
         case State.Move:
            GuildLine.Instance.Hide();
            break;
      }
      _currentState = newState;
      switch (newState)
      {
         case State.None:
            SetAlphaColorImage(1f);
            break;
         case State.MouseHover:
            SetAlphaColorImage(0.5f);
            break;
         case State.Move:
            GuildLine.Instance.Show();
            break;
      }
   }

   public static float GetDistanceTwoPoint(Point point1, Point point2)
   {
      return Vector2.Distance(point1.rectTransform.anchoredPosition, point2.rectTransform.anchoredPosition);
   }
   public static Vector2 GetDirectionFromTwoPoint(Point point1, Point point2)
   {
      return point2.rectTransform.anchoredPosition - point1.rectTransform.anchoredPosition;
   }
   
   private enum State
   {
      None,
      MouseHover,
      Move
   }
}
