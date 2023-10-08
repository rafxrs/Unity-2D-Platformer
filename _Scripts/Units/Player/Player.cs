using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main class for the player
/// This class deals with health, movement, mana, coins and player related UI
/// </summary>

//-------------------------------------------------------------------------------------------//
public class Player : MonoBehaviour
{
    //-------------------------------------------------------------------------------------------//
    // GLOBAL VARIABLES
    //-------------------------------------------------------------------------------------------//
    // PUBLIC
    public LayerMask _platformLayerMask;
    public CharacterController2D controller;
    public HealthBar healthBar;
    public ManaBar manaBar;
    //-------------------------------------------------------------------------------------------//
    public float currentMana;
    public int coins=0;
    public bool isClimbing = false;
    public bool roll = false;
    public bool tookDamage=false;
    //-------------------------------------------------------------------------------------------//
    // PRIVATE VARIABLES
    //-------------------------------------------------------------------------------------------//
    ScriptablePlayer _playerScriptable;
    Animator animator; 
    Rigidbody2D rb;
    UIManager uiManager;
    SpriteRenderer spriteRenderer;
    Color hurtColor;
    CapsuleCollider2D hitbox;
    GameObject[] impactPrefabs;
    //-------------------------------------------------------------------------------------------//
    // PRIMITIVES
    float horizontalInput = 0f;
    float nextClimb=-1f;
    int currentHealth;
    bool jump = false;
    bool isDead = false;
    bool isAttacking = false;
    bool hasKey = false;
    //-------------------------------------------------------------------------------------------//


    //-------------------------------------------------------------------------------------------//
    // START
    void Start()
    {
        _playerScriptable = Resources.Load<ScriptablePlayer>("ScriptableObjects/Player");
        if (_playerScriptable==null)
        {
            Debug.LogError("PlayerScriptable is null");
        }

        currentHealth = _playerScriptable.baseStats.maxHealth;
        currentMana = _playerScriptable.avancedStats.maxMana;
        hurtColor = Color.red;

        manaBar.SetMaxMana(currentMana);
        manaBar.SetMana(currentMana);
        healthBar.SetMax(_playerScriptable.baseStats.maxHealth);
        healthBar.Set(_playerScriptable.baseStats.maxHealth);

        uiManager = GameObject.Find("Main Canvas").GetComponent<UIManager>();
        if (_playerScriptable==null)
        {
            Debug.LogError("UI Manager is null");
        }
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitbox = GameObject.Find("PlayerHitbox").GetComponent<CapsuleCollider2D>();
        if (hitbox == null)
        {
            Debug.LogError("Hitbox is null");
        }

        InvokeRepeating("RegainMana",0f,0.01f);
    }

