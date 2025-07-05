using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForceTouchScroll : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect scrollRect;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        scrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
    }
}
