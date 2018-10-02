using System.Collections.Generic;
using UnityEngine;

namespace Quxios.Input {
  [System.Serializable]
  public class InputActionInfo {
    public string group;
    public bool rebindable;

    public List<KeyCode> inputsKeyboard;
    public List<KeyCode> inputsGamepad;
  }
}