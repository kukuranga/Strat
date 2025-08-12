using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothAxisMovement : MonoBehaviour
{
    public enum MovementAxis { X, Y, Z }
    
    [Header("Movement Settings")]
    public MovementAxis axis = MovementAxis.Y; // Default to Y axis (up/down)
    public float movementDistance = 2f;       // Total distance to move
    public float movementSpeed = 1f;         // Speed of movement
    
    [Header("Advanced Settings")]
    public bool useUnscaledTime = false;      // Use unscaled time for pause-resistant movement
    public AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1); // Movement easing
    
    private Vector3 startPosition;
    private float currentLerpTime;
    private bool movingUp = true;

    void Start()
    {
        // Store the initial position as our starting point
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the delta time based on settings
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        
        // Update the lerp time based on direction and speed
        currentLerpTime += (movingUp ? 1 : -1) * movementSpeed * deltaTime;
        currentLerpTime = Mathf.Clamp01(currentLerpTime);
        
        // Calculate the eased t value using the animation curve
        float easedT = movementCurve.Evaluate(currentLerpTime);
        
        // Calculate the target offset based on the selected axis
        Vector3 offset = Vector3.zero;
        switch (axis)
        {
            case MovementAxis.X:
                offset = Vector3.right * movementDistance;
                break;
            case MovementAxis.Y:
                offset = Vector3.up * movementDistance;
                break;
            case MovementAxis.Z:
                offset = Vector3.forward * movementDistance;
                break;
        }
        
        // Lerp between start and end positions
        transform.position = Vector3.Lerp(startPosition, startPosition + offset, easedT);
        
        // Reverse direction when reaching either end
        if (currentLerpTime >= 1f || currentLerpTime <= 0f)
        {
            movingUp = !movingUp;
        }
    }

    // Optional: Draw gizmos in the editor to visualize the movement path
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            startPosition = transform.position;
        }
        
        Vector3 endPosition = startPosition;
        switch (axis)
        {
            case MovementAxis.X:
                endPosition += Vector3.right * movementDistance;
                break;
            case MovementAxis.Y:
                endPosition += Vector3.up * movementDistance;
                break;
            case MovementAxis.Z:
                endPosition += Vector3.forward * movementDistance;
                break;
        }
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPosition, endPosition);
        Gizmos.DrawSphere(startPosition, 0.1f);
        Gizmos.DrawSphere(endPosition, 0.1f);
    }
}