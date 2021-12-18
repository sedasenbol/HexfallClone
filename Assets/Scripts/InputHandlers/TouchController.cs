using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputHandlers
{
    public class TouchController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public static event Action<PointerEventData> OnPlayerTapEnded;
        public static event Action<PointerEventData> OnPlayerDragBegan;
        public static event Action<PointerEventData> OnPlayerDragged;
        public static event Action OnPlayerDragEnded; 

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPlayerTapEnded?.Invoke(eventData);
        }
    
        public void OnDrag(PointerEventData eventData)
        {
            OnPlayerDragged?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnPlayerDragBegan?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnPlayerDragEnded?.Invoke();
        }
    }
}
