using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BaseUnit
{
    [Header("Movement")]
    public float _ATBMoveCost = 5;
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;
    public List<Vector2> Moves; // Common move definitions or stats
    public bool isMoving = false;
    public List<Tile> _SelectedUnitsTilesMovement;

    [Header("Attack")]
    public Attack attack; // Scriptable Object reference to the attack logic
    public GameObject rangeIndicator;
    public bool isAttacking = false;
    public float nextAutoAttackTime = 0f;
    public int _ATBCombatCost;
    public float autoAttackCooldown;

    protected override void Start()
    {
        base.Start(); // Call BaseUnit Start
        _Type = BaseUnitType.Character;

        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * (attack.attackRange * 2f); // Adjusting based on attack range
            rangeIndicator.SetActive(false);
        }
    }

    protected override void Update()
    {
        base.Update();
        // Additional Character-specific update logic
    
    }

    public virtual void Ability1()
    {
        Debug.Log($"{UnitName} uses Ability1.");
    }

    public virtual void Ability2()
    {
        Debug.Log($"{UnitName} uses Ability2.");
    }

    public void MoveToTile(Tile targetTile)
    {
        // Prevent movement while attacking
        if (isAttacking)
        {
            Debug.Log($"{UnitName} is attacking and cannot move yet.");
            return;
        }
        StartCoroutine(MoveUnitRoutine(targetTile.transform.position));
    }

    private IEnumerator MoveUnitRoutine(Vector3 targetPos)
    {
        // Check if we have enough ATB for movement
        if (ATBManager.Instance.GetATBAmount() < _ATBMoveCost)
        {
            Debug.Log("Not enough ATB to move.");
            yield break;
        }

        isMoving = true;

        Vector3 startPos = transform.position;
        float startZ = startPos.z;
        targetPos.z = startZ;

        // 1) Rotate first
        {
            Vector3 direction = targetPos - transform.position;
            direction.z = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                angle -= 90f;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

                while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                {
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotateSpeed * Time.deltaTime
                    );
                    yield return null;
                }
                transform.rotation = targetRotation;
            }
        }

        // 2) Move second
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    // --------------------------------------------------
    // Attack Logic (Replaced with Scriptable Object)
    // --------------------------------------------------
    public virtual void TryAutoAttack(HurtBox hurtBox)
    {
        if (isMoving)
        {
            Debug.Log($"{UnitName} is moving; cannot auto-attack yet.");
            return;
        }

        if (isAttacking || Time.time < nextAutoAttackTime)
        {
            Debug.Log($"{UnitName} is attacking or on cooldown; cannot auto-attack yet.");
            return;
        }

        // Use the Scriptable Object Attack Logic
        attack.PerformAttack(this, hurtBox);
    }

    public void ToggleAutoAttackRangeVisual(bool show)
    {
        if (rangeIndicator != null)
            rangeIndicator.SetActive(show);
    }

    public override void OnSelectiion()
    {
        base.OnSelectiion();

        ToggleAutoAttackRangeVisual(true);
        if (_SelectedUnitsTilesMovement == null)
            _SelectedUnitsTilesMovement = new List<Tile>();

        _SelectedUnitsTilesMovement = GridManager.Instance.GetAllTilesInRange(
            Moves,
            OccupiedTile,
            true
        );

        foreach (Tile t in _SelectedUnitsTilesMovement)
        {
            t.SetSelectedTile(true);
        }
    }

    public override void ClearSelection()
    {
        base.ClearSelection();

        if (_SelectedUnitsTilesMovement != null)
        {
            foreach (Tile t in _SelectedUnitsTilesMovement)
            {
                if (t != null) t.SetSelectedTile(false);
            }
            _SelectedUnitsTilesMovement.Clear();
        }

        ToggleAutoAttackRangeVisual(false);
    }
    public IEnumerator RotateUnitTowards(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.z = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle -= 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );
                yield return null;
            }
            transform.rotation = targetRotation;
        }
    }
}
