using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public RotationAxis axis = RotationAxis.Y; // Default axis is Y
    public float rotationSpeed = 50f; // Speed of rotation in degrees per second

    void Update()
    {
        // Determine the axis to rotate around based on the selection in the Inspector
        Vector3 rotationAxis = Vector3.up; // Default to Y axis

        switch (axis)
        {
            case RotationAxis.X:
                rotationAxis = Vector3.right;
                break;
            case RotationAxis.Y:
                rotationAxis = Vector3.up;
                break;
            case RotationAxis.Z:
                rotationAxis = Vector3.forward;
                break;
        }

        // Rotate the object around the selected axis
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}