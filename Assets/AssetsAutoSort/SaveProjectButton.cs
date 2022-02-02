//using System.Collections;
//using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveProjectButton : Editor
{
    public void CreateButton(Transform panel, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
    {
        GameObject button = new GameObject();
        //button.transform.parent = panel;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.transform.position = position;
        //button.GetComponent<RectTransform>().SetSize(size);
        button.GetComponent<Button>().onClick.AddListener(method);
    }

    //EditorApplication.ExecuteMenuItem("File/Save Project");
	
	
} 