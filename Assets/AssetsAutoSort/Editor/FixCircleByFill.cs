using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixCircleByFill : EditorWindow
{
    [MenuItem("AssetTools/FixCircleByFill")]
    [MenuItem("GameObject/AssetTools/FixCircleByFill", false, 0)]

    private static void Fix()
    {
        var selGo = Selection.gameObjects;
        var selSprite = Selection.GetFiltered(
                typeof(Sprite),
                SelectionMode.Editable | SelectionMode.TopLevel);

        List<Image> imGo = new List<Image>();
        for (int i = 0; i < selGo.Length; i++)
        {
            Image im = selGo[i].GetComponent<Image>();
            if (im != null)
            {
                imGo.Add(im);
            }
        }
        Undo.SetCurrentGroupName("Fix Circles");
        int group = Undo.GetCurrentGroup();
        Undo.RecordObjects(imGo.ToArray(), "Fix Fill selected images");

        string tempStr = "";
        for (int i = 0; i < selGo.Length; i++)
        {
            Image im = selGo[i].GetComponent<Image>();
            if (im != null)
            {
                im.type = Image.Type.Filled;
                im.fillMethod = Image.FillMethod.Vertical;
                im.fillOrigin = 1;
                var fillOriginName = ((Image.OriginVertical) im.fillOrigin).ToString();
                tempStr += "\t" + selGo[i].name + " : " + fillOriginName + "\n";
                im.fillAmount = 0.5f;
            }
        }
        Debug.Log(tempStr);
        Undo.CollapseUndoOperations(group);
    }
}