using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [SerializeField] private float stopDuration = 0.1f; // Duration of the hit stop
    [SerializeField] private float slowMotionScale = 0.05f; // Time scale during the hit stop

    public void TriggerHitStop()
    {
        StartCoroutine(DoHitStop());
    }

    private IEnumerator DoHitStop()
    {
        float originalTimeScale = Time.timeScale;

        // Slow down time
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust physics time step

        // Wait for the duration
        yield return new WaitForSecondsRealtime(stopDuration);

        // Restore time
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f * originalTimeScale; // Reset physics time step
    }
}
