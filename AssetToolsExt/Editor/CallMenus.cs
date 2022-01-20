
using UnityEngine;
using UnityEditor;
using AlexDoveTools;

public class callMenus
{
    [MenuItem("AssetTools/Transform/RoundPosition")]
    [MenuItem("GameObject/AssetTools/Transform/RoundPosition", false, 0)]
    [MenuItem("CONTEXT/Transform/RoundPosition")]
    public static void RoundPosition()
    {
        GameObject ago = Selection.activeGameObject;
        ago.transform.RoundPos(2);
    }
}
