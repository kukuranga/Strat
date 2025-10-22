using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityInputHelper : MonoBehaviour
{
    public InputActionReference _InputAbility1;
    public InputActionReference _InputAbility2;

    private void OnEnable()
    {
        _InputAbility1.action.started += UseAbility1;
        _InputAbility2.action.started += UseAbility2;
    }

    private void OnDisable()
    {
        _InputAbility1.action.started -= UseAbility1;
        _InputAbility2.action.started -= UseAbility2;
    }

    public void UseAbility1(InputAction.CallbackContext obj)
    {
        if (PlayerManager.Instance._SelectedUnit == null)
        {
            Debug.LogWarning("No unit selected! Cannot cast an ability.");
            return;
        }

        // 2) Try casting the selected unit to a 'Character'
        Character character = PlayerManager.Instance._SelectedUnit as Character;
        if (character == null)
        {
            Debug.LogWarning("Selected unit is not a Character. It does not have abilities.");
            return;
        }

        character.Ability1();
    }
    public void UseAbility2(InputAction.CallbackContext obj)
    {
        if (PlayerManager.Instance._SelectedUnit == null)
        {
            Debug.LogWarning("No unit selected! Cannot cast an ability.");
            return;
        }

        // 2) Try casting the selected unit to a 'Character'
        Character character = PlayerManager.Instance._SelectedUnit as Character;
        if (character == null)
        {
            Debug.LogWarning("Selected unit is not a Character. It does not have abilities.");
            return;
        }

        character.Ability2();
    }
}
