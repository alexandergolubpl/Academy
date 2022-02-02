using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum AlignType
{
    Left = 1,
    HorizontalCenter = 2, //Center horizontally
    Right = 3,
    Top = 4,
    VerticalCenter = 5, //Center vertically
    Bottom = 6,
    Horizontal = 7, //Horizontal distribution
    Vertical = 8, //Vertical distribution
}

public class UGUIAlign : Editor
{
    static void AlignLeft()
    {
        Align(AlignType.Left);
    }
    //[MenuItem("GameObject/UI/Align/HorizontalCenter [horizontal center]")]
    static void AlignHorizontalCenter()
    {
        Align(AlignType.HorizontalCenter);
    }
    //[MenuItem("GameObject/UI/Align/Right [Align Right]")]
    static void AlignRight()
    {
        Align(AlignType.Right);
    }
    //[MenuItem("GameObject/UI/Align/Top [Align Top]")]
    static void AlignTop()
    {
        Align(AlignType.Top);
    }
    //[MenuItem("GameObject/UI/Align/VerticalCenter [Vertical Center]")]
    static void AlignVerticalCenter()
    {
        Align(AlignType.VerticalCenter);
    }
    //[MenuItem("GameObject/UI/Align/Bottom [Bottom Alignment]")]
    static void AlignBottom()
    {
        Align(AlignType.Bottom);
    }
    //[MenuItem("GameObject/UI/Align/Horizontal [Horizontal Distribution]")]
    static void AlignHorizontal()
    {
        Align(AlignType.Horizontal);
    }
    //[MenuItem("GameObject/UI/Align/Vertical [Vertical Distribution]")]
    static void AlignVertical()
    {
        Align(AlignType.Vertical);
    }

    public static void Align(AlignType type)
    {
        List<RectTransform> rects = new List<RectTransform>();
        GameObject[] objects = Selection.gameObjects;
        if (objects != null && objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                RectTransform rect = objects[i].GetComponent<RectTransform>();
                if (rect != null)
                    rects.Add(rect);
            }
        }

