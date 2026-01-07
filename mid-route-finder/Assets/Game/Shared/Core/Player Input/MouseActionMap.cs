using System;
using UnityEngine;

namespace Game.Shared.Core.Player_Input {

public class MouseActionMap : BaseActionMap {
    public Action<Vector2> MousePosition {
        get => _inputMouse.MousePosition;
        set => _inputMouse.MousePosition = value;
    }

    public Action<Vector2Int> MousePositionInt {
        get => _inputMouse.MousePositionInt;
        set => _inputMouse.MousePositionInt = value;
    }

    public Action DragStarted {
        get => _inputMouse.DragStarted;
        set => _inputMouse.DragStarted = value;
    }

    public Action<Vector2> DragMousePositionDiff {
        get => _inputMouse.DragMousePositionDiff;
        set => _inputMouse.DragMousePositionDiff = value;
    }

    public Action<Vector2Int> DragMousePositionDiffInt {
        get => _inputMouse.DragMousePositionDiffInt;
        set => _inputMouse.DragMousePositionDiffInt = value;
    }

    public Action ConfirmSelect {
        get => _inputMouse.ClickPeformed;
        set => _inputMouse.ClickPeformed = value;
    }

    public Action<Vector2> Look {
        get => _inputMouse.Look;
        set => _inputMouse.Look = value;
    }

    public Action DragEnded {
        get => _inputMouse.DragEnded;
        set => _inputMouse.DragEnded = value;
    }

    protected readonly InputMouse _inputMouse;

    public MouseActionMap(ref InputMouse inputMouse) {
        _inputMouse = inputMouse;
    }

    public override void Dispose() {
        throw new System.NotImplementedException();
    }

    public override void Enable() {
        throw new System.NotImplementedException();
    }

    public override void Disable() {
        throw new System.NotImplementedException();
    }
}

}
