using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttacksPlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player")
        {
            GameObject gameOverObj = GameObject.Find("GameOverObject");

            if(gameOverObj != null)
            {
                gameOverObj.GetComponent<Text>().text = "Game Over";
            }
        }
    }
}
