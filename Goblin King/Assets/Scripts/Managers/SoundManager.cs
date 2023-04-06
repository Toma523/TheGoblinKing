using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header ("PLAYER:")]
    [Header ("Whoosh")]
    [SerializeField] AudioClip whoosh1;
    [SerializeField] AudioClip whoosh2;
    [Header ("Heavy Whoosh")]
    [SerializeField] AudioClip hvWhoosh1;
    [SerializeField] AudioClip hvWhoosh2;
    [Header ("Hit")]
    [SerializeField] AudioClip hit1;
    [SerializeField] AudioClip hit2;
    [SerializeField] AudioClip hit3;
    [SerializeField] AudioClip hit4;
    [SerializeField] AudioClip hit5;
    [SerializeField] AudioClip hit6;
    [SerializeField] AudioClip hit7;
    [Header ("Heavy Hit")]
    [SerializeField] AudioClip hvHit1;
    [SerializeField] AudioClip hvHit2;
    [Header ("Hitting wall")]
    [SerializeField] AudioClip wall1;
    [SerializeField] AudioClip wall2;
    [Header ("Friendly Stunn")]
    [SerializeField] AudioClip frStunn1;
    [SerializeField] AudioClip frStunn2;
    [Header ("Bouncing")]
    [SerializeField] AudioClip bouncing1;
    [SerializeField] AudioClip bouncing2;
    [SerializeField] AudioClip bouncing3;
    AudioSource audioSource;
    [Header ("Dash")]
    [SerializeField] AudioClip dash1;
    [Header ("Charge")]
    [SerializeField] AudioClip charge1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayWhoosh(){
        // Set random clip out of available
        if(Random.Range(0,2) == 0){
            audioSource.clip = whoosh1;
        }
        else{
            audioSource.clip = whoosh2;
        }
        // Play clip
        audioSource.pitch = 2f;
        audioSource.Play();
    }

    public void PlayHit(){
        // Set random clip out of available
        int r = Random.Range(0,7);
        if(r == 0){
            audioSource.clip = hit1;
        }
        if(r == 1){
            audioSource.clip = hit2;
        }
        if(r == 2){
            audioSource.clip = hit3;
        }
        if(r == 3){
            audioSource.clip = hit4;
        }
        if(r == 4){
            audioSource.clip = hit5;
        }
        if(r == 5){
            audioSource.clip = hit6;
        }
        if(r == 6){
            audioSource.clip = hit7;
        }
        // Play clip
        audioSource.pitch = 1.2f;
        audioSource.volume = 0.8f;
        audioSource.Play();
    }

    public void PlayHitWall(){
        // Set random clip out of available
        if(Random.Range(0,2) == 0){
            audioSource.clip = wall1;
        }
        else{
            audioSource.clip = wall2;
        }
        // Play clip
        audioSource.pitch = 0.8f;
        audioSource.Play();
    }

    public void PlayHvHit(){
        // Set random clip out of available
        if(Random.Range(0,2) == 0){
            audioSource.clip = hvHit1;
        }
        else{
            audioSource.clip = hvHit2;
        }
        // Play clip
        audioSource.pitch = 1.2f;
        audioSource.volume = 0.8f;
        audioSource.Play();
    }

    public void PlayHvWhoosh(){
        // Set random clip out of available
        if(Random.Range(0,2) == 0){
            audioSource.clip = hvWhoosh1;
        }
        else{
            audioSource.clip = hvWhoosh2;
        }
        // Play clip
        audioSource.pitch = 2f;
        audioSource.Play();
    }

    public void PlayDash(){
        // Play clip
        audioSource.clip = dash1;
        audioSource.pitch = 1f;
        audioSource.Play();
    }

    public void PlayCharge(){
        // Play clip
        audioSource.clip = charge1;
        audioSource.pitch = 2.5f;
        audioSource.Play();
    }
}
