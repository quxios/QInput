using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Quxios.Input {
  [CustomEditor(typeof(InputBindings))]
  public class InputBindingsEditor : Editor {
    private SerializedProperty keysProp;
    private SerializedProperty valuesProp;

    private string newKey;
    private bool waitingForInput;
    private string inputForProp;
    private int inputForKeyI;
    private int inputForValueI;

    private string prevFocus;

    private void OnEnable() {
      keysProp = serializedObject.FindProperty("keys");
      valuesProp = serializedObject.FindProperty("values");
    }

    private void OnDisable() {
      waitingForInput = false;
    }

    public override bool RequiresConstantRepaint() {
      return waitingForInput;
    }

    public override void OnInspectorGUI() {
      serializedObject.Update();
      int delKey = -1;
      for (int i = 0; i < keysProp.arraySize; i++) {
        SerializedProperty key = keysProp.GetArrayElementAtIndex(i);
        SerializedProperty value = valuesProp.GetArrayElementAtIndex(i);
        SerializedProperty group = value.FindPropertyRelative("group");
        SerializedProperty rebindable = value.FindPropertyRelative("rebindable");
        EditorGUILayout.BeginHorizontal();
        key.isExpanded = EditorGUILayout.Foldout(key.isExpanded, key.stringValue);
        if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUIStyle.none, GUILayout.Width(20))) {
          GUI.FocusControl("");
          delKey = i;
        }
        EditorGUILayout.EndHorizontal();
        if (key.isExpanded) {
          EditorGUI.indentLevel++;
          EditorGUILayout.PropertyField(key, new GUIContent("Name"));
          EditorGUILayout.PropertyField(group);
          EditorGUILayout.PropertyField(rebindable);
          InputList("Keyboard inputs", value, "inputsKeyboard", i);
          InputList("Gamepad inputs", value, "inputsGamepad", i);
          EditorGUI.indentLevel--;
        }
      }
      if (delKey >= 0) {
        keysProp.DeleteArrayElementAtIndex(delKey);
      }

      GUILayout.Space(20);

      EditorGUILayout.BeginHorizontal();
      GUI.SetNextControlName("NewInput");
      newKey = EditorGUILayout.TextField("New Action", newKey);
      if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUIStyle.none, GUILayout.Width(20))) {
        int i = keysProp.arraySize++;
        keysProp.GetArrayElementAtIndex(i).stringValue = newKey;
        valuesProp.arraySize = keysProp.arraySize;
        SerializedProperty value = valuesProp.GetArrayElementAtIndex(i);
        value.FindPropertyRelative("group").stringValue = "";
        value.FindPropertyRelative("rebindable").boolValue = true;
        value.FindPropertyRelative("inputsKeyboard").arraySize = 1;
        value.FindPropertyRelative("inputsGamepad").arraySize = 1;
        newKey = "";
      }
      EditorGUILayout.EndHorizontal();

      if (waitingForInput) {
        bool focusedChanged = GUI.GetNameOfFocusedControl() != prevFocus;
        if (focusedChanged) {
          prevFocus = GUI.GetNameOfFocusedControl();
        }
        if (focusedChanged || Event.current.type == EventType.MouseDown) {
          waitingForInput = false;
        } else if (Event.current.type == EventType.KeyUp) {
          waitingForInput = false;
          SerializedProperty inputs = valuesProp.GetArrayElementAtIndex(inputForKeyI).FindPropertyRelative(inputForProp);
          inputs.GetArrayElementAtIndex(inputForValueI).intValue = (int)Event.current.keyCode;
          Repaint();
        }
      }
      serializedObject.ApplyModifiedProperties();
    }

    private void InputList(string label, SerializedProperty value, string prop, int keyI) {
      SerializedProperty inputs = value.FindPropertyRelative(prop);
      if (inputs.arraySize == 0) {
        inputs.arraySize++;
      }
      int delValue = -1;
      GUILayout.Label(label);
      EditorGUI.indentLevel++;
      for (int j = 0; j < inputs.arraySize; j++) {
        EditorGUILayout.BeginHorizontal();
        int currentIndex = inputs.GetArrayElementAtIndex(j).intValue;
        KeyCode currentCode = (KeyCode)currentIndex;
        if (waitingForInput && inputForProp == prop && inputForKeyI == keyI && inputForValueI == j) {
          GUILayout.Label("-WAITING FOR INPUT-", EditorStyles.miniButton);
        } else {
          KeyCode tempCode = (KeyCode)EditorGUILayout.EnumPopup(currentCode);
          if (tempCode != currentCode) {
            inputs.GetArrayElementAtIndex(j).intValue = (int)tempCode;
          }
        }
        if (GUILayout.Button("Press", EditorStyles.miniButton, GUILayout.Width(50))) {
          GUI.FocusControl("");
          waitingForInput = true;
          inputForProp = prop;
          inputForKeyI = keyI;
          inputForValueI = j;
        }
        if (j > 0 && GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUIStyle.none, GUILayout.Width(20))) {
          GUI.FocusControl("");
          delValue = j;
        }
        EditorGUILayout.EndHorizontal();
      }
      if (delValue >= 0) {
        inputs.DeleteArrayElementAtIndex(delValue);
      }
      EditorGUI.indentLevel--;
      EditorGUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("Add another input");
      if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUIStyle.none, GUILayout.Width(20))) {
        GUI.FocusControl("");
        int newJ = inputs.arraySize++;
        inputs.GetArrayElementAtIndex(newJ).intValue = 0;
      }
      EditorGUILayout.EndHorizontal();
    }
  }
}