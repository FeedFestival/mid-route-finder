using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Billboard : MonoBehaviour {
    Camera _cam;

    void Awake() {
        _cam = Camera.main;
    }

    void LateUpdate() {
        faceCamera();
    }

    [Button]
    void lookAtCamera() {
        _cam = Camera.main;
        faceCamera();
    }

    void faceCamera() {
        transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward,
            _cam.transform.rotation * Vector3.up);
    }
}
