using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Text), typeof(TextMeshProUGUI))]
public class LettersUppercase : MonoBehaviour
{
    [MenuItem("GameObject/AssetTools/Text/UPPERCASE", false, 0)]
    [MenuItem("AssetTools/Text/UPPERCASE")]
    public static void Main()
    {
        Object[] sel = Selection.objects;

        for (int i = 0; i < sel.Length; i++)
        {
            GameObject go = sel[i] as GameObject;
            if (go.GetComponent<Text>() != null)
            {
                go.GetComponent<Text>().text = go.GetComponent<Text>().text.ToUpper();
            }
            else
            {
                go.GetComponent<TextMeshProUGUI>().text = go.GetComponent<TextMeshProUGUI>().text.ToUpper();
            }
        }
    }
}
