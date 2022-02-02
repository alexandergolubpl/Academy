using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    // Start is called before the first frame update
    private int _itemsCount = 4;
    public Canvas canvas;
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject invField;
    [SerializeField] private GameObject gridSell;
    [SerializeField] private GameObject gridBuy;
    [HideInInspector]
    [SerializeField] private Button[] itemSell;
    [HideInInspector]
    [SerializeField] private Button[] itemBuy;

    void Start()
    {
        itemSell = gridSell.GetComponentsInChildren<Button>();
        itemBuy = gridBuy.GetComponentsInChildren<Button>();

        PlaceItems();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlaceItems()
    {
        for (int i = 0; i < _itemsCount; i++)
        {
            GameObject go = (GameObject)Instantiate(item, itemSell[i].transform.position,Quaternion.identity, itemSell[i].transform);
            var myItemComp = go.GetComponent<ButtonPointers>();
            myItemComp.canvas = canvas;
            myItemComp.InitPos = itemSell[i].transform.position;
        }
    }
}
