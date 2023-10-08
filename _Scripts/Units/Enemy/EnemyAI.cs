using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{

//-------------------------------------------------------------------------------------------//
    // PUBLIC GLOBAL VARIABLES
    public Transform leftBoundary;
    public Transform rightBoundary;
    public Transform bottomBoundary;
    public Transform upBoundary;
    public bool isChasing = false;
//-------------------------------------------------------------------------------------------//
    // PRIVATE PRIMITIVES
//-------------------------------------------------------------------------------------------//
    int _currentWaypoint = 0;
    float _speed;
    float _velocity =0f;
    float _distToPlayer=0f;
    float _attackRate; // in seconds (1.5 attack rate means enemy attacks every 1.5s)
    float _chaseSpeed;
    float _patrolSpeed;
    float _nextAttackTime = -1f;
    float _attackRange; // distance from the player under which enemies attack
    float _dir= -1f; // direction of patrolling
    float _nextWayPointDist;
    bool _hasAttacks;
    bool _isStatic;
    [SerializeField] bool _isInBounds = false;
    bool _reachedEndOfPath;
    bool _wasInBounds=false;

//-------------------------------------------------------------------------------------------//
    // PRIVATE NON PRIMITIVES
//-------------------------------------------------------------------------------------------//
    Path _path;
    Animator _animator;
    Enemy _enemy;
    Seeker _seeker;
    Rigidbody2D _rb;
    Vector3 _spawnPosition;
    ScriptableEnemy _enemyScriptable;
    Transform _playerTransform;
    SpriteRenderer _spriteRenderer;

//-------------------------------------------------------------------------------------------//
// START AND UPDATE
//-------------------------------------------------------------------------------------------//
    // Start is called before the first frame update
    void Start()
    {
        _spawnPosition = transform.position;
        _speed = _chaseSpeed;
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();
        _enemy = GetComponent<Enemy>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        _enemyScriptable = _enemy.enemyScriptable;
        _attackRange = _enemyScriptable.avancedStats.weaponAttackRange;
        _attackRate = _enemyScriptable.avancedStats.attackRate;
        _chaseSpeed = _enemyScriptable.avancedStats.chaseSpeed;
        _patrolSpeed = _enemyScriptable.avancedStats.patrolSpeed;
        _nextWayPointDist = _enemyScriptable.avancedStats.nextWayPointDist;
        _hasAttacks = _enemyScriptable.avancedStats.hasAttacks;
        _isStatic = _enemyScriptable.avancedStats.isStatic;

        if (!_isStatic)
        {
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }
    }
    void Update()
    {
        _distToPlayer = Mathf.Abs(transform.position.x - _playerTransform.position.x);
        if (_playerTransform.position.x == transform.position.x)
        {
            _rb.velocity = Vector2.zero;
        }
        if ((_playerTransform.position.x > leftBoundary.position.x) && (_playerTransform.position.x < rightBoundary.position.x)
            && (_playerTransform.position.y > bottomBoundary.position.y) && (_playerTransform.position.y < upBoundary.position.y))
        {
            _isInBounds = true;
        }
        else
        {
            _isInBounds = false;
        }

        _velocity = _rb.velocity.x;

        if (_isStatic && _hasAttacks)
            {
                if(_playerTransform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-1f, 1f,1f);
                }
                else if (_playerTransform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(1f, 1f,1f);
                }

                if (_distToPlayer<_enemyScriptable.avancedStats.attackDistance && Time.time > _nextAttackTime && _isInBounds)
                    {
                        // Debug.Log("Static attack");
                        _nextAttackTime = Time.time + _enemyScriptable.avancedStats.attackRate;
                        _enemy.AttackAnimation();
                    }  
            }
        else if (_hasAttacks)
        {
            if (!_isStatic)
            {
                if (!_isInBounds) 
                {
                    if (_wasInBounds && _enemyScriptable.avancedStats.canFly)
                    {
                        ChaseAndAttack();
                    }
                    else 
                    {
                        Patrol();
                    }
                    
                }
                else 
                {   
                    ChaseAndAttack();
                }
            }
        }
        else 
        {
            if (!_isStatic)
            {
                Patrol();
            }
            
        }
        

        
    }
