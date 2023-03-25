using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [SerializeField] GameObject greenGoblin;
    [SerializeField] GameObject redGoblin;
    [SerializeField] Waves waves;
    public float spawningTime = 3f;
    List<GameObject> enemiesList;
    PlayerMovement playerMovement;
    GoblinEnemy goblinEnemy;
    int enemiesKilled;

    void Start() {
        playerMovement = FindObjectOfType<PlayerMovement>();
        goblinEnemy = FindObjectOfType<GoblinEnemy>();


        // Idzie do osobnej funkcji
        SpawnEnemies();
    }

    void SpawnEnemies(){
        for (int i = 0; i < waves.enemiesAmount; i++){

            if(waves.redGoblin){

                if(Random.Range(1, 100) <= waves.redGoblinChance){
                    Instantiate(redGoblin, new Vector3(Random.Range(-10,10), Random.Range(5,-5), 0), Quaternion.identity);
                }
                else if(waves.greenGoblin){
                    Instantiate(greenGoblin, new Vector3(Random.Range(-10,10), Random.Range(5,-5), 0), Quaternion.identity);
                }
            }
            else if(waves.greenGoblin){
                Instantiate(greenGoblin, new Vector3(Random.Range(-10,10), Random.Range(5,-5), 0), Quaternion.identity);
            }
        }

        StartCoroutine(SpawningTime());
    }

    IEnumerator SpawningTime()
    {
        // Enemies can't move when they spawn and can't damage player
        playerMovement.ChangeWaveProtection();
        yield return new WaitForSeconds(spawningTime);
        // Enemies can move and player can take dmg
        Debug.Log("Enemies can move");
        playerMovement.ChangeWaveProtection();
    }

    public void EnemyKilled(){
        // Add enemy to killed enemies count
        enemiesKilled += 1;
        // Check if player killed all enemies in current wave
        if(enemiesKilled == waves.enemiesAmount){
            enemiesKilled = 0;
            TriggerNextWave();
        }
    }

    void TriggerNextWave(){
        Debug.Log("Next wave!");
        SpawnEnemies();
    }

    //******************************* Return functions *********************************//


}

