using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public Text pointsText;
    public GameObject Complete;
    public int points = 0;
    public GameObject itemsContainer;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void AddPoints()
    {
        points++;
        pointsText.text = points.ToString();

    }
    public void checkGameComplete()
    {
        var itemsTr = itemsContainer.transform.GetComponentsInChildren<Collider2D>();
        Debug.Log(itemsTr.Length);
        if (itemsTr.Length == 0)
        {
            ShowGameComplete();
        }

    }
    void ShowGameComplete()
    {
        Debug.Log("ShowGameComplete");
        Complete.transform.localScale = Vector2.zero;
        Complete.SetActive(true);
        Complete.transform.DOScale(1, 1f);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
