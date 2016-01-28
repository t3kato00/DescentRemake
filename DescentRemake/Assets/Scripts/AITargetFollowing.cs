using UnityEngine;
using System.Collections;
using System;

public class AITargetFollowing : MonoBehaviour, AIMovementController {
    [HideInInspector]
    public Rigidbody FollowedTarget;

    public float MinPreferredDistance;
    public float MaxPreferredDistance;

    new protected Rigidbody rigidbody;
    protected AIMovement movement;
    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        movement = GetComponent<AIMovement>();

        movement.Register(AIMovementPriority.FollowingMovement, this);
    }
    protected void OnDestroy()
    {
        movement.Unregister(AIMovementPriority.FollowingMovement);
    }

    public Vector3 AccelerationStep(AIMovementPriority priority, float maxAcceleration, Vector3 lowPriorityAcceleration)
    {
        if(!enabled)
            return lowPriorityAcceleration;

        Vector3 relativeTargetPosition = FollowedTarget.transform.position - transform.position;
        float targetDistance = relativeTargetPosition.magnitude;

        Vector3 followStep;
        if( targetDistance > MaxPreferredDistance )
        {
            followStep = Vector3.zero;
        } else if( targetDistance < MinPreferredDistance )
        {
            followStep = Vector3.zero;
        }
        else
        {
            followStep = Vector3.zero;
        }

        return AIMovement.CombineAcceleration(followStep, lowPriorityAcceleration, maxAcceleration);
    }

    public Vector3 AngularAccelerationStep(AIMovementPriority priority, float maxAngularAcceleration, Vector3 lowPriorityAcceleration)
    {
        return lowPriorityAcceleration;
    }
}
