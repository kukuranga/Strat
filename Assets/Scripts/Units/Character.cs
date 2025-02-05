using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BaseUnit
{

    [Header("Movement")]
    public float _ATBMoveCost = 5;
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;
    public bool isMoving = false;
    public List<Tile> _SelectedUnitsTilesMovement;


    [Header("Attack")]
    public float _ATBCombatCost = 2f;  // Example cost to perform an auto-attack
    public float autoAttackRange = 3f;
    public float autoAttackCooldown = 2f;
    public GameObject rangeIndicator;
    public bool isAttacking = false;
    public float nextAutoAttackTime = 0f;

    protected override void Start()
    {
        base.Start(); // Call BaseUnit Start

        _Type = BaseUnitType.Character;
        // If you want a range indicator
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * (autoAttackRange * 2f);
            rangeIndicator.SetActive(false);
        }
    }

    protected override void Update()
    {
        base.Update();
        // Additional Character-specific update logic
    }

    // --------------------------------------------------
    // Movement Logic
    // --------------------------------------------------
    /// <summary>
    /// Move to a target tile position, if not attacking.
    /// </summary>
    public void MoveToTile(Tile targetTile)
    {
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
            yield break; // <-- Immediately stop the coroutine
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
    // Attack Logic
    // --------------------------------------------------
    /// <summary>
    /// Show or hide the attack range indicator.
    /// </summary>
    public void ToggleAutoAttackRangeVisual(bool show)
    {
        if (rangeIndicator != null)
            rangeIndicator.SetActive(show);
    }

    /// <summary>
    /// Attempt an auto-attack on a HurtBox in range.
    /// </summary>
    public virtual void TryAutoAttack(HurtBox hurtBox)
    {

    }

    // --------------------------------------------------
    // (Optional) Abilities
    // --------------------------------------------------
    public virtual void Ability1()
    {
        Debug.Log($"{UnitName} uses Ability1.");
    }

    public virtual void Ability2()
    {
        Debug.Log($"{UnitName} uses Ability2.");
    }

    public override void OnSelectiion()
    {
        base.OnSelectiion();

        //Todo: Move the activation section in the player manager to here
        //also show all menu options available to the character

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

        //clear all the selected tiles by the on selected method

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
}
