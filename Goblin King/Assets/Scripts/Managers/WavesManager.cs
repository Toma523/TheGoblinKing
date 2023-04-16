using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavesManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject greenGoblin;
    [SerializeField] GameObject redGoblin;
    [SerializeField] GameObject metalGoblin;
    [SerializeField] int activeChallenge = 1;
    [SerializeField] Waves[] challenge1Array;
    [SerializeField] Waves[] challenge2Array;
    [SerializeField] Waves[] challenge3Array;
    [SerializeField] bool canTriggerNextWave = true;
    public float spawningTime = 3f;
    List<GameObject> enemiesList;
    PlayerMovement playerMovement;
    GoblinEnemy goblinEnemy;
    Waves currentWave;
    MenuController menuController;
    int enemiesKilled;
    int enemiesAmount;
    int waveNumber;
    Waves[] activeChallengeArray;
    [Header("Infinite Attack")]
    float timeToNextWave = 10f;
    int enemiesToSpawn = 1;
    bool skipNextWaveTime;
    int totalEnemiesKilled;

    void Start() {
        playerMovement = FindObjectOfType<PlayerMovement>();
        goblinEnemy = FindObjectOfType<GoblinEnemy>();
        menuController = FindObjectOfType<MenuController>();
        // Check if started infinite attack mode
        if(menuController.ReturnInfiniteAttack()){
            Debug.Log("Infinite Attack");
            canTriggerNextWave = false;
            skipNextWaveTime = true;
            scoreText.gameObject.SetActive(true);
            waveText.gameObject.SetActive(false);
            StartCoroutine(InfiniteAttack());
        }
        else{
            Debug.Log("Challenges");
            canTriggerNextWave = true;
            scoreText.gameObject.SetActive(false);
            waveText.gameObject.SetActive(true);
        }
        // Set challenge as the chosen one
        activeChallenge = menuController.ReturnChallengeIndex();
        StartCoroutine(StartSpawningWaves());
    }

    IEnumerator InfiniteAttack(){
        if(skipNextWaveTime){
            yield return new WaitForSeconds(0);
        }
        else{
            yield return new WaitForSeconds(timeToNextWave);
        }
        skipNextWaveTime = false;
        waveNumber++;
        waveText.text = "Wave: " + waveNumber;
        if(waveNumber == 1){enemiesToSpawn++;} // 2
        if(waveNumber == 2){enemiesToSpawn++;} // 3
        if(waveNumber == 3){enemiesToSpawn++;} // 4
        if(waveNumber == 6){enemiesToSpawn++;} // 5
        if(waveNumber == 11){enemiesToSpawn++;} // 6
        if(waveNumber == 16){enemiesToSpawn++;} // 7
        if(waveNumber == 21){enemiesToSpawn++;} // 8
        if(waveNumber == 26){enemiesToSpawn++;} // 9
        if(waveNumber == 31){enemiesToSpawn++;} // 10
        if(waveNumber == 36){enemiesToSpawn++;} // 11
        if(waveNumber == 41){enemiesToSpawn++;} // 12
        enemiesAmount = enemiesToSpawn;
        if(!playerMovement.isDead){
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // Calculate the chance of spawning special Goblins
                int chance = Random.Range(1, 101);
                if(chance <= 40){
                    // Spawn Red Goblin
                    Instantiate(redGoblin, new Vector3(Random.Range(-10,11), Random.Range(5,-6), 0), Quaternion.identity);
                }
                else if(chance <= 50){
                    // Spawn Metal Goblin
                    Instantiate(metalGoblin, new Vector3(Random.Range(-10,11), Random.Range(5,-6), 0), Quaternion.identity);
                }
                else{
                    // Spawn Green Goblin
                    Instantiate(greenGoblin, new Vector3(Random.Range(-10,11), Random.Range(5,-6), 0), Quaternion.identity);
                }    
            }

            StartCoroutine(InfiniteAttack());
        }
        
    }

    IEnumerator StartSpawningWaves(){
        yield return new WaitForSecondsRealtime(1);

        if(activeChallenge == 1){
            activeChallengeArray = challenge1Array;
        }
        if(activeChallenge == 2){
            activeChallengeArray = challenge2Array;
        }
        if(activeChallenge == 3){
            activeChallengeArray = challenge3Array;
        }
        TriggerNextWave();
    }

    void SpawnEnemies(){
        // Spawn given amount of enemies
        enemiesAmount = Random.Range(currentWave.minEnemiesAmount, currentWave.maxEnemiesAmount + 1);
        for (int i = 0; i < enemiesAmount; i++){
            // Check if can spawn any special Goblin
            if(currentWave.redGoblin || currentWave.metalGoblin){
                // Calculate the chance of spawning special Goblins
                int chance = Random.Range(1, 101);
                if(chance <= currentWave.redGoblinChance){
                    // Spawn Red Goblin
                    Instantiate(redGoblin, new Vector3(Random.Range(-10,11), Random.Range(5,-6), 0), Quaternion.identity);
                }
                else if(chance <= currentWave.redGoblinChance + currentWave.metalGoblinChance){
                    // Spawn Metal Goblin
                    Instantiate(metalGoblin, new Vector3(Random.Range(-10,11), Random.Range(5,-6), 0), Quaternion.identity);
                }
                else if(currentWave.greenGoblin){
                    // Spawn Green Goblin
                    Instantiate(greenGoblin, new Vector3(Random.Range(-10,11), Random.Range(5,-6), 0), Quaternion.identity);
                }
            }
            // Check if can spawn Green Goblin
            else if(currentWave.greenGoblin){
                // Spawn Green Goblin
                Instantiate(greenGoblin, new Vector3(Random.Range(-10,11), Random.Range(5,-6), 0), Quaternion.identity);
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
        enemiesKilled++;
        totalEnemiesKilled++;
        scoreText.text = "Score: " + totalEnemiesKilled.ToString();
        // Check if player killed all enemies in current wave
        if(enemiesKilled == enemiesAmount){
            enemiesKilled = 0;
            TriggerNextWave();
        }
    }

    void TriggerNextWave(){
        if(!canTriggerNextWave){return;}
        // Check if there is any wave left to come
        if(waveNumber > activeChallengeArray.Length - 1){
            Debug.Log("No more waves!");
            return;
        }
        // Set up next wave
        currentWave = activeChallengeArray[waveNumber];
        waveNumber++;
        waveText.text = "Wave: " + waveNumber;
        SpawnEnemies();
    }

    //******************************* Return functions *********************************//

}