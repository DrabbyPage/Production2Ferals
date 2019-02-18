using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLerpScript : MonoBehaviour
{
    public Transform target;

    [Range(0, 1)]
    public float followSpeed;

    Vector3 offset;

    void Start()
    {
        offset = target.position - transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position - offset, followSpeed);
    }
}
