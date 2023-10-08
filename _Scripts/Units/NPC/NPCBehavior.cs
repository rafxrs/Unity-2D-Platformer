using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (this.gameObject.name == "NPC_Blacksmith")
        {
            animator.SetBool("isSmithing",true);
            InvokeRepeating("FlipSmith",5f,Random.Range(5f,10f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FlipSmith()
    {
        if (animator.GetBool("isSmithing"))
        {
            animator.SetBool("isSmithing",false);
        }
        else 
        {
            animator.SetBool("isSmithing",true);
        }
        
    }
}
