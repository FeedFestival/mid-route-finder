using System;
using TMPro;
using UnityEngine;

public class TestScript : MonoBehaviour {
    [SerializeField] TextMeshPro _testText;

    int _mouseDownCounter;
    float _deltaTime = 0.0f;
    string _fps;

    public Camera playerCamera;
    public float maxDistance = 100f;

    void Start() {
        playerCamera = Camera.main;
    }

    public void OnRayHit() {
        _mouseDownCounter++;

        _testText.text = $"({_mouseDownCounter}) {_fps}";
    }

    void Update() {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fps = $"FPS: {Mathf.Ceil(fps)}";

        _testText.text = $"({_mouseDownCounter}) {_fps}";

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance)) {
                Debug.Log($"hit.collider.gameObject: {hit.collider.gameObject}");
                hit.collider.gameObject.SendMessage(
                    "OnRayHit",
                    SendMessageOptions.DontRequireReceiver
                );
            }
        }
    }
}
