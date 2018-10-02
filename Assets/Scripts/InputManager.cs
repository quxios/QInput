using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quxios.Input {
  public class InputManager : MonoBehaviour {
    private static InputManager _current;
    public static InputManager current {
      get {
        if (_current == null) {
          GameObject obj = new GameObject();
          obj.name = "InputManager";
          _current = obj.AddComponent<InputManager>();
        }
        return _current;
      }
    }

    public InputDictionary bindings {
      get; private set;
    }

    private List<KeyCode> _codes;
    private List<KeyCode> codes {
      get {
        if (_codes == null) {
          _codes = new List<KeyCode>();
          foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode))) {
            _codes.Add(code);
          }
        }
        return _codes;
      }
    }

    public enum Mode {
      keyboard, gamepad
    }
    public Mode currentMode {
      get; private set;
    }

    private void Awake() {
      if (_current == null) {
        _current = this;
      } else if (_current != this) {
        Destroy(this.gameObject);
        return;
      }
      DontDestroyOnLoad(this);
      LoadBindings();
    }

    public void SaveBindings() {
      FileStream file = File.Create(Application.persistentDataPath + "input.dat");
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize(file, bindings);
      file.Close();
    }

    public void LoadBindings() {
      ResetBinds();
      if (File.Exists(Application.persistentDataPath + "input.dat")) {
        FileStream file = File.Open(Application.persistentDataPath + "input.dat", FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        bindings = (InputDictionary)bf.Deserialize(file);
        file.Close();
      }
    }

    private void Update() {
      KeyCode currKey = AnyKeyDown();
      if (currKey != KeyCode.None) {
        currentMode = (int)currKey >= 330 ? Mode.gamepad : Mode.keyboard;
        // TODO add a check for gamepad axis
      }
    }

    /// <summary>
    /// Returns true while the Input Action is held down.
    /// Similar to Unitys Input.GetKey() or Input.GetButton()
    /// </summary>
    /// <param name="inputAction">The Input Action to check for</param>
    public static bool GetAction(string inputAction) {
      if (!current.bindings.ContainsKey(inputAction)) {
        Debug.Log("The Input Action: " + inputAction + " was not found.");
        return false;
      }
      List<KeyCode> inputs;
      if (current.currentMode == Mode.gamepad) {
        inputs = current.bindings[inputAction].inputsGamepad;
      } else {
        inputs = current.bindings[inputAction].inputsKeyboard;
      }
      for (int i = 0; i < inputs.Count; i++) {
        if (UnityEngine.Input.GetKey(inputs[i])) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Returns true on the frame the Input Action was pressed.
    /// Similar to Unitys Input.GetKeyDown() or Input.GetButtonDown()
    /// </summary>
    /// <param name="inputAction">The Input Action to check for</param>
    public static bool GetActionDown(string inputAction) {
      if (!current.bindings.ContainsKey(inputAction)) {
        Debug.Log("The Input Action: " + inputAction + " was not found.");
        return false;
      }
      List<KeyCode> inputs;
      if (current.currentMode == Mode.gamepad) {
        inputs = current.bindings[inputAction].inputsGamepad;
      } else {
        inputs = current.bindings[inputAction].inputsKeyboard;
      }
      for (int i = 0; i < inputs.Count; i++) {
        if (UnityEngine.Input.GetKeyDown(inputs[i])) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Returns true if any of the passed keys were pressed this frame.
    /// </summary>
    /// <param name="inputActions">
    /// Collection of Input Actions strings to check. If null will check against all Input Actions
    /// </param>
    public static bool AnyActionDown(ICollection<string> inputActions = null) {
      if (inputActions == null) {
        inputActions = new List<string>(current.bindings.Keys);
      }
      foreach (string key in inputActions) {
        if (GetActionDown(key)) return true;
      }
      return false;
    }

    /// <summary>
    /// Returns true if any of the passed keys were pressed this frame.
    /// </summary>
    /// <param name="inputActions">
    /// Collection of Input Keys to check. If null will check against all Input Actions
    /// </param>
    public static bool AnyInputKeyDown(ICollection<InputAction> inputActions = null) {
      if (inputActions == null) {
        inputActions = new List<InputAction>(current.bindings.Keys.Select(s => new InputAction(s)));
      }
      foreach (InputAction key in inputActions) {
        if (GetActionDown(key)) return true;
      }
      return false;
    }

    /// <summary>
    /// Checks if any Key was pressed this frame. Returns The Input Actioncode of The Input Action that was
    /// pressed. Returns KeyCode.None if none were pressed
    /// </summary>
    /// <param name="keyCodes">Collection of keyCodes to check for. If null it will check all keyCodes</param>
    /// <returns></returns>
    public static KeyCode AnyKeyDown(ICollection<KeyCode> keyCodes = null) {
      if (keyCodes == null) {
        keyCodes = current.codes;
      }
      foreach (KeyCode code in keyCodes) {
        if (UnityEngine.Input.GetKeyDown(code)) {
          return code;
        }
      }
      return KeyCode.None;
    }

    /// <summary>
    /// Get The Input Actioncode of the Input Action as a string
    /// </summary>
    /// <param name="inputAction">Which Input Action</param>
    /// <returns></returns>
    public static string GetButtonName(string inputAction) {
      if (!current.bindings.ContainsKey(inputAction)) {
        Debug.Log("The Input Action: " + inputAction + " was not found.");
        return "";
      }
      List<KeyCode> inputs;
      if (current.currentMode == Mode.gamepad) {
        inputs = current.bindings[inputAction].inputsGamepad;
      } else {
        inputs = current.bindings[inputAction].inputsKeyboard;
      }
      return inputs.Count >= 0 ? inputs[0].ToString() : "";
    }

    public static InputActionInfo GetInputInfo(string inputAction) {
      if (!current.bindings.ContainsKey(inputAction)) {
        Debug.Log("The Input Action: " + inputAction + " was not found.");
        return null;
      }
      return current.bindings[inputAction];
    }

    public static void Rebind(string key, KeyCode code) {
      if (!current.bindings.ContainsKey(key)) {
        Debug.Log("The Input Action: " + key + " was not found.");
        return;
      }
      List<KeyCode> inputs;
      if (code.ToString().Contains("Joystick")) {
        inputs = current.bindings[key].inputsGamepad;
      } else {
        inputs = current.bindings[key].inputsKeyboard;
      }
      inputs[0] = code;
    }

    public static void ResetBinds() {
      current.bindings = new InputDictionary();
      foreach (var item in InputBindings.Instance.Dictionary) {
        InputActionInfo clone = new InputActionInfo();
        clone.group = item.Value.group;
        clone.rebindable = item.Value.rebindable;
        clone.inputsKeyboard = new List<KeyCode>(item.Value.inputsKeyboard);
        clone.inputsGamepad = new List<KeyCode>(item.Value.inputsGamepad);
        current.bindings[item.Key] = clone;
      }
    }


  }
}
