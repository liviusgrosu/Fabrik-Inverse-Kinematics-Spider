using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR;

public class FastIKFabric : MonoBehaviour
{
    public int chainLength = 2;
    public Transform target, pole;
    public int iterations = 10;
    public float delta = 0.001f;
    [Range(0, 1)]
    public float snapBackStrength = 1f;

    protected float[] bonesLength;
    [HideInInspector]
    public float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;

    protected Vector3[] startDirectionSuccessor;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;

    private bool startIK;

    private LegTargetRay.IKCallback ikResolverCallback;

    void Init()
    {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];

        startDirectionSuccessor = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];

        startRotationTarget = target.rotation;
        completeLength = 0;

        Transform current = transform;
        for(int i = bones.Length - 1; i >= 0; i--)
        {
            //Leaf will be last and root will be first index
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if (i == bones.Length - 1)
            {
                // Lead bone
                startDirectionSuccessor[i] = target.position - bones[i].position;
            }
            else
            {
                // Mid bone
                startDirectionSuccessor[i] = bones[i + 1].position - bones[i].position;
                //Get the length from the current bone to the last one 
                bonesLength[i] = (bones[i + 1].position - current.position).magnitude;
                completeLength += bonesLength[i];
            }

            current = current.parent;
        }
    }

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    public Vector3 GetInitialTargetPos()
    {
        return target.position;
    }

    public void ProvideNewPosition(Vector3 position)
    {
        if(!startIK)
        {
            Init();
            startIK = true;
        }
        target.position = position;
    }

    public void ProvideNewIKResolverCallback(LegTargetRay.IKCallback callback)
    {
        ikResolverCallback = callback;
    }

    private void ResolveIK()
    {
        if (target == null)
            return;

        if (bonesLength.Length != chainLength)
            Init();

        //get position
        for (int i = 0; i < bones.Length; i++)
            positions[i] = bones[i].position;


        Quaternion rootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        Quaternion rootRotDiff = rootRot * Quaternion.Inverse(startRotationRoot);

        //calculation

        //This is the same Vector.distance >= complete length but this avoids having to square it so its faster
        if ((target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength) 
        {
            Vector3 direction = (target.position - positions[0]).normalized;
            for (int i = 1; i < positions.Length; i++)
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
        }
        else
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] +  startDirectionSuccessor[i], snapBackStrength);
            }

            for (int ite = 0; ite < iterations; ite++)
            {
                //backwards
                for(int i = positions.Length - 1; i > 0; i--)
                {
                    if (i == positions.Length - 1)
                    {
                        //Set the leaf node to target
                        positions[i] = target.position;
                    }
                    else
                    {
                        //Set the current node along the line of the previous node direction to the current node
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];
                    }
                }

                //fowards
                for(int i = 1; i < positions.Length - 1; i++)
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];

                // If we're close enough to the target then don't bother checking
                if ((positions[positions.Length - 1] - target.position).sqrMagnitude < delta * delta)
                    break;
            }
        }

        // Pole
        if (pole != null)
        {
            for (int i = 1; i < positions.Length - 1; i++)
            {
                // Create that plane on the previous bone with the normal being the previous and next bone
                Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                // Project the pole onto the plane
                Vector3 projectedPole = plane.ClosestPointOnPlane(pole.position);
                // Project the current boner onto the plane
                Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                // Find the angle between the 2 projections
                float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                // Rotate the current bone around the previous bone in a circle with the axis being the normal of that plane
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        // set position
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
                bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            else
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSuccessor[i], positions[i + 1] - positions[i]) * startRotationBone[i];

            bones[i].position = positions[i];
        }

        if (ikResolverCallback != null)
        {
            ikResolverCallback();
            ikResolverCallback = null;
        }
    }

    private void OnDrawGizmos()
    {
        Transform current = this.transform;
        for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            float scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
}
