using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public Action action;
    public bool leverState=false;
    public GameObject[] doActionOn;

    float timeBetweenSwitch = 1f;
    float nextSwitch = -1f;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OntriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Weapon")
        {
            // ScriptableWeapon w = other.GetComponent<ScriptableWeapon>();
            SwitchLeverState();
        }
    }

    public void SwitchLeverState()
    {
        if (Time.time > nextSwitch)
        {
            leverState = !leverState;
            animator.SetBool("State", leverState);
            DoAction();
            nextSwitch = Time.time + timeBetweenSwitch;
        }
        else 
        {
            return;
        }
        
        
    }

    [System.Serializable]
    public enum Action
    {
        Door,
        Platform,

    }

    void DoAction()
    {
        switch (action)
        {
            case Action.Door:
                foreach (GameObject gobj in doActionOn)
                {
                    gobj.SetActive(false);
                }
                break;
            case Action.Platform:
                foreach (GameObject gobj in doActionOn)
                {
                    gobj.SetActive(leverState);
                }
                break;
            default:
                break;
        }
        
    }
    

}
