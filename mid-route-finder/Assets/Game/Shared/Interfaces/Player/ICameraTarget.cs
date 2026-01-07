using UnityEngine;

namespace Game.Shared.Interfaces.Player {

public interface ICameraTarget {
    Transform Transform { get; }
    Transform Target { get; }
}

}
