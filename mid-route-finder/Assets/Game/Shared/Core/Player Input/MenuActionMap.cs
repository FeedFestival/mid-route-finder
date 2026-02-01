using System;
using UnityEngine.InputSystem;

namespace Game.Shared.Core.Player_Input {

public class MenuActionMap: BaseActionMap {
    readonly InputManager.MenuActions _actionMap;

    public Action Close { get; set; }

    public MenuActionMap(
        ref InputManager inputManager
    ) {
        _actionMap = inputManager.Menu;

        _actionMap.Close.performed += closePerformed;
    }

    public override void Dispose() {
        _actionMap.Close.performed -= closePerformed;
    }

    void closePerformed(InputAction.CallbackContext ctx) {
        if (ctx.performed)
            Close?.Invoke();
    }

    public override void Enable() {
        _actionMap.Enable();
    }

    public override void Disable() {
        _actionMap.Disable();
    }
}

}
