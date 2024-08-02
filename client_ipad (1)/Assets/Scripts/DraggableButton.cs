using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform buttonRect; // 버튼의 RectTransform을 인스펙터 창에서 드래그 앤 드롭할 수 있도록 퍼블릭 변수로 지정
    public Vector2 targetPosition;
    private float snapDistance = 30f;

    private CanvasGroup canvasGroup;
    private bool isDragging = false;
    private bool isSnapped = false;

    private void Awake()
    {
        canvasGroup = buttonRect.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = buttonRect.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSnapped)
        {
            isDragging = true;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            buttonRect.anchoredPosition += eventData.delta;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            canvasGroup.blocksRaycasts = true;

            float distance = Vector2.Distance(buttonRect.anchoredPosition, targetPosition);

            if (distance < snapDistance)
            {
                buttonRect.anchoredPosition = targetPosition;
                isSnapped = true; // 스냅된 상태로 설정
                DraggableButtonManager.Instance.CheckAllButtonsSnapped(); // 매니저에게 스냅 상태 알림
            }
        }
    }

    public bool IsSnapped()
    {
        return isSnapped;
    }
}
