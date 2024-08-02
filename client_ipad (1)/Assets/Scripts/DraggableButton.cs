using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform buttonRect; // ��ư�� RectTransform�� �ν����� â���� �巡�� �� ����� �� �ֵ��� �ۺ� ������ ����
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
                isSnapped = true; // ������ ���·� ����
                DraggableButtonManager.Instance.CheckAllButtonsSnapped(); // �Ŵ������� ���� ���� �˸�
            }
        }
    }

    public bool IsSnapped()
    {
        return isSnapped;
    }
}
