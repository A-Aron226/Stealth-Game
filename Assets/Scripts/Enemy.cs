using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized; //target position - player position; normalize to shorten length of distance (to only get direction)
        Vector3 forwardDirection = transform.forward;

        float dot = Vector3.Dot(directionToTarget, forwardDirection); //dot product is a decimal of the range from 1 (front) to 0 (sides) to -1 (back)

        if (dot > 0.5f)
        {
            Debug.Log("Player Detected!");
        }

        if (dot < -0.5f)
        {
            Debug.Log("Lost sight of Player");
        }

        //Debug.Log(dot);
    }
}
