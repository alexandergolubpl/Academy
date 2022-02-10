using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    [SerializeField] private CharacterData data;

    [SerializeField] private Transform[] legTransforms;
    [SerializeField] private Transform[] kneeTransforms;
    [SerializeField] private Transform[] chestTransforms;
    [SerializeField] private Transform[] neckTransforms;
    [SerializeField] private Transform[] breastTransforms;
       // Start is called before the first frame update
       void Start()
    {
        UpdateCharacter();
    }
#if UNITY_EDITOR
    void Update()
    {
        if (Selection.activeGameObject != gameObject)
        {
            return;
        }
        UpdateCharacter();
    }
#endif
    // Update is called once per frame
    void UpdateCharacter()
    {
        foreach (var t in legTransforms)
        {
            t.localScale = data.GetLegSize(t.localScale);
        }
        foreach (var t in legTransforms)
        {
            t.localScale = data.GetLegSize(t.localScale);
        }
        foreach (var t in legTransforms)
        {
            t.localScale = data.GetLegSize(t.localScale);
        }
        //chestTransforms.localScale = data.GetLegSize;
        //neckTransforms.localScale = data.GetNeckSize;
    }
}
