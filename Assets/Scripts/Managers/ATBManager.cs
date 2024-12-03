using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ATBManager : Singleton<ATBManager>
{
    public float ATBAmount = 0;
    public float ATBIncreaseRate = 0.10f;

    [SerializeField] private Slider atbSlider; // Reference to the UI Slider (if using a Slider)
    [SerializeField] private Image atbImage;  // Reference to the UI Image (if using an Image)

    private void Start()
    {
        // Initialize UI element values
        UpdateATBUI();
    }

    private void FixedUpdate()
    {
        // Increase ATB and clamp its value
        ATBAmount += ATBIncreaseRate;
        ATBAmount = Mathf.Clamp(ATBAmount, 0, 100);

        // Update UI
        UpdateATBUI();
    }

    private void UpdateATBUI()
    {
        if (atbSlider != null)
        {
            atbSlider.value = ATBAmount / 100f; // Normalize to a 0-1 range
        }

        if (atbImage != null)
        {
            atbImage.fillAmount = ATBAmount / 100f; // Normalize to a 0-1 range
        }
    }

    public bool PayATBCost(float _cost)
    {
        if (ATBAmount >= _cost)
        {
            ATBAmount -= _cost;
            UpdateATBUI(); // Update UI after spending ATB
            return true;
        }
        else
            return false;
    }

    public void AddToATB(float _amount)
    {
        ATBAmount += _amount;
        ATBAmount = Mathf.Clamp(ATBAmount, 0, 100);
        UpdateATBUI(); // Update UI after adding to ATB
    }

    public float GetATBAmount()
    {
        return ATBAmount;
    }
}
