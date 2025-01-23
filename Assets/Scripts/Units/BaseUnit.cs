using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [Header("Unit Data")]
    public int _AssignedPlayer;
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public List<Vector2> Moves;  // Stores all available moves this unit can make

    [Header("Health")]
    public int _MaxHealth;
    public int _CurrentHealth;
    [SerializeField] private HealthBar _HealthBar;

    [Header("ATB Costs")]
    public float _ATBMoveCost;       // Cost to move
    public float _ATBSpawnCost;      // Cost to spawn
    public float _ATBCombatCost;     // Cost to perform an auto-attack (for projectile usage)

    [Header("Auto-Attack Settings")]
    [Tooltip("Range in world units (visual only). The actual detection is handled by the child's trigger collider.")]
    public float autoAttackRange = 3f;
    public GameObject CollisionGameObjects;

    [Tooltip("Prefab that contains the ProjectileHitBox component.")]
    public GameObject projectilePrefab;

    [Tooltip("Where to spawn the projectile. The projectile will become a child of this transform.")]
    public Transform projectileSpawnParent;

    [Tooltip("Time in seconds between attacks (cooldown). Used by AutoAttackLogic on the child.")]
    public float autoAttackCooldown = 2f;

    [Tooltip("A visual bubble (sphere mesh). Must have a trigger collider if used for detection.")]
    public GameObject rangeIndicator;

    [Header("Movement")]
    [Tooltip("How fast the unit moves to a new tile (units/second).")]
    public float moveSpeed = 3f;

    [Tooltip("How fast the unit rotates (degrees/second) toward its direction of movement.")]
    public float rotateSpeed = 360f;

    // Tracks if the unit is currently moving to a tile
    public bool isMoving = false;

    // Whether the unit is currently interactable
    private bool _Interactable;

    private void Awake()
    {
        _HealthBar = GetComponentInChildren<HealthBar>();
    }

    private void Start()
    {
        InitializeUnit();

        // Optionally adjust the rangeIndicator scale for visuals (not for detection)
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * (autoAttackRange * 2f);
            rangeIndicator.SetActive(false); // default hidden
        }
    }

    private void Update()
    {
        SetHurtBox(); // Possibly optimize, but okay for now
    }

    /// <summary>
    /// Initializes health or other stats.
    /// </summary>
    public void InitializeUnit()
    {
        _CurrentHealth = _MaxHealth;
        if (_HealthBar != null)
        {
            _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);
        }
    }

    private void SetHurtBox()
    {
        if (_Interactable) CollisionGameObjects?.SetActive(true);
        else CollisionGameObjects?.SetActive(false);
    }

    /// <summary>
    /// Show or hide the visual range indicator if desired.
    /// </summary>
    public void ToggleAutoAttackRangeVisual(bool show)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(show);
        }
    }

    public void ToggleInteraction(bool val)
    {
        _Interactable = val;
    }

    public void SetTile(Tile target)
    {
        OccupiedTile = target;
    }

    public void SetAssignedPlayer(int i)
    {
        _AssignedPlayer = i;
    }

    public void TakeDamage(int damage)
    {
        _CurrentHealth -= damage;
        if (_HealthBar != null)
            _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);

        // Optional: add some ATB if needed
        ATBManager.Instance.AddToATB(damage * 0.5f);

        if (_CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void ResetUnit()
    {
        _CurrentHealth = _MaxHealth;
        if (_HealthBar != null)
            _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);

        OccupiedTile = null;
        ToggleInteraction(false);

        // Potentially reset ATB, if desired
        //ATBManager.Instance.AddToATB(-ATBManager.Instance.GetATBAmount());
    }

    public void Die()
    {
        // Notify that this unit died
        ObjectiveManager.Instance.OnUnitKilled(this);

        // If this dying unit is the currently selected unit, clear it from PlayerManager
        if (PlayerManager.Instance._SelectedUnit == this)
        {
            PlayerManager.Instance.ClearSelectedUnit();
        }

        // Clear the tile if occupied
        if (OccupiedTile != null)
        {
            OccupiedTile.occupiedUnit = null;
            OccupiedTile = null;
        }

        // Return the unit to the pool
        UnitManager.Instance.KillUnit(this);
    }

    /// <summary>
    /// Public method to move this unit to the target tile's position (smoothly).
    /// Also rotates to face the direction of movement.
    /// </summary>
    public void MoveToTile(Tile targetTile)
    {
        StartCoroutine(MoveUnitRoutine(targetTile.transform.position));
    }

    private IEnumerator MoveUnitRoutine(Vector3 targetPos)
    {
        isMoving = true;

        // Preserve current Z position
        Vector3 startPos = transform.position;
        float startZ = startPos.z;
        targetPos.z = startZ;

        // STEP 1: Fully rotate before moving
        {
            // Compute direction on the 2D plane
            Vector3 direction = targetPos - transform.position;
            direction.z = 0f;

            // Only rotate if there's a non-zero direction
            if (direction.sqrMagnitude > 0.0001f)
            {
                // Calculate desired angle so that +Y axis faces forward
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                angle -= 90f;  // Adjust so sprite's +Y points at the target
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

                // Rotate in place until within a small threshold
                while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                {
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotateSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                // Snap to exact rotation
                transform.rotation = targetRotation;
            }
        }

        // STEP 2: Move to target position
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Snap to final position
        transform.position = targetPos;

        // Movement done
        isMoving = false;
    }


}
