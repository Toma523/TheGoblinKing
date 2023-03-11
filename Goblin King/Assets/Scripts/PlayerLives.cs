using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    [SerializeField] int playerLives = 100;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] float dmgProtTime = 3f;
    [SerializeField] List<GameObject> livesList;
    GoblinEnemy goblinEnemy;
    RedGoblin redGoblin;
    PlayerMovement playerMovement;
    public bool isStunned;
    bool canDamagePlayer;
    float saveDmgProtTime;
    bool isProtected;
    int enemyDmg;
    int livesListIndex;

    void Start() 
    {
        goblinEnemy = FindObjectOfType<GoblinEnemy>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        livesText.text = playerLives.ToString();
        saveDmgProtTime = dmgProtTime;
        redGoblin = FindObjectOfType<RedGoblin>();
        SetListOnStart();
    }

    void Update()
    {
        if(isProtected)
        {
            dmgProtTime -= 1f * Time.deltaTime;
            
            if(dmgProtTime <= 0)
            {
                isProtected = false;
                playerMovement.StopDmgActions();
            }
        }
    }

    void SetListOnStart()
    {
        for (int i = 0; i < playerLives; i++)
        {
            livesList[i].SetActive(true);
        }
    }

    void ChangeList()
    {
        livesListIndex = playerLives;
        livesList[livesListIndex].SetActive(false);
    }

    public void ProcessDamageTaken(GameObject enemy)
    {   
        if(enemy.tag == "Goblin")
        {
            enemyDmg = enemy.GetComponent<GoblinEnemy>().ReturnDamage();
        }

        if(enemy.tag == "RedGoblin")
        {
            enemyDmg = enemy.GetComponent<RedGoblin>().ReturnDamage();
        }

        if(!isProtected)
        {
            if(enemyDmg > 0)
            {
                isProtected = true;
                TakePlayerLives(enemy.tag);
                dmgProtTime = saveDmgProtTime;
            }
        }
    }

    public void TakePlayerLives(string enemyTag)
    {
        playerLives -= enemyDmg;
        livesText.text = playerLives.ToString();
        playerMovement.ManageDmgAnimations();
        ChangeList();
        if(playerLives <= 0)
        {
            playerMovement.DeathAnimation();
        }
    }
}