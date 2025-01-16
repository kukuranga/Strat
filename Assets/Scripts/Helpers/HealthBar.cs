using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _Slider;

    private Camera _mainCamera;

    private void Start()
    {
        // Cache the main camera reference
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // Make the health bar face the camera
        if (_mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.forward);
        }
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        _Slider.value = currentValue / maxValue;
    }
}
