using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UGUIAlignWindow : EditorWindow
{
    //private Dictionary<AlignType,Texture> alignTexture = new Dictionary<AlignType, Texture>(); 
    private Dictionary<AlignType, string> alignString = new Dictionary<AlignType, string>();
    void OnEnable()
    {
        //Texture leftTexture = Resources.Load<Texture>("Textures/Left");
        //Texture horizontalCenterTexture = Resources.Load<Texture>("Textures/HorizontalCenter");
        //Texture rightTexture = Resources.Load<Texture>("Textures/Right");
        //Texture topTexture = Resources.Load<Texture>("Textures/Top");
        //Texture verticalCenterTexture = Resources.Load<Texture>("Textures/VerticalCenter");
        //Texture bottomTexture = Resources.Load<Texture>("Textures/Bottom");
        //Texture horizontalTexture = Resources.Load<Texture>("Textures/Horizontal");
        //Texture verticalTexture = Resources.Load<Texture>("Textures/Vertical");
        //alignTexture.Add(AlignType.Left, leftTexture);
        //alignTexture.Add(AlignType.HorizontalCenter, horizontalCenterTexture);
        //alignTexture.Add(AlignType.Right, rightTexture);
        //alignTexture.Add(AlignType.Top, topTexture);
        //alignTexture.Add(AlignType.VerticalCenter, verticalCenterTexture);
        //alignTexture.Add(AlignType.Bottom, bottomTexture);
        //alignTexture.Add(AlignType.Horizontal, horizontalTexture);
        //alignTexture.Add(AlignType.Vertical, verticalTexture);

        alignString.Add(AlignType.Left, "left");
        alignString.Add(AlignType.HorizontalCenter, "horC");
        alignString.Add(AlignType.Right, "right");
        alignString.Add(AlignType.Top, "top");
        alignString.Add(AlignType.VerticalCenter, "verC");
        alignString.Add(AlignType.Bottom, "bottom");
        alignString.Add(AlignType.Horizontal, "hor");
        alignString.Add(AlignType.Vertical, "ver");
    }

    [MenuItem("AssetTools/UI/Align")]
    [MenuItem("GameObject/AssetTools/UI/Align")]
    public static UGUIAlignWindow GetWindow()
    {
        var window = GetWindow<UGUIAlignWindow>();
        window.titleContent = new GUIContent("UGUI Align");
        window.Focus();
        window.Repaint();
        return window;
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        for (int i = (int) AlignType.Left; i <= (int) AlignType.Vertical; i++)
        {
            if (GUILayout.Button(alignString[(AlignType) i]))
            {
                UGUIAlign.Align((AlignType) i);
            }
            if (i % 3 == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }
        GUILayout.EndVertical();
    }
}