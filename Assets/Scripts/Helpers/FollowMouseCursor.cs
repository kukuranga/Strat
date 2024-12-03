using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouseCursor : MonoBehaviour
{
    public float cursorDepth = -1f; // Fixed depth at which the object will follow the cursor

    void Update()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        // Set the Z value for depth conversion to -1 (fixed depth)
        mousePosition.z = cursorDepth;

        // Convert screen space to world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Update the object's position while keeping its Z value fixed at -1
        transform.position = new Vector3(worldPosition.x, worldPosition.y, cursorDepth);
    }
}
