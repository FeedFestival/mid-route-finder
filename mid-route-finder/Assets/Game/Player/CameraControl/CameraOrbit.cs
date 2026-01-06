using Game.Shared.Constants.Store;
using Game.Shared.Core.Player_Input;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces.Player;
using R3;
using UnityEngine;

namespace Game.Player.CameraControl {

public class CameraOrbit : MonoBehaviour, ICameraOrbit {
    [Header("Cinemachine")]
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    Vector2 _lookInput;
    bool _isHoldingRightClick;
    const float _threshold = 0.001f;

    // cinemachine
    float _cinemachineTargetYaw;
    float _cinemachineTargetPitch;

    ICameraTarget _cameraTarget;

    public bool AnalogControl { get; set; }

    public void Init(ICameraTarget cameraTarget) {
        _cameraTarget = cameraTarget;
        _cinemachineTargetYaw = _cameraTarget.Target.rotation.eulerAngles.y;

        var d = Disposable.CreateBuilder();

        Store2.State.Gameplay
            .Subscribe(gameplay => {
                if (gameplay == Gameplay.Exploration) {
                    PlayerInput.ExplorationActionMap.Look += look;
                    PlayerInput.ExplorationActionMap.RightClick += rightClick;
                }
                else OnDestroy();
            })
            .AddTo(ref d);

        d.RegisterTo(this.destroyCancellationToken);
    }

    void LateUpdate() {
        cameraRotation();
    }

    void OnDestroy() {
        if (PlayerInput.ExplorationActionMap == null) return;

        PlayerInput.ExplorationActionMap.Look -= look;
        PlayerInput.ExplorationActionMap.RightClick -= rightClick;
    }

    public void look(Vector2 look) => _lookInput = look;
    public void rightClick(bool isHoldingRightClick) => _isHoldingRightClick = isHoldingRightClick;

    void cameraRotation() {
        if (_isHoldingRightClick) {
            // if there is an input and camera position is not fixed
            if (_lookInput.sqrMagnitude >= _threshold && !LockCameraPosition) {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = !AnalogControl ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _lookInput.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _lookInput.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        }

        // Cinemachine will follow this target
        _cameraTarget.Target.rotation = Quaternion.Euler(
            _cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw,
            0.0f
        );
    }

    static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}

}
