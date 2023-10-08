using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements arrow behaviour
/// </summary>
public class Arrow : MonoBehaviour
{
    public Side side;
    public ScriptableWeapon w;
    Rigidbody2D rb;
    bool isInAir = true;
    Player player;

    [System.Serializable]
    public enum Side
    {
        Player,
        Enemy,
    }


    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 5f);
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isInAir)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x)* Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground" || other.tag == "Obstacle")
        {
            StopMotion();
            PolygonCollider2D arrow = GetComponent<PolygonCollider2D>();
            arrow.enabled = false;
            isInAir =false;
            Invoke("StopMotion",0.05f);

        }
        if (side == Side.Player)
        {
            if (other.tag == "Enemy")
            {
                Debug.Log("arrow hit "+other.name);
                Enemy enemy= other.GetComponent<Enemy>();
                enemy.TakeDamage(w.attackDamage);
                Destroy(this.gameObject);
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
                // Player player= other.GetComponent<Player>();
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
