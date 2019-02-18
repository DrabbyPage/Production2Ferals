using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float forwardMoveSpeed = 10.0f;

    [SerializeField]
    float backMoveSpeed = 10.0f;

    [SerializeField]
    float sniffSpeed = 0.0f;

    [SerializeField]
    float turnSpeed = 3.0f;

    [SerializeField]
    float linDrag = 10.0f;

    KeyCode moveForward;
    KeyCode moveBackward;
    KeyCode turnLeft;
    KeyCode turnRight;

    KeyCode sniffButton;

	// Use this for initialization
	void Start ()
    {
        moveForward = KeyCode.W;
        moveBackward = KeyCode.S;
        turnLeft = KeyCode.A;
        turnRight = KeyCode.D;

        sniffButton = KeyCode.Space;

        GetComponent<Rigidbody>().drag = linDrag;
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckPlayerInput();
	}

    void CheckPlayerInput()
    {
        if (Input.GetKey(moveForward))
        {
            GetComponent<Rigidbody>().AddForce(transform.up * forwardMoveSpeed);
        }
        if (Input.GetKey(moveForward) && Input.GetKey(sniffButton))
        {
            GetComponent<Rigidbody>().AddForce(transform.up * sniffSpeed);
        }
        if (Input.GetKey(moveBackward) && !Input.GetKey(sniffButton))
        {
            GetComponent<Rigidbody>().AddForce(transform.up * -backMoveSpeed);
        }
        if (Input.GetKey(turnLeft))
        {
            Vector3 objEulerAngle = gameObject.transform.eulerAngles;
            gameObject.transform.Rotate(new Vector3(0, 0, -turnSpeed));
        }
        if (Input.GetKey(turnRight))
        {
            Vector3 objEulerAngle = gameObject.transform.eulerAngles;
            gameObject.transform.Rotate(new Vector3(0, 0, turnSpeed));
        }
        
    }

    void SetRotationThroughMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 objPos = gameObject.transform.position;

        float angle = Mathf.Atan2(mousePos.z - objPos.z, mousePos.x - objPos.x) * Mathf.Rad2Deg;

        gameObject.transform.localEulerAngles = new Vector3(90, 0, angle);
    }
}
