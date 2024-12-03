using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyUnit : MonoBehaviour
{
    //public GameObject _BaseUnitGO;
    //public BaseUnit _BaseUnit; // The unit prefab to spawn
    public int _AssignedPlayer; // The player who is buying the unit
    //public GameObject _UnitSpawnPoint; // The spawn point for the unit

    //private GameObject _spawnedUnit; // The instantiated unit

    public UnitType _UnitType;

    public void _OnClick()
    {
        switch(_UnitType)
        {
            case UnitType.TestUnit:
                 UnitManager.Instance.GetTestUnit(); //do something with the true/false statement coming from this
                break;
        }


        //// Access the PlayerManager or GameManager to get the player's ATB gauge

        //BaseUnit _BaseUnit = _BaseUnitGO.GetComponent<BaseUnit>();
        //// Check if the player has enough ATB to buy the unit
        //if (ATBManager.Instance.PayATBCost(_BaseUnit._ATBCost))
        //{
        //    // Spawn the unit prefab
        //    _spawnedUnit = Instantiate(_BaseUnitGO);

        //    // Set the initial position to the position of the UnitSpawnPoint
        //    Vector3 spawnPosition = _UnitSpawnPoint.transform.position;

        //    // Optionally, adjust the z-value to -1 (if necessary)
        //    spawnPosition.z = -1f;

        //    // Apply the new spawn position to the instantiated unit
        //    _spawnedUnit.transform.position = spawnPosition;

        //    // Assign the unit to the correct player
        //    BaseUnit unitScript = _spawnedUnit.GetComponent<BaseUnit>();
        //    unitScript.SetAssignedPlayer(_AssignedPlayer);

        //    // Optional: Disable interaction while the unit is being moved
        //    unitScript.ToggleInteraction(false);

        //    // Attach the unit to the cursor via PlayerManager
        //    PlayerManager.Instance.SetUnitInHand(_BaseUnit);
        //}
        //else
        //{
        //    Debug.LogWarning("Not enough ATB to buy this unit.");
        //}
    }



    private void Update()
    {
        // If a unit is attached to the cursor, move it to the cursor's position
        //if (_isUnitAttachedToCursor && _spawnedUnit != null)
        //{
        //    Vector3 mousePosition = Input.mousePosition;
        //    mousePosition.z = Camera.main.nearClipPlane; // Adjust as needed for the camera
        //    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //    // Update the unit's position to follow the mouse
        //    _spawnedUnit.transform.position = new Vector3(worldPosition.x, worldPosition.y, _spawnedUnit.transform.position.z);

        //    // Place the unit on left mouse click
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        if (Physics.Raycast(ray, out RaycastHit hit))
        //        {
        //            Tile targetTile = hit.collider.GetComponent<Tile>();
        //            if (targetTile != null && targetTile._isWalkable)
        //            {
        //                // Place the unit on the tile
        //                _spawnedUnit.transform.position = targetTile.transform.position;

        //                // Assign the tile to the unit
        //                BaseUnit unitScript = _spawnedUnit.GetComponent<BaseUnit>();
        //                unitScript.SetTile(targetTile);

        //                // Enable interaction after placement
        //                unitScript.ToggleInteraction(true);

        //                // Clear cursor attachment
        //                _isUnitAttachedToCursor = false;
        //                _spawnedUnit = null;

        //                Debug.Log($"Unit placed on tile: {targetTile.name}");
        //            }
        //            else
        //            {
        //                Debug.LogWarning("Cannot place unit here. Tile is not walkable.");
        //            }
        //        }
        //   }
        //}
    }
}
