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
    [SerializeField] bool infiniteAttack;

    void Start(){
        DontDestroyOnLoad(gameObject);
        gameManager = FindObjectOfType<GameManager>();
        challengesCanvas.SetActive(false);
    }

    public void GoToChallenges(){
        infiniteAttack = false;
        menuCanvas.SetActive(false);
        challengesCanvas.SetActive(true);
    }

    public void StartChallenge1(){
        challengeIndex = 1;
        SceneManager.LoadScene(1);
    }

    public void StartChallenge2(){
        challengeIndex = 2;
        SceneManager.LoadScene(1);
    }

    public void StartChallenge3(){
        challengeIndex = 3;
        SceneManager.LoadScene(1);
    }

    public void GoToMenu(){
        challengesCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void GoToOptions(){
        
    }

    public void StartInfiniteAttack(){
        infiniteAttack = true;
        SceneManager.LoadScene(1);
    }

    public void Quit(){
        Debug.Log("Quit!");
        Application.Quit();
    }

    //**************************** Return Functions *************************//

    public int ReturnChallengeIndex(){
        return challengeIndex;
    }

    public bool ReturnInfiniteAttack(){
        return infiniteAttack;
    }
}
