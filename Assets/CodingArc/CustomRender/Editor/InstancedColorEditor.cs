using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InstancedColor))]
class InstancedColorEditor : Editor
{
    private InstancedColor mTargetCache;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(mTargetCache == null)
        {
            mTargetCache = target as InstancedColor;
        }
        EditorGUILayout.BeginVertical();
        if(GUILayout.Button("Random Color"))
        {
            mTargetCache.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            mTargetCache.Refresh();
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
