using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FallingWagon : MonoBehaviour, IFallingWagon {
    [SerializeField] Renderer _cubeRenderer;
    [SerializeField] GameObject _boxCollider;
    Rigidbody _rb;

    bool _hasTouchedFloor;
    Action<IFallingWagon> _wagonReady;

    public TeamColor TeamColor { get; private set; }
    public SpatialData SpatialData => new(transform.position, transform.rotation);
    public GameObject Go => gameObject;

    public void Init(TeamColor teamColor, Action<IFallingWagon> wagonReady) {
        TeamColor = teamColor;

        _wagonReady = wagonReady;
        _cubeRenderer.material = ResourceLibrary._.WagonMaterials[teamColor];
    }

    void Awake() {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) {
            Debug.LogError("Wagon requires a Rigidbody!");
        }
    }

    void OnCollisionEnter(Collision c) {
        if (_hasTouchedFloor)
            return;

        _hasTouchedFloor = true;

        float randomDelay = Random.Range(6f, 8f);
        StartCoroutine(disablePhysicsAfterDelay(randomDelay));
    }

    IEnumerator disablePhysicsAfterDelay(float minDelay) {
        if (_rb == null)
            yield break;

        yield return new WaitForSeconds(minDelay);

        const float velocityThreshold = 0.01f;
        const float angularThreshold = 0.01f;
        const float checkInterval = 0.18f;

        while (_rb.linearVelocity.sqrMagnitude > velocityThreshold ||
               _rb.angularVelocity.sqrMagnitude > angularThreshold) {
            yield return new WaitForSeconds(checkInterval);
        }

        _wagonReady?.Invoke(this);
    }
}
