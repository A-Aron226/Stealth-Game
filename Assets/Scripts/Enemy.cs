using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GuardStates
{
    Patrol,
    Investigate,
    Pursue
}


public class Enemy : MonoBehaviour
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] LayerMask environment; //checks for a certain layer
    [SerializeField] LayerMask Player;
    [SerializeField] float speed;

    GuardStates state = GuardStates.Patrol;
    Rigidbody rb;
    NavMeshPath path;

    public Transform[] waypoints;
    int waypointIndex;
    Vector3 destination;

    bool wall;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();

        agent.CalculatePath(target.position, path);

        UpdateDestination();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized; //target position - player position; normalize to shorten length of distance (to only get direction)
        Vector3 forwardDirection = transform.forward;

        float dot = Vector3.Dot(directionToTarget, forwardDirection); //dot product is a decimal of the range from 1 (front) to 0 (sides) to -1 (back)

        if (!WallCheck())
        {
            if (dot > 0.5f)
            {
                Debug.Log("Player Detected!");
            }

            if (dot < -0.5f)
            {
                Debug.Log("Lost sight of Player");
            }

        }

        switch (state)
        {
            case GuardStates.Patrol:
                UpdatePatrol();
                break;
            case GuardStates.Investigate:
                UpdateInves();
                break;
            case GuardStates.Pursue:
                UpdatePursue();
                break;
        }
    }

    void FixedUpdate()
    {
      //rb.velocity = transform.forward * speed;
    }

    bool WallCheck()
    {

        

        if (Physics.Raycast(transform.position, (target.transform.position - transform.position), environment))
        {
            Debug.Log("Wall");
            wall = true;
        }

        else if ((Physics.Raycast(transform.position, (target.transform.position - transform.position), Player)))
        {
            wall = false;
        }

        return wall;
    }

    IEnumerator WaitToMove() //Coroutine
    {
        destination = waypoints[waypointIndex].position;
        agent.SetDestination(destination);

        yield return new WaitForSeconds(2.0f);
    }

    void UpdatePatrol()
    {
        if (Vector3.Distance(transform.position, destination) < 1)
        {
            IterateWaypoint();
            UpdateDestination();
        }
        
    }

    void UpdateInves()
    {

    }

    void UpdatePursue()
    {

    }



    void UpdateDestination()
    {
        
        StartCoroutine(WaitToMove());
    }

    void  IterateWaypoint()
    {
        waypointIndex++; //increments to next waypoint

        if (waypointIndex == waypoints.Length )
        {
            waypointIndex = 0; //goes back to first waypoint once it has reached the maximum length of waypoints
        }
    }
}