        if (rects.Count > 1)
        {
            Align(type, rects);
        }
    }

    public static void Align(AlignType type, List<RectTransform> rects)
    {
        RectTransform tenplate = rects[0];

        //for (int i = 0; i < rects.Count; i++)
        //{
        //    if (tenplate.sizeDelta.x < rects[i].sizeDelta.x)
        //    {
        //        tenplate = rects[i];
        //    }
        //}
        float w = tenplate.sizeDelta.x * tenplate.lossyScale.x;
        float h = tenplate.sizeDelta.y * tenplate.localScale.y;

        float x = tenplate.position.x - tenplate.pivot.x * w;
        float y = tenplate.position.y - tenplate.pivot.y * h;

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Align Objects");
        var undoGroupIndex = Undo.GetCurrentGroup();

        switch (type)
        {
            case AlignType.Top:
                for (int i = 0; i < rects.Count; i++)
                {
                    if (rects[i] == tenplate)
                    {
                        continue;
                    }
                    Undo.RegisterUndo(rects[i], "Align");
                    RectTransform trans = rects[i];
                    float th = trans.sizeDelta.y * trans.localScale.y;
                    Vector3 pos = trans.position;
                    pos.y = y + h - th + trans.pivot.y * th;
                    trans.position = pos;
                }
                break;
            case AlignType.Left:
                for (int i = 0; i < rects.Count; i++)
                {
                    if (rects[i] == tenplate)
                    {
                        continue;
                    }
                    Undo.RegisterUndo(rects[i], "Align");
                    RectTransform trans = rects[i];
                    float tw = trans.sizeDelta.x * trans.lossyScale.x;
                    Vector3 pos = trans.position;
                    pos.x = x + tw * trans.pivot.x;
                    trans.position = pos;
                }
                break;
            case AlignType.Right:
                for (int i = 0; i < rects.Count; i++)
                {
                    if (rects[i] == tenplate)
                    {
                        continue;
                    }
                    Undo.RegisterUndo(rects[i], "Align");
                    RectTransform trans = rects[i];
                    float tw = trans.sizeDelta.x * trans.lossyScale.x;
                    Vector3 pos = trans.position;
                    pos.x = x + w - tw + tw * trans.pivot.x;
                    trans.position = pos;
                }
                break;
            case AlignType.Bottom:
                for (int i = 0; i < rects.Count; i++)
                {
                    if (rects[i] == tenplate)
                    {
                        continue;
                    }
                    Undo.RegisterUndo(rects[i], "Align");
                    RectTransform trans = rects[i];
                    float th = trans.sizeDelta.y * trans.localScale.y;
                    Vector3 pos = trans.position;
                    pos.y = y + th * trans.pivot.y;
                    trans.position = pos;
                }
                break;
            case AlignType.HorizontalCenter:
                for (int i = 0; i < rects.Count; i++)
                {
                    if (rects[i] == tenplate)
                    {
                        continue;
                    }
                    Undo.RegisterUndo(rects[i], "Align");
                    RectTransform trans = rects[i];
                    float tw = trans.sizeDelta.x * trans.lossyScale.x;
                    Vector3 pos = trans.position;
                    pos.x = x + 0.5f * w - 0.5f * tw + tw * trans.pivot.x;
                    trans.position = pos;
                }
                break;
            case AlignType.VerticalCenter:
                for (int i = 0; i < rects.Count; i++)
                {
                    if (rects[i] == tenplate)
                    {
                        continue;
                    }
                    Undo.RegisterUndo(rects[i], "Align");
                    RectTransform trans = rects[i];
                    float th = trans.sizeDelta.y * trans.localScale.y;
                    Vector3 pos = trans.position;
                    pos.y = y + 0.5f * h - 0.5f * th + th * trans.pivot.y;
                    trans.position = pos;
                }
                break;
            case AlignType.Horizontal:
                float minX = GetMinX(rects);
                float maxX = GetMaxX(rects);
                rects.Sort(SortListRectTransformByX);
                float distance = (maxX - minX) / (rects.Count - 1);
                for (int i = 0; i < rects.Count - 1; i++)
                {
                    if (rects[i] == tenplate)
                    {
                        continue;
                    }
                    Undo.RegisterUndo(rects[i], "Align");
                    RectTransform trans = rects[i];
                    Vector3 pos = trans.position;
                    pos.x = minX + i * distance;
                    trans.position = pos;
                }
                break;
            case AlignType.Vertical:
                float minY = GetMinY(rects);
                float maxY = GetMaxY(rects);
                rects.Sort(SortListRectTransformByY);
                float distanceY = (maxY - minY) / (rects.Count - 1);
                for (int i = 0; i < rects.Count - 1; i++)
                {
                    if (rects[i] == tenplate)
                    {
                        continue;
                    }
                    Undo.RegisterUndo(rects[i], "Align");
                    RectTransform trans = rects[i];
                    Vector3 pos = trans.position;
                    pos.y = minY + i * distanceY;
                    trans.position = pos;
                }
                break;
        }
    }

    private static int SortListRectTransformByX(RectTransform r1, RectTransform r2)
    {
        float w = r1.sizeDelta.x * r1.lossyScale.x;
        float x1 = r1.position.x - r1.pivot.x * w;
        w = r2.sizeDelta.x * r2.lossyScale.x;
        float x2 = r2.position.x - r2.pivot.x * w;
        if (x1 >= x2)
            return 1;
        else
            return -1;
    }

    private static int SortListRectTransformByY(RectTransform r1, RectTransform r2)
    {
        float w = r1.sizeDelta.y * r1.lossyScale.y;
        float y1 = r1.position.y - r1.pivot.y * w;
        w = r2.sizeDelta.y * r2.lossyScale.y;
        float y2 = r2.position.y - r2.pivot.y * w;
        if (y1 >= y2)
            return 1;
        else
            return -1;
    }

    private static float GetMinX(List<RectTransform> rects)
    {
        if (null == rects || rects.Count == 0)
            return 0;
        RectTransform tenplate = rects[0];
        float minx = tenplate.position.x;
        float tempX = 0;
        for (int i = 1; i < rects.Count; i++)
        {
            tempX = rects[i].position.x;
            if (tempX < minx)
                minx = tempX;
        }
        return minx;
    }

    private static float GetMaxX(List<RectTransform> rects)
    {
        if (null == rects || rects.Count == 0)
            return 0;
        RectTransform tenplate = rects[0];
        float maxX = tenplate.position.x;
        float tempX = 0;
        for (int i = 1; i < rects.Count; i++)
        {
            tempX = rects[i].position.x;
            if (tempX > maxX)
                maxX = tempX;
        }
        return maxX;
    }

    private static float GetMinY(List<RectTransform> rects)
    {
        if (null == rects || rects.Count == 0)
            return 0;
        RectTransform tenplate = rects[0];
        float minY = tenplate.position.y;
        float tempX = 0;
        for (int i = 1; i < rects.Count; i++)
        {
            tempX = rects[i].position.y;
            if (tempX < minY)
                minY = tempX;
        }
        return minY;
    }

    private static float GetMaxY(List<RectTransform> rects)
    {
        if (null == rects || rects.Count == 0)
            return 0;
        RectTransform tenplate = rects[0];
        float maxY = tenplate.position.y;
        float tempX = 0;
        for (int i = 1; i < rects.Count; i++)
        {
            tempX = rects[i].position.y;
            if (tempX > maxY)
                maxY = tempX;
        }
        return maxY;
    }
    private static Vector3 FindCenterOfTransforms(List<Transform> transforms)
    {
        var bound = new Bounds(transforms[0].position, Vector3.zero);
        for (int i = 1; i < transforms.Count; i++)
        {
            bound.Encapsulate(transforms[i].position);
        }
        return bound.center;
    }
}