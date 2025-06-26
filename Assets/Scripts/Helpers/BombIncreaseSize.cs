using UnityEngine;

public class BombIncreaseSize : MonoBehaviour
{
    public Bomb _bomb;

    [Header("Size Settings")]
    [SerializeField] private float initialSize = 0.1f;
    [SerializeField] private float maxSize = 2f;
    [SerializeField] private float growthSpeed = 0.5f;

    [Header("Timing")]
    [SerializeField] private float delayBeforeGrowth = 0.2f;

    private float currentSize;
    private float timer;
    private bool isGrowing = false;

    private void OnEnable()
    {
        // Initialize size when object becomes active
        transform.localScale = Vector3.one * initialSize;
        currentSize = initialSize;
        timer = 0f;
        isGrowing = false;
    }

    private void Awake()
    {
        //this.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!isGrowing)
        {
            // Wait for the delay before starting growth
            timer += Time.fixedDeltaTime;
            if (timer >= delayBeforeGrowth)
            {
                isGrowing = true;
                timer = 0f;
            }
            return;
        }

        if (currentSize < maxSize)
        {
            // Increase size gradually
            currentSize += growthSpeed * Time.fixedDeltaTime;
            currentSize = Mathf.Min(currentSize, maxSize);
            transform.localScale = Vector3.one * currentSize;
        }
        else
        {
            _bomb.EndExplosion();
        }
    }
}