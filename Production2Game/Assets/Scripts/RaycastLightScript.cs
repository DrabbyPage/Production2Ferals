using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastLightScript : MonoBehaviour
{
    // this is to make multiple raycasting easy without needing to use
    // a 2d array for raycasts
    struct RaycastHolder
    {
        public RaycastHit[] rayHit;
        public Vector3 rayDirection;
        public float rayDistance;
    }

    // one singular way to test raycasting
    RaycastHit[] testRay;

    // these are objects that hold the sprite masks not actual sprite masks
    GameObject[] masksArray;

    [SerializeField]
    Sprite spriteMaskSprite;

    [SerializeField]
    float angleOfLight;

    [SerializeField]
    float amountOfRaycasts = 16;

    [SerializeField]
    public float maxLightDist = 10;

    [SerializeField]
    float angleOffset = 45;

    [SerializeField]
    float maskYValue = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        masksArray = new GameObject[(int)(amountOfRaycasts)];
        //Debug.Log(spriteMaskSprite);
    }

    // Update is called once per frame
    void Update()
    {
        //ShowMultipleRaycasts();
        MultRaysWithStruct();
    }

    void MultRaysWithStruct()
    {
        RaycastHolder[] raycastHolderArray = new RaycastHolder[(int)(amountOfRaycasts)];

        for(int i = 0; i < amountOfRaycasts; i++)
        {
            float angle = (angleOfLight * (i / amountOfRaycasts));

            RaycastHolder holder = raycastHolderArray[i];

            holder.rayDirection = GetRayAngledDirection(angle);
            holder.rayHit = Physics.RaycastAll(gameObject.transform.position, holder.rayDirection);
            holder.rayDistance = maxLightDist;
            float minDist = maxLightDist;
            for(int m = 0; m < holder.rayHit.Length; m++)
            {
                GameObject objHit = holder.rayHit[m].collider.gameObject;
                if (objHit.tag == "Environment")
                {
                    if (holder.rayHit[m].distance < minDist)
                    {
                        minDist = holder.rayHit[m].distance;
                        holder.rayDistance = holder.rayHit[m].distance;
                    }
                }
                else if(objHit.tag == "Enemy")
                {
                    if (holder.rayHit[m].distance < minDist)
                    {
                        Vector3 enemyDir = holder.rayHit[m].collider.gameObject.transform.position - gameObject.transform.position;
                        RaycastHit[] fleeCheck = Physics.RaycastAll(gameObject.transform.position, enemyDir, maxLightDist);

                        bool somethingInWay = false;
                        for(int n = 0; n < fleeCheck.Length; n++)
                        {
                            if(fleeCheck[n].distance < enemyDir.magnitude && 
                                fleeCheck[n].collider.gameObject.tag == "Environment")
                            {
                                somethingInWay = true;
                                break;
                            }
                        }

                        if(!somethingInWay)
                        {
                            objHit.GetComponent<AIFleeScript>().SetFleeState(true);
                        }
                    }
                }
            }

            holder.rayDirection = holder.rayDirection.normalized * holder.rayDistance;

            Debug.DrawRay(gameObject.transform.position, holder.rayDirection, Color.green);

            CreateLight(holder.rayDirection, holder.rayDistance, angle, i);
        }
    }

    // creates a sprite mask and sets the size according to the size of the ray
    void CreateLight(Vector3 rayDirection, float distance, float angle, int maskID)
    {
        GameObject currentMask = masksArray[maskID];

        if(currentMask == null)
        {
            currentMask = Instantiate(Resources.Load("Prefabs/SpriteMaskObj")) as GameObject;
        }

        currentMask.name = "SpriteMask" + maskID;
        currentMask.transform.position = gameObject.transform.position + ((rayDirection)/2);

        
        float objEulerX = gameObject.transform.eulerAngles.x;
        float maskAnglePt2 = Mathf.Atan2(rayDirection.z, rayDirection.x) * Mathf.Rad2Deg;
        currentMask.transform.rotation = Quaternion.Euler(objEulerX, 0, -maskAnglePt2);

        currentMask.GetComponent<SpriteMask>().sprite = spriteMaskSprite;
        currentMask.GetComponent<Transform>().localScale = new Vector3(distance, maskYValue, 1);

        masksArray[maskID] = currentMask;
    }

    // this duplicates the Raycasts (next function)
    void ShowMultipleRaycasts()
    {
        for(int i = 0; i < amountOfRaycasts; i++)
        {
            float angle = angleOfLight * (i / amountOfRaycasts);

            ShowRaycast(angle);
        }

    }

    // this is used for scene to show how the raycasts will work
    void ShowRaycast(float angle)
    {
        // shows only one ray
        Vector3 rayDirection = new Vector3(0,0,0);
        
        rayDirection = GetRayAngledDirection(angle);
        testRay = Physics.RaycastAll(gameObject.transform.position, rayDirection);
        float dist = maxLightDist;
        for(int i = 0; i < testRay.Length; i++)
        {
            if (testRay[i].collider.gameObject.tag == "Environment")
            {
                if (testRay[i].distance < maxLightDist)
                {
                    //Debug.Log("collision with a non node");
                    dist = testRay[i].distance;
                }
            }
        }

        rayDirection = rayDirection.normalized * dist;
        Debug.DrawRay(gameObject.transform.position, rayDirection, Color.green);

        //CreateLight(rayDirection, dist);
    }

    // this is for the direction compared to the mouse
    Vector3 GetRayDirection()
    {
        Vector3 direction = new Vector3(0,0,0);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 objPos = gameObject.transform.position;
        
        direction = mousePos - objPos;
        direction.y = 0;
        return direction.normalized;
    }

    // pass in a divisor of 360 and get the direction
    Vector3 GetRayAngledDirection(float angle)
    {
        Vector3 direction = Vector3.right;

        float angleToRad = (gameObject.transform.eulerAngles.y + (angle + angleOffset)) * Mathf.Deg2Rad;

        Vector3 objPos = gameObject.transform.position;

        direction = new Vector3(Mathf.Cos(angleToRad), 0, Mathf.Sin(angleToRad));//angleToRad;

        direction.y = 0;

        return direction.normalized;
    }

    // this allows for aspects to be done in scene time
    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        ShowMultipleRaycasts(); // works with scene
        //MultRaysWithStruct(); // doesnt work with scene
    }
    
}
