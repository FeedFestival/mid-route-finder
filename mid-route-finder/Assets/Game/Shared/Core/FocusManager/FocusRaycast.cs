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
        // var entityId = hit.transform.GetEntityId();
        var instanceId = hit.transform.GetInstanceID();

        Store2.SetFocusedTriggerID(instanceId);

        var focusHitProxy = hit.transform.GetComponent<IFocusHitProxy>();
        focusHitProxy?.OnHit(instanceId);
    }

    void onMiss() {
        Store2.SetFocusedTriggerID(-1);
    }
}

}
