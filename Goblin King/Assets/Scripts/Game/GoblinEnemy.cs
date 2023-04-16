// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : MonoBehaviour
{
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
    int enemyTypeIndex;
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
    bool isMetal;
    //***********************************************//
    string goblin_idle;
    string goblin_bouncing;
    string goblin_attack;
    string goblin_charge;
    string goblin_death;
    string goblin_stunn;
    string spawn_portal;
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
    int saveDmg;
    bool isBouncingFast;
    bool isLightDmgProof;
    Vector2 futureVelocity;
    bool isAttacking;
    Vector3 direction;
    int stunnsAmount;
    bool canRotate;

    void Start()
    {
        enemyTypeIndex = enemyType.enemyTypeIndex;
        //***********************************************************//
        goblin_idle = enemyType.goblin_idle;
        goblin_bouncing = enemyType.goblin_bouncing;
        goblin_attack = enemyType.goblin_attack;
        goblin_charge = enemyType.goblin_charge;
        goblin_death = enemyType.goblin_death;
        goblin_stunn = enemyType.goblin_stunn;
        spawn_portal = enemyType.spawn_portal;
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
        isMetal = enemyType.isMetal;
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
        
        if(isMetal){
            isLightDmgProof = true;
        }
        else
        {
            StartCoroutine(LatencyLoop());
        }
    }

    void Update()
    {
        if(isFollowingPlayer && !playerMovement.isDead){
            playerPosition = player.GetComponent<Transform>().position;
        }
        IsTouchingPlayer();
        CheckDistance();
        AnimationSwitch();
        if(!isSmashed && canRotate)
        {
            if(isAttacking){
                direction = (velocity).normalized;
            }
            else{
                direction = (playerPosition - transform.position).normalized;
            }
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            myRigidbody.rotation = angle;
        }
    }

    void FixedUpdate() 
    {
        Move();
    }

    IEnumerator LatencyLoop(){
        // Makes enemies chase late position of the player

        // Updates chase direction every given seconds
        yield return new WaitForSeconds(0.15f);

        if(!isSmashed){
            // velocity is based on previous player position
            velocity = futureVelocity;
            // saves current player position for next loop
            futureVelocity = (playerPosition - myTransform.position).normalized;
        }

        StartCoroutine(LatencyLoop());
    }

    IEnumerator DontMoveOnSpawn(){
        bodySpin.SetRotationOnSpawn(90f);
        canRotate = false;
        myAnimator.Play(spawn_portal);
        canMove = false;
        myBodyCollider.enabled = false;
        yield return new WaitForSeconds(wavesManager.spawningTime);
        bodySpin.SetNormalRotation();
        canRotate = true;
        myAnimator.Play(goblin_idle);
        canMove = true;
        myBodyCollider.enabled = true;
    }

    public int ReturnEnemyTypeIndex(){
        return enemyTypeIndex;
    }

    void CheckDistance()
    {
        if(isSmashed){return;}

        if(canAttack && canMove)
        {
            if(Vector3.Distance(playerPosition,myTransform.position) <= detectionDistance)
            {
                // if(isMetal){
                //     playerPosition = (playerPosition - myTransform.position) * 10;
                //     isFollowingPlayer = false;
                // }
                canAttack = false;
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        // Is charging attack
        myAnimator.Play(goblin_charge);
        moveSpeed = 0;
        yield return new WaitForSeconds(readyTime);
        // Is attacking
        isAttacking = true;
        if(!isSmashed && canMove){
            myAnimator.Play(goblin_attack);
        }
        myTrailRenderer.emitting = true;
        moveSpeed = saveMoveSpeed * jumpAttackMultiplier;
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        // Is resting
        yield return new WaitForSeconds(restTime);
        // Returns to chase state
        if(!isSmashed && canMove){
            myAnimator.Play(goblin_idle);
        }
        if(!isSmashed){
            myTrailRenderer.emitting = false;
        }
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

    public bool IsLightDmgProof(){
        return isLightDmgProof;
    }

    public float ReturnFrStunnTime(){
        return frStunnTime;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        //other.tag == "Goblin" || other.tag == "RegGoblin"
        if(other.tag == "Goblin" && !isSmashed)
        {
            stunnsAmount++;
            PlayFrStunn();
            StartCoroutine(FriendlyStunn(other.gameObject.GetComponent<GoblinEnemy>()));
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
                if(isLightDmgProof){
                    if(isMetal){
                        Instantiate(crystal, transform.position, new Quaternion(0,0,0,0));
                    }
                    Instantiate(crystal, transform.position, new Quaternion(0,0,0,0));
                    return;
                }
                bounceSpeed = saveBounceSpeed * smashedMultiplier;
                frStunnTime = savefrStunnTime;
                isBouncingFast = false;
            }
            bodySpin.StartBodySpin();
            if(isMetal){
                Instantiate(crystal, transform.position, new Quaternion(0,0,0,0));
            }
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

        if(other.tag == "Death Wall")
        {
            wavesManager.EnemyKilled();
            gameObject.SetActive(false);
        }
    }

    IEnumerator FriendlyStunn(GoblinEnemy friend)
    {
        myAnimator.Play(goblin_stunn);
        canMove = false;
        damage = 0;
        if(isMetal){
            yield return new WaitForSeconds(friend.ReturnFrStunnTime() * 0.5f);
        }
        else{
            yield return new WaitForSeconds(friend.ReturnFrStunnTime());
        }
        stunnsAmount--;
        if(stunnsAmount == 0){
            if(!isSmashed){myAnimator.Play(goblin_idle);}
            yield return new WaitForSeconds(0.3f);
            canMove = true;
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
                bodySpin.SetNormalRotation();
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
            myAnimator.Play(goblin_death);
            bounceSpeed = 0;
            myBodyCollider.enabled = false;
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        bodySpin.SetNormalRotation();
        yield return new WaitForSeconds(1f);
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
                myAnimator.Play(goblin_bouncing);
                isDoingSmashAnimation = true;
            }
        }
        else
        {
            if(!isDoingSmashAnimation){return;}

            if(!canDoSmashAnimation)
            {
                myAnimator.Play(goblin_idle);
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

                if(isMetal && !isAttacking){
                    velocity = (playerPosition - myTransform.position).normalized;
                }

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
        audioSource.volume = 0.3f;
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
        audioSource.volume = 0.3f;
        audioSource.Play();
    }
}