// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : MonoBehaviour
{
    // [SerializeField] float moveSpeed = 5f;
    // [SerializeField] float bounceSpeed = 16f;
    // [SerializeField] float smashedMultiplier = 1.2f;
    // [SerializeField] float hvSmashedMultiplier = 1.5f;
    // [SerializeField] float detectionDistance = 6f;
    // [SerializeField] int damage = 10;
    // [SerializeField] float jumpAttackMultiplier = 2f;
    // [SerializeField] float readyTime = 0.2f;
    // [SerializeField] float attackTime = 0.5f;
    // [SerializeField] float restTime = 0.2f;
    // [SerializeField] float attackCooldown = 2f;
    // [SerializeField] int lives = 15;
    // [SerializeField] float bouncingTime = 1f;
    // [SerializeField] float frStunnTime = 2f;
    // [SerializeField] float hvFSTMultiplier = 2f;

    [SerializeField] ManaCrystal crystal;
    [SerializeField] EnemyType enemyType;

    //*************************** Sounds ****************************//
    AudioClip frStunn1;
    AudioClip frStunn2;
    AudioClip bouncing1;
    AudioClip bouncing2;
    AudioClip bouncing3;
    //***********************************************//
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
    AudioSource audioSource;
    //***********************************************//
    float moveSpeed;
    float bounceSpeed;
    float smashedMultiplier;
    float hvSmashedMultiplier;
    float detectionDistance;
    int damage;
    float jumpAttackMultiplier;
    float readyTime;
    float attackTime;
    float restTime;
    float attackCooldown;
    int lives;
    float bouncingTime;
    float frStunnTime;
    float hvFSTMultiplier;
    //***********************************************//
    string Goblin_idle;
    string Goblin_bouncing;
    string Goblin_attack;
    string Goblin_charge;
    string Goblin_death;
    string Goblin_stunn;
    //***********************************************//
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

    void Start()
    {
        Goblin_idle = enemyType.Goblin_idle;
        Goblin_bouncing = enemyType.Goblin_bouncing;
        Goblin_attack = enemyType.Goblin_attack;
        Goblin_charge = enemyType.Goblin_charge;
        Goblin_death = enemyType.Goblin_death;
        Goblin_stunn = enemyType.Goblin_stunn;
        //***********************************************************//
        moveSpeed = enemyType.moveSpeed;
        bounceSpeed = enemyType.bounceSpeed;
        smashedMultiplier = enemyType.smashedMultiplier;
        hvSmashedMultiplier = enemyType.hvSmashedMultiplier;
        detectionDistance = enemyType.detectionDistance;
        damage = enemyType.damage;
        jumpAttackMultiplier = enemyType.jumpAttackMultiplier;
        readyTime = enemyType.readyTime;
        attackTime = enemyType.attackTime;
        restTime = enemyType.restTime;
        attackCooldown = enemyType.attackCooldown;
        lives = enemyType.lives;
        bouncingTime = enemyType.bouncingTime;
        frStunnTime = enemyType.frStunnTime;
        hvFSTMultiplier = enemyType.hvFSTMultiplier;
        //***********************************************************//
        frStunn1 = enemyType.frStunn1;
        frStunn2 = enemyType.frStunn2;
        bouncing1 = enemyType.bouncing1;
        bouncing2 = enemyType.bouncing2;
        bouncing3 = enemyType.bouncing3;
        //***********************************************************//
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
        audioSource = GetComponent<AudioSource>();

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
            PlayFrStunn();
            StartCoroutine(FriendlyStunn());
        }

        if(other.tag == "Player" && !other.isTrigger)
        {
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
            PlayBouncing();
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
            PlayBouncing();
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
            PlayBouncing();
            if(gameObject.activeSelf == false){return;}
            StartCoroutine(BouncingTimer());
            StopBouncing();
            if(!isSmashed){return;}
            yPositionRevert = -1;
            xPositionRevert = -1;
            velocity = new Vector2(myRigidbody.velocity.x * xPositionRevert, myRigidbody.velocity.y * yPositionRevert).normalized;
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
            }
        }
        else
        {
            if(!isDoingSmashAnimation){return;}

            if(!canDoSmashAnimation)
            {
                myAnimator.Play(Goblin_idle);
                isDoingSmashAnimation = false;
            }
        }
    }

    void Move()
    {
        Debug.Log(moveSpeed);
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

    //****************************** Sounds ******************************//

    void PlayBouncing(){
        // Set random clip out of available
        
        int r = Random.Range(0,3);
        if(r == 0){
            audioSource.clip = bouncing1;
        }
        if(r == 1){
            audioSource.clip = bouncing2;
        }
        if(r == 2){
            audioSource.clip = bouncing3;
        }
        // Play clip
        audioSource.pitch = 1f;
        audioSource.Play();
    }

    public void PlayFrStunn(){
        // Set random clip out of available
        
        if(Random.Range(0,2) == 0){
            audioSource.clip = frStunn1;
        }
        else{
            audioSource.clip = frStunn2;
        }
        // Play clip
        audioSource.pitch = 1f;
        audioSource.Play();
    }
}