    //-------------------------------------------------------------------------------------------//
    // UPDATE
    void Update()
    {
        if (GameManager.playerControl && !isDead)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal") * _playerScriptable.avancedStats.speed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            if (isGrounded())
            {
                animator.SetBool("isClimbing",false);
            }

            if (isClimbing && (Time.time>nextClimb) && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) ) {
                
                Climb();
            }
            else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !roll && !isClimbing)
            {
                
                Jump();
            }
            if (Input.GetButtonDown("Roll") && !roll && (currentMana>=_playerScriptable.avancedStats.rollManaCost) && isGrounded() && !isClimbing && Mathf.Abs(horizontalInput) > 0.01)  
            {
                Debug.Log("Rolling");
                Roll();
                
            } 
            else if (Input.GetButtonDown("Roll") && (currentMana<_playerScriptable.avancedStats.rollManaCost) && isGrounded() && !isClimbing && Mathf.Abs(horizontalInput) > 0.01)
            {
                transform.Find("MissingMana").gameObject.SetActive(true);
                Invoke("ResetMissingMana",0.75f);
            }
        }
        else 
        {
            horizontalInput =0f;
        }
        
        
    }

    void FixedUpdate()
    {
        if (GameManager.playerControl)
        {
            controller.Move(horizontalInput* Time.fixedDeltaTime, roll, jump);
            jump = false;
        }
        

    }

    //-------------------------------------------------------------------------------------------//

    /*
    MOVEMENT FUNCTIONS
    */
    public void OnLanding()
    {
        animator.SetBool("isJumping", false);
    }
    public void OnRoll()
    {
        animator.SetBool("isRolling", false);
    }

    public bool isGrounded()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        bool grounded = collider.IsTouchingLayers(_platformLayerMask);
        return grounded;
    }
    public void Jump()
    {
        jump = true;
        if (!isAttacking)
        {
            animator.SetBool("isJumping", true);
        }
        
    }
    public void Roll()
    {
        roll = true;
        animator.SetBool("isRolling", true);
        currentMana -= _playerScriptable.avancedStats.rollManaCost;
        manaBar.SetMana(currentMana);
        StartCoroutine(RollDownRoutine());
    }
    public void Climb()
    {
        animator.SetBool("isClimbing", true);
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(rb.velocity.x, verticalInput * _playerScriptable.avancedStats.climbSpeed);
        rb.velocity = climbVelocity;
        
        
    }
    public void Bounce()
    {
        Debug.Log("Boucing off");
        hitbox.enabled = false;
        Invoke("EnableHitbox", 0.2f);
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(0f, _playerScriptable.avancedStats.bounceForce));
        // animator.SetBool("isJumping", true);
    }

    void EnableHitbox()
    {
        hitbox.enabled = true;
    }

    public void StopMotion()
    {
        animator.SetFloat("Speed",0f);
        rb.velocity = Vector2.zero;
    }
    //-------------------------------------------------------------------------------------------//

    /*
    COLLIDER FUNCTIONS
    */
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DeathFloor"))
        {
            
            TakeDamage(_playerScriptable.baseStats.maxHealth);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag=="Climbable")
        {
            Debug.Log("We can climb");
            isClimbing = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag=="Climbable")
        {
            nextClimb = Time.time +0.5f;
            isClimbing = false;
            animator.SetBool("isClimbing", false);
        }
    }

    //-------------------------------------------------------------------------------------------//
    // ROUTINES
    //-------------------------------------------------------------------------------------------//

    IEnumerator RollDownRoutine() 
    {
        yield return new WaitForSeconds(0.5f);
        roll = false;
    }

    //-------------------------------------------------------------------------------------------//
    //HEALTH & COMBAT FUNCTIONS
    //-------------------------------------------------------------------------------------------//

    public void TakeDamage(int damage)
    {
        tookDamage = true;
        int randomImpact = Random.Range(0,2);
        Instantiate(_playerScriptable.impactPrefabs[randomImpact], transform.position, Quaternion.identity);
        spriteRenderer.color = hurtColor;
        Invoke("ResetColor",0.25f);
        currentHealth -= damage;
        healthBar.Set(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void ResetColor()
    {
        spriteRenderer.color = new Color(255,255,255,255);
    }
    public void Knockback(Collider2D other)
    {
        rb.velocity = Vector2.zero;
        GameManager.playerControl = false;
        // animator.SetTrigger("Stun");
        Invoke("EnablePlayerControl", .5f);
        Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;

        // Apply the knockback force to the player
        
        rb.AddForce(knockbackDirection * 100f, ForceMode2D.Impulse);
    }
    public void IsAttacking()
    {
        isAttacking = true;
        Invoke("ResetIsAttacking", 0.8f);
    }
    void ResetIsAttacking()
    {
        isAttacking= false;
    }
    void RegainMana()
    {
        if (currentMana >= _playerScriptable.avancedStats.maxMana)
        {
            currentMana =100f;
            manaBar.SetMana(currentMana);
        }
        else
        {
            currentMana+=0.1f;
            manaBar.SetMana(currentMana);
        }
    }
    //-------------------------------------------------------------------------------------------//
    void Die()
    {
        Invoke("GameOverSequence",1.5f);
        isDead = true;
        animator.SetBool("isJumping", false);
        animator.SetBool("isClimbing", false);
        animator.SetTrigger("Death");
        GetComponent<PlayerCombat>().enabled = false;
        GetComponent<CharacterController2D>().enabled = false;
        // GetComponent<BoxCollider2D>().enabled = false;
        // GetComponent<CircleCollider2D>().enabled = false;
        Destroy(this.gameObject, 4f);
    }
    //-------------------------------------------------------------------------------------------//
    // UI AND SCORE FUNCTIONS
    //-------------------------------------------------------------------------------------------//
    void GameOverSequence()
    {
        uiManager.GameOverSequence();
    }
    public void LevelComplete()
    {
        uiManager.LevelComplete();
    }
    public void ResetMissingMana()
    {
        transform.Find("MissingMana").gameObject.SetActive(false);
    }
    //-------------------------------------------------------------------------------------------//
    // INTERACTION FUNTIONS
    //
    public void AddCoin(int coin)
    {
        coins += coin;
        uiManager.UpdateCoin(coins);
    }
    public void CollectKey()
    {
        Debug.Log("Collected a key");
        hasKey =true;
    }
    public bool UnlockChest()
    {
        if (hasKey)
        {
            Debug.Log("Unlocking chest");
            // unlock
            hasKey = false;
            return true;
            
        }
        else 
        {
            Debug.Log("No key");
            // show missing key on top of player
            return false;
        }
    }

    public void HealthPotion()
    {
        currentHealth += 50;
        if (currentHealth>= _playerScriptable.baseStats.maxHealth)
        {
            currentHealth = _playerScriptable.baseStats.maxHealth;
        }
        healthBar.Set(currentHealth);

    }

}
