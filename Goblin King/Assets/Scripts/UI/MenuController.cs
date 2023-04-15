using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject challengesCanvas;
    GameManager gameManager;
    WavesManager wavesManager;

    void Start(){
        DontDestroyOnLoad(gameObject);
        gameManager = FindObjectOfType<GameManager>();
        wavesManager = FindObjectOfType<WavesManager>();
        challengesCanvas.SetActive(false);
    }

    public void GoToChallenges(){
        menuCanvas.SetActive(false);
        challengesCanvas.SetActive(true);
    }

    public void StartChallenge1(){
        wavesManager.SetActiveChallenge(1);
        gameManager.OpenGameScene();
    }

    public void StartChallenge2(){
        wavesManager.SetActiveChallenge(2);
        gameManager.OpenGameScene();
    }

    public void StartChallenge3(){
        wavesManager.SetActiveChallenge(3);
        gameManager.OpenGameScene();
    }

    public void GoToMenu(){
        challengesCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void GoToOptions(){
        
    }

    public void StartInfiniteAttack(){

    }

    public void Quit(){
        Debug.Log("Quit!");
        Application.Quit();
    }
}
