using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [SerializeField] float spawningTime = 3f;
    List<GameObject> enemiesList;
    PlayerMovement playerMovement;
    GoblinEnemy goblinEnemy;
    int enemiesKilled;
    int enemiesAmount;

    void Start() {
        playerMovement = FindObjectOfType<PlayerMovement>();
        goblinEnemy = FindObjectOfType<GoblinEnemy>();


        // Idzie do osobnej funkcji
        playerMovement.NewWaveProtection();
        StartCoroutine(SpawningTime());
    }

    IEnumerator SpawningTime()
    {
        // Enemies can't move when they spawn
        goblinEnemy.CanNotMove();
        yield return new WaitForSeconds(spawningTime);
        // Enemies can move and player can take dmg
        Debug.Log("Enemies can move");
        goblinEnemy.CanMove();
        playerMovement.NewWaveProtection();
    }

    public void EnemyKilled(){
        // Add enemy to killed enemies count
        enemiesKilled += 1;
        // Check if player killed all enemies in current wave
        if(enemiesKilled >= enemiesAmount){
            TriggerNextWave();
        }
    }

    void TriggerNextWave(){
        Debug.Log("Next wave!");
    }

    //******************************* Return functions *********************************//


}

