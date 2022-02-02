using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var go = eventData.pointerDrag;
            go.GetComponent <ButtonPointers>().InitPos = transform.position;
            Debug.Log(go.GetComponent<ButtonPointers>().InitPos);
            //go.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            go.transform.position = transform.position;
            go.transform.localScale = Vector3.one * 1.5f;
            go.transform.DOScale(Vector3.one * 1f, 1f).SetEase(Ease.OutElastic);
            go = null;
        }
    }
}