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
    [SerializeField] float noAttackTime = 1f;
    [SerializeField] List<GameObject> livesList;
    GoblinEnemy goblinEnemy;
    PlayerMovement playerMovement;
    EnemyType enemyType;
    public bool isStunned;
    bool canDamagePlayer;
    float saveDmgProtTime;
    float saveNoAttackTime;
    bool isProtected;
    int enemyDmg;
    int livesListIndex;
    int enemyTypeIndex;

    void Start() 
    {
        goblinEnemy = FindObjectOfType<GoblinEnemy>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        livesText.text = playerLives.ToString();
        saveDmgProtTime = dmgProtTime;
        saveNoAttackTime = noAttackTime;
        SetListOnStart();
    }

    void SetListOnStart()
    {
        for (int i = 0; i < livesList.Count; i++)
        {
            livesList[i].SetActive(false);
            Debug.Log(i);
        }

        for (int i = 0; i < playerLives; i++)
        {
            livesList[i].SetActive(true);
        }
    }

    void Update()
    {
        if(isProtected)
        {
            dmgProtTime -= 1f * Time.deltaTime;
            noAttackTime -= 1f * Time.deltaTime;

            if(noAttackTime <= 0)
            {
                playerMovement.AllowActions();
            }
            
            if(dmgProtTime <= 0)
            {
                isProtected = false;
                playerMovement.StopDmgActions();
            }
        }
    }

    void ChangeList()
    {
        livesListIndex = playerLives;
        livesList[livesListIndex].SetActive(false);
    }

    public void ProcessDamageTaken(GameObject enemy)
    {
        enemyDmg = enemy.GetComponent<GoblinEnemy>().ReturnDamage();

        if(!isProtected)
        {
            if(enemyDmg > 0)
            {
                isProtected = true;
                TakePlayerLives(enemy.tag);
                dmgProtTime = saveDmgProtTime;
                noAttackTime = saveNoAttackTime;
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
