namespace Quxios.Input {
  [System.Serializable]
  public class InputAction {
    public string key;

    public InputAction(string value) {
      key = value;
    }

    public static implicit operator string(InputAction selector) {
      return selector.key;
    }

    public static implicit operator InputAction(string str) {
      return new InputAction(str);
    }
  }
}