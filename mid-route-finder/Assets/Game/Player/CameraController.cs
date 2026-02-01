using Game.Shared.Core.Store;
using UnityEngine;
using Game.Shared.Constants.Store;
using Game.Shared.Core.FocusManager;
using Game.Shared.Core.Player_Input;
using Game.Shared.Interfaces.Player;
using R3;
using Unity.Cinemachine;

namespace Game.Player {

public class CameraController : MonoBehaviour, ICameraController {
    [Header("References")]
    [SerializeField]
    protected Camera _mainCamera;

    [SerializeField] protected CinemachineCamera _cinemachineCamera;

    protected FocusRaycast _focusRaycast;

    void Awake() {
        _focusRaycast = GetComponent<FocusRaycast>();
        GameObject[] rootGos = gameObject.scene.GetRootGameObjects();

        IPlayer player = null;

        foreach (GameObject go in rootGos) {
            if (player != null) break;

            player = go.GetComponent<IPlayer>();
        }

        if (player == null) {
            Debug.LogError("CameraController needs to be a sibling of PLAYER.");
        }
    }

    void Start() {
        init();
    }

    protected virtual void init() {
        var d = Disposable.CreateBuilder();

        Store2.State.Gameplay.Subscribe((gameplay) => {
            if (!gameObject.activeSelf) return;

            if (gameplay == Gameplay.Exploration) {
                PlayerInput.ExplorationActionMap.MousePosition += mousePositionChanged;
            }
            else {
                PlayerInput.ExplorationActionMap.MousePosition -= mousePositionChanged;
            }
        }).AddTo(ref d);

        d.RegisterTo(this.destroyCancellationToken);
    }

    protected virtual void mousePositionChanged(Vector2 mousePosition) {
        _focusRaycast.LookForFocusable(mousePosition, ref _mainCamera);
    }
}

}
