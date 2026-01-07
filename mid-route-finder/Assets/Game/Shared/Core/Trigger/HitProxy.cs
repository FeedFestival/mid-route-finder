using System;
using Game.Shared.Interfaces.Core;
using UnityEngine;

namespace Game.Shared.Core {

public class HitProxy : MonoBehaviour, IFocusHitProxy {
    Action<int> _onHit;

    public Transform t => transform;

    public void Init(Action<int> onHit) {
        _onHit = onHit;
    }

    public void OnHit(int instanceId) {
        _onHit.Invoke(instanceId);
    }
}

}
