using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is the main class for any attacking related code for the player. It deals with input and sets the correct weapons on the animator
/// </summary>
public class PlayerCombat : MonoBehaviour
{

// -------------------------------------------------------------------------------------------------------------------------- //
// PUBLIC VARIABLES
// -------------------------------------------------------------------------------------------------------------------------- //

    public ScriptableWeapon[] weapons;
    public ScriptableWeapon weapon;
    public RuntimeAnimatorController defaultController;
    public Transform[] attackPoints;
    public LayerMask enemyLayer,interactableLayer;

// -------------------------------------------------------------------------------------------------------------------------- //
    Player _player;
    Animator _animator;
    [SerializeField] string _weaponName;
    [SerializeField] float _attackRange;
    [SerializeField] float _attackRate;
    [SerializeField] float _nextAttackTime = 0f;
    [SerializeField] int _attackDamage;
    float _comboTime=0f;
    float _throwSpearManaCost = 50f;
    int _combo =0;
    bool _isCombo = false;

// -------------------------------------------------------------------------------------------------------------------------- //
    void Start() 
    {
        _animator = GetComponent<Animator>(); NullCheck.CheckNull(_animator);
        _player = GetComponent<Player>(); 
        _animator.runtimeAnimatorController = defaultController;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (weapon != null && GameManager.playerControl && Time.time>_nextAttackTime && !_player.roll)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                AttackAnimation();
                _player.IsAttacking();
                _nextAttackTime = Time.time + _attackRate;
            }
            else if ((_player.currentMana>=_throwSpearManaCost) && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) && _weaponName=="Spear")
            {
                if (!_player.isGrounded())
                {
                    _animator.SetBool("isJumping", false);
                }
                ThrowSpearAnimation();
                _nextAttackTime = Time.time + _attackRate;
                _player.IsAttacking();
            }
            else if ((_player.currentMana<_throwSpearManaCost) && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) && _weaponName=="Spear")
            {
                transform.Find("MissingMana").gameObject.SetActive(true);
                Invoke("ResetMissingMana", 1f);
                
            }
            if (_weaponName=="Bow")
            {
                if (Input.GetKeyDown(KeyCode.Space)|| Input.GetMouseButtonDown(0))
                {
                    _animator.SetTrigger("Attack1");
                    // Invoke("ResetBowForce",1f);
                    // Debug.Log("Pressed at "+bowButtonDown);
                    // bowButtonDown = Time.time;
                    _player.IsAttacking();
                    _nextAttackTime = Time.time + _attackRate;

                }
            }
        }
    }
// -------------------------------------------------------------------------------------------------------------------------- //
    void AttackAnimation()
    {
        int _randomAttack = Random.Range(1,3);
        string _attackNumber= "Attack"+_randomAttack;
        if (_weaponName=="Sword")
        {
            if (_comboTime!=0f)
            {
                Debug.Log("Check");
                if (((Time.time - _comboTime < 1f))&& _combo==1)
                {
                    Debug.Log("Building Combo");
                    _combo = 2;
                    _comboTime = Time.time;
                    _isCombo = false;
                    _attackNumber= "Attack"+_combo;
                    _animator.SetTrigger(_attackNumber);
                    _animator.SetBool("isJumping", false);
                }
                else if (((Time.time - _comboTime < 1f))&& _combo==2)
                {
                    _combo = 3;
                    _attackNumber= "Attack"+_combo;
                    _animator.SetBool(_attackNumber, true);
                    _animator.SetBool("isJumping", false);
                    Invoke("FinishAttack3", 0.5f);
                    _combo = 0;
                    _comboTime = 0f;
                }
                else 
                {
                    _isCombo = false;
                    _comboTime = 0f;
                    _animator.SetTrigger(_attackNumber);
                    _animator.SetBool("isJumping", false);
                    _combo = 1;
                }
            }
            else
            {
                _isCombo = false;
                Debug.Log("Combo time is 0");
                _animator.SetTrigger(_attackNumber);
                _animator.SetBool("isJumping", false);
                _comboTime = Time.time;
                _combo = 1;
            }
            

        }
        else {
            _animator.SetTrigger(_attackNumber);
            _animator.SetBool("isJumping", false);
        }
        
    }
