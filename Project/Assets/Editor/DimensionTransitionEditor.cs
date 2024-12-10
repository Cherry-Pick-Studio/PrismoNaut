using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DimensionTransition))]
public class DimensionTransitionEditor : Editor
{
    DimensionTransition creator;

    void OnSceneGUI()
    {
        if (creator == null)
            creator = (DimensionTransition)target;

        Draw();
    }

    void Draw()
    {
        Handles.color = Color.red;
        creator.endPoint = (Vector2)MoveTangentHandle(creator.endPoint);
        creator.UpdateBoxColliders();
        
        Handles.color = Color.blue;
        creator.startTangent = MoveTangentHandle(creator.startTangent);
        creator.endTangent = MoveTangentHandle(creator.endTangent);
    }

    private Vector3 MoveTangentHandle(Vector3 tangent)
    {
        Vector3 newPos = Handles.FreeMoveHandle(
            tangent, 
            0.5f, 
            Vector3.zero, 
            Handles.SphereHandleCap
        );

        if (tangent != newPos)
        {
            Undo.RecordObject(creator, "Move Tangent Handle");
            return newPos;
        }
        return tangent;
    }
}
