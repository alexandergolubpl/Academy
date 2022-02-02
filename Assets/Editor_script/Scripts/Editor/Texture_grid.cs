using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.UI;
using System.IO;

using UnityEngine.EventSystems;
using UnityEngine.Events;

[ExecuteAlways]
public class Texture_grid : EditorWindow
{
    private Color paintColor = Color.black;
    private Color eraseColor = Color.white;
    private Color _currColor = Color.black;

    private int _currButtonNum;


    public GameObject targetObject;

    private string imagePath;

    private string TextureSrc;

    private float UIshortwidth;

    private Object TextureFolder;
    private string TextureFolderPath;

    private Vector2 gridSize = new Vector2(12, 12);
    private List<Color> _tileColor = new List<Color>(144);
    private int tileSize = 24;
    private int _currMouseButton;
    private int _currPixelStep = 1;

    private bool _styleInitialized = false;
    private GUIStyle style;
    private void OnDisable()
    {
        _styleInitialized = false;
    }
    private void InitStyles()
    {
        if (EditorStyles.helpBox == null)
        {
            style = new GUIStyle(GUI.skin.box);
        }
        else
        {
            style = new GUIStyle(EditorStyles.helpBox);
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);
            style.border = new RectOffset(0, 0, 0, 0);
        }
        _styleInitialized = true;
    }

    [MenuItem("Tools/Paint Object")]
    [MenuItem("GameObject/Paint Object")]
    private static void OpenWin()
    {
        EditorWindow.GetWindow<Texture_grid>("Texture Grid");
    }
    void Start()
    {
        Debug.Log(_tileColor.Count);
        imagePath = "Assets";
        targetObject = (Selection.activeGameObject != null) ? Selection.activeGameObject : null;
    }
    void CreateFields()
    {
        using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
        {
            using (new GUILayout.VerticalScope("HelpBox"))
            {
                GUILayout.Space(UIshortwidth);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(UIshortwidth);
                }
                GUILayout.Space(UIshortwidth);

                paintColor = EditorGUILayout.ColorField("Paint: ", paintColor, GUILayout.ExpandWidth(true));
                eraseColor = EditorGUILayout.ColorField("Erase: ", eraseColor, GUILayout.ExpandWidth(true));
                GUILayout.Space(UIshortwidth);
                if (GUILayout.Button("Fill"))
                {
                    FillTiles();
                }
                GUILayout.FlexibleSpace();
                targetObject = (GameObject)EditorGUILayout.ObjectField("targetObject", targetObject, typeof(GameObject), true);
                GUILayout.Space(UIshortwidth);
                if (targetObject == null)
                {
                    GUI.backgroundColor = new Color(1, 0, 0);
                    GUI.enabled = false;
                }
                else
                {
                    GUI.backgroundColor = new Color(1, 1, 1);
                }
                if (GUILayout.Button("Apply To Object"))
                {
                    ApplyToObject();
                }
                GUI.enabled = true;
                GUI.backgroundColor = new Color(1, 1, 1);
            }
            using (new GUILayout.VerticalScope("HelpBox"))
            {
                using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
                {
                    CreateGrid();
                }
            }
        }

    }
    void FillTiles()
    {
        for (int i = 0; i < _tileColor.Count; i++)
        {
            _tileColor[i] = paintColor;
        }
    }
    void CreateGrid()
    {
        var indx = 0;
        using (new GUILayout.VerticalScope("HelpBox"))
        {
            for (int iy = 0; iy < gridSize.y; iy++)
            {
                using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                {
                    for (int ix = 0; ix < gridSize.x; ix++)
                    {
                        _tileColor.Add(GetColor(-1));
                        indx++;
                        if (GUILayout.Button(DrawTexture2D(ix, iy, indx), GUILayout.Width(32), GUILayout.Height(32)))
                        {
                            _currButtonNum = indx;
                            if (_currMouseButton == 0)
                            {
                                _tileColor[_currButtonNum] = paintColor;
                            }
                            if (_currMouseButton == 1)
                            {
                                _tileColor[_currButtonNum] = eraseColor;
                            }
                        }
                    }
                }
            }
        }
    }
    Color GetColor(int indx)
    {
        if (indx < 0)
        {
            return new Color(Random.RandomRange(0f, 1f), Random.RandomRange(0f, 1f), Random.RandomRange(0f, 1f));
        }
        else
        {
            return _tileColor[indx];
        }
    }

    private Image CreateImage(Texture2D _texture)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(imagePath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + imagePath);

        return null;
    }
    private Texture DrawTexture2D(int ix, int iy, int indx)
    {
        //_curColor;
        Texture2D texture2D = new Texture2D(tileSize, tileSize);
        //targetObject.GetComponent<Renderer>().material.mainTexture = texture;
        var pix = 0;
        for (int y = 0; y < texture2D.height; y++)
        {
            for (int x = 0; x < texture2D.width; x++)
            {
                texture2D.SetPixel(x, y, GetColor(indx));
                pix++;
            }
        }
        texture2D.Apply();
        return texture2D;
    }
    private Texture DrawMapTexture2D()
    {
        //_curColor;

        Texture2D texture2D = new Texture2D(tileSize, tileSize);
        var indx = 0;
        var localy = 0;
        var localx = 0;
        for (int ly = 0; ly < tileSize; ly++)
        {
            for (int lx = 0; lx < tileSize; lx++)
            {
                texture2D.SetPixel(lx, ly, GetColor(indx));
                indx++;
            }
        }

        texture2D.Apply();
        return texture2D;
    }
    void ApplyToObject()
    {
        targetObject.GetComponent<Renderer>().material.mainTexture = DrawMapTexture2D();
    }

    void OnGUI()
    {
        CheckPressKey();
        CreateFields();
    }


    void CheckPressKey()
    {
        Event e = Event.current;
        _currMouseButton = e.button;
        if (e.button == 0 && e.isMouse)
        {
            //Debug.Log("Left Click");
        }
        else if (e.button == 1)
        {
            //Debug.Log("Right Click");
        }
        else if (e.button == 2)
        {
            //Debug.Log("Middle Click");
        }
        else if (e.button > 2)
        {
            //Debug.Log("Another button in the mouse clicked");
        }
    }

}
