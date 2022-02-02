using System.Reflection;
using UnityEngine;
using UnityEditor;

public class InspectorLockToggle
{
    private void OnEnable() {
        if (GameObject.Find("CardBaseTool(Clone)") != null)
        {
            GameObject Card = GameObject.Find("CardBaseTool(Clone)");
            Selection.activeGameObject = Card;
        }
    }

    [MenuItem("AssetTools/Panels/Toggle Inspector Lock &q")]
    private static void ToggleInspectorLock()
    {
        if (GameObject.Find("CardBaseTool(Clone)") != null)
        {
            GameObject Card = GameObject.Find("CardBaseTool(Clone)");
            Selection.activeGameObject = Card;
        }
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }

    [MenuItem("AssetTools/Panels/Toggle Inspector Mode &d")]
    private static void ToggleInspectorDebug()
    {
        {
            System.Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("ActiveEditorTracker.sharedTracker");
            FieldInfo field = type.GetField("m_InspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
            InspectorMode mode = (InspectorMode) field.GetValue(ActiveEditorTracker.sharedTracker);
            mode = (mode == InspectorMode.Normal ? InspectorMode.Debug : InspectorMode.Normal);
            //Debug.Log("New Inspector Mode: " + mode.ToString());

            MethodInfo method = type.GetMethod("SetMode", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(ActiveEditorTracker.sharedTracker, new object[] { mode });

            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
    }
}