using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameObject containers;
    GameManager gameManager;
    public Text coinText;
    public Text gameOverCoinText;
    public Text levelCompleteCoinText;
    public int coins;
    // Start is called before the first frame update
    void Start()
    {        
        containers = GameObject.Find("Containers");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // if (gameManager==null)
        // {
        //     Debug.Log("game manager is null");
        // }
        // coins = PlayerPrefs.GetInt("coins", 0);
        coinText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCoin(int coin)
    {
        coins = coin;
        coinText.text = coin.ToString();
        levelCompleteCoinText.text = coin.ToString();
        gameOverCoinText.text = coin.ToString();
    }
    public void LevelComplete()
    {
        gameManager.LevelComplete();
    }
    public void GameOverSequence()
    {
        gameManager.GameOver();
        // StartCoroutine(GameOverRoutine());
        Destroy(containers);

    }
}
