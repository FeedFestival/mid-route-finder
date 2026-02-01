using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Shared.Core.Player_Input {

public class ExplorationActionMap : MouseActionMap {
    public Action<Vector2, bool> Movement { get; set; }
    public Action<bool> Sprint { get; set; }
    public Action<bool> RightClick { get; set; }
    public Action<bool> SnapToCharacter { get; set; }

    readonly InputManager.ExplorationActions _actionMap;

    Subject<(Vector2 pos, bool cancel)> _movePerformed_s = new();

    IDisposable _disposables;
    readonly InputMouse _inputMouse;

    public ExplorationActionMap(
        ref InputManager inputManager,
        ref InputMouse inputMouse
    ) : base(ref inputMouse) {
        _actionMap = inputManager.Exploration;
        _inputMouse = inputMouse;

        _inputMouse.SetupForDrag();
        _actionMap.Fire.started += _inputMouse.clkStarted;
        _actionMap.Fire.canceled += _inputMouse.clkEnded;
        _actionMap.MousePosition.performed += _inputMouse.MouseMoved;
        _actionMap.MousePosition.canceled += _inputMouse.MouseMoved;

        _actionMap.Look.performed += _inputMouse.LookPerformed;
        _actionMap.Look.canceled += _inputMouse.LookCanceled;

        _actionMap.Movement.performed += movementPerformed;
        _actionMap.Movement.canceled += movementCanceled;

        _actionMap.Sprint.performed += sprintPerformed;
        _actionMap.Sprint.canceled += sprintCanceled;

        _actionMap.RightClick.performed += rightClickPerformed;
        _actionMap.RightClick.canceled += rightClickCanceled;

        _actionMap.SnapToCharacter.performed += snapToCharacterPerformed;
        _actionMap.SnapToCharacter.canceled += snapToCharacterCanceled;

        var d = Disposable.CreateBuilder();

        _movePerformed_s = new();
        _movePerformed_s
            .Debounce(TimeSpan.FromMilliseconds(50))
            .Do(((Vector2 pos, bool cancel) payload) => { Movement?.Invoke(payload.pos, payload.cancel); })
            .Subscribe()
            .AddTo(ref d);

        _disposables = d.Build();
    }

    public override void Dispose() {
        _inputMouse.DisposeForDrag();
        _actionMap.Fire.started -= _inputMouse.clkStarted;
        _actionMap.Fire.canceled -= _inputMouse.clkEnded;
        _actionMap.MousePosition.performed -= _inputMouse.MouseMoved;
        _actionMap.MousePosition.canceled -= _inputMouse.MouseMoved;

        _actionMap.Look.performed -= _inputMouse.LookPerformed;
        _actionMap.Look.canceled -= _inputMouse.LookCanceled;

        _actionMap.Movement.performed -= movementPerformed;
        _actionMap.Movement.canceled -= movementCanceled;

        _actionMap.Sprint.performed -= sprintPerformed;
        _actionMap.Sprint.canceled -= sprintCanceled;

        _actionMap.RightClick.performed -= rightClickPerformed;
        _actionMap.RightClick.canceled -= rightClickCanceled;

        _movePerformed_s.Dispose();
        _movePerformed_s = null;

        _disposables.Dispose();

        Movement = null;
    }

    void movementPerformed(InputAction.CallbackContext context) {
        var position = context.ReadValue<Vector2>();
        _movePerformed_s.OnNext((position, false));
    }

    void movementCanceled(InputAction.CallbackContext context) {
        var position = context.ReadValue<Vector2>();
        _movePerformed_s.OnNext((position, true));
    }

    void sprintPerformed(InputAction.CallbackContext context) => Sprint?.Invoke(true);
    void sprintCanceled(InputAction.CallbackContext context) => Sprint?.Invoke(false);

    void rightClickPerformed(InputAction.CallbackContext context) => RightClick?.Invoke(true);

    void rightClickCanceled(InputAction.CallbackContext context) => RightClick?.Invoke(false);

    void snapToCharacterPerformed(InputAction.CallbackContext ctx) => SnapToCharacter?.Invoke(true);
    void snapToCharacterCanceled(InputAction.CallbackContext ctx) => SnapToCharacter?.Invoke(false);

    public override void Enable() {
        _actionMap.Enable();
    }

    public override void Disable() {
        _actionMap.Disable();
    }
}

}
