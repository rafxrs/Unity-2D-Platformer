using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Start is called before the first frame update
    public Type type;
    Player player;
    [SerializeField] bool isReward;
    public GameObject destroyFX;
    [SerializeField] float pickupTime = 0.0f;
    float awakeTime;

    [System.Serializable]
    public enum Type
    {
        Coin,
        Key,
        HealthPotion,
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Awake()
    {
        awakeTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "CoinCollider"))
        {
            switch (type)
            {
                case Type.Coin:
                    if (!isReward)
                    {
                        Instantiate(destroyFX, transform.position, Quaternion.identity);
                        Destroy(this.gameObject);
                        player.AddCoin(1);
                    }
                    else if (isReward && Time.time > awakeTime+pickupTime )
                    {
                        Instantiate(destroyFX, transform.position, Quaternion.identity);
                        Destroy(this.gameObject);
                        player.AddCoin(1);
                    }
                    break;
                case Type.Key:
                    if (!isReward)
                    {
                        Instantiate(destroyFX, transform.position, Quaternion.identity);
                        Destroy(this.gameObject);
                        player.CollectKey();
                    }
                    else if (isReward && Time.time > awakeTime+pickupTime )
                    {
                        Instantiate(destroyFX, transform.position, Quaternion.identity);
                        Destroy(this.gameObject);
                        player.CollectKey();

                    }
                    break;
                case Type.HealthPotion:
                    if (!isReward)
                    {
                        Destroy(this.gameObject);
                        player.HealthPotion();
                    }
                    else if (isReward && Time.time > awakeTime+pickupTime )
                    {
                        Destroy(this.gameObject);
                        player.HealthPotion();

                    }
                    break;
                default:
                    break;
            }
            
        }

    }
}