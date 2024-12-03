using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;

    public BaseUnit unitPrefab;
}

public enum Faction
{
   Hero = 0,
   Enemy = 1,
   NPC = 2
}