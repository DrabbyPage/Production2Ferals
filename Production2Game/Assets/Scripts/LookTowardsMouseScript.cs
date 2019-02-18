using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTowardsMouseScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetRotationThroughMouse();
    }

    void SetRotationThroughMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 objPos = gameObject.transform.position;

        float angle = Mathf.Atan2(mousePos.z - objPos.z,  mousePos.x - objPos.x) * Mathf.Rad2Deg;

        Quaternion newRot = Quaternion.Euler(-90,0, angle);             //Raycast lighting
        //Quaternion newRot = Quaternion.Euler(-90,0, -angle+90);       //Mask lighting

        transform.rotation = newRot;
    }
}
