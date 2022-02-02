using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class StickersGroundPos : EditorWindow
{
	

    [MenuItem("AssetTools/Stickers/StickersGroundPos")]
    [MenuItem("GameObject/AssetTools/Stickers/StickersGroundPos", false, 0)]
    public static void Open() {
        GameObject thgo = Selection.activeObject as GameObject;

        RectTransform gort = thgo.GetComponent<RectTransform>();
        Undo.RecordObject(thgo, "StickersGroundPos");
        Vector3 gortpos = gort.transform.localPosition;
        gort.pivot = new Vector2(0.5f, 1f);
        gort.transform.localPosition = new Vector3(gortpos.x, gortpos.y+ gort.rect.height/2* gort.localScale.y, 300.0f);
        gort.transform.localRotation = Quaternion.Euler(30.0f, gort.transform.localRotation.y, gort.transform.localRotation.z);
        var someObject = thgo.transform.parent.GetChild(0).gameObject;
        var tempDelta = someObject.GetComponent<RectTransform>().sizeDelta;
        var tempScale = someObject.transform.localScale;

        gort.sizeDelta = tempDelta;
        gort.transform.localScale = tempScale;
    }
}