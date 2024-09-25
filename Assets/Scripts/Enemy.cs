using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    bool wall = false;

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

        if (WallCheck())
        {
            if (dot > 0.7f)
            {
                Debug.Log("Player Detected!");
                state = GuardStates.Pursue;
            }

            if (dot < -0.4f)
            {
                Debug.Log("Lost sight of Player");
                state = GuardStates.Investigate;

                StartCoroutine(secDelay());
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

        wall = Physics.CheckCapsule(transform.position, transform.forward, environment);

        if (wall)
        {
            UpdatePatrol();
        }


        /*aycastHit ray = Physics.Raycast(transform.position, target.transform.position - transform.position);



           if (ray.collider != null)
           {
               wall = ray.collider.CompareTag("Player");

               if(wall)
               {
                   Debug.DrawRay(transform.position, (target.transform.position - transform.position), Color.magenta); //If it can see player
               }

               else
               {
                   Debug.DrawRay(transform.position, (target.transform.position - transform.position), Color.red); //if it cannot see player
               }
           }*/

        /*  else if ((Physics.Raycast(transform.position, (target.transform.position - transform.position), Player)))
            {
                wall = false;
            } */

        return wall;
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

        agent.SetDestination(tempHold); //Moves AI towards last known Location
    }

    IEnumerator secDelay() //A timer to switch back to patrol state
    {
        yield return new WaitForSeconds(7);
        state = GuardStates.Patrol;
    }

    public void UpdatePursue()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        //agent.SetDestination(target.transform.position);
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
