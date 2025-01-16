using System.Collections;
using TMPro;
using UnityEngine;

public class BannerMovement : MonoBehaviour
{
    public RectTransform bannerTransform; // Reference to the banner's RectTransform
    public Vector3 startPosition;        // Starting position of the banner
    public Vector3 targetPosition;       // Position the banner moves to
    public Vector3 endPosition;          // Position the banner moves out to
    public float fastSpeed = 1500f;      // Speed for the fast movement
    public float slowSpeed = 200f;       // Speed for the slow movement
    public float slowDownDuration = 2f; // Time for the banner to stay visible
    public float delayBeforeExit = 1f;   // Time to wait before moving out
    public TextMeshProUGUI _ObjectiveName;

    private void OnEnable()
    {
        // Initialize the banner's starting position
        bannerTransform.anchoredPosition = startPosition;

        // Update the TMP text to the Objective name
        _ObjectiveName.text  = ObjectiveManager.Instance._ActiveObjective.name;

        // Start the movement coroutine
        StartCoroutine(MoveBanner());
    }

    private IEnumerator MoveBanner()
    {
        // Move the banner quickly to the target position
        yield return MoveToPosition(targetPosition, fastSpeed);

        // Wait while the banner is in the center
        yield return new WaitForSeconds(slowDownDuration);

        // Move the banner out of the screen quickly
        yield return MoveToPosition(endPosition, fastSpeed);

        // Deactivate the banner object
        gameObject.SetActive(false);
    }

    private IEnumerator MoveToPosition(Vector3 destination, float speed)
    {
        // Move the banner towards the target position
        while (Vector3.Distance(bannerTransform.anchoredPosition, destination) > 0.1f)
        {
            bannerTransform.anchoredPosition = Vector3.MoveTowards(
                bannerTransform.anchoredPosition,
                destination,
                speed * Time.deltaTime
            );
            yield return null;
        }

        // Ensure the banner snaps exactly to the destination
        bannerTransform.anchoredPosition = destination;
    }

    private void OnDrawGizmos()
    {
        if (bannerTransform == null) return;

        // Draw the start position as a green sphere
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(startPosition), 10f);

        // Draw the target position as a yellow sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(targetPosition), 10f);

        // Draw the end position as a red sphere
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(endPosition), 10f);

        // Draw lines connecting the positions
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.TransformPoint(startPosition), transform.TransformPoint(targetPosition));
        Gizmos.DrawLine(transform.TransformPoint(targetPosition), transform.TransformPoint(endPosition));
    }
}
