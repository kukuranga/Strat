
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    //This script will be used to control the logic of the bomb when its tossed 

    //how: when the ability is called on the main unit it will create the object this is attached to
    //  after that the initialize method will be called to pass in any information the bomb needs to function (direction etc)

    Quaternion _Direction;

    [Header("Movement Settings")]
    public float amplitude = 1f;       // Height of the wave
    public float frequency = 1f;       // Speed of the wave
    public float movementSpeed = 1f;   // Forward movement speed

    private Vector3 startPosition;

    public void Initialized(Quaternion Direction)
    {
        _Direction = Direction;

    }
    private void Start()
    {

        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new position using sine function
        float newZ = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
        float newX = startPosition.x + Time.time * movementSpeed;

        // Apply the new position
        transform.position = new Vector3(newX, startPosition.y, newZ);
    }
}
