using UnityEngine;
using TMPro;
using Quxios.Input;

public class DrawInputKey : MonoBehaviour {
  public InputAction key;
  private TextMeshProUGUI text;

  private void Awake() {
    text = GetComponent<TextMeshProUGUI>();
  }

  private void Update() {
    text.text = InputManager.GetButtonName(key);
  }
}