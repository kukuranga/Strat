using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AbilityType
{
    Ability1,
    Ability2
}

public class AbilityButton : MonoBehaviour
{
    [Header("Ability Settings")]
    [Tooltip("Select which ability this button will trigger.")]
    public AbilityType selectedAbility = AbilityType.Ability1;
    public Slider _AbilitySlider;

    private void Update()
    {
        //TODO: FIX THE VISUALS ON THE SLIDERS, AND THE VALUES ON EACH SLIDER UPDATING
        if(PlayerManager.Instance._SelectedUnit != null)
        {
            Character _Char = PlayerManager.Instance._SelectedUnit.GetComponent<Character>();

            if (selectedAbility == AbilityType.Ability1)
            {
                _AbilitySlider.value = _Char.Ability1CoolDown / _Char.Ability1CoolDownTime;
            }
            else if (selectedAbility == AbilityType.Ability2)
            {
                _AbilitySlider.value = _Char.Ability2CoolDown / _Char.Ability2CoolDownTime;
            }
        }
    }

    public void OnButtonClicked()
    {
        // 1) Check if there is a selected unit at all
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

        // 3) If the cast succeeds, call Ability1 or Ability2
        switch (selectedAbility)
        {
            case AbilityType.Ability1:
                character.Ability1();
                break;

            case AbilityType.Ability2:
                character.Ability2();
                break;
        }
    }
}
