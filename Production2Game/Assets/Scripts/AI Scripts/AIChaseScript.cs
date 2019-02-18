using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChaseScript : MonoBehaviour
{
    public GameObject playerObj;

    [SerializeField]
    float lockOnDistance = 5.0f;

	// Use this for initialization
	void Start ()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");	
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckDistanceToPlayer();	
	}

    void CheckDistanceToPlayer()
    {
        Vector3 diff = playerObj.transform.position - gameObject.transform.position;
        //Debug.Log(diff.magnitude);
        if (GetComponent<AIMovementScript>().aiMoveState != AIMovementScript.MoveState.flee)
        {
            if (diff.magnitude < lockOnDistance)
            {
                //Debug.Log("setting the moves state to chase (in chase script)");

                GetComponent<AIMovementScript>().ChangeAIMoveState(AIMovementScript.MoveState.chase);

            }
            else
            {
                //Debug.Log("setting the moves state to wander (in chase script)");
                GetComponent<AIMovementScript>().ChangeAIMoveState(AIMovementScript.MoveState.wander);
            }
        }
    }

    public Vector3 GetChasePoint()
    {
        return playerObj.transform.position;
    }

}
