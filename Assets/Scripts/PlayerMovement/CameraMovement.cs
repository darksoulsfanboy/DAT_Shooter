using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition;

    private void Update()
    {
        transform.position = _cameraPosition.position;
    }
}
