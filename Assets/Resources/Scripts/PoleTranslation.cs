using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleTranslation : MonoBehaviour
{
    public Transform Target;
    
    private float _heightOffset;

    private void Start()
    {
        _heightOffset = transform.position.y - Target.position.y ;
    }

    private void Update()
    {
        transform.position = new Vector3(Target.position.x, Target.position.y + _heightOffset, Target.position.z);
    }
}
