using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public int _PlayerNumber;
    public bool _HasUnitInHand = false;
    public BaseUnit _UnitInHand;
    public GameObject _BaseUnitGameObject;

    public float fixedZPosition = -1f; // Fixed Z position for the unit in world space

    // To be called by the multiplayer manager to set the correct player number to this player
    public void SetPlayerNumber(int i)
    {
        _PlayerNumber = i;
    }

    // Sets the unit in hand and allows the player to move it to a selected square
    public void SetUnitInHand(BaseUnit _unit, GameObject _GameObj)
    {
        _UnitInHand = _unit;
        _BaseUnitGameObject = _GameObj;
        _HasUnitInHand = true;

        // Optional: Disable unit interaction while it's in hand
        _UnitInHand.ToggleInteraction(false);
        MenuManager.Instance.ShowSelectedHero(_UnitInHand);
    }

    // Clears the unit from the player's hand
    public void EmptyHand()
    {
        if (_UnitInHand != null)
        {
            // Optional: Re-enable unit interaction
            _UnitInHand.ToggleInteraction(true);
        }

        _UnitInHand = null;
        _BaseUnitGameObject = null;
        _HasUnitInHand = false;
        MenuManager.Instance.ClearSelectedHero();
    }

    private void Update()
    {
        if (_HasUnitInHand && _UnitInHand != null && _BaseUnitGameObject != null)
        {
            //FollowMouse();
        }
    }

    //TODO: this does not work with the current camera setup, to fix make the raycast to each object in the game have the orb set its posiiton to that orb when hovered
    private void FollowMouse()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        // Convert the mouse position to world space and set the Z position to the fixed depth
        mousePosition.z = Camera.main.nearClipPlane; // Adjust near clip plane if necessary
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Update the GameObject's position to follow the cursor while keeping its Z position fixed
        _BaseUnitGameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, fixedZPosition);
    }
}
