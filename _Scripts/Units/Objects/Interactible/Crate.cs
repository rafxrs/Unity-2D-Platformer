using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public int maxHealth=50;
    int currentHealth;
    [SerializeField] private GameObject[] impactPrefabs;
    Animator animator;
    RewardSpawner rewardSpawner;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth=maxHealth;
        animator = GetComponent<Animator>();
        rewardSpawner = GetComponent<RewardSpawner>();
        impactPrefabs[0] = Resources.Load<GameObject>("Prefabs/FX/Impacts/ImpactFX1");
        impactPrefabs[1] = Resources.Load<GameObject>("Prefabs/FX/Impacts/ImpactFX2");
    }


    void SpawnRewards(int amount)
    {
        for (int i=0;i<amount;i++)
        {
            rewardSpawner.Reward(transform.position);
        }
        
    }

    void OntriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Weapon")
        {
            ScriptableWeapon w = other.GetComponent<ScriptableWeapon>();
            TakeDamage(w.attackDamage);
        }
    }
    public void TakeDamage(int attackDamage)
    {
        currentHealth-=attackDamage;
        int rand = Random.Range(0,2);
        Instantiate(impactPrefabs[rand], transform.position, Quaternion.identity);
        if (currentHealth <= 0)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log("Destroy");
            animator.SetTrigger("Destroy");
            SpawnRewards(3);
            Destroy(this.gameObject,1.5f);
            
        }
    }

    


}
