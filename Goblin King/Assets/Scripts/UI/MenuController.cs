using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    void GoToChallenges(){
        
    }
    void StartChallenge1(){

    }
    void StartChallenge2(){
        
    }
    void StartChallenge3(){
        
    }
    void GoToOptions(){
        
    }
    void StartInfiniteAttack(){

    }
    void Quit(){
        Debug.Log("Quit!");
        Application.Quit();
    }
}
