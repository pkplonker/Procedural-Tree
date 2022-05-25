using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TreeDataSO))]

public class TreeDataSOEditor : Editor
{
    private TreeDataSO t;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        t = (TreeDataSO) target;
        EditorGUILayout.Space();
        LineBreak();
        EditorGUILayout.Space();
        // draw checkbox for the bool
        t.randomise = EditorGUILayout.Toggle("Randomise?", t.randomise);
        if (t.randomise) // if bool is true, show other fields
        {
            t.randomFactor =
                EditorGUILayout.Slider("Random Factor",t.randomFactor,t.MIN_RANDOM_FACTOR,t.MAX_RANDOM_FACTOR);
        }
    }

    private void LineBreak(int height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
