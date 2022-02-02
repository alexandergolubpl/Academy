using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Diagnostics;

public class CanvasCreate : MonoBehaviour
{
    [MenuItem("GameObject/UI/Canvas: 1080x1920 Create", false, 0)]
    [MenuItem("GameObject/AssetTools/Canvas: 1080x1920 Create", false, 0)]
    [MenuItem("AssetTools/Canvas: 1080x1920 Create %#h")]
    private static void Create()
    {
        // create game object and child object
        GameObject myGO = new GameObject();
        Camera maincam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        maincam.fieldOfView = 24.14f;
        //Debug.Log("Cam1 = " + maincam.name);
        myGO.name = "TestCanvas";

        Canvas myCanvas = myGO.AddComponent<Canvas>();
        if (myCanvas != null)
        {
            myCanvas.renderMode = RenderMode.WorldSpace;
            myCanvas.worldCamera = maincam;

            myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            myCanvas.planeDistance = 44.0f;
            //Debug.Log(myCanvas.name+".worldCamera = " + myCanvas.worldCamera);
        }

        CanvasScaler myCanvasScaler = myGO.AddComponent<CanvasScaler>();
        myCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        myCanvasScaler.referenceResolution = new Vector2(1080, 1920);
        myCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        //myCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        //myCanvasScaler.matchWidthOrHeight = 1;

        GraphicRaycaster myRaycaster = myGO.AddComponent<GraphicRaycaster>();

        Undo.RegisterCreatedObjectUndo(myGO, "Create Test Canvas");

        CreateRefElement(myGO);
    }

   // [Conditional("UNITY_2020_3_OR_NEWER")]
    private static void CreateRefElement(GameObject thgo)
    {
        GameObject myRefEl = new GameObject("Ref");
        myRefEl.tag = "EditorOnly";

        myRefEl.transform.parent = thgo.transform;

        Image refImage = myRefEl.AddComponent<Image>();
        refImage.raycastTarget = false;

        Color tempAlpha = refImage.color;
        tempAlpha.r = 0f;
        tempAlpha.g = 0f;
        tempAlpha.b = 0f;
        tempAlpha.a = .2f;
        refImage.color = tempAlpha;
#if !UNITY_2020_3_OR_NEWER
        refImage.SetAlpha(0.5f);
#endif

        myRefEl.transform.localPosition = Vector2.zero;

        //myRefEl.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        //myRefEl.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        RectTransform recTrans = myRefEl.GetComponent<RectTransform>();

        recTrans.anchorMin = new Vector2(0, 0);
        recTrans.anchorMax = new Vector2(1, 1);
        recTrans.offsetMin = new Vector2(0, 0);
        recTrans.offsetMax = new Vector2(0, 0);
        recTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //recTrans.pivot = new Vector2(0.5f, 0.5f);

        GameObject myContentEl = new GameObject("Content");
        myContentEl.transform.parent = thgo.transform;

        myContentEl.transform.localPosition = Vector2.zero;
        RectTransform recTransContent = myContentEl.AddComponent<RectTransform>();
        recTransContent.anchorMin = new Vector2(0, 0);
        recTransContent.anchorMax = new Vector2(1, 1);
        recTransContent.offsetMin = new Vector2(0, 0);
        recTransContent.offsetMax = new Vector2(0, 0);
        recTransContent.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        Selection.activeObject = myContentEl;
    }
}