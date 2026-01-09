using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: Check if it makes sense to extend the Wagon for this WagonRegidbody
// TODO: Rename to FallingWagon -> since that's his purpose
public class WagonRigidbody : MonoBehaviour {
    [SerializeField] Renderer _cubeRenderer;
    [SerializeField] GameObject _boxCollider;
    Rigidbody _rb;

    bool _hasTouchedFloor;
    Action<WagonRigidbody> _wagonReady;
    internal TeamColor TeamColor;

    void Awake() {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) {
            Debug.LogError("Wagon requires a Rigidbody!");
        }
    }

    public void Init(TeamColor teamColor, Action<WagonRigidbody> wagonReady) {
        TeamColor = teamColor;
        _wagonReady = wagonReady;
        _cubeRenderer.material = ResourceLibrary._.WagonMaterials[teamColor];
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

        // Rigidbody is settled, disable physics
        // _rb.linearVelocity = Vector3.zero;
        // _rb.angularVelocity = Vector3.zero;
        // _rb.isKinematic = true;

        _wagonReady?.Invoke(this);
    }
}
