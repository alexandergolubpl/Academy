using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public class Hierarchy_Extend : MonoBehaviour
{
    private static Vector2 offset = new Vector2(0, 2);
    public static Color gameObjectFontColor = new Color(0.7f, 0.7f, 0.7f, 0f);
    public static Color gameObjectBGColor = new Color(0.22f, 0.22f, 0.22f, 0f);
    public static bool prefabBoldFont = false;
    public static bool isShowEnabled = true;
    public static Color prefabOrgFontColor = new Color(0.0f, 0.0f, 0.0f, 0f);
    public static Color prefabModFontColor = new Color(0.6f, 0.7f, 0.8f, 0f);
    public static Color inActiveColor = new Color(0.2f, 0.2f, 0.2f, 0f);

    //private bool _tabsCreated = false;

    static string icon_folder = "Assets/AssetsAutoSort/HierarchyStyle/";
    static string clone_texture;
    static string image_texture;
    static string rect_texture;
    static string text_texture;


    static Texture2D icon_to_display;
    static string tooltip;

    static Color fontColor = gameObjectFontColor;
    static Color backgroundColor = gameObjectBGColor;
    static FontStyle styleFont = FontStyle.Normal;

    [MenuItem("GameObject/AssetTools/Hierarchy/Copy icons hide", false, 0)]
    [MenuItem("AssetTools/Hierarchy/Copy icons hide")]
    private static void DisableUpdateHierarchyIcons()
    {
        isShowEnabled = false;
    }
    [MenuItem("GameObject/AssetTools/Hierarchy/Copy icons show", false, 0)]
    [MenuItem("AssetTools/Hierarchy/Copy icons show")]
    private static void EnableUpdateHierarchyIcons()
    {
        isShowEnabled = true;
    }



    static Hierarchy_Extend()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }
    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        if (isShowEnabled)
        {
            image_texture = icon_folder + "icon_img.png";
            rect_texture = icon_folder + "icon_rect.png";
            text_texture = icon_folder + "icon_text.png";
            clone_texture = icon_folder + "icon_copy.png";
            Texture2D icon_copy = (Texture2D) AssetDatabase.LoadAssetAtPath(clone_texture, typeof(Texture2D));
            Texture2D icon_img = (Texture2D) AssetDatabase.LoadAssetAtPath(image_texture, typeof(Texture2D));
            Texture2D icon_text = (Texture2D) AssetDatabase.LoadAssetAtPath(text_texture, typeof(Texture2D));
            Texture2D icon_rect = (Texture2D) AssetDatabase.LoadAssetAtPath(rect_texture, typeof(Texture2D));


            icon_to_display = icon_rect;

            var obj = EditorUtility.InstanceIDToObject(instanceID)/* as GameObject*/;
            if (obj is GameObject gameObj)
            {
                if (gameObj.GetComponent<Image>() != null)
                {
                    icon_to_display = icon_img;
                }
                if (gameObj.GetComponent<Text>() != null)
                {
                    icon_to_display = icon_text;
                }
                if (Event.current.modifiers == (EventModifiers.Alt))
                {
                    icon_to_display = icon_copy;
                    tooltip = "Clone inside";
                }
                else
                {
                    tooltip = "Create image sprite inside (Hold Alt-Key to Clone)";
                }

                if (Selection.instanceIDs.Contains(instanceID))
                {
                    //backgroundColor = new Color(0.24f, 0.48f, 0.90f);
                    //fontColor = new Color(1f, 1f, 1f);
                }
                if (obj != null)
                {
                    var prefabType = PrefabUtility.GetPrefabType(obj);
                    if (gameObj.activeInHierarchy == false)
                    {
                        //fontColor = new Color(1f, 1f, 1f, 1f);
                        backgroundColor = inActiveColor;
                    }
                    if (prefabType == PrefabType.PrefabInstance)
                    {
                        styleFont = (prefabBoldFont) ? FontStyle.Bold : FontStyle.Normal;
                        PropertyModification[] prefabMods = PrefabUtility.GetPropertyModifications(obj);
                        foreach (PropertyModification prefabMod in prefabMods)
                        {
                            if (prefabMod.propertyPath.ToString() != "m_Name" && prefabMod.propertyPath.ToString() != "m_LocalPosition.x" && prefabMod.propertyPath.ToString() != "m_LocalPosition.y" && prefabMod.propertyPath.ToString() != "m_LocalPosition.z" && prefabMod.propertyPath.ToString() != "m_LocalRotation.x" && prefabMod.propertyPath.ToString() != "m_LocalRotation.y" && prefabMod.propertyPath.ToString() != "m_LocalRotation.z" && prefabMod.propertyPath.ToString() != "m_LocalRotation.w" && prefabMod.propertyPath.ToString() != "m_RootOrder" && prefabMod.propertyPath.ToString() != "m_IsActive")
                            {
                                fontColor = prefabModFontColor;
                                break;
                            }
                        }
                        if (fontColor != prefabModFontColor)
                            fontColor = prefabOrgFontColor;
                    }

                    Insert_Clone(selectionRect, gameObj);

                    Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
                    EditorGUI.LabelField(offsetRect, gameObj.name, new GUIStyle()
                    {
                        normal = new GUIStyleState() { textColor = fontColor },
                        fontStyle = styleFont
                    }
                    );
                }
            }
        }
    }

    [MenuItem("AssetTools/Hierarchy/Insert selected Graphics inside")]
    [MenuItem("GameObject/AssetTools/Hierarchy/Insert selected Graphics inside")]
    private static void Insert_Clone(Rect selectionRect, GameObject gameObj)
    {
        Rect fullRect = new Rect(new Vector2(selectionRect.position.x, selectionRect.position.y), selectionRect.size);
        EditorGUI.DrawRect(fullRect, backgroundColor);

        //Texture2D btnAddTexture = Resources.Load<Texture2D>("skins/lightskin/images/ol plus act.png");
        //GUI.Button(new Rect(selectionRect.position.x + selectionRect.width - 16, selectionRect.position.y, 16, 16), "+");
        if (gameObj.GetComponent<RectTransform>() != null)
        {
            //var size = EditorStyles.miniButton.CalcSize(new GUIContent(clone_pic));
            if (GUI.Button(new Rect(selectionRect.position.x + selectionRect.width - 16, selectionRect.position.y, 16, 16), new GUIContent(icon_to_display, tooltip), EditorStyles.miniLabel))
            {
                GameObject go;
                if (Event.current.modifiers == (EventModifiers.Alt))
                {
                    go = Instantiate(gameObj);
                    go.name = gameObj.name + "_" + "ui";
                    go.transform.SetParent(gameObj.transform);
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;

                    Undo.RegisterCreatedObjectUndo(go, "Create " + go.name + " object");
                    //Debug.Log("Creating " + go.name + " with all parent components");
                    /*foreach (var v in gameObj.GetComponents<Component>())
                    {
                        go.AddComponent(v.GetType());
                    }*/
                }
                else
                {
                    go = new GameObject("img" + "_" + "ui");
                    go.transform.SetParent(gameObj.transform);
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;

                    go.name = "img" + "_" + "ui";

                    Undo.RegisterCreatedObjectUndo(go, "Create " + "img" + " object");
                    //Debug.Log("Creating new UI object: " + go.name + "");
                    go.AddComponent<RectTransform>();
                    Image goImage = go.AddComponent<Image>();
                    goImage.raycastTarget = false;

                    RectTransform gort = go.GetComponent<RectTransform>();
                    //gort.anchoredPosition = Vector2.zero;
                    gort.anchorMin = new Vector2(0.5f, 0.5f);
                    gort.anchorMax = new Vector2(0.5f, 0.5f);
                    gort.pivot = new Vector2(0.5f, 0.5f);
                }
                Selection.activeGameObject = go;
                //EditorGUIUtility.PingObject(Selection.activeObject);
            }
        }
    }
}