using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerScript : MonoBehaviour
{
    GameObject playerObj;
	// Use this for initialization
	void Start ()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePosition();
	}

    void UpdatePosition()
    {
        float playX = playerObj.transform.position.x;
        //float playY = playerObj.transform.position.y;
        float playZ = playerObj.transform.position.z;

        gameObject.transform.position = new Vector3(playX, gameObject.transform.position.y, playZ);
    }
}