// -------------------------------------------------------------------------------------------------------------------------- //
    void Attack()
    {
        // Debug.Log("Attacking");
        Attack(_weaponName, transform, _isCombo);
    }
    public void Attack(string name, Transform player, bool combo)
    {
        switch (name)
        {
            case "Spear":
                Debug.Log("Spear Attack");
                Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoints[0].position, new Vector3(weapon.attackRange, 1, 1), 0f, enemyLayer);
                Collider2D[] hitObjects = Physics2D.OverlapBoxAll(attackPoints[0].position, new Vector3(weapon.attackRange, 1, 1), 0f, interactableLayer);

                foreach(Collider2D enemy in hitEnemies)
                {
                    Debug.Log("Our weapon hit "+ enemy.name);
                    enemy.GetComponent<Enemy>().TakeDamage(weapon.attackDamage);
                }
                foreach(Collider2D obj in hitObjects)
                {
                    Debug.Log("Our weapon hit "+ obj.name);
                    switch(obj.tag)
                    {
                        case "Crate":
                            obj.GetComponent<Crate>().TakeDamage(weapon.attackDamage);
                            break;
                        case "Lever":
                            obj.GetComponent<Lever>().SwitchLeverState();
                            break;
                        default:
                            break;
                    }

                }
                break;
            case "Bow":
                break;
            case "Sword":
                // Debug.Log("Sword Attack");
                hitEnemies = Physics2D.OverlapCircleAll(attackPoints[1].position, weapon.attackRange, enemyLayer);
                hitObjects = Physics2D.OverlapCircleAll(attackPoints[1].position, weapon.attackRange, interactableLayer);
                
                foreach(Collider2D enemy in hitEnemies)
                {
                    Debug.Log("Our weapon hit "+ enemy.name);
                    if (!combo)
                    {
                        enemy.GetComponent<Enemy>().TakeDamage(weapon.attackDamage);
                    }
                    else 
                    {
                        Debug.Log("COMBO!");
                        enemy.GetComponent<Enemy>().TakeDamage((weapon.attackDamage)*3);
                    }
                }
                foreach(Collider2D obj in hitObjects)
                {
                    Debug.Log("Our weapon hit "+ obj.name);
                    switch(obj.tag)
                    {
                        case "Crate":
                            obj.GetComponent<Crate>().TakeDamage(weapon.attackDamage);
                            break;
                        case "Lever":
                            obj.GetComponent<Lever>().SwitchLeverState();
                            break;
                        default:
                            break;
                    }

                }
                break;
            default:
                break;       
        }
    }

// -------------------------------------------------------------------------------------------------------------------------- //
    void ThrowSpearAnimation()
    {
        _animator.SetBool("Attack3", true);
        Invoke("FinishAttack3", 0.5f);
    }
    void ThrowSpear()
    {
        _player.currentMana -= _throwSpearManaCost;
        _player.manaBar.SetMana(_player.currentMana);
        Rigidbody2D spearInstance = Instantiate(weapon.weaponPrefab, transform.position, transform.rotation);
        Vector2 throwDirection = transform.right;

        // Calculate the required initial velocity for the desired arc
        float horizontalDistance = weapon.launchForce * weapon.launchDuration;
        float verticalDistance = weapon.launchArcHeight;
        Vector2 initialVelocity = CalculateInitialVelocity(throwDirection, horizontalDistance, verticalDistance);

        // Apply the initial velocity to the spear
        spearInstance.velocity = initialVelocity;
    }
// -------------------------------------------------------------------------------------------------------------------------- //
    void ShootArrow()
    {
        // Debug.Log("ShootingArrow");
        Rigidbody2D arrowInstance = Instantiate(weapon.weaponPrefab, transform.position, Quaternion.identity);
        Vector2 throwDirection = transform.right;

        // Calculate the required initial velocity for the desired arc
        float horizontalDistance = weapon.launchForce * weapon.launchDuration;
        float verticalDistance = weapon.launchArcHeight;
        Vector2 initialVelocity = CalculateInitialVelocity(throwDirection, horizontalDistance, verticalDistance);

        // Apply the initial velocity to the spear
        arrowInstance.velocity = initialVelocity;
    }
// -------------------------------------------------------------------------------------------------------------------------- //
    private Vector2 CalculateInitialVelocity(Vector2 direction, float horizontalDistance, float verticalDistance)
    {
        // Calculate the initial velocity components for the desired arc
        float horizontalVelocity = horizontalDistance / weapon.launchDuration;
        float verticalVelocity = (verticalDistance - (0.5f * Physics2D.gravity.y * Mathf.Pow(weapon.launchDuration, 2f))) / weapon.launchDuration;

        // Combine the components with the direction vector
        Vector2 initialVelocity = direction * horizontalVelocity;
        initialVelocity.y = verticalVelocity;

        return initialVelocity;
    }   
// -------------------------------------------------------------------------------------------------------------------------- //
    void SwordComboAttack()
    {
        Debug.Log("combo");
        _isCombo = true;
        Attack();
        Invoke("FinishCombo", 1f);
    }
    void FinishCombo()
    {
        _isCombo = false;
    }
    private void FinishAttack3()
    {
        _animator.SetBool("Attack3", false);
    }
// -------------------------------------------------------------------------------------------------------------------------- //
    public void SetWeapon(ScriptableWeapon w)
    {
        _weaponName = w.name;
        weapon = w;
        _attackDamage = w.attackDamage;
        _attackRate = w.attackRate;
        _attackRange = w.attackRange;
    }
    public void ChooseWeapon(int number) //0 is spear, 1 is bow, 2 is sword
    {
        weapon = weapons[number];
        ChangeController(weapon);
        SetWeapon(weapon);
    }
    public void ChangeController(ScriptableWeapon weapon)
    {
        _animator.runtimeAnimatorController = weapon.controller;
    }
// -------------------------------------------------------------------------------------------------------------------------- //

    void ResetMissingMana()
    {
        transform.Find("MissingMana").gameObject.SetActive(false);
    }

    
}
