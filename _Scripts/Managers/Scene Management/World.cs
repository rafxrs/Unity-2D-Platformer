using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : SceneSwitch
{
    public Transform player;
    public float posX;
    public float posY;
    public string previous;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        if (player==null)
        {
            player = GameObject.Find("Player").GetComponent<Transform>();

        }
        if (previousScene == previous)
        {
            player.position = new Vector2(posX,posY);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
