using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAttackGizmos : MonoBehaviour
{
    void Start()
    {

    }
    void OnDrawGizmosSelected()
    {
        
        if (this.tag == "Player")
        {
            if (this.name == "SwordAttackPoint"){
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position,0.75f);
            }
            else if (this.name == "SpearAttackPoint")
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(transform.position, new Vector3(2,1,1));
            }
            
        }
        else if (this.tag == "Enemy" && this.GetComponentInParent<Enemy>().enemyScriptable.avancedStats.hasAttacks)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, GetComponentInParent<Enemy>().enemyScriptable.avancedStats.weaponAttackRange);
        }
        
    }
}
