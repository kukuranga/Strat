using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Structure
{

    //this object will be used to create resources over time. 
    //todo: settle on the resource system of the game before continuing with this object

    public float RewardInterval;
    public BaseUnit _ownerUnit;

    public void Initalize(BaseUnit owner)
    {
        _ownerUnit = owner;
        StartCoroutine(rewardRoutiine());
    }

    private void GainReward()
    {
        ResourceManager.Instance.AddBlueResource(1);
        _CharacterTextBox.UpdateMessage("+1 Magic");
    }

    private IEnumerator rewardRoutiine()
    {
        while(true)
        {
            yield return new WaitForSeconds(RewardInterval);
            GainReward();
        }
    }
}
