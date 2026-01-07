using DG.Tweening;
using Game.Shared.Constants.Store;
using Game.Shared.Core.Player_Input;
using Game.Shared.Core.Store;
using R3;
using Game.Shared.Interfaces.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Player {

public class CameraTarget : MonoBehaviour, ICameraTarget {
    [SerializeField] CinemachineThirdPersonFollow _thirdPersonFollow;
    [SerializeField] Transform _followTarget;
    [SerializeField] GameObject _target;

    [Header("Zoom Settings")]
    [SerializeField]
    int _currentZoom = 14;

    [SerializeField] int _maxZoom = 24;
    [SerializeField] int _minZoom = 8;
    [SerializeField] int _changeZoom = 4;

    Tweener _zoomTweener;

    [Header("Move Camera Target")]
    [SerializeField]
    float _dragSpeedMultiplier = 32;

    [SerializeField] float _speedMultiplier = 6;
    [SerializeField] public float _edgeThresholdPercent = 8f;

    float edgeThresholdWidth;
    float edgeThresholdHeight;
    Vector3? _edgeMoveDir;

    bool _doFollowTarget = false;
    Tweener _followTargetTweener;
    Vector3? _moveDir;
    Tweener _moveTweener;
    bool _isDragging;

    public Transform Target => _target.transform;

    public Transform Transform => transform;

    //---------------------------------------------------------------------



    #region Monobehaviour


    /**
     * This script should be a sibling of (ThirdMotor) MainUnit GameObject
     */
    void Start() {
        ICameraOrbit cameraOrbit = null;

        foreach (Transform sibling in transform.parent) {
            if (cameraOrbit == null)
                cameraOrbit = sibling.gameObject.GetComponent<ICameraOrbit>();
        }

        cameraOrbit?.Init(this);

        init();
    }

    void Update() {
        if (_doFollowTarget) {
            transform.position = _followTarget.position; //+ (_followTarget.forward * 0.3f);
        }

        var mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheel > 0) {
            zoom();
        }
        else if (mouseWheel < 0) {
            zoom(false);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members",
        Justification = "<Pending>")]
    void FixedUpdate() {
        if (_doFollowTarget || _isDragging) return;

        if (_moveDir.HasValue) {
            transform.position = moveCameraTarget(_moveDir.Value);
        }

        if (_edgeMoveDir.HasValue) {
            transform.position = moveCameraTarget(_edgeMoveDir.Value);
        }
    }

    void OnDestroy() {
        _followTargetTweener?.Kill();
        _moveTweener?.Kill();
    }


    #endregion



    //---------------------------------------------------------------------

    void init() {
        edgeThresholdWidth = Screen.width * (_edgeThresholdPercent / 100);
        edgeThresholdHeight = Screen.height * (_edgeThresholdPercent / 100);

        var d = Disposable.CreateBuilder();

        Store2.State.Gameplay.Subscribe(onGameplayChange).AddTo(ref d);

        d.RegisterTo(this.destroyCancellationToken);
    }

    void onGameplayChange(Gameplay gameplay) {
        unsubscribe();

        if (gameplay != Gameplay.Exploration) return;

        PlayerInput.ExplorationActionMap.SnapToCharacter += snapToCharacter;

        PlayerInput.ExplorationActionMap.DragStarted += dragStarted;
        PlayerInput.ExplorationActionMap.DragMousePositionDiffInt += moveByDrag;
        PlayerInput.ExplorationActionMap.DragEnded += dragEnded;

        // else if (gameplay == Gameplay.StrategicExploration)
        // _doFollowTarget = false;
    }

    void unsubscribe() {
        PlayerInput.ExplorationActionMap.Movement -= move;
        PlayerInput.ExplorationActionMap.MousePositionInt -= moveCameraByEdge;
        PlayerInput.ExplorationActionMap.DragStarted -= dragStarted;
        PlayerInput.ExplorationActionMap.DragMousePositionDiffInt -= moveByDrag;
        PlayerInput.ExplorationActionMap.DragEnded -= dragEnded;
    }

    void moveCameraByEdge(Vector2Int mousePos) {
        _edgeMoveDir = calculateEdgeMoveDir(mousePos);
    }

    void dragStarted() {
        _doFollowTarget = false;
        _isDragging = true;
    }

    void dragEnded() => _isDragging = false;

    void followTarget() {
        _doFollowTarget = true;
    }

    void move(Vector2 pos, bool cancel) {
        if (cancel) {
            _moveDir = null;
            return;
        }

        _moveDir = getCameraOrientedPos(pos);
    }

    void moveByDrag(Vector2Int mouseDiff) {
        var camT = Camera.main?.transform;
        if (camT == null) return;

        float X = (float)mouseDiff.x / Screen.width;
        float Y = (float)mouseDiff.y / Screen.height;

        var delta = new Vector2(X, Y) * _dragSpeedMultiplier;

        var forward = Vector3.ProjectOnPlane(camT.forward, Vector3.up).normalized;
        var right = camT.right;

        var movement = (right * delta.x) + (forward * delta.y);

        if (_moveTweener != null) {
            _moveTweener.Kill();
        }

        _moveTweener = DOVirtual.Vector3(
            transform.position,
            moveCameraTarget(movement),
            0.32f,
            (value) => transform.position = value
        );
    }

    Vector3 moveCameraTarget(Vector3 moveDir) {
        var zoomMultiplier = ((_currentZoom - _minZoom) / _changeZoom) + 1;
        return transform.position + moveDir * _speedMultiplier * zoomMultiplier * Time.deltaTime;
    }

    Vector3? calculateEdgeMoveDir(Vector2 mousePos) {
        Vector2 movement = Vector3.zero;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Scaling factor based on how close the mouse is to the edge
        float scaleX = 0f;
        float scaleY = 0f;

        // Check horizontal edges
        if (mousePos.x < edgeThresholdWidth) { // Near the left edge
            scaleX = 1 - (mousePos.x / edgeThresholdWidth);
            movement.x = -1;
        }
        else if (mousePos.x > screenWidth - edgeThresholdWidth) { // Near the right edge
            scaleX = 1 - ((screenWidth - mousePos.x) / edgeThresholdWidth);
            movement.x = 1;
        }

        // Check vertical edges
        if (mousePos.y < edgeThresholdHeight) { // Near the bottom edge
            scaleY = 1 - (mousePos.y / edgeThresholdHeight);
            movement.y = -1;
        }
        else if (mousePos.y > screenHeight - edgeThresholdHeight) { // Near the top edge
            scaleY = 1 - ((screenHeight - mousePos.y) / edgeThresholdHeight);
            movement.y = 1;
        }

        float scale = Mathf.Max(scaleX, scaleY);
        scale = Mathf.Clamp(scale, 0.1f, 1.0f);

        if (movement == Vector2.zero) {
            return null;
        }

        return getCameraOrientedPos(movement.normalized * scale);
    }

    void snapToCharacter(bool snap) {
        _moveDir = null;
        _edgeMoveDir = null;

        if (_followTargetTweener != null) {
            _followTargetTweener.onComplete -= followTarget;
            _followTargetTweener.Kill();
        }

        _followTargetTweener = DOVirtual.Vector3(
            transform.position,
            _followTarget.position,
            0.64f,
            (value) => transform.position = value
        ).SetEase(Ease.OutQuint);
        _followTargetTweener.onComplete += followTarget;
    }

    void zoom(bool zoomIn = true) {
        var fromZoom = _currentZoom;
        _currentZoom += zoomIn ? -_changeZoom : _changeZoom;

        if (_currentZoom > _maxZoom) _currentZoom = _maxZoom;
        if (_currentZoom < _minZoom) _currentZoom = _minZoom;


        if (_zoomTweener != null) {
            _zoomTweener.Kill();
        }

        var toLocalPos = new Vector3(0, 0, -_currentZoom);
        _zoomTweener = DOVirtual.Float(
            fromZoom,
            _currentZoom,
            0.32f,
            (value) => { _thirdPersonFollow.CameraDistance = value; }
        );
    }

    Quaternion getCameraRot() {
        var cameraForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        var cameraRotation = Quaternion.LookRotation(cameraForward);
        return cameraRotation;
    }

    Vector3 getCameraOrientedPos(Vector2 pos) {
        var camRotation = getCameraRot();
        return camRotation * new Vector3(pos.x, 0, pos.y);
    }
}

}
