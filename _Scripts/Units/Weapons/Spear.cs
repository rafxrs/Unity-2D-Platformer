using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements the spear behaviour
/// </summary>
public class Spear : MonoBehaviour
{
    public Side side;
    [SerializeField] private float rotationSpeed = 20f;
    Rigidbody2D rb;
    public ScriptableWeapon w;
    bool isInAir = true;
    Player player;

    [System.Serializable]
    public enum Side
    {
        Player,
        Enemy,
    }

    void Start()
    {
        Destroy(this.gameObject, 5f);
        rb = GetComponent<Rigidbody2D>();
        player= GameObject.Find("Player").GetComponent<Player>();
    }
    void FixedUpdate()
    {
        // float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x)* Mathf.Rad2Deg;
        // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (isInAir)
        {
            ChangeSpriteDirection();
        }
        
        
    }
    void ChangeSpriteDirection()
    {
        if (rb.velocity != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, rb.velocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed*Time.deltaTime);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground" || other.tag == "Obstacle")
        {
            StopMotion();
            CircleCollider2D arrow = GetComponent<CircleCollider2D>();
            arrow.enabled = false;
            isInAir =false;
            Invoke("StopMotion",0.05f);

        }
        if (side == Side.Player)
        {
            if (other.tag == "Enemy")
            {
                
                Enemy enemy= other.GetComponent<Enemy>();
                enemy.TakeDamage(w.attackDamage);
                // if (enemy.isDead)
                // {
                Destroy(this.gameObject);
                // }
                // else 
                // {
                //     Invoke("StopMotion",0.05f);
                // }
            }
            else if (other.tag == "Crate")
            {
                
                Destroy(this.gameObject);
                other.GetComponent<Crate>().TakeDamage(w.attackDamage);
            }
            else if (other.tag == "Lever")
            {
                other.GetComponent<Lever>().SwitchLeverState();
            }
        }
        else if (side == Side.Enemy)
        {
            if (other.tag == "PlayerHitbox")
            {
                
                
                player.TakeDamage(w.attackDamage);
                Destroy(this.gameObject);
            }
        }
    }
    void StopMotion()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale =0;
    }
}
