using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour
{
    [SerializeField]
    private int flockSize = 20;

    [SerializeField]
    private float speedModifier = 5;

    [SerializeField]
    private float alignmentWeight = 1;

    [SerializeField]
    private float cohesionWeight = 1;

    [SerializeField]
    private float separationWeight = 1;

    [SerializeField]
    private float followWeight = 5;

    [Header("Boid Data")]
    [SerializeField]
    private Boid prefab;
    [SerializeField]
    private float spawnRadius = 3.0f;
    private Vector3 spawnLocation = Vector3.zero;
    [SerializeField]
    private Vector3 boundsMin;
    [SerializeField]
    private Vector3 boundsMax;
    [SerializeField]
    private float minDistance;
    

    [Header("Target Data")]
    [SerializeField]
    public Transform target;

    private Vector3 flockCenter;

    private Vector3 flockDirection;

    private Vector3 targetDirection;

    private Vector3 separation;

    public List<Boid> flockList = new List<Boid>();
    private string currentMode;

    [Header("Circle A Tree")]
    private Vector3[] waypoints = new Vector3[12];
    private int nextWaypoint;

    private Vector3 randomPos;

    public float SpeedModifier { get { return speedModifier; } }

    private void Awake()
    {
        float posX, posY, posZ;

        flockList = new List<Boid>(flockSize);
        //target = Transform.Find("target");
        for(int i = 0; i < flockSize; i++)
        {
            spawnLocation = Random.insideUnitSphere * spawnRadius + transform.position;
            Boid boid = Instantiate(prefab, spawnLocation, transform.rotation) as Boid;
            boid.transform.parent = transform;
            boid.FlockController = this;
            flockList.Add(boid);
        }
        for (int i = 0; i < waypoints.Length; i++)
        {
            posX = Random.Range(boundsMin.x, boundsMax.x);
            posY = Random.Range(boundsMin.y, boundsMax.y);
            posZ = Random.Range(boundsMin.z, boundsMax.z);

            waypoints[i] = new Vector3(posX, posY, posZ);
        }

        nextWaypoint = 0;

        currentMode = "lazy";
    }

    public string GetMode()
    {
        return currentMode;
    }

    public Vector3 Flock(Boid boid, Vector3 boidPosition, Vector3 boidDirection)
    {
        flockDirection = Vector3.zero;
        flockCenter = Vector3.zero;
        targetDirection = Vector3.zero;
        separation = Vector3.zero;

        for(int i = 0; i < flockList.Count; i++)
        {
            Boid neighbor = flockList[i];

            if(neighbor != boid)
            {
                flockDirection += neighbor.Direction;

                flockCenter += neighbor.transform.localPosition;

                separation += neighbor.transform.localPosition - boidPosition;
                separation *= -1;
            }
        }

        flockDirection /= flockSize;
        flockDirection = flockDirection.normalized * alignmentWeight;


        flockCenter /= flockSize;
        flockCenter = flockCenter.normalized * cohesionWeight;


        separation /= flockSize;
        separation = separation.normalized * separationWeight;

        
        return flockDirection + flockCenter + separation;
    }

    public Vector3 getDirection(Boid boid, Vector3 dest)
    {
        targetDirection = dest - boid.transform.position;
        targetDirection = targetDirection * followWeight;

        return targetDirection;
    }

    public Vector3 FollowTarget(Boid boid)
    {
        if(target.gameObject.activeSelf == false)
        {
            target.gameObject.SetActive(true);
        }
        return getDirection(boid, target.localPosition);
    }

    public Vector3 CircleATree(Boid boid)
    {
        target.gameObject.SetActive(false);
        if((boid.transform.position - waypoints[nextWaypoint]).magnitude <= minDistance)
        {
            nextWaypoint++;
            if(nextWaypoint >= 12)
            {
                nextWaypoint = 0;
            }
        }

        return getDirection(boid, waypoints[nextWaypoint]);
    }

    public Vector3 LazyFlight(Boid boid)
    {
        target.gameObject.SetActive(false);
        if((boid.transform.position - randomPos).magnitude <= minDistance)
        {
            randomPos = getRandomPos();
        }

        return getDirection(boid, randomPos);
    }

    Vector3 getRandomPos()
    {
        float x = Random.Range(boundsMin.x, boundsMax.x);
        float y = Random.Range(boundsMin.y, boundsMax.y);
        float z = Random.Range(boundsMin.z, boundsMax.z);

        return new Vector3(x, y, z);
    }

    public void setLazy()
    {
        currentMode = "lazy";
        randomPos = getRandomPos();
    }

    public void setCircle()
    {
        currentMode = "circle";
    }

    public void setFollow()
    {
        currentMode = "follow";
    }

}
