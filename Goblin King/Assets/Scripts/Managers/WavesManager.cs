using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavesManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] GameObject greenGoblin;
    [SerializeField] GameObject redGoblin;
    [SerializeField] Waves[] wavesArray;
    [SerializeField] bool canTriggerNextWave = true;
    public float spawningTime = 3f;
    List<GameObject> enemiesList;
    PlayerMovement playerMovement;
    GoblinEnemy goblinEnemy;
    Waves currentWave;
    int enemiesKilled;
    int enemiesAmount;
    int i;

    void Start() {
        playerMovement = FindObjectOfType<PlayerMovement>();
        goblinEnemy = FindObjectOfType<GoblinEnemy>();


        // Idzie do osobnej funkcji
        TriggerNextWave();
    }

    void SpawnEnemies(){
        // Spawn given amount of enemies
        enemiesAmount = Random.Range(currentWave.minEnemiesAmount, currentWave.maxEnemiesAmount + 1);
        for (int i = 0; i < enemiesAmount; i++){
            // Check if can spawn Red Goblin
            if(currentWave.redGoblin){
                // Calculate the chance of spawning Red Goblin
                if(Random.Range(1, 100) <= currentWave.redGoblinChance){
                    // Spawn Red Goblin
                    Instantiate(redGoblin, new Vector3(Random.Range(-10,10), Random.Range(5,-5), 0), Quaternion.identity);
                }
                else if(currentWave.greenGoblin){
                    // Spawn Green Goblin
                    Instantiate(greenGoblin, new Vector3(Random.Range(-10,10), Random.Range(5,-5), 0), Quaternion.identity);
                }
            }
            // Check if can spawn Green Goblin
            else if(currentWave.greenGoblin){
                // Spawn Green Goblin
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
        if(enemiesKilled == enemiesAmount){
            enemiesKilled = 0;
            TriggerNextWave();
        }
    }

    void TriggerNextWave(){
        if(!canTriggerNextWave){return;}
        // Check if there is any wave left to come
        if(i > wavesArray.Length - 1){
            Debug.Log("No more waves!");
            return;
        }
        // Set up next wave
        currentWave = wavesArray[i];
        i++;
        waveText.text = "Wave " + i;
        SpawnEnemies();
        Debug.Log("Next wave!");
    }

    //******************************* Return functions *********************************//

}

