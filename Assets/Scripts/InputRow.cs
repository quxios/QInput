using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Quxios.Input {
  public class InputRow : MonoBehaviour {
    [SerializeField] private InputAction inputAction;
    [SerializeField] private TextMeshProUGUI inputName;
    [SerializeField] private Button btn;
    private TextMeshProUGUI btnText;

    [SerializeField] List<InputAction> inputActionBlackList;

    private string key;

    private bool waitingForInput;

    private void Start() {
      Set(inputAction);
    }

    public void Set(string input) {
      key = input;
      inputName.text = input;
      var info = InputManager.GetInputInfo(input);
      if (info == null) return;
      btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
      btnText.text = InputManager.GetButtonName(input);
      btn.interactable = info.rebindable;
      btn.onClick.RemoveAllListeners();
      btn.onClick.AddListener(OnClick);
    }

    private void OnClick() {
      btnText.text = "";
      waitingForInput = true;
    }

    private void Update() {
      if (waitingForInput) {
        if (InputManager.AnyInputKeyDown(inputActionBlackList)) {
          btnText.text = InputManager.GetButtonName(key);
          waitingForInput = false;
          return;
        }
        KeyCode code = InputManager.AnyKeyDown();
        if (code != KeyCode.None) {
          InputManager.Rebind(key, code);
          btnText.text = InputManager.GetButtonName(key);
          waitingForInput = false;
        }
      }
    }

  }
}