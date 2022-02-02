using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Planet : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    //private event Action onPlanetClick;
    public void Setup(Sprite sprite, Action onClickCallBack)
    {
        image.transform.localPosition = Vector3.zero;
        image.color = Color.white;
        image.sprite = sprite;

        //onPlanetClick = onClickCallBack();

    }
    public void onPlanetClick(PointerEventData eventData) { }
    public void onPlanetEnter(PointerEventData eventData) {
    image.transform.}
    public void onPlanetExit(PointerEventData eventData) { }

}
