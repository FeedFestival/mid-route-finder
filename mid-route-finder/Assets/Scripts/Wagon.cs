using UnityEngine;

public class Wagon : MonoBehaviour {
    public TeamColor TeamColor;

    public void Init(TeamColor teamColor) {
        TeamColor = teamColor;
    }

    public void RemoveRigidbody() {
        var wagonRigidbody = GetComponent<WagonRigidbody>();
        wagonRigidbody.Remove();
        Destroy(wagonRigidbody);
    }
}
