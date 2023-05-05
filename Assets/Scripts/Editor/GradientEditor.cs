using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

[CustomEditor(typeof(GradientCreator))]
public class GradientEditor : Editor
{
    private GradientCreator creator;
    private void OnEnable()
    {
        creator = (GradientCreator)target;
    }

    private void OnSceneGUI()
    {
        Input();
    }
    
    private void Input()
    {
        var e = Event.current;
        
        if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
        {
            var mousePos = e.mousePosition;
            var ppp = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = Camera.current.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;
 
            var ray = Camera.current.ScreenPointToRay(mousePos);
            RaycastHit hit;
 
            if (Physics.Raycast(ray, out hit))
                creator.CreateGradient(hit.point);
            e.Use();
        }
    }
}
