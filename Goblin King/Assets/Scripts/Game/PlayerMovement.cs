using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float attackCooldown = 0.2f;
    [SerializeField] float dashTime = 0.5f;
    [SerializeField] float dashForce = 5f;
    [SerializeField] float dashCooldown = 0.5f;
    [SerializeField] float dmgProtTime = 0.5f;
    [SerializeField] Animator spriteAnimator;
    [SerializeField] int damageGiven = 2;
    [SerializeField] int hvDamageGiven = 3;
    [SerializeField] PhysicsMaterial2D normalMaterial;
    [SerializeField] float hvChargeAmount = 3f;
    [SerializeField] float hvAttackCooldown = 1f;
    [SerializeField] int neededManaAmount = 8;
    [SerializeField] int manaAmount;
    [SerializeField] Vector2 attackHitboxSize = new Vector2(1,1);
    [SerializeField] Vector2 hvAttackHitboxSize = new Vector2(0,0);
    [SerializeField] float noInputTime = 3f;
    Rigidbody2D myRigidBody;
    BoxCollider2D myAttackHitbox;
    CircleCollider2D myBodyCollider;
    PlayerLives playerLives;
    GoblinEnemy goblinEnemy;
    GameObject hitBy;
    TrailRenderer myTrailRenderer;
    ParticleSystem particles;
    GameManager gameManager;
    WavesManager wavesManager;
    SoundManager soundManager;
    bool isReadyToAttack = true;
    float saveMoveSpeed;
    bool canMove = true;
    bool canDash = true;
    bool isStunned;
    Vector2 lastMove;
    bool isDmgProtected;
    bool isReadyToHvAttack;
    float saveCharge;
    bool canStartChargeTimer;
    bool isUsingMouse = true;
    Quaternion saveRotation;
    bool canStartDmgAnimation = true;
    bool isAttacking;
    bool isHvAttacking;
    Vector3 saveEulerAngles;
    float saveNoInputTime;
    float noLookInputTime;
    bool hasEnoughMana;
    bool canMakeManaParticles = true;
    bool isPressingRTrigger;
    bool pressedAttack;
    bool isGamePaused;
    bool isNewWaveProtected;
    bool isDashing;
    bool canPlayChargeSound = true;
    bool canRotate = true;
    public bool isDead;

    void Start() 
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAttackHitbox = GetComponent<BoxCollider2D>();
        myAttackHitbox.enabled = false;
        playerLives = FindObjectOfType<PlayerLives>();
        myBodyCollider = GetComponent<CircleCollider2D>();
        goblinEnemy = FindObjectOfType<GoblinEnemy>();
        saveMoveSpeed = moveSpeed;
        myRigidBody.sharedMaterial = normalMaterial;
        myTrailRenderer = GetComponent<TrailRenderer>();
        myAttackHitbox.size = attackHitboxSize;
        saveCharge = hvChargeAmount;
        noLookInputTime = noInputTime;
        saveNoInputTime = noInputTime;
        particles = GetComponent<ParticleSystem>();
        gameManager = FindObjectOfType<GameManager>();
        wavesManager = FindObjectOfType<WavesManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Update() 
    {
        ChargeTimer();
        if(isUsingMouse && !isGamePaused)
        {
            if(isHvAttacking || !canRotate){return;}
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - transform.position;
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            transform.eulerAngles = new Vector3 (0, 0, angle);
            if(saveEulerAngles == transform.eulerAngles)
            {
                noLookInputTime -= 1f * Time.unscaledDeltaTime;

                if(noLookInputTime < -99)
                {
                    noLookInputTime = 0;
                }
            }
            else
            {
                noLookInputTime = saveNoInputTime;
                saveEulerAngles = transform.eulerAngles;
                spriteAnimator.SetBool("isSleeping", false);
            }
        }
        else
        {
            if(isHvAttacking || !canRotate || isGamePaused){return;}
            float horizontal = Input.GetAxis ("Horizontal");
            // * Time.deltaTime * 1    niewiem po co to   ^
            float vertical = Input.GetAxis ("Vertical");

            float angle = Mathf.Atan2 (horizontal, vertical) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
            saveRotation = transform.rotation;

            if(angle == 0f){
                noLookInputTime -= 1f * Time.unscaledDeltaTime;

                if(noLookInputTime < -99)
                {
                    noLookInputTime = 0;
                }
            }
            else{
                noLookInputTime = saveNoInputTime;
                spriteAnimator.SetBool("isSleeping", false);
            }

            // * Time.deltaTime *   niewiem po co to  ^
            // if(horizontal == 0f || vertical == 0f)
            // {
            //     transform.rotation = saveRotation;
            // }
            // else
            // {
            //     float angle = Mathf.Atan2 (horizontal, vertical) * Mathf.Rad2Deg;
            //     transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
            //     saveRotation = transform.rotation;
            // }
        }

        if(!Input.anyKey)
        {
            noInputTime -= 1f * Time.unscaledDeltaTime;

            if(noInputTime < -99)
            {
                noInputTime = 0;
            }
        }
        else
        {
            noInputTime = saveNoInputTime;
            spriteAnimator.SetBool("isSleeping", false);
        }

        if(noLookInputTime <= 0 && noInputTime <= 0)
        {
            spriteAnimator.SetBool("isSleeping", true);
        }

        if(hasEnoughMana)
        {
            if(Input.GetKey(KeyCode.Mouse1) || isPressingRTrigger)
            {
                if(!isReadyToAttack && !isDashing)
                {
                    hvChargeAmount = saveCharge;
                    isReadyToHvAttack = false;
                    return;
                }

                if(!isReadyToHvAttack)
                {
                    canStartChargeTimer = true;
                }
            }
        }

    }

    //************************* Managing Taken Dmg ************************//

    IEnumerator DashDmgProtection()
    {
        yield return new WaitForSeconds(dmgProtTime);
        isDmgProtected = false;
    }

    public void CheckDmgProtection(GameObject enemy)
    {
        if(!isDmgProtected)
        {
            hitBy = enemy.gameObject;
            playerLives.ProcessDamageTaken(enemy.gameObject);
        }
    }

    public void ChangeWaveProtection()
    {
        isNewWaveProtected = !isNewWaveProtected;
    }

    public void ManageDmgAnimations()
    {
        if(canStartDmgAnimation)
        {
            pressedAttack = false;
            isReadyToAttack = false;
            Debug.Log("Hit by walking enemy");
            spriteAnimator.SetBool("isDamaged", true);
            spriteAnimator.SetBool("canBeDamaged", true);
            canStartDmgAnimation = false;
        }
    }
    
    public void AllowActions()
    {
        spriteAnimator.SetBool("isDamaged", false);
        if(!isAttacking){
            isReadyToAttack = true;
        }
    }

    public void StopDmgActions()
    {
        canStartDmgAnimation = true;
    }

    public void DeathAnimation()
    {
        spriteAnimator.SetTrigger("death");
        moveSpeed = 1f;
    }

    public void GameOver()
    {
        isDead = true;
        gameManager.ShowGameOverScreen();
        gameObject.SetActive(false);
    }

    //************************* Managing Given Dmg ************************//
    
    IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(attackCooldown);
        isReadyToAttack = true;
    }

    IEnumerator Attack()
    {
        canRotate = false;
        isReadyToAttack = false;
        isAttacking = true;
        spriteAnimator.SetBool("isAttacking", true);
        soundManager.PlayWhoosh();
        yield return new WaitForSecondsRealtime(attackCooldown);
        canRotate = true;
        // Attack again if pressed the button to soon
        if(pressedAttack)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            pressedAttack = false;
            StartCoroutine(Attack());
        }
        else
        {
            isAttacking = false;
            isReadyToAttack = true;
        }
    }

    public void ShowAttackHitbox()
    {
        myAttackHitbox.enabled = true;
    }

    public void HideAttackHitbox()
    {
        myAttackHitbox.enabled = false;
        myAttackHitbox.size = attackHitboxSize;
    }

    void ChargeTimer()
    {
        if(canStartChargeTimer)
        {
            spriteAnimator.SetBool("isHvCharging", true);
            hvChargeAmount -= 1f * Time.deltaTime;
            PlayChargeSound();

            if(hvChargeAmount <= 0)
            {
                isReadyToHvAttack = true;
            }
        }
    }

    IEnumerator HvAttack()
    {
        yield return new WaitUntil(() => !isDashing);
        pressedAttack = false;
        isHvAttacking = true;
        myAttackHitbox.size = hvAttackHitboxSize;
        particles.Stop();
        soundManager.PlayHvWhoosh();
        yield return new WaitForSeconds(hvAttackCooldown);
        isHvAttacking = false;
        isReadyToHvAttack = false;
    }

    public bool ReturnHvAttack()
    {
        return isReadyToHvAttack;
    }
    
    public int ReturnDamageGiven()
    {
        if(isReadyToHvAttack)
        {
            return hvDamageGiven;
        }
        else
        {
            return damageGiven;
        }
    }

    //************************* Managing Movement ************************//

    IEnumerator Dash()
    {
        myAttackHitbox.enabled = false;
        myTrailRenderer.emitting = true;
        isDmgProtected = true;
        canDash = false;
        canMove = false;
        myRigidBody.velocity = lastMove * dashForce;
        isDashing = true;
        isReadyToAttack = false;
        soundManager.PlayDash();
        yield return new WaitForSeconds(dashTime);
        StartCoroutine(DashDmgProtection());
        isDashing = false;
        if(!isAttacking){
            isReadyToAttack = true;
        }
        canMove = true;
        myRigidBody.velocity = lastMove;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void CollectCrystal(GameObject crystal, int addAmount)
    {
        crystal.SetActive(false);
        // Add mana from collected crystal to player
        if(manaAmount < neededManaAmount)
        {
            for (int i = 0; i < addAmount; i++)
            {
                // Don't go over max mana capacity
                if(manaAmount < neededManaAmount)
                {
                    manaAmount += 1;
                }            
            }
        }       
        // Signal that needed mana for heavy attack is collected
        if(manaAmount == neededManaAmount)
        {
            hasEnoughMana = true;
            // Make signal particles
            if(canMakeManaParticles)
            {
                MakeManaParticles();
            }
        }
        else
        {
            // Allow signal particles to show when restarted the mana cycle
            canMakeManaParticles = true;
        }
    }

    void MakeManaParticles()
    {
        canMakeManaParticles = false;
        particles.Play();
    }

    //************************* Sounds ************************//

    void PlayChargeSound(){
        // Don't repeat charge sound
        if(canPlayChargeSound){
            canPlayChargeSound = false;
            soundManager.PlayCharge();
        }
    }

    //************************* On Input Actions ************************//

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Goblin")
        {
            if(!isDmgProtected && !isNewWaveProtected)
            {
                hitBy = other.gameObject;
                playerLives.ProcessDamageTaken(other.gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(myAttackHitbox.enabled == false){return;}
        
        if(other.tag == "Goblin"){
            if(isHvAttacking){
                soundManager.PlayHvHit();
                return;
            }

            if(other.gameObject.GetComponent<GoblinEnemy>().IsLightDmgProof()){
                soundManager.PlayMetalHit();
                return;
            }
            
            soundManager.PlayHit();
        }
        else if(other.tag == "Hor Wall" || other.tag == "Ver Wall" || other.tag == "Bounce Pad"){
            soundManager.PlayHitWall();
        }
    }

    void OnAttack(InputValue value)
    {
        if(isReadyToAttack && !isReadyToHvAttack)
        {
            StartCoroutine(Attack());
        }
        else
        {
            if(isAttacking && !isReadyToHvAttack)
            {
                pressedAttack = true;
            }
        }
    }

    void OnChargeHvAttack(InputValue value)
    {
        isPressingRTrigger = true;
    }

    void OnReleaseHvAttack(InputValue value)
    {
        isPressingRTrigger = false;
        canStartChargeTimer = false;
        hvChargeAmount = saveCharge;
        spriteAnimator.SetBool("isHvCharging", false);
        canPlayChargeSound = true;

        if(isReadyToHvAttack)
        {
            spriteAnimator.SetBool("isHvAttacking", true);
            isReadyToAttack = false;
            hasEnoughMana = false;
            manaAmount -= neededManaAmount;
            StartCoroutine(AttackTimer());
            StartCoroutine(HvAttack());
            Debug.Log("Heavy Attack");
        }
    }

    void OnMove(InputValue value)
    {
        if(moveSpeed == 0f){return;}
        
        if(canMove)
        {
            myRigidBody.velocity = value.Get<Vector2>() * moveSpeed;
            lastMove = myRigidBody.velocity;
        }
    }

    void OnDash(InputValue value)
    {
        if(canDash && !isStunned)
        {
            StartCoroutine(Dash());
        }
    }

    void OnPause(InputValue value)
    {
        if(isGamePaused)
        {
            // Resume game
            isGamePaused = false;
            gameManager.ResumeGame();
        }
        else
        {
            // Pause game
            isGamePaused = true;
            gameManager.PauseGame();
        }
    }

    void OnChangeInput(InputValue value)
    {
        isUsingMouse = !isUsingMouse;
    }

    //************************* Unused ************************//

    // public void PlayerStunn()
    // {
    //     if(isDmgProtected){return;}
    //     StartCoroutine(Stunn());
    // }

    // public void BounceFromStunn(Vector2 otherVelocity)
    // {
    //     myRigidBody.velocity = otherVelocity.normalized * stunnForce;
    // }

    // IEnumerator Stunn()
    // {
    //     isStunned = true;
    //     moveSpeed = 0f;
    //     hitBy.SendMessage("ReturnVelocity");
    //     myRigidBody.sharedMaterial = bouncingMaterial;

    //     yield return new WaitForSeconds(safeStunnTime);

    //     playerLives.isStunned = true;

    //     yield return new WaitForSeconds(stunnTime);

    //     isStunned = false;
    //     moveSpeed = saveMoveSpeed;
    //     myRigidBody.velocity = myRigidBody.velocity / stunnForce * moveSpeed;

    //     yield return new WaitForSeconds(inviTime);

    //     playerLives.isStunned = false;
    //     myRigidBody.sharedMaterial = normalMaterial;
    // }

    // public void SetHitBy(GameObject enemy)
    // {
    //     hitBy = enemy;
    // }

    // [SerializeField] PhysicsMaterial2D bouncingMaterial;

    // [SerializeField] float stunnTime = 2f;
    // [SerializeField] float inviTime = 0.5f;
    // [SerializeField] float stunnForce;
    // [SerializeField] float safeStunnTime = 0.5f;
}