using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class LettersLowercase : MonoBehaviour
{
    [MenuItem("GameObject/AssetTools/Text/lowercase", false, 0)]
    [MenuItem("AssetTools/Text/lowercase")]
    public static void Main()
    {
        Object[] sel = Selection.objects;

        for (int i = 0; i < sel.Length; i++)
        {
            GameObject go = sel[i] as GameObject;
            Text currtxt = go.GetComponent<Text>();
            currtxt.text = currtxt.text.ToLower();
        }
    } 
}
