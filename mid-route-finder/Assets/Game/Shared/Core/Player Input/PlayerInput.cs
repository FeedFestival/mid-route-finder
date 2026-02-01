using R3;
using System;
using Game.Shared.Constants.Store;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces.Core;
using UnityEngine;

namespace Game.Shared.Core.Player_Input {

public static class PlayerInput {
    public static ExplorationActionMap ExplorationActionMap;
    public static MenuActionMap MenuActionMap;

    static InputMouse _inputMouse;
    static InputManager _input;

    static IDisposable _disposables;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeMethodLoad() {
        Debug.Log($"PlayerInput -> OnRuntimeMethodLoad: 0");
        _input = new InputManager();

        MenuActionMap = new(ref _input);

        _input.Disable();

        var gameplay = Store2.State.Gameplay.CurrentValue;
        onGameplayChange(gameplay);

        var d = Disposable.CreateBuilder();

        Store2.State.Gameplay.Subscribe(onGameplayChange).AddTo(ref d);

        _disposables = d.Build();
    }

    public static void SetInputMouse(IInputMouse inputMouse) {
        _inputMouse = inputMouse as InputMouse;
        _inputMouse!.constructor();

        ExplorationActionMap = new(ref _input, ref _inputMouse);
    }

    public static void Reset() {
        ExplorationActionMap.Dispose();
        ExplorationActionMap = null;

        MenuActionMap.Dispose();
        MenuActionMap = null;

        _inputMouse = null;
        _input = null;

        _disposables.Dispose();
    }

    static void onGameplayChange(Gameplay gameplay) {
        ExplorationActionMap?.Disable();
        MenuActionMap.Disable();

        switch (gameplay) {
            case Gameplay.Exploration:
                ExplorationActionMap?.Enable();
                break;
            case Gameplay.MainMenu:
                MenuActionMap.Enable();
                break;
            case Gameplay.Loading:
            default:
                // Everything remains disabled as stated at the beginning of the function
                break;
        }
    }
}

}
