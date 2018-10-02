using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Quxios.Input {
  [CustomPropertyDrawer(typeof(InputAction))]
  public class InputActionDrawer : PropertyDrawer {
    private static List<string> keys;
    private static GUIContent[] contents;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      var key = property.FindPropertyRelative("key");
      var keys = GetKeys();
      int oldI = keys.IndexOf(key.stringValue);
      int i = EditorGUI.Popup(position, label, oldI, contents);
      key.stringValue = i >= 0 ? keys[i] : "";
    }

    private List<string> GetKeys() {
      if (keys == null) {
        keys = InputBindings.Instance.Dictionary.Keys.ToList();
        contents = keys.Select(s => new GUIContent(s)).ToArray();
      }
      return keys;
    }
  }
}