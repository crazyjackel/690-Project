using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ValidationToken)), CanEditMultipleObjects]
public class ValidationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Calculate Version Strings", EditorStyles.miniButton)){

            if (serializedObject.targetObject is ValidationToken token)
            {
                token.CalcVersion();
                token.CalcHashedString();
            }
        }
    }
}
