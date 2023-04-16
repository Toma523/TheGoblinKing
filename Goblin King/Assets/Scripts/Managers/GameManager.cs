using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject hudCanvas;
    [SerializeField] GameObject gameOverCanvas;
    int currentSceneIndex;
    MenuController menuController;

    void Start(){
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        menuController = FindObjectOfType<MenuController>();
        hudCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
    }

    public void ShowGameOverScreen(){
        gameOverCanvas.SetActive(true);
    }

    public void GoToMenu(){
        menuController.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void PlayAgain(){
        SceneManager.LoadScene(1);
    }

    public void PauseGame(){
        Time.timeScale = 0;
    }

    public void ResumeGame(){
        Time.timeScale = 1;
    }
}
