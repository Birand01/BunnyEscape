using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Gardener : MonoBehaviour
{
    //Event Manager
    public static event Action OnGardenerSpottedBunny;

    [Header("Gardener varibles and references")]
    [SerializeField] Transform pathHolder;
    Transform Player;
    [SerializeField] float speed = 5f;
    [SerializeField] float waitTime = .3f;
    [SerializeField] float turnSpeed = 90;
    float timeToSpotBunny=0.3f;
    float bunnyVisibleTimer;
    bool disableGardener;
    [Space]
    [Header("SpotLight")]
    [SerializeField] Light spotLight;
    [SerializeField] float viewDistance;
    float viewAngle;
    [SerializeField] LayerMask viewMask;
    Color originalSpotLightColor;
    public Animator anim;
    void Start()
    {
        OnGardenerSpottedBunny += DisableGardener;
        //SpotLight
        viewAngle = spotLight.spotAngle;
        originalSpotLightColor = spotLight.color;
        // Player reference(transform)
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        //Animator reference
        anim = GetComponent<Animator>();
        Vector3[] wayPoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i <wayPoints.Length; i++)
        {
            wayPoints[i] = pathHolder.GetChild(i).position;

        }
       
           StartCoroutine(FollowPath(wayPoints));
    }

    void Update()
    {
     if(CanSeeBunny())
        {
            bunnyVisibleTimer += Time.deltaTime;           
        }
     else
        {
            bunnyVisibleTimer -= Time.deltaTime;
        }
        bunnyVisibleTimer = Mathf.Clamp(bunnyVisibleTimer, 0, timeToSpotBunny);
        spotLight.color = Color.Lerp(originalSpotLightColor, Color.red, bunnyVisibleTimer / timeToSpotBunny);
        if (bunnyVisibleTimer >= timeToSpotBunny)
        {
            if (OnGardenerSpottedBunny != null)
            {
                anim.SetInteger("Dance", 1);
                OnGardenerSpottedBunny();
            }
        }


    }
    private void DisableGardener()
    {
        anim.SetFloat("Speed", 0.0f);
        transform.LookAt(Player.transform);
        disableGardener = true;
    }
    private void OnDestroy()
    {
        OnGardenerSpottedBunny -= DisableGardener;
    }

    bool CanSeeBunny()
    {
        if(Vector3.Distance(transform.position,Player.position)<viewDistance)
        {
            Vector3 dirToPlayer = (Player.position - transform.position).normalized;
            float angleBetweenBunnyAndGardener = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenBunnyAndGardener<viewAngle/2f)
            {
                if(!Physics.Linecast(transform.position,Player.position,viewMask))
                {
                    return true;
                }
            }   
        }
        return false;
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
       
        Vector3 dirTolookTarget = (lookTarget - transform.position).normalized; // calculate the direction between target and the guard (this.transform)
        float targetAngle = 90 - Mathf.Atan2(dirTolookTarget.z, dirTolookTarget.x) * Mathf.Rad2Deg; // calculate the target angel
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) // if y angle of the transform and the target is greater than 0.05f (calculate the abs value to move anticlockwise)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime); // rotate and move the guard to the target angle
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    IEnumerator FollowPath(Vector3[] wayPoints)
    {
       
            
            transform.position = wayPoints[0];  // assign guard initial position
            int targetwayPointIndex = 1; // initial destination of the guard since it starts from waypoint zero
            Vector3 targetWayPoint = wayPoints[targetwayPointIndex]; // general movement road of the guard (desired target)
            transform.LookAt(targetWayPoint); // guard initially looks at waypoint
            while (true)
            {

                if(!disableGardener)
            {
                anim.SetFloat("Speed", 1.0f);
                transform.position = Vector3.MoveTowards(transform.position, targetWayPoint, speed * Time.deltaTime); //move the guard on the road
            }
               
                if (transform.position == targetWayPoint) // if we reach the target position
                {
                    targetwayPointIndex = (targetwayPointIndex + 1) % wayPoints.Length;  // Move on to the next point calculate the modulo,if modulo=0 get back to the inital position
                    targetWayPoint = wayPoints[targetwayPointIndex];// assign target to
                    yield return new WaitForSeconds(waitTime); // wait 
                    yield return StartCoroutine(TurnToFace(targetWayPoint)); // then begin the rotation of the target
                }

                yield return null;
            }
        
      

    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform wayPoint in pathHolder)
        { 
            Gizmos.DrawSphere(wayPoint.position, 0.4f);
            Gizmos.DrawLine(previousPosition, wayPoint.position);
            previousPosition = wayPoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        // view Distance
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
