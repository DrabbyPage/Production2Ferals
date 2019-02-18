using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementScript : MonoBehaviour
{
    public enum MoveState
    {
        wander,
        chase,
        flee
    }

    [SerializeField]
    public MoveState aiMoveState = MoveState.wander;

    [SerializeField]
    float aiMoveSpeed = 12.0f;

    [SerializeField]
    float aiLinDrag = 10.0f;

    [SerializeField]
    public bool canAIMove = true;

    bool stateChanged = false;

    [SerializeField]
    Vector3 moveToPoint;

    [SerializeField]
    Vector3 endPoint;

    float minApproachDist = 0.6f;

    [SerializeField]
    float animationSwitchTime = 0.2f;
    float timer;

    [SerializeField]
    Sprite leftHandSprite;

    [SerializeField]
    Sprite rightHandSprite;

    Sprite currentSprite;

	// Use this for initialization
	void Start ()
    {
        moveToPoint = gameObject.transform.position;
        GetComponent<Rigidbody>().drag = aiLinDrag;
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckMovement();
	}

    void CheckMovement()
    {
        if(canAIMove)
        {
            CheckPathValidity();
            MoveTowardsGoal();
            LookTowardsGoal();

            UpdateSpriteAnim();
        }
    }

    void CheckPathValidity()
    {
        List<GameObject> localPath = GetComponent<PathfindingScript>().objectPath;

        // here we check to see if there was a change in state
        // if there was a change we want the ai to directly respond and go in the
        // direction it needs to go
        if(stateChanged)
        {
            Debug.Log("State Has Changed");
            // here we check which action to take
            stateChanged = false;

            FindEndPoint();
            GetComponent<PathfindingScript>().GeneratePath(gameObject.transform.position, endPoint);

            if(localPath.Count > 0)
            {
                moveToPoint = localPath[0].transform.position;
            }
        }

        // here we check to see if there is a path... if there is a path then we will
        // follow the path... if there isnt a path then we will check the ai's state
        // and choose the proper action / point to go
        if (localPath.Count > 0)
        {
            // make the point the most recent node
            moveToPoint = localPath[0].transform.position;
        }
        else
        {
            // here we check which action to take
            FindEndPoint();
            GetComponent<PathfindingScript>().GeneratePath(gameObject.transform.position, endPoint);
            if(localPath.Count > 0)
            {
                moveToPoint = localPath[0].transform.position;
            }
        }
    }

    void FindEndPoint()
    {
        if(aiMoveState == MoveState.wander)
        {
            GetComponent<AIWanderingScript>().RandomizePoint();
            endPoint = GetComponent<AIWanderingScript>().GetWanderPoint();
        }
        else if (aiMoveState == MoveState.chase)
        {
            endPoint = GetComponent<AIChaseScript>().GetChasePoint();
            //GetComponent<PathfindingScript>().GeneratePath(gameObject.transform.position, endPoint);
        }
        else if (aiMoveState == MoveState.flee)
        {
            //Debug.Log("fleeing");
            GetComponent<AIFleeScript>().MakeFleePoint();
            endPoint = GetComponent<AIFleeScript>().GetFleePoint();
        }
    }

    void MoveTowardsGoal()
    {
        Vector3 diff = moveToPoint - gameObject.transform.position;

        if (diff.magnitude > minApproachDist)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(diff.normalized * aiMoveSpeed);
        }
        else
        {
            GetComponent<PathfindingScript>().KnockOutPathNode();
        }
    }

    public void SetMovePoint(Vector3 newPoint)
    {
        moveToPoint = newPoint;
    }

    public void ChangeAIMoveState(MoveState newMoveState)
    {
        if(newMoveState != aiMoveState)
        {
            aiMoveState = newMoveState;
            stateChanged = true;
        }
    }

    void LookTowardsGoal()
    {
        Vector3 diff = moveToPoint - gameObject.transform.position;
        
        float objEulerX = gameObject.transform.eulerAngles.x;
        float maskAnglePt2 = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;

        gameObject.transform.rotation = Quaternion.Euler(objEulerX, 0, (-maskAnglePt2) - 90);

    }

    void UpdateSpriteAnim()
    {
        timer += Time.deltaTime;

        if(timer >= animationSwitchTime)
        {
            timer = 0;

            if(currentSprite == leftHandSprite)
            {
                currentSprite = rightHandSprite;
            }
            else
            {
                currentSprite = leftHandSprite;
            }
        }

        GetComponent<SpriteRenderer>().sprite = currentSprite;
    }
}
