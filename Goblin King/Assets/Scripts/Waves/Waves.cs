using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Basic Wave")]
public class Waves : ScriptableObject
{
    public int minEnemiesAmount;
    public int maxEnemiesAmount;
    public bool greenGoblin;
    public bool redGoblin;
    public int redGoblinChance;
}
