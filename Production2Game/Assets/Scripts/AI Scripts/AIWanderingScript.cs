using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWanderingScript : MonoBehaviour
{

    Vector3 wanderPoint;

    [SerializeField]
    float randomXRange;

    [SerializeField]
    float randomZRange;

    [SerializeField]
    float minXRange;

    [SerializeField]
    float minZRange;

    // Use this for initialization
    void Start()
    {
        wanderPoint = gameObject.transform.position;
    }

    public void RandomizePoint()
    {
        float randomX = Random.Range(minXRange, randomXRange);
        float randomZ = Random.Range(minZRange, randomZRange);

        float objX = gameObject.transform.position.x;
        float objZ = gameObject.transform.position.z;

        float addOrSub = Random.Range(1, 4);

        if(addOrSub == 1)
        {
            wanderPoint = new Vector3(objX + randomX, 0, objZ + randomZ);
        }
        else if( addOrSub == 2)
        {
            wanderPoint = new Vector3(objX - randomX, 0, objZ - randomZ);
        }
        else if(addOrSub == 3)
        {
            wanderPoint = new Vector3(objX + randomX, 0, objZ - randomZ);
        }
        else if(addOrSub == 4)
        {
            wanderPoint = new Vector3(objX - randomX, 0, objZ + randomZ);
        }
    }

    public Vector3 GetWanderPoint()
    {
        return wanderPoint;
    }
}
