using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterTextBox : MonoBehaviour
{
    public BaseUnit _OwnerUnit;

    // TMP pro for the text box
    public TextMeshProUGUI _Text;

    // Variables for text box movement and fading
    private Vector3 originalPosition;
    private Color originalColor;
    private float fadeDuration = 2f;
    private Coroutine currentAnimation;
    private bool _IsActive = false;

    // Reference to main camera for facing
    private Camera mainCamera;

    //TODO:fIX THE MOVING UP SCRIPT, MAKE THE SPEED OF THE FADE FASTER

    private void Awake()
    {
        // Store original position and color
        originalPosition = transform.localPosition;
        originalColor = _Text.color;
        mainCamera = Camera.main;
        ClearMsg();
    }

    private void LateUpdate()
    {
        // Make the health bar face the camera
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }

        if(_IsActive)
        {
            transform.position += new Vector3(0, 0, -0.01f);
        }
    }

    public void UpdateMessage(string msg)
    {
        _Text.text = msg;

        // Reset position and color
        transform.localPosition = originalPosition;
        _Text.color = originalColor;

        // Ensure the text is active
        _Text.gameObject.SetActive(true);

        // Stop any existing animation
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        // Start new animation
        currentAnimation = StartCoroutine(MoveTextUp());
    }

    private IEnumerator MoveTextUp()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.localPosition;
        Color startColor = _Text.color;
        _IsActive = true;

        while (elapsedTime < fadeDuration)
        {
            // Fade the text over time
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            _Text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset after animation completes
        ClearMsg();
    }

    public void ClearMsg()
    {
        _Text.text = "";
        _IsActive = false;
        _Text.gameObject.SetActive(false);
        transform.localPosition = originalPosition;
    }
}