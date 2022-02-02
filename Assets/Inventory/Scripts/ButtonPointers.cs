using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPointers : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    public Image image;

    public Vector3 InitPos;
    private Transform InitSource;
    private float DODuration = 0.3f;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 rInitPos;

    private float price;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        image.DOColor(new Color(1f, 0.9f, 0.9f, 1f), DODuration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.DOColor(Color.white, DODuration);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.transform.DOScale(Vector3.one * 1.1f, DODuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.transform.DOScale(Vector3.one, DODuration).SetEase(Ease.OutBack);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        InitPos = transform.position;
        transform.DOScale(Vector3.one * 1.1f, DODuration).SetEase(Ease.OutBack);
        //rInitPos = rectTransform.anchoredPosition;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GetPosOnCanvas();
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        image.transform.DOScale(Vector3.one, DODuration).SetEase(Ease.OutBack);
        transform.DOMove(InitPos, DODuration).SetEase(Ease.OutBack);
        //rectTransform.DOAnchorPos(rInitPos, DODuration).SetEase(Ease.OutBack);
    }

    private Vector3 GetPosOnCanvas()
    {
        var screenVector3 = Input.mousePosition;
        screenVector3.z = canvas.planeDistance;
        screenVector3 = Camera.main.ScreenToWorldPoint(screenVector3);
        return screenVector3;
    }
}