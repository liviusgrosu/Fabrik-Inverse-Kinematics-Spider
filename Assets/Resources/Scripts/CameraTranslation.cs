using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTranslation : MonoBehaviour
{
    public Transform Body;
    private float _heightOffset;
    void Awake()
    {
        _heightOffset = transform.position.y - Body.transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, Body.transform.position.y + _heightOffset, transform.position.z);
    }
}
