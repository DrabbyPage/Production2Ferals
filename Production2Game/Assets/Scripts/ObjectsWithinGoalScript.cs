using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsWithinGoalScript : MonoBehaviour
{
    GameObject objective;
    // Start is called before the first frame update
    void Start()
    {
        objective = GameObject.FindGameObjectWithTag("Objective");
    }

    // Update is called once per frame
    void Update()
    {
        CheckWithinDistance();
    }

    void CheckWithinDistance()
    {
        float minDist = 7;
        Vector3 gameObjPos = gameObject.transform.position;
        Vector3 objPos = objective.transform.position;

        Vector3 diff = objPos - gameObjPos;
        diff.y = 0;

        if(diff.magnitude < minDist)
        {
            Debug.Log("ay you got the ball");
            GameObject gameOverObj = GameObject.Find("GameOverObject");

            if (gameOverObj != null)
            {
                gameOverObj.GetComponent<Text>().text = "Mission Complete";
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Objective")
        {
            Debug.Log("ay you got the ball");
            GameObject gameOverObj = GameObject.Find("GameOverObject");

            if (gameOverObj != null)
            {
                gameOverObj.GetComponent<Text>().text = "Mission Complete";
            }
        }
    }
}
