using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FilePathSelectorAttribute))]
public class FilePathSelectorPropertyDrawer : PropertyDrawer
{

    private Action delayedAction;
    public void delay() {
        delayedAction();
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){

        FilePathSelectorAttribute filePathSelectorAttribute = (FilePathSelectorAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        int buttonWidth = 30;
        Rect textFieldPos = new Rect(position.x, position.y, position.width - buttonWidth, position.height);
        Rect buttonPos = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

        EditorGUI.PropertyField(textFieldPos, property, label);

        if (GUI.Button(buttonPos, "...")){
            delayedAction = new Action(() => {
                string path = EditorUtility.OpenFilePanel(filePathSelectorAttribute.Title, "", filePathSelectorAttribute.Extension);
                if (!string.IsNullOrEmpty(path)){
                    property.stringValue = path;
                    property.serializedObject.ApplyModifiedProperties();
                }
            });

            EditorApplication.delayCall += delay;

        }

        EditorGUI.EndProperty();
    }
}
