using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleTranslation : MonoBehaviour
{
    public Transform Target;
    public Transform Body;
    public ObstacleDetection ObstacleDetection;
    private float _heightOffset;

    private void Start()
    {
        _heightOffset = transform.position.y - Target.position.y ;
    }

    private void Update()
    {
        Vector3 curretSurfaceNormal = ObstacleDetection.GetSurfaceNormal();

        if(curretSurfaceNormal != Vector3.zero) {
            transform.position = Target.position + (curretSurfaceNormal.normalized * _heightOffset);
        }
    }
}
