using R3;
using System;
using Game.Shared.Interfaces.Core;
using Game.Shared.Interfaces.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Shared.Core {

public class InputMouse : MonoBehaviour, IInputMouse {
    bool _mouseMoved;
    Subject<Vector2?> _mousePos_s = new();
    Subject<bool> _dragCheck_s = new();

    float _clkPressedTime;
    bool _isPressing;
    bool _isDragging;
    Vector2 _dragOrigin;
    Vector2Int _dragOriginInt;

    internal Action ClickPeformed { get; set; }
    internal Action DragStarted { get; set; }
    internal Action DragEnded { get; set; }
    internal Action<Vector2> MousePosition { get; set; }
    internal Action<Vector2> DragMousePositionDiff { get; set; }
    internal Action<Vector2Int> MousePositionInt { get; set; }
    internal Action<Vector2Int> DragMousePositionDiffInt { get; set; }
    internal Action<Vector2> Look { get; set; }

    /**
     * This script should be on the PLAYER GameObject
     */
    void Awake() {
        var player = GetComponent<IPlayer>();
        if (player == null) {
            Debug.LogError("Player not found. Can't use InputMouse. Add InputMouse to PLAYER gameObject.");
            return;
        }

        player.SetInputMouse(this);

        // In order to use the player for anything specific to this script
        // - we can use `__.Player` to access its interface functions
    }

    internal void constructor() {
        var d = Disposable.CreateBuilder();

        _dragCheck_s
            .Debounce(TimeSpan.FromMilliseconds(100))
            .Do(_ => {
                if (_isPressing == false) return;
                _isDragging = true;
                var mousePos = InputMouse.GetMousePosition();
                if (mousePos.HasValue) {
                    _dragOrigin = mousePos.Value;
                    _dragOriginInt = InputMouse.ToVector2Int(mousePos.Value);
                }
                else {
                    _dragOrigin = Vector2.zero;
                    _dragOriginInt = Vector2Int.zero;
                }

                DragStarted?.Invoke();
            })
            .Subscribe()
            .AddTo(ref d);

        _mousePos_s
            .DistinctUntilChanged()
            .Select(mousePos => {
                if (mousePos.HasValue) {
                    mouseScreenPosition(mousePos.Value);
                }

                return ToVector2Int(mousePos.Value);
            })
            .DistinctUntilChanged()
            .Do(mouseScreenPositionInt)
            .Subscribe()
            .AddTo(ref d);

        d.RegisterTo(this.destroyCancellationToken);
    }

    internal static Vector2? GetMousePosition() {
        var mousePos = Mouse.current.position.ReadValue() as Vector2?;
        if (mousePos.HasValue) {
            return new Vector2((float)Math.Round(mousePos.Value.x, 2), (float)Math.Round(mousePos.Value.y, 2));
        }

        return null;
    }

    internal static Vector2Int ToVector2Int(Vector2 v) {
        return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    }

    internal void SetupForDrag() { }

    internal void DisposeForDrag() {
        _dragCheck_s.Dispose();
        _dragCheck_s = null;

        _clkPressedTime = 0;
        _isPressing = false;
        _isDragging = false;

        DragStarted = null;
        MousePosition = null;
        MousePositionInt = null;
        DragMousePositionDiff = null;
        DragMousePositionDiffInt = null;
        ClickPeformed = null;
        DragEnded = null;
        Look = null;
    }

    internal void MouseMoved(InputAction.CallbackContext ctx) {
        if (ctx.performed)
            _mouseMoved = true;
        else
            _mouseMoved = false;
    }

    internal void LookPerformed(InputAction.CallbackContext ctx) {
        var value = ctx.ReadValue<Vector2>();
        Look?.Invoke(value);
    }

    internal void LookCanceled(InputAction.CallbackContext ctx) {
        Look?.Invoke(ctx.ReadValue<Vector2>());
    }

    void FixedUpdate() {
        if (_mouseMoved) {
            _mousePos_s.OnNext(GetMousePosition());
        }
    }

    internal void clkStarted(InputAction.CallbackContext ctx) {
        _isPressing = true;
        _clkPressedTime = Time.time;

        _dragCheck_s.OnNext(true);
    }

    internal void clkEnded(InputAction.CallbackContext ctx) {
        _isPressing = false;
        float elapsedTime = Time.time - _clkPressedTime;

        if (elapsedTime < 0.1f && !_isDragging) {
            clkPerformed();
        }
        else {
            _isDragging = false;
            DragEnded?.Invoke();
        }
    }

    void clkPerformed() {
        ClickPeformed?.Invoke();
    }

    void mouseScreenPosition(Vector2 mouseScreenPos) {
        MousePosition?.Invoke(mouseScreenPos);

        if (_isDragging) {
            DragMousePositionDiff?.Invoke(_dragOrigin - mouseScreenPos);
        }
    }

    void mouseScreenPositionInt(Vector2Int mouseScreenPosInt) {
        MousePositionInt?.Invoke(mouseScreenPosInt);

        if (_isDragging) {
            DragMousePositionDiffInt?.Invoke(_dragOriginInt - mouseScreenPosInt);
        }
    }
}

}
