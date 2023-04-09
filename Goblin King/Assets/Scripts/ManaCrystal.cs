using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCrystal : MonoBehaviour
{
    PlayerMovement player;
    [SerializeField] Rigidbody2D myRgbd;
    [SerializeField] CircleCollider2D myCollider;
    [SerializeField] int addAmount = 1;
    [SerializeField] float flyingSpeed = 3f;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void FixedUpdate() {
        myRgbd.velocity = (player.transform.position - transform.position).normalized * flyingSpeed * 100 * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            player.CollectCrystal(gameObject, addAmount);
        }
    }
}
