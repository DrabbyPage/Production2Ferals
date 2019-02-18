using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFleeScript : MonoBehaviour
{
    [SerializeField]
    Vector3 fleePoint;

    [SerializeField]
    float fleeDistance;

    GameObject playerObj;

    void Start()
    {
        playerObj = GameObject.Find("RaycastLight");

        if(playerObj == null)
        {
            fleeDistance = 10;
            
        }
        else
        {
            fleeDistance = playerObj.GetComponent<RaycastLightScript>().maxLightDist+3;
        }
    }

    void Update()
    {
        CheckOutOfFlee();
    }

    void CheckOutOfFlee()
    {
        GameObject playerObj = GetComponent<AIChaseScript>().playerObj;
        Vector3 playerPos = playerObj.transform.position;
        Vector3 objPos = gameObject.transform.position;
        Vector3 direction = playerPos - objPos;

        if(direction.magnitude > fleeDistance)
        {
            SetFleeState(false);
        }
    }

    public void MakeFleePoint()
    {
        GameObject playerObj = GetComponent<AIChaseScript>().playerObj;
        Vector3 playerPos = playerObj.transform.position;
        Vector3 objPos = gameObject.transform.position;
        Vector3 direction = playerPos - objPos;

        fleePoint = playerPos + (direction.normalized * -fleeDistance);
    }

    public Vector3 GetFleePoint()
    {
        return fleePoint;
    }

    public void SetFleeState(bool fleeStateBool)
    {
        if (fleeStateBool)
        {
            //Debug.Log("setting the moves state to flee (in flee script)");
            
            GetComponent<AIMovementScript>().ChangeAIMoveState(AIMovementScript.MoveState.flee);
        }
        else
        {
            //Debug.Log("setting the moves state to wander (in flee script)");

            GetComponent<AIMovementScript>().ChangeAIMoveState(AIMovementScript.MoveState.wander);
        }
    }
}
