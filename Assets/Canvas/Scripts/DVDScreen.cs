using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DVDScreen : MonoBehaviour
{
    [SerializeField] private RectTransform screen;
    [SerializeField] private RectTransform dvdRectTransform;
    [SerializeField] private Image dvdImage;
    [SerializeField] private Color[] colors;
    [SerializeField] private float borderSize = 10;
    [SerializeField] private float speed;

    private Vector2 velocity;

    private void Start()
    {
        var random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
                velocity = new Vector2(1, 1).normalized * speed;
                break;

            case 1:
                velocity = new Vector2(1, -1).normalized * speed;
                break;

            case 2:
                velocity = new Vector2(-1, -1).normalized * speed;
                break;

            case 3:
                velocity = new Vector2(-1, 1).normalized * speed;
                break;
        }
    }


    private void Update()
    {
        dvdRectTransform.anchoredPosition += velocity * Time.deltaTime;

        var boundMaxX = screen.sizeDelta.x - borderSize;
        var boundMaxY = screen.sizeDelta.y - borderSize;
        var minX = dvdRectTransform.anchoredPosition.x;
        var minY = dvdRectTransform.anchoredPosition.y;

        var maxX = minX + dvdRectTransform.sizeDelta.x;
        var maxY = minY + dvdRectTransform.sizeDelta.y;

        if (minX <= borderSize || maxX >= boundMaxX)
        {
            velocity.x *= -1f;
            dvdImage.color = colors[Random.Range(0, colors.Length)];
        }
        if (minY <= borderSize || maxY >= boundMaxY)
        {
            velocity.y *= -1f;
            dvdImage.color = colors[Random.Range(0, colors.Length)];
        }
    }

}
