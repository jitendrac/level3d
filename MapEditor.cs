/************************************************************************
 *
 *  Copyright (c) 2012 Mario Rodríguez <yetatore@hush.com>
 *  This work is free. You can redistribute it and/or modify it under the
 *  terms of the Do What The Fuck You Want To Public License, Version 2,
 *  as published by Mario Rodríguez.
 *  See the copying.txt file for more details.
 *
 ***********************************************************************/

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
 
public class MapEditor : EditorWindow
{
    // Extern-Fruit
    private static MapEditor window = null;
    SceneView.OnSceneFunc onSceneGUIFunc = null;

    // Properties
    private static bool editing  = false;
    private static int  aliasing = 1;
    private static int  sizeX    = 0;
    private static int  sizeY    = 0;
    private static int  button   = -1;

    // Menu location
    [MenuItem("Terrain/MapEditor")]

    // Inicialización
    static void Init()
    {
        if (window == null)
        {
            window = EditorWindow.GetWindow<MapEditor >(false, "MapEditor", true);
            window.onSceneGUIFunc = new SceneView.OnSceneFunc(OnSceneGUI);
            SceneView.onSceneGUIDelegate += window.onSceneGUIFunc;
        }
    }
 
    // Destrucción
    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= onSceneGUIFunc;
        window = null;
    }
 
    // Update - GUI
    public void OnGUI()
    {
        editing = GUILayout.Toggle(editing, "Editing");
        if (!GameObject.Find("Plane"))
        {	
            if (!int.TryParse(GUILayout.TextField(sizeX.ToString()), out sizeX))
                sizeX = 0;
            if (!int.TryParse(GUILayout.TextField(sizeY.ToString()), out sizeY))
            	sizeY = 0;
            if (GUILayout.Button("CREATE!") && sizeX > 0 && sizeY > 0)
            {
                loadResource ("Plane", new Vector3(sizeX * 5, sizeY * 5, 5), new Vector3(sizeX, 1, sizeY));
                loadResource ("GlobalLight", Vector3.zero, Vector3.zero);
                loadResource ("Camera", Vector3.zero, Vector3.zero);
            }
        }
    }

    // Update - Scene
    public static void OnSceneGUI(SceneView sceneview)
    {
        if (Event.current.type == EventType.mouseDown)
            button = Event.current.button;
        if (Event.current.type == EventType.mouseUp)
            button = -1;
        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.N)
            editing = !editing;
        if (button > -1 && editing)
        {			
            editPrefab(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
            Event.current.Use();
        }
    }

    // Prefab instantiate
    private static void editPrefab (Ray worldRay)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(worldRay, out hitInfo))
        {
            if (hitInfo.collider.name == "Plane" && button == 0)
            {
                GameObject obj = (GameObject)Instantiate(Resources.Load("Cube"));
                obj.transform.position = new Vector3(normal(hitInfo.point.x), normal(hitInfo.point.y), normal(0.0f));
                obj.transform.localScale = new Vector3(aliasing, aliasing, 5);
            }
            if (hitInfo.collider.name != "Plane" && button == 1)
                DestroyImmediate(hitInfo.collider.gameObject);
        }
    }

    // Positioning
    private static float normal (float pos)
    {
        if (pos < 0)
            Debug.Log("Fuera de rango.");
        return pos + ((aliasing / 2f) - (pos % aliasing));
    }

    // LoadingEditorResource
    private void loadResource (string name, Vector3 position, Vector3 scale)
    {
        GameObject obj = (GameObject)Instantiate(Resources.Load(name));
        obj.name = name;
        obj.transform.position = position;
        if (scale != Vector3.zero)
            obj.transform.localScale = scale;
    }
    
}
