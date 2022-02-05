using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BombExplosion : MonoBehaviour
{
    Collider2D cldr;
    Transform childTr;
    List<GameObject> othrs = new List<GameObject>();
    private void Awake()
    {
        cldr = GetComponent<Collider2D>();
        cldr.enabled = false;

    }
    private void Start()
    {
        childTr = transform.GetChild(1);
        childTr.localScale = Vector2.one * 0.2f;
        childTr.DOScale(1.3f, 3f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            cldr.enabled = true;
            childTr.DOScale(0.1f, 0.3f).SetEase(Ease.InElastic).OnComplete(() =>
            {
                foreach (var item in othrs)
                {
                    Destroy(item);
                }
                Destroy(gameObject);
            });
        });
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Dectructible" && cldr.enabled)
        {
            othrs.Add(collision.gameObject);
        }
    }
}