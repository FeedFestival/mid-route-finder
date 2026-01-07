using UnityEngine;

namespace Game.Shared.Core.ExtensionMethods {

public static class UnityExtensionMethods {
    public static Vector2 GroundPlanePosition(this Transform transform) {
        return new Vector2(transform.position.x, transform.position.z);
    }
}

}
