using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] PlayerMovement movement;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] LayerMask player; //checks for a certain layer
    [SerializeField] float speed;

    GuardStates state = GuardStates.Patrol;
    Rigidbody rb;
    NavMeshPath path;

    public Transform[] waypoints;
    int waypointIndex;
    Vector3 destination;

    bool inSight = false;

    [SerializeField]List<GameObject> points;
    private Vector3 tempHold;

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

        if (SightCheck())
        {
            if (dot > 0.7f)
            {
                //Debug.Log("Player Detected!");
                state = GuardStates.Pursue;
            }

            if (dot < -0.4f)
            {
                //Debug.Log("Lost sight of Player");
                inSight = false;
                state = GuardStates.Patrol;
            }
        }

        if (movement.isHidden == true)
        {
            state = GuardStates.Investigate;
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

    bool SightCheck()
    {

        inSight = Physics.CheckCapsule(transform.position, transform.forward, player);

        if (!inSight)
        {
            state = GuardStates.Patrol;
        }

        return inSight;
    }

    IEnumerator WaitToMove() //Coroutine
    {
        destination = waypoints[waypointIndex].position;
        agent.SetDestination(destination);

        yield return new WaitForSeconds(2);
    }

    public void UpdatePatrol()
    {
        if (Vector3.Distance(transform.position, destination) < 1)
        {
            IterateWaypoint();
            UpdateDestination();
        }
        
    }

    public void UpdateInves()
    {
        tempHold = this.GetComponent<NavMeshAgent>().destination;

        AddInvesPoint();

        UpdatePatrol();
    }

    public void UpdatePursue()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
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

    void AddInvesPoint() //Adds gameobject from where Player was last seen
    {
        points.Add(new GameObject());
        points[points.Count - 1].transform.position = tempHold;
    }
}
