using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WagonRigidbody : MonoBehaviour {
    [SerializeField] Renderer _cubeRenderer;
    [SerializeField] GameObject _boxCollider;
    Rigidbody _rb;

    bool _hasTouchedFloor;
    Action<Wagon> _wagonReady;
    Wagon _wagon;

    void Awake() {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) {
            Debug.LogError("Wagon requires a Rigidbody!");
        }
    }

    public void Init(TeamColor teamColor, Action<Wagon> wagonReady) {
        _wagonReady = wagonReady;
        _wagon = gameObject.AddComponent<Wagon>();
        _wagon.Init(teamColor);

        _cubeRenderer.material = ResourceLibrary._.WagonMaterials[teamColor];
    }

    void OnCollisionEnter(Collision c) {
        if (_hasTouchedFloor)
            return;

        _hasTouchedFloor = true;

        float randomDelay = Random.Range(8f, 12f);
        StartCoroutine(disablePhysicsAfterDelay(randomDelay));
    }

    IEnumerator disablePhysicsAfterDelay(float minDelay) {
        if (_rb == null)
            yield break;

        yield return new WaitForSeconds(minDelay);

        const float velocityThreshold = 0.001f;
        const float angularThreshold = 0.001f;
        const float checkInterval = 0.18f;

        while (_rb.linearVelocity.sqrMagnitude > velocityThreshold ||
               _rb.angularVelocity.sqrMagnitude > angularThreshold) {
            yield return new WaitForSeconds(checkInterval);
        }

        // Rigidbody is settled, disable physics
        // _rb.linearVelocity = Vector3.zero;
        // _rb.angularVelocity = Vector3.zero;
        // _rb.isKinematic = true;

        _wagonReady?.Invoke(_wagon);
    }

    public void Remove() {
        if (_rb != null) {
            Destroy(_rb);
        }

        if (_boxCollider != null)
            Destroy(_boxCollider);
    }
}
