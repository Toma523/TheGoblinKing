using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject challengesCanvas;
    GameManager gameManager;
    int challengeIndex;

    void Start(){
        DontDestroyOnLoad(gameObject);
        gameManager = FindObjectOfType<GameManager>();
        challengesCanvas.SetActive(false);
    }

    public void GoToChallenges(){
        menuCanvas.SetActive(false);
        challengesCanvas.SetActive(true);
    }

    public void StartChallenge1(){
        gameManager.OpenGameScene();
        challengeIndex = 1;
    }

    public void StartChallenge2(){
        gameManager.OpenGameScene();
        challengeIndex = 2;
    }

    public void StartChallenge3(){
        gameManager.OpenGameScene();
        challengeIndex = 3;
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

    //**************************** Return Functions *************************//

    public int ReturnChallengeIndex(){
        return challengeIndex;
    }
}
