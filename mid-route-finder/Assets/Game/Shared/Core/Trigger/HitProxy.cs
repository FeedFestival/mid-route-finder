using System;
using Game.Shared.Interfaces.Core;
using UnityEngine;

namespace Game.Shared.Core {

public class HitProxy : MonoBehaviour, IFocusHitProxy {
    Action _onHit;

    public Transform t => transform;

    public int Init(Action onHit) {
        _onHit = onHit;

        return transform.GetInstanceID();
    }

    public void OnHit() {
        _onHit.Invoke();
    }
}

}
