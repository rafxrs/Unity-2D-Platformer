using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{

    //-------------------------------------------------------------------------------------------//
    public ScriptableEnemy enemyScriptable ;
    public bool isDead = false;
    //-------------------------------------------------------------------------------------------//
    [SerializeField] Transform _attackPoint;
    Player _player;
    EnemyAI _enemyAI;
    Animator _animator;
    Rigidbody2D _rb;
    RewardSpawner _rewardSpawner; 
    SpriteRenderer _spriteRenderer;
    FloatingHealthBar healthBar;
    int _currentHealth;
    bool _isAttacking = false;
    //-------------------------------------------------------------------------------------------//
    void Start()
    {
        _currentHealth = enemyScriptable.baseStats.maxHealth;
        _player = GameObject.Find("Player").GetComponent<Player>(); NullCheck.CheckNull(_player);
        _enemyAI = GetComponent<EnemyAI>(); NullCheck.CheckNull(_enemyAI);
        _rb = GetComponent<Rigidbody2D>(); NullCheck.CheckNull(_rb);
        _animator = GetComponent<Animator>(); NullCheck.CheckNull(_animator);
        _rewardSpawner = GetComponent<RewardSpawner>(); NullCheck.CheckNull(_rewardSpawner);
        _spriteRenderer = GetComponent<SpriteRenderer>(); NullCheck.CheckNull(_spriteRenderer);
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        if (healthBar == null )
        {
            Debug.LogError("health bar is null");
        }
        healthBar.SetMax(enemyScriptable.baseStats.maxHealth);
        // Debug.Log("success");
        enemyScriptable.impactPrefabs[0] = Resources.Load<GameObject>("Prefabs/FX/Impacts/ImpactFX1");
        enemyScriptable.impactPrefabs[1] = Resources.Load<GameObject>("Prefabs/FX/Impacts/ImpactFX2");
        SetEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {   
            if (!_isAttacking && !enemyScriptable.avancedStats.isStatic)
            {
                _animator.SetFloat("Speed", Mathf.Abs(_rb.velocity.x));
                if (enemyScriptable.avancedStats.canFly)
                {
                    if (_rb.velocity.x > 0.01f)
                    {
                        _animator.SetFloat("Speed", Mathf.Abs(_rb.velocity.x));
                    }
                    else if (_rb.velocity.x < 0.02f && _rb.velocity.y > 0.01f)
                    {
                        _animator.SetFloat("Speed", Mathf.Abs(_rb.velocity.y));
                    }
                    
                }

            }
            
        }
        
    }

    //-------------------------------------------------------------------------------------------//
    void SetEnemy()
    {
        _animator.runtimeAnimatorController = enemyScriptable.controller;
    }
    //-------------------------------------------------------------------------------------------//

    public void TakeDamage(int damage) 
    {
        int _rand = Random.Range(0,2);
        Instantiate(enemyScriptable.impactPrefabs[_rand], transform.position, Quaternion.identity);
        _spriteRenderer.color = Color.red;
        Invoke("ResetSprite",0.1f);
        // activate health bar
        // transform.Find("WorldHealthBar").gameObject.SetActive(true);
        _currentHealth -= damage;
        healthBar.Set(_currentHealth);
        // play hurt animation
        if (_currentHealth <=0)
        {
            Die();
        }
    }

    void ResetSprite()
    {
        _spriteRenderer.color = Color.white;
    }
    //-------------------------------------------------------------------------------------------//
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag=="Player" && enemyScriptable.avancedStats.isBouncable && !other.GetComponent<CharacterController2D>().m_Grounded)
        {
            TakeDamage(enemyScriptable.avancedStats.bounceDamage);
            _player.Bounce();
        }
        else if (other.tag=="PlayerHitbox" && enemyScriptable.baseStats.collisionDamage)
        {
            _player.TakeDamage(enemyScriptable.avancedStats.attackDamage);
            // player.Knockback(other);
        }
        else if (other.tag=="Boundary")
        {
            if (_enemyAI.isChasing)
            {
               return;

            }
            else 
            {
                _enemyAI.FlipPatrol();
            }   
        }
        
    }

//-------------------------------------------------------------------------------------------//
    void IsAttacking()
    {
        _isAttacking = true;
        Invoke("ResetIsAttacking", 0.75f);
    }
    void ResetIsAttacking()
    {
        _isAttacking = false;
    }
    public void Attack()
    {
        // _rb.velocity = Vector2.zero;
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(_attackPoint.position, enemyScriptable.avancedStats.weaponAttackRange, enemyScriptable.playerLayer);
        foreach(Collider2D hit in hitPlayer)
        {
            Debug.Log("We hit "+ hit.name);
            if (hit.tag == "PlayerHitbox")
            {
                _player.TakeDamage(enemyScriptable.avancedStats.attackDamage);
            }
            

        }
    }
    public void AttackAnimation()
    {
        IsAttacking();
        int attackNumber = Random.Range(0,2);
        string attackTrigger = "Attack"+attackNumber.ToString();
        _animator.SetTrigger(attackTrigger);
    }
//-------------------------------------------------------------------------------------------//
    void Die()
    {
        if (enemyScriptable.avancedStats.canFly)
        {
            _rb.gravityScale = 3;
        }
        Debug.Log("enemy died");
        isDead = true;

        // die animation
        _rb.velocity = Vector2.zero;
        _animator.SetTrigger("Death");     
        _rewardSpawner.Reward(transform.position);

        // disable enemy
        GetComponent<BoxCollider2D>().enabled = false;
        // GetComponent<PolygonCollider2D>().enabled = false;
        GetComponent<EnemyAI>().enabled = false;

        // destroy enemy after 1 second
        Destroy(this.gameObject, 1f);
        
    }
//-------------------------------------------------------------------------------------------//
    void OnDrawGizmosSelected()
    {
        if (_attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_attackPoint.position, enemyScriptable.avancedStats.weaponAttackRange);
    }
}
