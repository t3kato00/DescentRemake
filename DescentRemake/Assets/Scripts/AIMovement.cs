using UnityEngine;
using System.Collections.Generic;

public enum AIMovementPriority
{
    CollisionEvasion = 3,
    EvasiveManouver = 2,
    TrackingMovement = 1,
    FollowingMovement = 0,
    RandomManouver = -1,
    Station = -2,
    Patrol = -3,
}

public interface AIMovementController
{
    Vector3 AccelerationStep( AIMovementPriority priority, float maxAcceleration, Vector3 lowPriorityAcceleration );
    Vector3 AngularAccelerationStep( AIMovementPriority priority, float maxAngularAcceleration, Vector3 lowPriorityAcceleration );

    /* No op implementations:
        public Vector3 AccelerationStep(AIMovementPriority priority, float maxAcceleration, Vector3 lowPriorityAcceleration)
    {
        return lowPriorityAcceleration;
    }

    public Vector3 AngularAccelerationStep(AIMovementPriority priority, float maxAngularAcceleration, Vector3 lowPriorityAcceleration)
    {
        return lowPriorityAcceleration;
    }
    */
}

public class AIMovement : MonoBehaviour {
    public float Acceleration;
    public float AngularAcceleration;
    //public float DebugScale;

    SortedList<AIMovementPriority, AIMovementController> controllers = new SortedList<AIMovementPriority, AIMovementController>();
    public void Register(AIMovementPriority priority, AIMovementController controller )
    {
        controllers.Add(priority, controller);
    }
    public void Unregister(AIMovementPriority priority )
    {
        controllers.Remove(priority);
    }

    protected new Rigidbody rigidbody;
    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate ()
    {
        Vector3 acceleration = Vector3.zero;
        Vector3 angularAcceleration = Vector3.zero;
        foreach ( KeyValuePair<AIMovementPriority, AIMovementController> pair in controllers )
        {
            acceleration = pair.Value.AccelerationStep(pair.Key, Acceleration, acceleration);
            angularAcceleration = pair.Value.AngularAccelerationStep(pair.Key, AngularAcceleration, angularAcceleration);
        }
        rigidbody.velocity += acceleration;
        rigidbody.angularVelocity += angularAcceleration;
    }

    public static Vector3 CombineAcceleration(Vector3 high, Vector3 low, float max)
    {
        float highMagnitude = high.magnitude;
        high /= highMagnitude;
        float lowHighComponent = Vector3.Dot(high, low);
        low -= lowHighComponent * high;
        highMagnitude = Mathf.Max(highMagnitude, lowHighComponent);
        high *= highMagnitude;

        Vector3 final = high + low;
        float sqrMax = max * max;
        if (sqrMax >= final.sqrMagnitude)
            return final;
        
        low *= Mathf.Sqrt(sqrMax - highMagnitude * highMagnitude) / low.magnitude;
        return high + low;
    }

    public Vector3 CalculateAngularAccelerationStep(Quaternion targetRotation)
    {
        // Calculate the target rotation relative to current rotation.
        Quaternion relativeTargetRotation = targetRotation * Quaternion.Inverse(transform.rotation);

        // Get the components of relative target rotation.
        Vector3 relativeTargetRotationAxis; float relativeTargetRotationAngle;
        relativeTargetRotation.ToAngleAxis(out relativeTargetRotationAngle, out relativeTargetRotationAxis);
        if (relativeTargetRotationAngle > 180f)
        {
            relativeTargetRotationAngle = 360f - relativeTargetRotationAngle;
            relativeTargetRotationAxis = -relativeTargetRotationAxis;
        }
        relativeTargetRotationAngle *= Mathf.PI / 180f;
        relativeTargetRotationAxis.Normalize();

        // Get the current angular velocity.
        Vector3 angularVelocity0 = rigidbody.angularVelocity;

        // Calculate the maximum angular velocity we want.
        float angularVelocityLimit = Mathf.Sqrt(2 * AngularAcceleration * relativeTargetRotationAngle);

        // Accelerate toward a point that is the relative angular position truncated to the velocity limit.
        Vector3 angularVelocityTarget = angularVelocityLimit * relativeTargetRotationAxis;
        Vector3 angularAccelerationStep = angularVelocityTarget - angularVelocity0;
        angularAccelerationStep *= Mathf.Min(1.0f, AngularAcceleration * Time.fixedDeltaTime / angularAccelerationStep.magnitude);

        /*
        if (DebugScale > 0)
        {
            Debug.DrawLine(transform.position + DebugScale * angularVelocityTarget, transform.position + DebugScale * relativeTargetRotationAngle * relativeTargetRotationAxis, Color.blue, 0.5f, false);
            Debug.DrawRay(transform.position, DebugScale * angularVelocityTarget, Color.green, 0.5f, false);
            Debug.DrawRay(transform.position, DebugScale * angularVelocity0, Color.red, 0.5f, false);
        }
        */

        // Apply acceleration.
        if (!float.IsNaN(angularAccelerationStep.x + angularAccelerationStep.y + angularAccelerationStep.z))
            return angularAccelerationStep;
        else
            return Vector3.zero;
    }
}
