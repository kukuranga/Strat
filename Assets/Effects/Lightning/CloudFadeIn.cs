using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudFadeIn : MonoBehaviour
{
    [Tooltip("How long it takes for the objects to fully fade in (seconds).")]
    public float fadeDuration = 1f;

    // Store a list of all child renderers and their unique material instances
    private List<Renderer> childRenderers = new List<Renderer>();
    private List<Material> childMaterials = new List<Material>();

    private void OnEnable()
    {
        // Clear any existing data (in case OnEnable is called multiple times)
        childRenderers.Clear();
        childMaterials.Clear();

        // Get ALL renderers in children (including the parent itself if it has one)
        // If you only want children, you can modify as needed.
        Renderer[] renderersFound = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderersFound)
        {
            // Create a unique material instance for each renderer
            // so that we can modify alpha independently.
            Material matInstance = r.material;

            // Set initial alpha to 0 (invisible)
            Color initialColor = matInstance.color;
            initialColor.a = 0f;
            matInstance.color = initialColor;

            // Store references
            childRenderers.Add(r);
            childMaterials.Add(matInstance);
        }

        // Start the fade-in coroutine (if we have at least one material)
        if (childMaterials.Count > 0)
        {
            StartCoroutine(FadeInRoutine());
        }
    }

    private IEnumerator FadeInRoutine()
    {
        float elapsedTime = 0f;

        // While we're within the fade duration
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // Calculate the current alpha based on elapsed time
            float newAlpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Update alpha on each child's material
            for (int i = 0; i < childMaterials.Count; i++)
            {
                if (childMaterials[i] == null) continue;

                Color c = childMaterials[i].color;
                c.a = newAlpha;
                childMaterials[i].color = c;
            }

            yield return null; // Wait for next frame
        }

        // Ensure final alpha is exactly 1
        for (int i = 0; i < childMaterials.Count; i++)
        {
            if (childMaterials[i] == null) continue;

            Color c = childMaterials[i].color;
            c.a = 1f;
            childMaterials[i].color = c;
        }
    }
}
