using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordPreview : MonoBehaviour
{
    [SerializeField] private Transform rootPoint;
    [SerializeField] private Text namelabel;
    [SerializeField] private Text descriptionlabel;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text costlabel;
    [SerializeField] private Text damagelabel;

    [SerializeField] private SwordData swordData;

    private GameObject swordObj;

    void Start()
    {
        SetupData();
    }

    public void SetupData()
    {
        if (swordObj != null)
        {
            Destroy(swordObj);
        }
        swordObj = Instantiate(swordData.Prefab, rootPoint);

        namelabel.text = swordData.SwordName;
        descriptionlabel.text = swordData.Decription;
        iconImage.sprite = swordData.Icon;
        costlabel.text = $"Cost: {swordData.Cost}";
        damagelabel.text = $"Damage: {swordData.Damage}";
    }

    void Update()
    {
        rootPoint.Rotate(Time.deltaTime * swordData.RotationSpeed * Vector2.up, Space.World);
    }
}
