    using UnityEditor;

public class Hierarchy_Extend_Editor : EditorWindow
{
    [MenuItem("AssetTools/Hierarchy/Hierarchy_Extend_Editor")]
    public static void ShowWindow() {
        GetWindow<Hierarchy_Extend_Editor>("Hierarchy_Extend_Editor");
    }
    private void OnGUI() {
        Hierarchy_Extend.gameObjectFontColor = EditorGUILayout.ColorField("Original Font Color", Hierarchy_Extend.gameObjectFontColor);
        Hierarchy_Extend.gameObjectBGColor = EditorGUILayout.ColorField("Original BG Color", Hierarchy_Extend.gameObjectBGColor);
        Hierarchy_Extend.prefabOrgFontColor = EditorGUILayout.ColorField("Prefab Original Font Color", Hierarchy_Extend.prefabOrgFontColor);
        Hierarchy_Extend.prefabBoldFont = EditorGUILayout.Toggle("Prefab Original Font Bold", Hierarchy_Extend.prefabBoldFont);
        Hierarchy_Extend.prefabModFontColor = EditorGUILayout.ColorField("Prefab Modified Font Color", Hierarchy_Extend.prefabModFontColor);
        Hierarchy_Extend.inActiveColor = EditorGUILayout.ColorField("Inactive Color", Hierarchy_Extend.inActiveColor);
    }
}