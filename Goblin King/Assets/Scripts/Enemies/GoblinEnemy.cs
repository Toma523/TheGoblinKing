using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    //[SerializeField] float gigaSpeed = 50f;
    [SerializeField] float bounceSpeed = 16f;
    [SerializeField] float smashedMultiplier = 1.2f;
    [SerializeField] float hvSmashedMultiplier = 1.5f;
    [SerializeField] float detectionDistance = 6f;
    [SerializeField] int damage = 10;
    [SerializeField] float jumpAttackMultiplier = 2f;
    [SerializeField] float readyTime = 0.2f;
    [SerializeField] float attackTime = 0.5f;
    [SerializeField] float restTime = 0.2f;
    [SerializeField] float attackCooldown = 2f;
    [SerializeField] int lives = 15;
    [SerializeField] float bouncingTime = 1f;
    [SerializeField] float frStunnTime = 2f;
    [SerializeField] float hvFSTMultiplier = 2f;
    [SerializeField] ManaCrystal crystal;
    GameObject player;
    Rigidbody2D myRigidbody;
    CircleCollider2D myBodyCollider;
    Transform myTransform;
    Animator myAnimator;
    Vector3 playerPosition;
    Vector2 velocity;
    Quaternion playerRotation;
    PlayerLives playerLives;
    Vector2 retVelocity;
    PlayerMovement playerMovement;
    TrailRenderer myTrailRenderer;
    EnemySpin bodySpin;
    WavesManager wavesManager;
    SoundManager soundManager;
    bool isSmashed;
    bool isTouchingPlayer;
    bool isBouncing;
    int yPositionRevert = 1;
    int xPositionRevert = 1;
    bool canStopBouncing;
    bool canStartTimer = true;
    bool isHit;
    bool canMove = true;
    float saveBounceSpeed;
    float savefrStunnTime;
    float saveMoveSpeed;
    bool canAttack = true;
    bool isFollowingPlayer = true;
    bool canDoSmashAnimation;
    bool isDoingSmashAnimation;
    bool canStunn = true;
    int saveDmg;
    bool isBouncingFast;

    const string Goblin_idle = "Goblin_idle";
    const string Goblin_bouncing = "Goblin_bouncing";
    const string Goblin_attack = "Goblin_attack";
    const string Goblin_charge = "Goblin_charge";
    const string Goblin_death = "Goblin_death";
    const string Goblin_stunn = "Goblin_stunn";

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<CircleCollider2D>();
        myTransform = GetComponent<Transform>();
        player = GameObject.Find("Player");
        playerLives = FindObjectOfType<PlayerLives>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        saveBounceSpeed = bounceSpeed;
        savefrStunnTime = frStunnTime;
        saveMoveSpeed = moveSpeed;
        myAnimator = GetComponentInChildren<Animator>();
        myTrailRenderer = GetComponent<TrailRenderer>();
        saveDmg = damage;
        bodySpin = GetComponentInChildren<EnemySpin>();
        wavesManager = FindObjectOfType<WavesManager>();
        soundManager = FindObjectOfType<SoundManager>();

        StartCoroutine(DontMoveOnSpawn());
    }

    IEnumerator DontMoveOnSpawn(){
        canMove = false;
        myBodyCollider.enabled = false;
        yield return new WaitForSeconds(wavesManager.spawningTime);
        canMove = true;
        myBodyCollider.enabled = true;
    }

    void Update()
    {
        if(isFollowingPlayer)
        {
            playerPosition = player.GetComponent<Transform>().position;
        }
        IsTouchingPlayer();
        CheckDistance();
        AnimationSwitch();
        if(!isSmashed)
        {
            Vector3 direction = (playerPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            myRigidbody.rotation = angle;
        }
    }

    void FixedUpdate() 
    {
        Move();
    }

    void CheckDistance()
    {
        if(isSmashed){return;}

        if(canAttack && canMove)
        {
            if(Vector3.Distance(playerPosition,myTransform.position) <= detectionDistance)
            {
                playerPosition = (playerPosition - myTransform.position) * 10;
                canAttack = false;
                isFollowingPlayer = false;
                StartCoroutine(Attack());
                Debug.Log("Detected");
            }
        }
    }

    IEnumerator Attack()
    {
        myAnimator.Play(Goblin_charge);
        moveSpeed = 0;
        yield return new WaitForSeconds(readyTime);
        if(!isSmashed && canMove){myAnimator.Play(Goblin_attack);}
        myTrailRenderer.emitting = true;
        moveSpeed = saveMoveSpeed * jumpAttackMultiplier;
        yield return new WaitForSeconds(attackTime);
        yield return new WaitForSeconds(restTime);
        if(!isSmashed && canMove){myAnimator.Play(Goblin_idle);}
        if(!isSmashed){myTrailRenderer.emitting = false;}
        moveSpeed = saveMoveSpeed;
        isFollowingPlayer = true;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void IsTouchingPlayer()
    {
        if(isTouchingPlayer)
        {
            isTouchingPlayer = false;
            playerMovement.CheckDmgProtection(gameObject);
        }
    }

    public void ReturnVelocity()
    {
        //retVelocity = myRigidbody.velocity;
        Debug.Log("jo");
        playerMovement.BounceFromStunn(retVelocity);
    }

    public int ReturnDamage()
    {
        return damage;
    }

    public int ReturnLives()
    {
        return lives;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        //other.tag == "Goblin" || other.tag == "RegGoblin"
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !isSmashed)
        {
            soundManager.PlayFrStunn();
            StartCoroutine(FriendlyStunn());
            Debug.Log("WTF bro!?");
        }

        if(other.tag == "Goblin" && isSmashed)
        {
            Debug.Log("Kabooom!");
        }

        if(other.tag == "Player" && !other.isTrigger)
        {
            Debug.Log("Crunch!");
            isTouchingPlayer = true;
        }

        if(other.tag == "Player" && other.isTrigger)
        {
            if(playerMovement.ReturnHvAttack())
            {
                bounceSpeed = saveBounceSpeed * hvSmashedMultiplier;
                frStunnTime = frStunnTime * hvFSTMultiplier;
                isBouncingFast = true;
            }
            else
            {
                bounceSpeed = saveBounceSpeed * smashedMultiplier;
                frStunnTime = savefrStunnTime;
                isBouncingFast = false;
            }
            bodySpin.StartBodySpin();
            Instantiate(crystal, transform.position, new Quaternion(0,0,0,0));
            myTrailRenderer.emitting = true;
            lives -= playerMovement.ReturnDamageGiven();
            isSmashed = true;
            isBouncing = false;
            isHit = true;
            playerRotation = player.GetComponent<Transform>().rotation;
            myTransform.rotation = playerRotation;
        }

        if(other.tag == "Ver Wall" && isSmashed)
        {
            // x,y = -x,y
            if(isBouncingFast){
                bounceSpeed = saveBounceSpeed * smashedMultiplier;
            }
            else{
                bounceSpeed = saveBounceSpeed;
            }
            CheckLivesAmount();
            isBouncing = true;
            soundManager.PlayBouncing();
            if(gameObject.activeSelf == false){return;}
            StartCoroutine(BouncingTimer());
            StopBouncing();
            if(!isSmashed){return;}
            yPositionRevert = 1;
            xPositionRevert = -1;
            velocity = new Vector2(myRigidbody.velocity.x * xPositionRevert, myRigidbody.velocity.y * yPositionRevert).normalized;
        }

        if(other.tag == "Hor Wall" && isSmashed)
        {
            // x,y = x,-y
            if(isBouncingFast){
                bounceSpeed = saveBounceSpeed * smashedMultiplier;
            }
            else{
                bounceSpeed = saveBounceSpeed;
            }
            CheckLivesAmount();
            isBouncing = true;
            soundManager.PlayBouncing();
            if(gameObject.activeSelf == false){return;}
            StartCoroutine(BouncingTimer());
            StopBouncing();
            if(!isSmashed){return;}
            yPositionRevert = -1;
            xPositionRevert = 1;
            velocity = new Vector2(myRigidbody.velocity.x * xPositionRevert, myRigidbody.velocity.y * yPositionRevert).normalized;
        }

        if(other.tag == "Bounce Pad" && isSmashed)
        {
            // x,y = -x,-y
            if(isBouncingFast){
                bounceSpeed = saveBounceSpeed * smashedMultiplier;
            }
            else{
                bounceSpeed = saveBounceSpeed;
            }
            CheckLivesAmount();
            isBouncing = true;
            soundManager.PlayBouncing();
            if(gameObject.activeSelf == false){return;}
            StartCoroutine(BouncingTimer());
            StopBouncing();
            if(!isSmashed){return;}
            yPositionRevert = -1;
            xPositionRevert = -1;
            velocity = new Vector2(myRigidbody.velocity.x * xPositionRevert, myRigidbody.velocity.y * yPositionRevert).normalized;
            Debug.Log("Dunk!");
        }
    }

    IEnumerator FriendlyStunn()
    {
        canStunn = true;

        if(canStunn)
        {
            myAnimator.Play(Goblin_stunn);
            canStunn = false;
            canMove = false;
            damage = 0;
            yield return new WaitForSeconds(frStunnTime);
            if(!isSmashed){myAnimator.Play(Goblin_idle);}
            yield return new WaitForSeconds(0.3f);
            canMove = true;
            canStunn = true;
            damage = saveDmg;
        }
    }

    IEnumerator BouncingTimer()
    {
        if(canStartTimer)
        {
            if(gameObject.activeSelf == true)
            {
                canStartTimer = false;
                isHit = false;
                yield return new WaitForSeconds(bouncingTime);
                Debug.Log("koniec");
                canStopBouncing = true;
            }
        }
    }

    void StopBouncing()
    {
        if(canStopBouncing)
        {
            if(!isHit)
            {
                myTrailRenderer.emitting = false;
                isSmashed = false;
                isBouncing = false;
                bodySpin.StopBodySpin();
                StartCoroutine(MakeBodyNotTrigger());
            }
            canStopBouncing = false;
            canStartTimer = true;
            isHit = false;
        }
    }

    void CheckLivesAmount()
    {
        if(lives <= 0)
        {
            myAnimator.Play(Goblin_death);
            bounceSpeed = 0;
            myBodyCollider.enabled = false;
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        bodySpin.StopBodySpin();
        yield return new WaitForSeconds(2f);
        wavesManager.EnemyKilled();
        gameObject.SetActive(false);
    }

    void AnimationSwitch()
    {
        if(!myBodyCollider.enabled){return;}
        if(isSmashed)
        {
            if(isDoingSmashAnimation){return;}

            if(canDoSmashAnimation)
            {
                myAnimator.Play(Goblin_bouncing);
                isDoingSmashAnimation = true;
                Debug.Log("Bouncing");
            }
        }
        else
        {
            if(!isDoingSmashAnimation){return;}

            if(!canDoSmashAnimation)
            {
                myAnimator.Play(Goblin_idle);
                isDoingSmashAnimation = false;
                Debug.Log("Walking");
            }
        }
    }

    void Move()
    {
        if(isSmashed)
        {
            canDoSmashAnimation = true;
            myBodyCollider.isTrigger = true;

            if(isBouncing)
            {
                //bouncing state
                myRigidbody.velocity = velocity *  bounceSpeed;
                retVelocity = velocity *  bounceSpeed;
            }
            else
            {
                //flying state (when hit)
                myRigidbody.velocity = transform.up *  bounceSpeed;
                retVelocity = transform.up * bounceSpeed;
            }
        }
        else
        {
            //walking state
            canDoSmashAnimation = false;
            if(canMove)
            {
                if(isBouncing){return;}

                velocity = (playerPosition - myTransform.position).normalized;
                myRigidbody.velocity = velocity * moveSpeed;
                retVelocity = velocity * moveSpeed;
            }
            else
            {
                myRigidbody.velocity = new Vector2 (0,0);
                return;
            }
        }
    }

    IEnumerator MakeBodyNotTrigger()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        myBodyCollider.isTrigger = false;
    }
}