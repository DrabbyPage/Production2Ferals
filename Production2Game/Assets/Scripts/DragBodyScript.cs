using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBodyScript : MonoBehaviour
{
    [SerializeField]
    KeyCode dragBoyButton = KeyCode.LeftShift;

    GameObject[] objectives;

    [SerializeField]
    GameObject dragObject;

    //public int amountOfObjectives;

    [SerializeField]
    float minDistance = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        objectives = GameObject.FindGameObjectsWithTag("Objective");
        //amountOfObjectives = objectives.Length;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForButtonPress();
    }

    void CheckForButtonPress()
    {
        if(Input.GetKey(dragBoyButton))
        {
            CheckWithinDistance();
        }
        else
        {
            if(dragObject != null)
            {
                dragObject.transform.parent = null;
            }
        }
    }

    void CheckWithinDistance()
    {
        for(int i = 0; i < objectives.Length; i++)
        {
            Vector3 diff = gameObject.transform.position - objectives[i].transform.position;

            if(diff.magnitude < minDistance)
            {
                dragObject = objectives[i];
                dragObject.transform.parent = gameObject.transform;
                break;
            }
        }
    }
}
