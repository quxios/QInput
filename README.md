Made with Unity 2018.x

# About
An wrapper for Unity's Input class. This wrapper will allow you to rebind keys at runtime.

# How to use
## Input Bindings
First you must have an `InputBindings` scriptable object inside a `Resource` folder or it wont work correct! To create an `InputBindings`scriptable object click on `Assets/Config/InputBindings`

Once it's created go ahead and create some Input Actions. Input Actions will act like Unity's Input Buttons. 

## Using new Inputs
To use these Input Actions you created you need to use the new `InputManager` class, if an existing function just replace the `Input` functions with the related `InputManager` function.

The functions:
- `Quxios.Input.InputManager.GetAction(string)` returns true while any input from that action is pressed. Similar to Unity's `Input.GetButton(string)`
- `Quxios.Input.InputManager.GetActionDown(string)` returns true when any input from that action is first pressed. Similar to Unity's `Input.GetButtonDown(string)`
- `Quxios.Input.InputManager.AnyActionDown(List<string>)` returns true if any of that actions were pressed this frame. If argument is left out or null it will check against all Input Actions.
- `Quxios.Input.InputManager.AnyKeyDown(List<KeyCode>)` returns true if any of the key codes were pressed this frame. If argument is left out or null it will check against all KeyCodes.
- `Quxios.Input.InputManager.GetButtonName(string)` returns the name of the first input KeyCode of the passed input action.
- `Quxios.Input.InputManager.Rebind(string, KeyCode)` rebinds only the first input KeyCode of the passed input action.
- `Quxios.Input.InputManager.SaveBindings()` saves the current inputs bindings to a file named `input.dat` inside the [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html)
- `Quxios.Input.InputManager.LoadBindings()` load the inputs bindings from a file named `input.dat` inside the [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html)
- `Quxios.Input.InputManager.ResetBinds()` resets all input bindings to default values.

# Examples Folder
The examples folder contains a scene that uses the `InputRow` component to show how you can rebind inputs.

# Support
[Patreon](https://www.patreon.com/quxios)

[Ko-fi](https://ko-fi.com/quxios)