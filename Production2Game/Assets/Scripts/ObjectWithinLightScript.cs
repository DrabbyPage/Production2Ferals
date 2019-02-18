using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWithinLightScript : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            bool clearedRaycast = CheckForObjects(col.transform.position);
            Debug.Log("enemy in the light");
            if(clearedRaycast)
            {
                col.gameObject.GetComponent<AIFleeScript>().SetFleeState(true);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<AIFleeScript>().SetFleeState(false);
        }
    }

    bool CheckForObjects(Vector3 objPos)
    {
        Vector3 originPos = gameObject.transform.position;
        Vector3 direction = objPos - gameObject.transform.position;
        float dist = direction.magnitude;
        RaycastHit[] listOfCols = Physics.RaycastAll(originPos, direction, dist);

        for(int i = 0; i < listOfCols.Length; i++)
        {
            if(listOfCols[i].collider.gameObject.tag == "Environment")
            {
                return false;
            }
        }

        return true;
    }
}
