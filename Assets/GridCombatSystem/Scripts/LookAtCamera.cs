using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    private Camera mainCamera;
    private Transform mainCameraTransform;

    private void Start() {
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera.transform;
    }

    private void LateUpdate() {
        transform.LookAt(mainCameraTransform);
    }

}
