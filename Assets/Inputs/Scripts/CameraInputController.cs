using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputController : Singleton<CameraInputController>
{
    [SerializeField] InputActionReference _CameraMovement;
    [SerializeField] InputActionReference _ZoomMovement;
    [SerializeField] InputActionReference _TiltMovement;

    [SerializeField] private Transform _cam;
    [SerializeField] private float cameraAngle = 50f; // Initial camera angle
    [SerializeField] private float cameraDistance = 10f; // Fixed distance from the grid
    [SerializeField] private float moveSpeed = 5f; // Speed of movement
    [SerializeField] private float zoomSpeed = 2f; // Speed of zoom
    [SerializeField] private float rotationSpeed = 30f; // Speed of rotation
    [SerializeField] private bool useOrthographicCamera = true; // Toggle between orthographic and perspective cameras

    private Vector2 CameraMovementV2;
    private float ZoomMovementFL;
    private float TiltMovementFL;

    private int width, height;

    private void Start()
    {
        width = GridManager.Instance.width;
        height =GridManager.Instance.height;
    }

    private void Update()
    {
        HandleCameraControls();
    }

    private void FixedUpdate()
    {
        CameraMovementV2 = _CameraMovement.action.ReadValue<Vector2>();
        ZoomMovementFL = _ZoomMovement.action.ReadValue<float>();
        TiltMovementFL = _TiltMovement.action.ReadValue<float>();
    }

    private void HandleCameraControls()
    {
        // Horizontal and vertical movement
        float horizontal = CameraMovementV2.x;//Input.GetAxis("Horizontal"); // A/D
        float vertical = CameraMovementV2.y; //Input.GetAxis("Vertical");     // W/S
        _cam.transform.position += new Vector3(horizontal * moveSpeed * Time.deltaTime, vertical * moveSpeed * Time.deltaTime, 0);

        // Zoom in/out
        //if (Input.GetKey(KeyCode.Z)) // Zoom in
        if(ZoomMovementFL > 0)
        {
            if (useOrthographicCamera)
            {
                Camera camera = _cam.GetComponent<Camera>();
                if (camera != null && camera.orthographic)
                {
                    camera.orthographicSize -= zoomSpeed * Time.deltaTime;
                    camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 5f, 50f); // Clamp zoom level
                }
            }
            else
            {
                _cam.transform.position += _cam.transform.forward * zoomSpeed * Time.deltaTime;
            }
        }

        //if (Input.GetKey(KeyCode.X)) // Zoom out
        if(ZoomMovementFL < 0)
        {
            if (useOrthographicCamera)
            {
                Camera camera = _cam.GetComponent<Camera>();
                if (camera != null && camera.orthographic)
                {
                    camera.orthographicSize += zoomSpeed * Time.deltaTime;
                    camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 5f, 50f); // Clamp zoom level
                }
            }
            else
            {
                _cam.transform.position -= _cam.transform.forward * zoomSpeed * Time.deltaTime;
            }
        }

        // Rotate along the X-axis
        if (TiltMovementFL > 0)//(Input.GetKey(KeyCode.Q)) // Rotate left (clockwise around X-axis)
        {
            _cam.transform.RotateAround(GridManager.Instance.GetGridCenter(), Vector3.right, rotationSpeed * Time.deltaTime);
        }

        if (TiltMovementFL < 0)//(Input.GetKey(KeyCode.E)) // Rotate right (counterclockwise around X-axis)
        {
            _cam.transform.RotateAround(GridManager.Instance.GetGridCenter(), Vector3.right, -rotationSpeed * Time.deltaTime);
        }
    }

    public void AdjustCamera()
    {
        // Calculate grid center
        Vector3 gridCenter = new Vector3((width - 1) / 2f, (height - 1) / 2f, 0);

        if (!useOrthographicCamera)
        {
            Camera camera = _cam.GetComponent<Camera>();
            if (camera != null && camera.orthographic)
            {
                float gridAspectRatio = (float)width / height;
                float screenAspectRatio = (float)Screen.width / Screen.height;

                if (gridAspectRatio > screenAspectRatio)
                {
                    camera.orthographicSize = width / (2f * screenAspectRatio);
                }
                else
                {
                    camera.orthographicSize = height / 2f;
                }

                _cam.transform.position = new Vector3(gridCenter.x, gridCenter.y + cameraDistance, -cameraDistance);
                _cam.transform.rotation = Quaternion.Euler(new Vector3(cameraAngle, 0, 0));
            }
        }
        else
        {
            _cam.transform.position = new Vector3(gridCenter.x, gridCenter.y + cameraDistance * Mathf.Tan(Mathf.Deg2Rad * cameraAngle), -cameraDistance);
            _cam.transform.rotation = Quaternion.Euler(new Vector3(cameraAngle, 0, 0));
            _cam.transform.LookAt(gridCenter);
        }
    }
}
