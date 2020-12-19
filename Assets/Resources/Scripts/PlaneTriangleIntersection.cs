using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaneTriangleIntersection : MonoBehaviour
{
    public int intersectionPoints = 10;
    public float intersectionDistance = 1f;

    public float minAngle = 20f;
    public float maxAngle = 90f;

    private float angleStep;

    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        // Get the angle steps that will progres through the cirlce path
        angleStep = (minAngle + maxAngle) / intersectionPoints;
        //plane = new Plane(transform.position, -transform.right);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 palmRay = Quaternion.AngleAxis(-minAngle, transform.right) * transform.forward;
            for (int i = 0; i < intersectionPoints; i++)
            {
                Debug.DrawRay(transform.position, palmRay * intersectionDistance, Color.red);
                palmRay = Quaternion.AngleAxis(angleStep, transform.right) * palmRay;
                if(Physics.Raycast(transform.position, palmRay * intersectionDistance, out hit))
                {
                    Debug.Break();
                }
            }
        }
    }
}
