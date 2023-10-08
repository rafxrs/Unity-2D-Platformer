using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public bool playerDetected;
    // public Transform areaPos;
    // public float areaWidth;
    // public float areaHeight;
    // public LayerMask whatIsPlayer;
    public Action action;
    public GameObject doActionOn;
    GameObject player;
    Transform playerTransform;
    GameManager gameManager;

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // playerDetected = Physics2D.OverlapBox(areaPos.position, new Vector2(areaWidth, areaHeight),0,whatIsPlayer);

        if (playerDetected)
        {
            // do action
            switch (action)
            {
                case Action.Dialogue:
                    gameManager.ShowEButton();
                    if (Input.GetKeyDown(KeyCode.E))
                        {
                            GetComponent<DialogueTrigger>().TriggerDialogue();
                            gameManager.HideEButton();
                        }
                    break;
                case Action.Door:
                    gameManager.ShowEButton();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("Opening the door");
                        playerTransform.position = doActionOn.transform.position;
                        gameManager.HideEButton();
                    }
                    break;
                case Action.Tutorial:
                    GetComponent<DialogueTrigger>().TriggerDialogue();
                    break;
                case Action.SelectWeapon:
                    gameManager.ShowEButton();
                    // highlight specific weapon
                    doActionOn.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        int num=0;
                        switch (this.name)
                        {
                            case "Spear":
                                num = 0;
                                break;
                            case "Bow":
                                num = 1;
                                break;
                            case "Sword":
                                num = 2;
                                break;
                            default:
                                break;
                        }
                        player.GetComponent<PlayerCombat>().ChooseWeapon(num);
                        // GameObject ws = GameObject.Find("WeaponSelect");
                        // Destroy(ws);
                        Debug.Log("Selecting Weapon");
                        // doActionOn.SetActive(false);
                        gameManager.HideEButton();
                    }
                    break;
                case Action.EndOfLevel:
                    gameManager.ShowEButton();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("Finishing level");
                        gameManager.LevelComplete(); 
                        Destroy(this.gameObject);
                    }
                    break;
                case Action.Chest:
                    gameManager.ShowEButton();
                    // if key, unlock chest;
                    // if no key, show missing key
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        bool unlocked = player.GetComponent<Player>().UnlockChest();
                        if (unlocked)
                        {
                            Animator animator = GetComponent<Animator>();
                            if (animator !=null)
                            {
                                animator.SetTrigger("Open");
                            }
                            GetComponent<RewardSpawner>().Reward(transform.position);
                            gameManager.HideEButton();
                            this.enabled = false;;
                        }
                        else 
                        {
                            player.transform.Find("MissingKey").gameObject.SetActive(true);
                            Invoke("HideMissingKey", 1f);
                        }
                        
                        gameManager.HideEButton();
                    }
                    break;
                case Action.Lever:
                    gameManager.ShowEButton();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("Switching lever");
                        GetComponent<Lever>().SwitchLeverState();
                    }
                    break;
                case Action.Teleport:
                    Debug.Log("Teleporting the player");
                    playerTransform.position = doActionOn.transform.position;
                    gameManager.HideEButton();
                    break;
                default:
                    break;
            }
            
        }
    }
    
    [System.Serializable]
    public enum Action
    {
        Dialogue,
        Door,
        Tutorial,
        SelectWeapon,
        EndOfLevel,
        Chest,
        Lever,
        Teleport,

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerDetected = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            Debug.Log("Player left the collider");
            playerDetected = false;
            gameManager.HideEButton();

            if (action == Action.SelectWeapon)
            {
                doActionOn.SetActive(false);
            }
        }
    }

    void HideMissingKey()
    {
        player.transform.Find("MissingKey").gameObject.SetActive(false);
    }
    // void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireCube(areaPos.position, new Vector3(areaWidth,areaHeight,1));
    // }
}