//-------------------------------------------------------------------------------------------//
// PATH FUNCTIONS
//-------------------------------------------------------------------------------------------//
    void UpdatePath()
    {
        if (_seeker.IsDone())
        {
            // if (_isInBounds)
            // {
            _seeker.StartPath(_rb.position, _playerTransform.position, OnPathComplete);
            // }
            // else if (!_isInBounds && _enemyScriptable.avancedStats.canFly)
            // {
            //      _seeker.StartPath(_rb.position, _spawnPosition, OnPathComplete);
            // }
            
        }
        
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint =0;
        }
        else
        {

        }
    }
//-------------------------------------------------------------------------------------------//
// PATROL FUNCTIONS
//-------------------------------------------------------------------------------------------//
    public void Patrol()
    {
        _speed=_patrolSpeed;
        isChasing = false;
        Vector2 direction = new Vector2(_dir,0f);
        Vector2 force = direction*_speed*Time.deltaTime;
        _rb.AddForce(force);
        if (_enemyScriptable.enemyType == ScriptableEnemy.EnemyType.Spider)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if(!_wasInBounds)
            {
                _spriteRenderer.flipY = true;
            }
            else 
            {
                _spriteRenderer.flipY = false;
            }

            if(force.x >= 0.01)
            {
                transform.localScale = new Vector3(-1f, 1f,1f);
            }
            else if (force.x <= 0.01)
            {
                transform.localScale = new Vector3(1f, 1f,1f);
            }
        }
        else 
        {
            if(force.x >= 0.01)
            {
                transform.localScale = new Vector3(-1f, 1f,1f);
            }
            else if (force.x <= 0.01)
            {
                transform.localScale = new Vector3(1f, 1f,1f);
            }
        }
        
    }
    public void FlipPatrol()
    {
        if (_enemyScriptable.avancedStats.hasAttacks)
        {
            _rb.velocity = Vector2.zero;
        }
        _dir*=-1f;
    }

//-------------------------------------------------------------------------------------------//

    public void Chase()
    {
        if (_enemyScriptable.enemyType == ScriptableEnemy.EnemyType.Spider)
        {
            _spriteRenderer.flipY = false;
            _rb.gravityScale = 2;
        }
        _speed=_chaseSpeed;
        isChasing = true;
        if (_path ==null)
        {
            return;
        }
        
        if (_currentWaypoint >= _path.vectorPath.Count)
        {
            _reachedEndOfPath = true;
            return;
        }
        else
        {
            _reachedEndOfPath = false;
        }
        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
        Vector2 force = direction*_speed*Time.deltaTime;
        _rb.AddForce(force);
        float distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);
        _velocity = _rb.velocity.x;

        if (distance<_nextWayPointDist)
        {
            _currentWaypoint++;
        }

        if(force.x >= 0.01)
        {
            transform.localScale = new Vector3(-1f, 1f,1f);
        }
        else if (force.x <= 0.01)
        {
            transform.localScale = new Vector3(1f, 1f,1f);
        }
    }

    void ChaseAndAttack()
    {
        _wasInBounds = true;
        if (_distToPlayer<_enemyScriptable.avancedStats.attackDistance && Time.time < _nextAttackTime)
        {
            // Debug.Log("Cant attack");
            Chase();
        }  
        else if (_distToPlayer<_enemyScriptable.avancedStats.attackDistance && Time.time > _nextAttackTime)
        {
            _nextAttackTime = Time.time + _enemyScriptable.avancedStats.attackRate;
            _enemy.AttackAnimation();
        }  
        
        else
        {
            Chase();
        }
    }
//-------------------------------------------------------------------------------------------//
    public void Jump()
    {
        _rb.AddForce(new Vector2(0f, _enemyScriptable.avancedStats.jumpForce));
    }

    
}
