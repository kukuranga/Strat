using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    //contains all the information and values for all the resources in the game (Not ATB)

    public int _GreenResource;
    public int _RedResource;
    public int _BlueResource;

    public TextMeshProUGUI _GreenText;
    public TextMeshProUGUI _RedText;
    public TextMeshProUGUI _BlueText;

    private void Update()
    {
        UpdateTextBoxes();
    }

    #region SpendResource
    public bool SpendGreenResource(int amount)
    {
        if(amount > _GreenResource)
            return false;

        _GreenResource -= amount;
        return true;
    }
    public bool SpendRedResource(int amount)
    {
        if (amount > _RedResource)
            return false;

        _RedResource -= amount;
        return true;
    }
    public bool SpendBlueResource(int amount)
    {
        if (amount > _BlueResource)
            return false;

        _BlueResource -= amount;
        return true;
    }
    #endregion

    #region AddResource
    public void AddGreenResource(int val)
    {
        _GreenResource += val;
    }
    public void AddRedResource(int val)
    {
        _RedResource += val;
    }
    public void AddBlueResource(int val)
    {
        _BlueResource += val;
    }
    #endregion

    #region Getters
    public int GetGreenResource()
    {
        return _GreenResource;
    }
    public int GetRedResource()
    {
        return _RedResource;
    }
    public int GetBlueResource()
    {
        return _BlueResource;
    }
    #endregion

    public void UpdateTextBoxes()
    {
        _GreenText.text = _GreenResource.ToString();
        _RedText.text = _RedResource.ToString();
        _BlueText.text = _BlueResource.ToString();
    }
}
