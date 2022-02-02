using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



//[CustomEditor(typeof(DragAndDrop))]
//[CustomEditor(typeof(SpriteDragnDropOverride))]
public class SpriteDragnDropOverride : Editor
{
    //private static bool AllowOverride = false;
    private GameObject draggedObj;

    [MenuItem("AssetTools/SpriteDragnDropOverride/Enable")]//%#o
    private static void OverrideTrue()
    {
        //AllowOverride = true;
        Debug.Log($"<color=\"blue\">OverrideTrue</color>");
    }
    [MenuItem("AssetTools/SpriteDragnDropOverride/Disable")]//%#o
    private static void OverrideFalse()
    {
        //AllowOverride = false;
        Debug.Log($"<color=\"blue\">OverrideFalse</color>");
    }
    //private GameObject draggedObj;

    private void OnSceneGUI()
    {       
        Debug.Log($"<color=\"red\">OnSceneGUI</color>");
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {
            Debug.Log($"<color=\"blue\">Dragging</color>");//{ var }
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // show a drag-add icon on the mouse cursor

            if (draggedObj == null)
                draggedObj = (GameObject) Object.Instantiate(DragAndDrop.objectReferences[0]);

            // compute mouse position on the world y=0 plane
            Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y, 0.0f));
            if (mouseRay.direction.y < 0.0f)
            {
                float t = -mouseRay.origin.y / mouseRay.direction.y;
                Vector3 mouseWorldPos = mouseRay.origin + t * mouseRay.direction;
                mouseWorldPos.y = 0.0f;

                draggedObj.transform.position = new Vector2(33, 33); //terrain.SnapToNearestTileCenter(mouseWorldPos);
            }

            if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                draggedObj = null;
            }

            Event.current.Use();
        }
    }
}
