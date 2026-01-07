using System;
using UnityEngine;

namespace Game.Shared.Interfaces.Core {

public interface IFocusHitProxy {
    Transform t { get; }

    void Init(Action<int> onHit);
    void OnHit(int instanceId);
}

}
