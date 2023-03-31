using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sway : MonoBehaviour
{
    private float _timer;

    public float Intensity;
    public float Smooth;

    private Quaternion _originRotation;

    private void Start()
    {
        _originRotation = transform.localRotation;
    }

    private void Update()
    {
        UpdateSway(); 
    }

    private void UpdateSway()
    {
        float tMouseX = Input.GetAxis("Mouse X");
        float tMouseY= Input.GetAxis("Mouse Y");

        Quaternion targetAdjX = Quaternion.AngleAxis(-Intensity * tMouseX, Vector3.up);
        Quaternion targetAdjY = Quaternion.AngleAxis(Intensity * tMouseY, Vector3.right);
        Quaternion targetRotation = _originRotation * targetAdjY * targetAdjX;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * Smooth);
    }
}
