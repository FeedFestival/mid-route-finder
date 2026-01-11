using UnityEngine;

public class Wagon : MonoBehaviour {
    [SerializeField] Renderer _cubeRenderer;

    IFallingWagon _fallingWagon;

    public void Init(IFallingWagon fallingWagon) {
        _fallingWagon = fallingWagon;

        _cubeRenderer.material = ResourceLibrary._.WagonMaterials[fallingWagon.TeamColor];
    }

    public void Place() {
        transform.position = _fallingWagon.SpatialData.position;
        transform.rotation = _fallingWagon.SpatialData.rotation;

        var realFallingWagon = _fallingWagon as FallingWagon;
        if (realFallingWagon)
            Wagon.Destroy(realFallingWagon.gameObject);
    }
}
