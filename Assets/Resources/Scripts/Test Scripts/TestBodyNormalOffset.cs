using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBodyNormalOffset : MonoBehaviour
{
    public Transform newMarkerPos;

    private float _initialHeight;
    private RaycastHit _surfaceRay;

    void Awake()
    {
        if(Physics.Raycast(transform.position, -transform.up, out _surfaceRay, Mathf.Infinity)) {
            _initialHeight = (transform.position - _surfaceRay.point).magnitude;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, -transform.up, out _surfaceRay, Mathf.Infinity)) {
            float currHeight = (transform.position - _surfaceRay.point).magnitude;
            float heightDiff = _initialHeight - currHeight;
            
            if (Mathf.Abs(heightDiff) > 0.05f) {
                Vector3 pos = _surfaceRay.point + _surfaceRay.normal.normalized * _initialHeight;
                newMarkerPos.position = pos;
                transform.position = pos;
            }
            transform.rotation = Quaternion.FromToRotation(transform.up, _surfaceRay.normal) * transform.rotation;
        }
    }
}
