using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quxios.Input {
  [CreateAssetMenu(fileName = "InputBindings", menuName = "Config/InputBindings", order = 0)]
  public class InputBindings : ScriptableObject, ISerializationCallbackReceiver {
    private static InputBindings _instance;
    public static InputBindings Instance {
      get {
        if (_instance == null) {
          _instance = Resources.Load<InputBindings>("InputBindings");
          if (_instance == null) {
            Debug.LogWarning("No instance of InputBinding found in Resources.");
            _instance = CreateInstance<InputBindings>();
          }
        }
        return _instance;
      }
    }

    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<InputActionInfo> values = new List<InputActionInfo>();

    [SerializeField] private InputDictionary _dictionary = new InputDictionary();
    public InputDictionary Dictionary {
      get {
        return _dictionary;
      }
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize() {
      _dictionary = new InputDictionary();
      for (int i = 0; i < keys.Count; i++) {
        if (i >= values.Count) break;
        if (_dictionary.ContainsKey(keys[i])) {
          _dictionary[keys[i]].inputsKeyboard.AddRange(values[i].inputsKeyboard);
          _dictionary[keys[i]].inputsGamepad.AddRange(values[i].inputsGamepad);
        } else {
          _dictionary.Add(keys[i], values[i]);
        }
      }
    }
  }

  [System.Serializable]
  public class InputDictionary : Dictionary<string, InputActionInfo> { }
}
