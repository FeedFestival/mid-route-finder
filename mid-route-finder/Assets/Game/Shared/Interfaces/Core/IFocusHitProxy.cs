using System;
using UnityEngine;

namespace Game.Shared.Interfaces.Core {

public interface IFocusHitProxy {
    Transform t { get; }

    int Init(Action onHit);
    void OnHit();
}

}
