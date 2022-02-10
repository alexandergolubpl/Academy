using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    [SerializeField] [Range(0.75f,1.25f)] private float legSize =1f;
    [SerializeField] [Range(0.75f,1.4f)] private float kneeSize = 1f;
    [SerializeField] [Range(0.9f,1.1f)] private float chestSize = 1f;
    [SerializeField] [Range(0.8f,1.25f)] private float neckSize = 1f;
    [SerializeField] [Range(0.75f, 1.3f)] private float breastSize = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 GetLegSize(Vector3 size)
    {
        return new Vector3(legSize, size.y, legSize);
    }
    public Vector3 GetKneeSize(Vector3 size)
    {
        return new Vector3(kneeSize, size.y, legSize);
    }
    public Vector3 GetChestSize => new Vector3(chestSize, chestSize, chestSize);
    public Vector3 GetNeckSize => new Vector3(neckSize, neckSize, neckSize);
    public Vector3 GetBreastSize => new Vector3(breastSize, breastSize, breastSize);
}

