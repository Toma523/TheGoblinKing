using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Type", menuName = "Enemy Type")]
public class EnemyType : ScriptableObject
{
    //************************ Enemy type index (0-GreenGoblin, 1-RedGoblin, 2-MetalGoblin) ************************//

    public int enemyTypeIndex = 0;

    //************************ Stats ************************//

    [Header ("Stats")]
    public float moveSpeed = 2f;
    public float bounceSpeed = 8f;
    public float smashedMultiplier = 1.3f;
    public float hvSmashedMultiplier = 1.7f;
    public float detectionDistance = 3f;
    public int damage = 1;
    public float jumpAttackMultiplier = 3.5f;
    public float readyTime = 0.4f;
    public float attackTime = 0.5f;
    public float restTime = 0f;
    public float attackCooldown = 2f;
    public int lives = 5;
    public float bouncingTime = 2f;
    public float frStunnTime = 2f;
    public float hvFSTMultiplier = 2f;

    //************************ Enemy Type Traits ************************//

    public bool isMetal;

    //************************ Sounds ************************//

    [Header ("Friendly Stunn")]
    public AudioClip frStunn1;
    public AudioClip frStunn2;
    [Header ("Bouncing")]
    public AudioClip bouncing1;
    public AudioClip bouncing2;
    public AudioClip bouncing3;

    //************************ Animations ************************//

    [Header ("Animations")]
    public string Goblin_idle = "Goblin_idle";
    public string Goblin_bouncing = "Goblin_bouncing";
    public string Goblin_attack = "Goblin_attack";
    public string Goblin_charge = "Goblin_charge";
    public string Goblin_death = "Goblin_death";
    public string Goblin_stunn = "Goblin_stunn";
}
