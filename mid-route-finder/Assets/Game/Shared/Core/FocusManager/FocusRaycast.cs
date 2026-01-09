using Game.Shared.Constants.Layer;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using Game.Shared.Interfaces.Core;
using UnityEngine;

namespace Game.Shared.Core.FocusManager {

public class FocusRaycast : MonoBehaviour {
    [Header("Mouse Ray Check")]
    [SerializeField]
    float _checkRange = 5f;

    int _lastFocusedInstanceId = -1;

    public void LookForFocusable(Vector2 mousePosition, ref Camera camera) {
        if (!camera) {
            Debug.LogError("Camera not found");
            return;
        }

        var ray = camera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit, _checkRange, LayerConstants.INTERACT, QueryTriggerInteraction.Collide)) {
            onHit(hit);
        }
        else {
            onMiss();
        }
    }

    void onHit(RaycastHit hit) {
        var instanceId = hit.transform.GetInstanceID();
        if (instanceId == _lastFocusedInstanceId)
            return;
        _lastFocusedInstanceId = instanceId;

        Store2.SetFocusedInstanceID(instanceId);

        var focusHitProxy = hit.transform.GetComponent<IFocusHitProxy>();
        focusHitProxy?.OnHit();
    }

    void onMiss() {
        _lastFocusedInstanceId = -1;
        Store2.SetFocusedInstanceID(-1);
        Store2.SetFocusedID(0);
    }
}

}
