using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR;

/// <summary>
/// Calculate inverse kinematics on a leg with bones and joints
/// </summary>
/// <remarks>
/// This is using the FABRIK algorithm devised by Andreas Aristidou
/// </summary>
public class FastIKFabric : MonoBehaviour
{
    [HideInInspector]
    public float CompleteLength;
    public int ChainLength = 2;
    public Transform Target, Pole;
    public int Iterations = 10;
    public float Delta = 0.001f;
    [Range(0, 1)]
    public float SnapBackStrength = 1f;
    protected float[] _bonesLength;

    protected Transform[] _bones;
    protected Vector3[] _positions;
    protected Vector3[] _startDirectionSuccessor;
    protected Quaternion[] _startRotationBone;
    protected Quaternion _startRotationTarget;
    protected Quaternion _startRotationRoot;

    private bool _startIK;
    private TargetFinder.IKCallback _ikResolverCallback;

    /// <summary>
    /// Initialize the algorithm by setting up the bones and their positions, lengths, and rotation
    /// </summary>
    void Init() {
        _bones = new Transform[ChainLength + 1];
        _positions = new Vector3[ChainLength + 1];
        _bonesLength = new float[ChainLength];

        _startDirectionSuccessor = new Vector3[ChainLength + 1];
        _startRotationBone = new Quaternion[ChainLength + 1];

        _startRotationTarget = Target.rotation;
        CompleteLength = 0;

        Transform current = transform;
        for(int i = _bones.Length - 1; i >= 0; i--) {
            //Leaf/tail bone will be last index and the root will be first index
            _bones[i] = current;
            _startRotationBone[i] = current.rotation;

            // Get the successor bone direction
            if (i == _bones.Length - 1)
            {
                // Lead bone
                _startDirectionSuccessor[i] = Target.position - _bones[i].position;
            }
            else
            {
                // Mid bone
                _startDirectionSuccessor[i] = _bones[i + 1].position - _bones[i].position;
                //Get the length from the current bone to the last one 
                _bonesLength[i] = (_bones[i + 1].position - current.position).magnitude;
                // Add to the complete length of the leg
                CompleteLength += _bonesLength[i];
            }
            current = current.parent;
        }
    }

    private void Awake() {
        Init();
    }

    private void LateUpdate() {
        ResolveIK();
    }

    /// <summary>
    /// Get the initial target position
    /// </summary>
    /// <returns>Get the initial target position vector</returns>
    public Vector3 GetInitialTargetPos() {
        return Target.position;
    }

    /// <summary>
    /// Provide to the IK algorithm a new target point
    /// This will also force the IK to initialize if other scripts are trying to access it           
    /// </summary>
    /// <param name="position">New target point</param>
    public void ProvideNewPosition(Vector3 position) {
        if(!_startIK) {
            Init();
            _startIK = true;
        }
        Target.position = position;
    }

    /// <summary>
    /// Store a callback function to call when the first IK pass has been made
    /// </summary>
    /// <param name="callback">The callback function to call into</param>
    public void ProvideNewIKResolverCallback(TargetFinder.IKCallback callback)
    {
        _ikResolverCallback = callback;
    }

    /// <summary>
    /// Calculate IK algorithm
    /// </summary>
    private void ResolveIK() {
        if (Target == null) {
            return;
        }

        // Re-initialize if the chain length does not equal the bone length
        if (_bonesLength.Length != ChainLength) {
            Init();
        }

        for (int i = 0; i < _bones.Length; i++) {
            _positions[i] = _bones[i].position;
        }

        // If the distance to the target is greater then the leg length itself, then align each bone in the direction of the target
        // This will cause the leg to extend fully straight
        if ((Target.position - _bones[0].position).sqrMagnitude >= CompleteLength * CompleteLength) {
            Vector3 direction = (Target.position - _positions[0]).normalized;
            for (int i = 1; i < _positions.Length; i++) {
                _positions[i] = _positions[i - 1] + direction * _bonesLength[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < _positions.Length - 1; i++) {
                // Move all the positions back so that the root bone matches the origin
                _positions[i + 1] = Vector3.Lerp(_positions[i + 1], _positions[i] +  _startDirectionSuccessor[i], SnapBackStrength);
            }

            // Each iteration adds more precision to the bones correct positions
            for (int ite = 0; ite < Iterations; ite++) {
                // Iterate through the bones backwards (leaf -> root)
                for(int i = _positions.Length - 1; i > 0; i--) {
                    if (i == _positions.Length - 1) {
                        //Set the leaf node to Target
                        _positions[i] = Target.position;
                    }
                    else {
                        //Set the current node along the line of the previous node direction to the current node
                        _positions[i] = _positions[i + 1] + (_positions[i] - _positions[i + 1]).normalized * _bonesLength[i];
                    }
                }

                // Iterate through the bones forwards (root -> leaf)
                for(int i = 1; i < _positions.Length - 1; i++) {
                    _positions[i] = _positions[i - 1] + (_positions[i] - _positions[i - 1]).normalized * _bonesLength[i - 1];
                }

                // Allow a margin of displacement to the target
                if ((_positions[_positions.Length - 1] - Target.position).sqrMagnitude < Delta * Delta) {
                    break;
                }
            }
        }

        // Calculate if a pole exists
        if (Pole != null) {
            for (int i = 1; i < _positions.Length - 1; i++) {
                // Create that plane on the previous bone with the normal being the previous and next bone
                Plane plane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                // Project the Pole onto the plane
                Vector3 projectedPole = plane.ClosestPointOnPlane(Pole.position);
                // Project the current bone onto the plane
                Vector3 projectedBone = plane.ClosestPointOnPlane(_positions[i]);
                // Find the angle between the 2 projections
                float angle = Vector3.SignedAngle(projectedBone - _positions[i - 1], projectedPole - _positions[i - 1], plane.normal);
                // Rotate the current bone around the previous bone in a circle with the axis being the normal of that plane
                _positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) + _positions[i - 1];
            }
        }

        
        for (int i = 0; i < _positions.Length; i++) {
            // Set each bones rotation depending on their successor
            if (i == _positions.Length - 1) {
                _bones[i].rotation = Target.rotation * Quaternion.Inverse(_startRotationTarget) * _startRotationBone[i];
            }
            else {
                _bones[i].rotation = Quaternion.FromToRotation(_startDirectionSuccessor[i], _positions[i + 1] - _positions[i]) * _startRotationBone[i];
            }
            
            // Set each bones positions
            _bones[i].position = _positions[i];
        }

        if (_ikResolverCallback != null) {
            // Step into this callback which tells the BodyPositionOffset script that the first IK calculation has finished
            // This allows that script to correctly calculate for offset since before the legs are parallel to the body 
            _ikResolverCallback();
            _ikResolverCallback = null;
        }
    }
}
