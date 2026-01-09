using UnityEngine;

public class Wagon : MonoBehaviour {
    [SerializeField] Renderer _cubeRenderer;

    WagonRigidbody _wagonRb;

    public void Init(WagonRigidbody wagonRb) {
        _wagonRb = wagonRb;

        _cubeRenderer.material = ResourceLibrary._.WagonMaterials[wagonRb.TeamColor];
    }

    public void Place() {
        transform.position = _wagonRb.transform.position;
        transform.rotation = _wagonRb.transform.rotation;

        Wagon.Destroy(_wagonRb.gameObject);
    }
}
