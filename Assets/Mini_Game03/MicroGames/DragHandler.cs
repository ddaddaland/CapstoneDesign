using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGame
{
    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform dragArea;
        public System.Action onDrop;

        public void OnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragArea == null) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragArea, eventData.position, eventData.pressEventCamera, out var localPoint);
            GetComponent<RectTransform>().anchoredPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onDrop?.Invoke();
        }
    }
}
