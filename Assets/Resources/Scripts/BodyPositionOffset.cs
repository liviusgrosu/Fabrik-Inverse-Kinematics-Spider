using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPositionOffset : MonoBehaviour
{
    public Transform newPosMarker; 

    public Transform avergeLegMarker;

    public Transform[] legs;

    public List<LegTargetRay> rays;

    public List<Transform> tailBones;

    private float startingOffset;
    
    private bool initialized;

    // Update is called once per frame
    void Update()
    {
        bool allowedToCalculate = true;
        foreach(LegTargetRay ray in rays)
        {
            if (!ray.firstIKResolved)
            {
                allowedToCalculate = false;
            }
        }
        if(allowedToCalculate)
        {
            if (!initialized)
            {
                Vector3 bodyPos = transform.position;
                startingOffset = bodyPos.y - CalculateAverageY();
                avergeLegMarker.position = new Vector3(transform.position.x, CalculateAverageY(), transform.position.z);
                initialized = true;
            }
            newPosMarker.position = new Vector3(transform.position.x, startingOffset + CalculateAverageY(), transform.position.z);
            transform.position = new Vector3(transform.position.x, startingOffset + CalculateAverageY(), transform.position.z);
        }
    }

    float CalculateAverageY()
    {
        float averageHeight = 0;
        foreach (Transform tailBone in tailBones)
        {
            averageHeight += tailBone.position.y;
        }
        averageHeight /= tailBones.Count;
        return averageHeight;
    }
}
